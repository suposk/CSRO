using CSRO.Common.AzureSdkServices;
using CSRO.Server.Api.Messaging;
using CSRO.Server.Domain;
using CSRO.Server.Entities.Entity;
using CSRO.Server.Entities.Enums;
using CSRO.Server.Infrastructure;
using CSRO.Server.Infrastructure.MessageBus;
using CSRO.Server.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CSRO.Server.Api.Commands
{
    public class VmOperationRequestCommand : IRequest<ResponseMessage<VmTicket>>    
    {
        public VmTicket VmTicket { get; set; }

        /// <summary>
        /// Should wait for reboot to complete. Applicable in some case
        /// </summary>
        public bool WaitForActionToComplete { get; set; }
    }

    public class VmOperationRequestCommandHandler : IRequestHandler<VmOperationRequestCommand, ResponseMessage<VmTicket>>
    {
        private readonly string _userId;
        private readonly IMediator _mediator;
        private readonly IVmSdkService _vmSdkService;
        private readonly IMessageBus _messageBus;
        private readonly IVmTicketRepository _repository;
        private readonly IVmTicketHistoryRepository _vmTicketHistoryRepository;
        private readonly ILogger<VmOperationRequestCommandHandler> _logger;
        private readonly ServiceBusConfig _serviceBusConfig;

        public VmOperationRequestCommandHandler(
            IConfiguration configuration,
            IApiIdentity apiIdentity,
            IMediator mediator,
            IVmSdkService vmSdkService,
            IMessageBus messageBus,
            IVmTicketRepository repository,
            IVmTicketHistoryRepository vmTicketHistoryRepository,
            ILogger<VmOperationRequestCommandHandler> logger)
        {
            _userId = apiIdentity.GetUserName();
            _mediator = mediator;
            _vmSdkService = vmSdkService;
            _messageBus = messageBus;
            _repository = repository;
            _vmTicketHistoryRepository = vmTicketHistoryRepository;
            _logger = logger;
            _serviceBusConfig = configuration.GetSection(nameof(ServiceBusConfig)).Get<ServiceBusConfig>();
        }

        public async Task<ResponseMessage<VmTicket>> Handle(VmOperationRequestCommand request, CancellationToken cancellationToken)
        {
            var result = new ResponseMessage<VmTicket>();
            try
            {
                var ticket = request.VmTicket;
                if (!Enum.TryParse(ticket.Operation, out VmOperatioType vmOperatioType) || vmOperatioType == VmOperatioType.Unknown)
                {
                    result.Message = string.IsNullOrWhiteSpace(ticket.Operation) ? $"{nameof(ticket.Operation)} is missing" : $"Unsupported type of {nameof(ticket.Operation)} {ticket.Operation}";
                    return result;
                }

                //validate sub name and tags
                var canReboot = await _vmSdkService.IsRebootAllowed(ticket.SubcriptionId, ticket.ResorceGroup, ticket.VmName).ConfigureAwait(false);
                if (!canReboot.success)
                {
                    result.Success = false;
                    result.Message = canReboot.errorMessage;                    
                    return result;
                }

                //save                
                _repository.Add(ticket, _userId);
                if (!await _repository.SaveChangesAsync())
                {
                    result.Success = false;
                    result.Message = "Failed to save to DB";
                    return result;
                }

                await _vmTicketHistoryRepository.Create(ticket);                

                //send message to bus
                BusMessageBase message = new VmOperationRequestMessage { Vm = ticket.VmName, UserId = _userId, TicketId = ticket.Id }.CreateBaseMessage();
                try
                {
                    if (request.WaitForActionToComplete == false)
                    {                        
                        await _messageBus.PublishMessageTopic(message, _serviceBusConfig.VmOperationRequesTopic);

                        result.ReturnedObject = ticket;
                        result.Success = true;
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to PublishMessageTopic {ex?.Message}", message);
                }

                //failover if message is not sent to bus, or want respond
                if (request.WaitForActionToComplete || result.Success == false)
                {
                    var dto = message as VmOperationRequestMessage;
                    var vmOperationExecuteCommand = new VmOperationExecuteCommand() { VmOperationRequestMessage = dto };
                    var response = await _mediator.Send(vmOperationExecuteCommand);
                    //update result
                    result = response;
                    return result;
                }
                result.ReturnedObject = ticket;
            }
            catch (Exception ex)
            {
                //this shouldn't stop the API from doing else so this can be logged
                _logger.LogError($"{nameof(VmOperationRequestCommandHandler)} failed due to: {ex.Message}");
            }
            return result;
        }
    }
}

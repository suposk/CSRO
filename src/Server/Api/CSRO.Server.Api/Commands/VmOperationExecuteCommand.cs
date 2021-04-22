using CSRO.Common.AzureSdkServices;
using CSRO.Server.Api.Messaging;
using CSRO.Server.Domain;
using CSRO.Server.Entities.Entity;
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
    public class VmOperationExecuteCommand : IRequest<ResponseMessage<VmTicket>>
    {
        public VmOperationRequestMessage VmOperationRequestMessage { get; set; }
    }

    public class VmOperationExecuteCommandHandler : IRequestHandler<VmOperationExecuteCommand, ResponseMessage<VmTicket>>
    {
        private readonly string _userId;
        private readonly IVmSdkService _vmSdkService;
        private readonly IMessageBus _messageBus;
        private readonly IVmTicketRepository _repository;
        private readonly ILogger<VmOperationExecuteCommandHandler> _logger;
        private readonly ServiceBusConfig _serviceBusConfig;

        public VmOperationExecuteCommandHandler(
            IConfiguration configuration,
            IApiIdentity apiIdentity,
            IVmSdkService vmSdkService,
            IMessageBus messageBus,
            IVmTicketRepository repository,
            ILogger<VmOperationExecuteCommandHandler> logger)
        {
            _userId = apiIdentity.GetUserName();
            _vmSdkService = vmSdkService;
            _messageBus = messageBus;
            _repository = repository;
            _logger = logger;
            _serviceBusConfig = configuration.GetSection(nameof(ServiceBusConfig)).Get<ServiceBusConfig>();
        }

        public async Task<ResponseMessage<VmTicket>> Handle(VmOperationExecuteCommand request, CancellationToken cancellationToken)
        {
            var result = new ResponseMessage<VmTicket>();
            try
            {
                var ticket = await _repository.GetId(request.VmOperationRequestMessage.TicketId);

                //save
                ticket.Status = "Processing";
                ticket.VmState = "Restart in Progress";
                if (!await _repository.SaveChangesAsync())
                {
                    result.Success = false;
                    result.Message = "Failed to save to DB";
                    return result;
                }

                //var status = await _vmSdkService.GetStatus(ticket.SubcriptionId, ticket.ResorceGroup, ticket.VmName).ConfigureAwait(false);
                //if (status != null && status.DisplayStatus.Contains("deallocat"))
                //{
                //    result.Message = $"Unable to Reboot, Vm is {status.DisplayStatus ?? "Stopped"}";
                //    return result;
                //}                    

                var reb = await _vmSdkService.RebootVmAndWaitForConfirmation(ticket.SubcriptionId, ticket.ResorceGroup, ticket.VmName).ConfigureAwait(false);
                if (reb.success)
                {
                    ticket.Status = "Completed";
                    ticket.VmState = reb.status;
                }
                else
                {
                    ticket.Status = "Rejected";
                    ticket.VmState = reb.errorMessage;
                }
                if (!await _repository.SaveChangesAsync())
                {
                    result.Success = false;
                    result.Message = "Failed to save to DB";
                    return result;
                }

                result.ReturnedObject = ticket;
            }
            catch (Exception ex)
            {
                //this shouldn't stop the API from doing else so this can be logged
                _logger.LogError($"{nameof(VmOperationExecuteCommandHandler)} failed due to: {ex.Message}");
            }
            return result;
        }
    }
}

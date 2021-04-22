using AutoMapper;
using CSRO.Server.Infrastructure.MessageBus;
using MediatR;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using CSRO.Server.Api.Messaging;

namespace CSRO.Server.Api.BackgroundTasks
{
    public class AzServiceBusConsumer : BackgroundService, IServiceBusConsumer
    {
        private readonly IReceiverClient _wmOperationRequestMessageReceiverClient;             
        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;
        private readonly IMediator _mediator;
        private readonly ILogger<AzServiceBusConsumer> _logger;
        private readonly ServiceBusConfig _serviceBusConfig;

        public AzServiceBusConsumer(
            IConfiguration configuration,
            IMessageBus messageBus,
            IMediator mediator,
            IMapper mapper,
            ILogger<AzServiceBusConsumer> logger)
        {
            _configuration = configuration;
            _messageBus = messageBus;
            _mediator = mediator;
            _logger = logger;

            var serviceBusConnectionString = configuration.GetConnectionString("AzureServiceBus");
            _serviceBusConfig = configuration.GetSection(nameof(ServiceBusConfig)).Get<ServiceBusConfig>();

            _wmOperationRequestMessageReceiverClient = new SubscriptionClient(serviceBusConnectionString, _serviceBusConfig.VmOperationRequesTopic, _serviceBusConfig.VmOperationRequesSub);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Start();
            return Task.CompletedTask;
        }

        public void Start()
        {
            var messageHandlerOptions = new MessageHandlerOptions(OnServiceBusException) { MaxConcurrentCalls = 5, AutoComplete = false };
            _wmOperationRequestMessageReceiverClient?.RegisterMessageHandler(OnVmOperationReceived, messageHandlerOptions);
        }

        private async Task OnVmOperationReceived(Message message, CancellationToken arg2)
        {
            try
            {
                var body = Encoding.UTF8.GetString(message.Body);//json from service bus
                var dto = JsonConvert.DeserializeObject<VmOperationRequestMessage>(body);

                //var createApprovedAdoProjectsCommand = new CreateApprovedAdoProjectIdsCommand() { Approved = dto.ApprovedAdoProjectIds, UserId = dto.UserId };
                //var created = await _mediator.Send(createApprovedAdoProjectsCommand);
                //if (created.IsNullOrEmptyCollection() || created.Count != dto.ApprovedAdoProjectIds.Count)
                //    _logger.LogWarning($"{nameof(OnVmOperationReceived)} Unxcepted result from {nameof(CreateApprovedAdoProjectIdsCommand)} ", created, dto);

                //await _wmOperationRequestMessageReceiverClient.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(OnVmOperationReceived)} {ex.Message}", ex);
            }
        }

        private Task OnServiceBusException(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            _logger.LogError(exceptionReceivedEventArgs.Exception, exceptionReceivedEventArgs?.Exception?.Message);
            return Task.CompletedTask;
        }


        public void Stop()
        {

        }
    }
}

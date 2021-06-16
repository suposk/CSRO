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
using CSRO.Server.Api.Commands;

namespace CSRO.Server.Api.BackgroundTasks
{
    public class AzServiceBusConsumer : BackgroundService, IServiceBusConsumer
    {
        private IReceiverClient _wmOperationRequestMessageReceiverClient;             
        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;
        private readonly IMediator _mediator;
        private readonly ILogger<AzServiceBusConsumer> _logger;
        private ServiceBusConfig _serviceBusConfig;

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
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Start();
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await Stop();
            await base.StopAsync(cancellationToken);
        }

        public async Task Start()
        {

            var serviceBusConnectionString = _configuration.GetConnectionString("AzureServiceBus");
            _serviceBusConfig = _configuration.GetSection(nameof(ServiceBusConfig)).Get<ServiceBusConfig>();
            var busConfig = _configuration.GetSection(nameof(BusConfig)).Get<BusConfig>();
            if (busConfig != null)
            {
                _logger.LogWarning($"{busConfig.BusType} with ConnectionString {serviceBusConnectionString.ReplaceWithStars(20)} will delay start for {busConfig.BusDelayStartInSec} ");
                await Task.Delay(busConfig.BusDelayStartInSec);
                _logger.LogWarning($"{busConfig.BusType} connecting...");
            }

            _wmOperationRequestMessageReceiverClient = new SubscriptionClient(serviceBusConnectionString, _serviceBusConfig.VmOperationRequesTopic, _serviceBusConfig.VmOperationRequesSub);
            var messageHandlerOptions = new MessageHandlerOptions(OnServiceBusException) { MaxConcurrentCalls = 5, AutoComplete = false };
            _wmOperationRequestMessageReceiverClient?.RegisterMessageHandler(OnVmOperationReceived, messageHandlerOptions);
        }

        private async Task OnVmOperationReceived(Message message, CancellationToken arg2)
        {
            try
            {
                var body = Encoding.UTF8.GetString(message.Body);//json from service bus
                var dto = JsonConvert.DeserializeObject<VmOperationRequestMessage>(body);

                var vmOperationExecuteCommand = new VmOperationExecuteCommand() {  VmOperationRequestMessage = dto };
                var response = await _mediator.Send(vmOperationExecuteCommand);

                await _wmOperationRequestMessageReceiverClient.CompleteAsync(message.SystemProperties.LockToken);
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


        public async Task Stop()
        {
            await _wmOperationRequestMessageReceiverClient?.CloseAsync();
        }
    }
}

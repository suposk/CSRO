using AutoMapper;
using CSRO.Common.AdoServices;
using CSRO.Server.Ado.Api.Commands;
using CSRO.Server.Ado.Api.Messaging;
using CSRO.Server.Ado.Api.Services;
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

namespace CSRO.Server.Ado.Api.BackgroundTasks
{
    public class AzServiceBusConsumer : BackgroundService, IServiceBusConsumer
    {        
        private IReceiverClient _approvedAdoProjcetMessageReceiverClient;
        private IReceiverClient _rejectedAdoProjcetMessageReceiverClient;
        private IQueueClient _queueReceiverClient;

        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;
        private readonly IMediator _mediator;
        private readonly IProjectAdoServices _projectAdoServices;
        private readonly IAdoProjectRepository _adoProjectRepository;
        private readonly IAdoProjectHistoryRepository _adoProjectHistoryRepository;
        private readonly ILogger<AzServiceBusConsumer> _logger;
        private ServiceBusConfig _serviceBusConfig;

        public AzServiceBusConsumer(
            IConfiguration configuration,
            IMessageBus messageBus,
            IMediator mediator,
            IProjectAdoServices projectAdoServices,
            IAdoProjectRepository adoProjectRepository,
            IAdoProjectHistoryRepository adoProjectHistoryRepository,
            IMapper mapper,
            ILogger<AzServiceBusConsumer> logger)
        {
            _configuration = configuration;
            _messageBus = messageBus;
            _mediator = mediator;
            _projectAdoServices = projectAdoServices;
            _adoProjectRepository = adoProjectRepository;
            _adoProjectHistoryRepository = adoProjectHistoryRepository;
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
                await Task.Delay(busConfig.BusDelayStartInSec * 1000);
                _logger.LogWarning($"{busConfig.BusType} connecting...");
            }

            _approvedAdoProjcetMessageReceiverClient = new SubscriptionClient(serviceBusConnectionString, _serviceBusConfig.ApprovedAdoProjectsTopic, _serviceBusConfig.ApprovedAdoProjectsSub);
            _rejectedAdoProjcetMessageReceiverClient = new SubscriptionClient(serviceBusConnectionString, _serviceBusConfig.RejectedAdoProjectsTopic, _serviceBusConfig.RejectedAdoProjectsSub);
            //queueReceiverClient = new QueueClient(serviceBusConnectionString, _serviceBusConfig.QueueNameTest);

            var messageHandlerOptions = new MessageHandlerOptions(OnServiceBusException) { MaxConcurrentCalls = 5 , AutoComplete = false };

            _approvedAdoProjcetMessageReceiverClient?.RegisterMessageHandler(OnApprovedReceived, messageHandlerOptions);
            _rejectedAdoProjcetMessageReceiverClient?.RegisterMessageHandler(OnRejectedReceived, messageHandlerOptions);
            //queueReceiverClient?.RegisterMessageHandler(ExecuteQueueMessageProcessing, messageHandlerOptions);
        }

        private async Task OnApprovedReceived(Message message, CancellationToken arg2)
        {
            try
            {
                var body = Encoding.UTF8.GetString(message.Body);//json from service bus
                var dto = JsonConvert.DeserializeObject<ApprovedAdoProjectsMessage>(body);

                var createApprovedAdoProjectsCommand = new CreateApprovedAdoProjectIdsCommand() { Approved = dto.ApprovedAdoProjectIds, UserId = dto.UserId };
                var created = await _mediator.Send(createApprovedAdoProjectsCommand);
                if (created.IsNullOrEmptyCollection() || created.Count != dto.ApprovedAdoProjectIds.Count)                
                    _logger.LogWarning($"{nameof(OnApprovedReceived)} Unxcepted result from {nameof(CreateApprovedAdoProjectIdsCommand)} ", created, dto);
                
                await _approvedAdoProjcetMessageReceiverClient.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(OnApprovedReceived)} {ex.Message}", ex);
            }
        }

        private async Task OnRejectedReceived(Message message, CancellationToken arg2)
        {
            try
            {
                var body = Encoding.UTF8.GetString(message.Body);//json from service bus
                var dto = JsonConvert.DeserializeObject<RejectedAdoProjectsMessage>(body);

                //TOTO sent email beack to user

                await _rejectedAdoProjcetMessageReceiverClient.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(OnRejectedReceived)} {ex.Message}", ex);
            }
        }

        //private async Task ExecuteQueueMessageProcessing(Message message, CancellationToken arg2)
        //{
        //    var body = Encoding.UTF8.GetString(message.Body);//json from service bus
        //    var dto = JsonConvert.DeserializeObject<ApprovedAdoProjectsMessage>(body);

        //    var createApprovedAdoProjectsCommand = new CreateApprovedAdoProjectIdsCommand() { Approved = dto.ApprovedAdoProjectIds, UserId = dto.UserId };
        //    await _mediator.Send(createApprovedAdoProjectsCommand);

        //    await queueReceiverClient.CompleteAsync(message.SystemProperties.LockToken);
        //}

        private Task OnServiceBusException(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            _logger.LogError(exceptionReceivedEventArgs.Exception, exceptionReceivedEventArgs?.Exception?.Message);
            return Task.CompletedTask;
        }


        public async Task Stop()
        {
            await _approvedAdoProjcetMessageReceiverClient?.CloseAsync();
            await _rejectedAdoProjcetMessageReceiverClient?.CloseAsync();
            //await _queueReceiverClient?.CloseAsync();
        }
    }
}

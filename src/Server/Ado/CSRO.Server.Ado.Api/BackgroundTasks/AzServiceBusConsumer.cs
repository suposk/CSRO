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
        private readonly IReceiverClient approvedAdoProjcetMessageReceiverClient;
        private readonly IReceiverClient rejectedAdoProjcetMessageReceiverClient;
        private readonly IQueueClient queueReceiverClient;

        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;
        private readonly IMediator _mediator;
        private readonly IProjectAdoServices _projectAdoServices;
        private readonly IAdoProjectRepository _adoProjectRepository;
        private readonly IAdoProjectHistoryRepository _adoProjectHistoryRepository;
        private readonly ILogger<AzServiceBusConsumer> _logger;
        private readonly ServiceBusConfig _serviceBusConfig;

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

            var serviceBusConnectionString = configuration.GetConnectionString("AzureServiceBus");
            _serviceBusConfig = configuration.GetSection(nameof(ServiceBusConfig)).Get<ServiceBusConfig>();
            
            approvedAdoProjcetMessageReceiverClient = new SubscriptionClient(serviceBusConnectionString, _serviceBusConfig.ApprovedAdoProjectsTopic, _serviceBusConfig.ApprovedAdoProjectsSub);
            rejectedAdoProjcetMessageReceiverClient = new SubscriptionClient(serviceBusConnectionString, _serviceBusConfig.RejectedAdoProjectsTopic, _serviceBusConfig.RejectedAdoProjectsSub);
            
            //queueReceiverClient = new QueueClient(serviceBusConnectionString, _serviceBusConfig.QueueNameTest);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Start();
            return Task.CompletedTask;
        }

        public void Start()
        {
            var messageHandlerOptions = new MessageHandlerOptions(OnServiceBusException) { MaxConcurrentCalls = 5 , AutoComplete = false };

            approvedAdoProjcetMessageReceiverClient?.RegisterMessageHandler(OnApprovedReceived, messageHandlerOptions);
            rejectedAdoProjcetMessageReceiverClient?.RegisterMessageHandler(OnRejectedReceived, messageHandlerOptions);

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
                
                await approvedAdoProjcetMessageReceiverClient.CompleteAsync(message.SystemProperties.LockToken);
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

                await rejectedAdoProjcetMessageReceiverClient.CompleteAsync(message.SystemProperties.LockToken);
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


        public void Stop()
        {

        }
    }
}

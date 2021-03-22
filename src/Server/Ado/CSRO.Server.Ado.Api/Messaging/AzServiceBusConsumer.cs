using AutoMapper;
using CSRO.Common.AdoServices;
using CSRO.Server.Ado.Api.Commands;
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

namespace CSRO.Server.Ado.Api.Messaging
{
    public class AzServiceBusConsumer : BackgroundService, IServiceBusConsumer
    {
        private readonly string subscriptionName = "approvedadoprojectssub";
        //private readonly IReceiverClient checkoutMessageReceiverClient;
        private readonly IReceiverClient approvedAdoProjcetMessageReceiverClient;

        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;
        private readonly IMediator _mediator;
        private readonly IProjectAdoServices _projectAdoServices;
        private readonly IAdoProjectRepository _adoProjectRepository;
        private readonly IAdoProjectHistoryRepository _adoProjectHistoryRepository;
        private readonly ILogger<AzServiceBusConsumer> _logger;
        //private readonly string checkoutMessageTopic;
        //private readonly string orderPaymentRequestMessageTopic;
        private readonly string orderPaymentUpdatedMessageTopic;

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
            //checkoutMessageTopic = _configuration.GetValue<string>("CheckoutMessageTopic");
            //orderPaymentRequestMessageTopic = _configuration.GetValue<string>("OrderPaymentRequestMessageTopic");
            //orderPaymentUpdatedMessageTopic = _configuration.GetValue<string>("OrderPaymentUpdatedMessageTopic");

            //checkoutMessageReceiverClient = new SubscriptionClient(serviceBusConnectionString, checkoutMessageTopic, subscriptionName);
            approvedAdoProjcetMessageReceiverClient = new SubscriptionClient(serviceBusConnectionString, "approvedadoprojects", subscriptionName);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Start();
            return Task.CompletedTask;
        }

        public void Start()
        {
            var messageHandlerOptions = new MessageHandlerOptions(OnServiceBusException) { MaxConcurrentCalls = 4 };
            approvedAdoProjcetMessageReceiverClient.RegisterMessageHandler(OnOrderPaymentUpdateReceived, messageHandlerOptions);
        }

        private async Task OnOrderPaymentUpdateReceived(Message message, CancellationToken arg2)
        {
            var body = Encoding.UTF8.GetString(message.Body);//json from service bus
            var dto = JsonConvert.DeserializeObject<ApprovedAdoProjectsMessage>(body);

            var createApprovedAdoProjectsCommand = new CreateApprovedAdoProjectIdsCommand() { Approved = dto.ApprovedAdoProjectIds, UserId = dto.UserId };             
            await _mediator.Send(createApprovedAdoProjectsCommand);
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

using CSRO.Server.Infrastructure.MessageBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace CSRO.Server.Ado.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        //public static IServiceBusConsumer Consumer { get; set; }

        //public static IApplicationBuilder UseAzServiceBusConsumer(this IApplicationBuilder app)
        //{
        //    Consumer = app.ApplicationServices.GetService<IServiceBusConsumer>();
        //    var life = app.ApplicationServices.GetService<IHostApplicationLifetime>();

        //    life.ApplicationStarted.Register(OnStarted);
        //    life.ApplicationStopping.Register(OnStopping);

        //    return app;
        //}

        //private static void OnStarted()
        //{
        //    Consumer.Start();
        //}

        //private static void OnStopping()
        //{
        //    Consumer.Stop();
        //}
    }
}

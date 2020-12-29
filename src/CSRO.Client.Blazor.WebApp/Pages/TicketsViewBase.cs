using CSRO.Client.Services;
using CSRO.Client.Services.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Client.Blazor.WebApp.Pages
{
    public class TicketsViewBase : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        IBaseDataStore<Ticket> TicketDataStore { get; set; }

        [Inject]
        public ILogger<TicketsViewBase> Logger { get; set; }


        public Ticket model { get; set; } = new Ticket();
        
        public List<Ticket> Tickets { get; set; }

        protected async override Task OnInitializedAsync()
        {

            try
            {
                Tickets = await TicketDataStore.GetItemsAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(OnInitializedAsync));
            }
        }
    }
}

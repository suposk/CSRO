using CSRO.Client.Blazor.UI;
using CSRO.Client.Blazor.UI.Services;
using CSRO.Client.Blazor.WebApp.Components;
using CSRO.Client.Services;
using CSRO.Client.Services.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Dialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Client.Blazor.WebApp.Pages
{
    public class TicketsViewBase : CsroComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public ICsroDialogService CsroDialogService { get; set; }

        [Inject]
        IBaseDataService<Ticket> TicketDataService { get; set; }

        [Inject]
        public ILogger<TicketsViewBase> Logger { get; set; }


        public Ticket Model { get; set; } = new Ticket();
        
        public List<Ticket> Tickets { get; set; }

        protected async override Task OnInitializedAsync()
        {

            try
            {
                ShowLoading();                
                Tickets = null;
                Tickets = await TicketDataService.GetItemsAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(OnInitializedAsync));
            }
            HideLoading();
        }

        public async Task DeleteTicketAsync(Ticket ticket)
        {
            var ok = await CsroDialogService.ShowDialog("Delete Ticket", $"Do you really want to delete these record Id: {ticket.Id}?", "Delete");
            if (ok)
            {
                var res = await TicketDataService.DeleteItemAsync(ticket.Id);
                if (res)
                {
                    Tickets.Remove(ticket);
                }
            }
        }

    }
}

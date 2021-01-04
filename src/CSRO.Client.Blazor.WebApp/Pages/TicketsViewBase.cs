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
    public class TicketsViewBase : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

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

        public async Task DeleteTicketAsync(Ticket ticket)
        {
            var parameters = new DialogParameters();
            parameters.Add("ContentText", $"Do you really want to delete these record {ticket.Id}-{ticket.Description}?");
            parameters.Add("ButtonText", "Delete");
            parameters.Add("Color", Color.Error);

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Small };
            var userSelect = DialogService.Show<DialogTemplateExample_Dialog>("Delete Ticket", parameters, options);
            var result = await userSelect.Result;
            if (!result.Cancelled)
            {
                var res = await TicketDataStore.DeleteItemAsync(ticket.Id);
                if (res)
                {
                    Tickets.Remove(ticket);
                }
            }
        }

        public void DeleteTicket(int id)
        {
            ;
        }
    }
}

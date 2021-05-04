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
    public class VmBase : CsroComponentBase
    {
        #region Params and Injects

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public ICsroDialogService CsroDialogService { get; set; }

        [Inject]
        IVmTicketDataService VmTicketDataService { get; set; }

        [Inject]
        public ILogger<VmBase> Logger { get; set; }

        #endregion

        public VmTicket model { get; set; } = new VmTicket();
        
        public List<VmTicket> Tickets { get; set; }

        protected async override Task OnInitializedAsync()
        {

            try
            {
                ShowLoading();
                
                Tickets = null;
                Tickets = await VmTicketDataService.GetItemsAsync();
            }
            catch (Exception ex)
            {
                Logger.LogErrorCsro(ex);
            }
            HideLoading();
        }

        public async Task DeleteTicketAsync(VmTicket ticket)
        {
            var ok = await CsroDialogService.ShowDialog("Delete Vm Ticket", $"Do you really want to delete these record Id: {ticket.Id}?", "Delete");
            if (ok)
            {
                var res = await VmTicketDataService.DeleteItemAsync(ticket.Id);
                if (res)
                {
                    Tickets.Remove(ticket);
                }
            }
        }
    }
}

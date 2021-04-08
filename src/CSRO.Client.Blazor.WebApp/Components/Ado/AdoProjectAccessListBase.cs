﻿using CSRO.Client.Blazor.UI;
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

namespace CSRO.Client.Blazor.WebApp.Components.Ado
{
    public class AdoProjectAccessListBase : CsroComponentBase
    {
        #region Params and Injects

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public ICsroDialogService CsroDialogService { get; set; }

        [Inject]
        public IAdoProjectAccessDataService AdoProjectAccessDataService { get; set; }

        [Inject]
        public IAuthCsroService AuthCsroService { get; set; }

        [Inject]
        public ILogger<AdoProjectAccessListBase> Logger { get; set; }

        #endregion

        public List<AdoProjectAccessModel> Requests { get; set; } = new List<AdoProjectAccessModel>();

        protected async override Task OnInitializedAsync()
        {

            try
            {
                ShowLoading();
                Requests.Clear();
                //if admin
                Requests = await AdoProjectAccessDataService.GetItemsAsync();
                //else
                //Requests = await AdoProjectAccessDataService.GetItemsByUserId(await AuthCsroService.GetCurrentUserId());
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(OnInitializedAsync));
                await CsroDialogService.ShowError("Error", ex?.Message);
            }
            HideLoading();
        }

        public async Task DeleteTicketAsync(AdoProjectAccessModel ticket)
        {
            var ok = await CsroDialogService.ShowDialog("Delete Ticket", $"Do you really want to delete these record Id: {ticket.Id}?", "Delete");
            if (ok)
            {
                var res = await AdoProjectAccessDataService.DeleteItemAsync(ticket.Id);
                if (res)
                {
                    Requests.Remove(ticket);
                }
            }
        }

    }
}

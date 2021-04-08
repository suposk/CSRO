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

        protected List<AdoProjectAccessModel> Requests { get; set; } = new List<AdoProjectAccessModel>();


        /// <summary>
        /// Only for DEV
        /// </summary>
        private bool _CanApprove = false;
        public bool CanApprove
        {
            get { return _CanApprove; }
            set 
            {
                _CanApprove = value;
                CallLoad();
            }
        }


        protected async override Task OnInitializedAsync()
        {
            await Load();            
        }

        /// <summary>
        /// Only for DEV
        /// </summary>
        async void CallLoad()
        {
            await Load();
            StateHasChanged(); //must be here refresh state to bind
        }

        private async Task Load()
        {
            try
            {
                ShowLoading();
                Requests.Clear();
                //if admin
                if (CanApprove)
                {
                    var all = await AdoProjectAccessDataService.GetItemsAsync();
                    Requests = all?.FindAll(a => a.Status == Status.Submitted);
                }
                else
                    Requests = await AdoProjectAccessDataService.GetItemsByUserId(await AuthCsroService.GetCurrentUserId());
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(OnInitializedAsync));
                await CsroDialogService.ShowError("Error", ex?.Message);
            }
            HideLoading();
        }

        public Task CanApproveChecked(bool value)
        {
            CanApprove = value;
            return Task.CompletedTask;
        }

        public async Task DeleteRequestAsync(AdoProjectAccessModel ticket)
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

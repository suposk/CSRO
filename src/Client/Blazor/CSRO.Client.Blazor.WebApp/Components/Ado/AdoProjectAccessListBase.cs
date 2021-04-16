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

        protected HashSet<AdoProjectAccessModel> selectedItems = new();
        protected bool IsButtonDisabled => IsLoading || selectedItems.Count == 0;
        protected bool CanApprove { get; set; }
        protected OperatioTypeIdPair OperatioTypeIdPair { get; set; } = new();

        protected bool ShowDetails { get; set; }

        /// <summary>
        /// Only for DEV
        /// </summary>
        private bool _AdminMode = false;
        public bool AdminMode
        {
            get { return _AdminMode; }
            set 
            {
                _AdminMode = value;
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
                CanApprove = await AuthCsroService.HasPermision(Core.PoliciesCsro.CanApproveAdoRequestPolicy);

                //if admin
                if (AdminMode)
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

        //public Task CanApproveChecked(bool value)
        //{
        //    CanApprove = value;
        //    return Task.CompletedTask;
        //}

        public void CreateNew(bool value)
        {
            ShowDetails = value;
            OperatioTypeIdPair = new();
        }

        public async Task ApproveAsync()
        {
            try
            {
                if (selectedItems?.Count == 0)
                    return;

                var toBeApproved = selectedItems.Select(a => a.Id).ToList();
                ShowLoading($"Approving {toBeApproved.Count} project(s)");

                var approved = await AdoProjectAccessDataService.ApproveAdoProject(toBeApproved);
                if (toBeApproved.Count == approved?.Count)
                    await CsroDialogService.ShowMessage("Success", $"All {toBeApproved.Count} project(s) were approved.");
                else
                    await CsroDialogService.ShowMessage("Partial Success", $"Only {approved.Count} out of {toBeApproved.Count} project(s) were approved.");

                await Load();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(OnInitializedAsync));
                await CsroDialogService.ShowError("Error", $"Detail error: {ex.Message}");
            }
            HideLoading();
        }


        public async Task RejectAsync()
        {
            try
            {
                if (selectedItems?.Count == 0)
                    return;

                var toBeRejected = selectedItems.Select(a => a.Id).ToList();

                var text = $"Reason to Reject {toBeRejected.Count} project(s)";
                var rejectReason = await CsroDialogService.ShowDialogWithEntry("Enter reason", text);
                if (string.IsNullOrWhiteSpace(rejectReason))
                    return;    //cancel pressed

                ShowLoading($"Rejecting {toBeRejected.Count} project(s)");

                var rejected = await AdoProjectAccessDataService.RejectAdoProject(toBeRejected, rejectReason);
                if (toBeRejected.Count == rejected?.Count)
                    await CsroDialogService.ShowMessage("Success", $"All {toBeRejected.Count} project(s) were rejected.");
                else
                    await CsroDialogService.ShowMessage("Partial Success", $"Only {rejected.Count} out of {toBeRejected.Count} project(s) were rejected.");

                await Load();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(OnInitializedAsync));
                await CsroDialogService.ShowError("Error", $"Detail error: {ex.Message}");
            }
            HideLoading();
        }

        public Task EditRequestAsync(AdoProjectAccessModel ticket)
        {
            ShowDetails = true;
            OperatioTypeIdPair = new() { OperatioTypeEnum = OperatioType.Edit, Id = ticket.Id.ToString() };
            return Task.CompletedTask;
        }

        public Task ViewRequestAsync(AdoProjectAccessModel ticket)
        {
            ShowDetails = true;
            OperatioTypeIdPair = new() { OperatioTypeEnum = OperatioType.View, Id = ticket.Id.ToString() };
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

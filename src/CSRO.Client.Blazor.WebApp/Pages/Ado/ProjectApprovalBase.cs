using CSRO.Client.Blazor.UI;
using CSRO.Client.Blazor.UI.Services;
using CSRO.Client.Blazor.WebApp.Components;
using CSRO.Client.Services;
using CSRO.Client.Services.Models;
using CSRO.Common.AdoServices.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Dialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Client.Blazor.WebApp.Pages.Ado
{
    public class ProjectApprovalBase : CsroComponentBase
    {
        #region Params and Injects

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public ICsroDialogService CsroDialogService { get; set; }

        [Inject]
        public IAdoProjectDataService AdoProjectDataService { get; set; }

        [Inject]
        public ILogger<ProjectApprovalBase> Logger { get; set; }

        #endregion

        protected List<ProjectAdo> Tickets { get; set; } = new();

        protected HashSet<ProjectAdo> selectedItems = new();

        protected bool IsButtonDisabled => IsLoading || selectedItems.Count == 0;


        protected async override Task OnInitializedAsync()
        {
            await Load();
        }

        private async Task Load()
        {
            try
            {
                ShowLoading();

                selectedItems?.Clear();
                Tickets?.Clear();                
                Tickets = await AdoProjectDataService.GetProjectsForApproval();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(OnInitializedAsync));
                await CsroDialogService.ShowError("Error", $"Detail error: {ex.Message}");
            }
            HideLoading();
        }

        public async Task ApproveAsync()
        {
            try
            {
                if (selectedItems?.Count == 0)
                    return;

                var toBeApproved = selectedItems.Select(a => a.Id).ToList();
                ShowLoading($"Approving {toBeApproved.Count} project(s)");
                
                var approved = await AdoProjectDataService.ApproveAdoProject(toBeApproved);
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
                var rejectionText = await CsroDialogService.ShowDialogWithEntry("Enter reason", text);     
                if (string.IsNullOrWhiteSpace(rejectionText))                                  
                    return;    //cancel pressed

                ShowLoading($"Rejecting {toBeRejected.Count} project(s)");

                var rejected = await AdoProjectDataService.RejectAdoProject(toBeRejected);
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

    }
}

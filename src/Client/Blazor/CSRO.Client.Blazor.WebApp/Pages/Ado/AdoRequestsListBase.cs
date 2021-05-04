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
    public class AdoRequestsListBase : CsroComponentBase
    {
        #region Params and Injects

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public ICsroDialogService CsroDialogService { get; set; }

        [Inject]
        public IAdoProjectDataService AdoProjectDataService { get; set; }

        [Inject]
        public ILogger<AdoRequestsListBase> Logger { get; set; }

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
                Tickets = await AdoProjectDataService.GetItemsAsync();
            }
            catch (Exception ex)
            {
                Logger.LogErrorCsro(ex);
                await CsroDialogService.ShowError("Error", $"Detail error: {ex.Message}");
            }
            HideLoading();
        }

        public async Task DeleteTicketAsync(ProjectAdo ticket)
        {
            try
            {
                var ok = await CsroDialogService.ShowDialog("Delete Ticket", $"Do you really want to delete these record Id: {ticket.Id}?", "Delete");
                if (ok)
                {
                    var res = await AdoProjectDataService.DeleteItemAsync(ticket.Id);
                    if (res)
                    {
                        Tickets.Remove(ticket);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogErrorCsro(ex);
                await CsroDialogService.ShowError("Error", $"Detail error: {ex.Message}");
            }
        }

        //public async Task ApproveAsync()
        //{
        //    try
        //    {
        //        if (selectedItems?.Count == 0)
        //            return;

        //        var toBeApproved = selectedItems.Select(a => a.Id).ToList();
        //        ShowLoading($"Approving {toBeApproved.Count} project(s)");

        //        var approved = await AdoProjectDataService.ApproveAdoProject(toBeApproved);
        //        if (toBeApproved.Count == approved?.Count)
        //            await CsroDialogService.ShowMessage("Success", $"All {toBeApproved.Count} project(s) were approved and created.");
        //        else
        //            await CsroDialogService.ShowMessage("Partial Success", $"Only {approved.Count} out of {toBeApproved.Count} project(s) were approved and created.");

        //        await Load();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError(ex, nameof(OnInitializedAsync));
        //        await CsroDialogService.ShowError("Error", $"Detail error: {ex.Message}");
        //    }
        //    HideLoading();
        //}
    }
}

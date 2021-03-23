using CSRO.Client.Blazor.UI;
using CSRO.Client.Blazor.UI.Services;
using CSRO.Client.Services;
using CSRO.Client.Services.Models;
using CSRO.Common.AdoServices;
using CSRO.Common.AdoServices.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Client.Blazor.WebApp.Components.Ado
{
    public class AdoProjectBase : CsroComponentBase
    {
        #region Params and Injects

        [Parameter]
        public string RequestId { get; set; }

        [Parameter]
        public OperatioType OperationTypeTicket { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IAdoProjectDataService AdoProjectDataService { get; set; }

        [Inject]
        public IProjectAdoServices ProjectAdoServices { get; set; }

        [Inject]
        public ICsroDialogService CsroDialogService { get; set; }

        [Inject]
        public IProcessAdoServices ProcessAdoServices { get; set; }

        [Inject]
        public ILogger<AdoProjectBase> Logger { get; set; }

        #endregion

        public ProjectAdo Model { get; set; } = new ProjectAdo { Status = Status.Draft };
        protected bool IsReadOnly => OperationTypeTicket == OperatioType.View || 
            (OperationTypeTicket == OperatioType.Edit && Model.Status > Status.Submitted);
        protected string Title => OperationTypeTicket.ToString() + " Project";

        protected List<string> Processes = new List<string>();

        protected List<string> Organizations = new List<string>();

        protected async override Task OnInitializedAsync()
        {
            await Load();
        }

        private async Task Load()
        {
            try
            {
                ShowLoading();

                var prs = await ProcessAdoServices.GetAdoProcessesName(null);
                Processes.Clear();
                if (prs != null)
                    Processes = prs;

                var orgs = await ProcessAdoServices.GetOrganizationNames();
                Organizations.Clear();
                if (orgs != null)
                    Organizations = orgs;

                if (OperationTypeTicket != OperatioType.Create)
                {
                    Model.Id = int.Parse(RequestId);
                    var server = await AdoProjectDataService.GetItemByIdAsync(Model.Id);
                    if (server != null)
                        Model = server;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(OnInitializedAsync));
            }
            HideLoading();
        }

        //public Task OnOrganizationChanged(string value)
        //{
        //    if (value != null)
        //        Model.Organization = value;

        //    return Task.CompletedTask;
        //}

        //public Task OnProcessNameChanged(string value)
        //{
        //    if (value != null)
        //        Model.ProcessName = value;

        //    return Task.CompletedTask;
        //}

        public async Task OnValidSubmit(EditContext context)
        {
            ShowProcessing();
            var valid = context.Validate();
            if (valid)
            {
                try
                {
                    await Task.Delay(1 * 1000); // todo fix workaround
                    valid = context.Validate();

                    if (valid && OperationTypeTicket == OperatioType.Create)
                    {
                        Model.Status = Status.Submitted;
                        var added = await AdoProjectDataService.AddItemAsync(Model);
                        if (added != null)
                        {
                            Success = true;
                            Model = added;
                        }
                    }
                    else if (OperationTypeTicket == OperatioType.Edit)
                    {
                        var updated = await AdoProjectDataService.UpdateItemAsync(Model);
                        if (updated)
                        {
                            Success = true;
                        }
                        else
                        {
                            var ok = await CsroDialogService.ShowWarning("Update Error", $"Conflic Detected, Please refresh and try again", "Refresh");
                            if (ok)
                                await Load();
                        }
                    }
                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, nameof(OnValidSubmit));
                    await CsroDialogService.ShowError("Error", $"Detail error: {ex.Message}");
                }

            }
            HideLoading();
        }

        public async Task SaveAsDraftAsync()
        {
            try
            {
                ShowLoading("Saving...");
                Model.Status = Status.Draft;
                var saved = await AdoProjectDataService.AddItemAsync(Model);
                if (saved != null)
                    await CsroDialogService.ShowMessage("Success", $"Request was saved.");

                //await Load();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(OnInitializedAsync));
                await CsroDialogService.ShowError("Error", $"Detail error: {ex.Message}");
            }
            HideLoading();
        }

        public void GoBack()
        {
            NavigationManager.NavigateTo("/ado/AdoRequestsList");
        }

    }
}

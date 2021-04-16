using CSRO.Client.Blazor.UI;
using CSRO.Client.Blazor.UI.Services;
using CSRO.Client.Services;
using CSRO.Client.Services.Models;
using CSRO.Common.AdoServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Client.Blazor.WebApp.Components.Ado
{
    public class AdoProjectAccessBase : CsroComponentBase
    {
        #region Params and Injects

        [Parameter]
        public OperatioTypeIdPair OperatioTypeIdPair { get; set; } = new();

        /// <summary>
        /// Tags compenents example depands on sub Id
        /// </summary>
        [Parameter]
        public EventCallback<bool> SavedAdoProjectAccess { get; set; }

        //[Parameter]
        //public string RequestId { get; set; }

        //[Parameter]
        //public OperatioType OperationTypeTicket { get; set; }

        public string RequestId => OperatioTypeIdPair.Id;
        public OperatioType OperationTypeTicket => OperatioTypeIdPair.OperatioTypeEnum;

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IAdoProjectAccessDataService AdoProjectAccessDataService { get; set; }

        [Inject]
        public IProjectAdoServices ProjectAdoServices { get; set; }

        [Inject]
        public ICsroDialogService CsroDialogService { get; set; }

        [Inject]
        public IProcessAdoServices ProcessAdoServices { get; set; }

        [Inject]
        public ILogger<AdoProjectAccessBase> Logger { get; set; }

        #endregion

        public AdoProjectAccessModel Model { get; set; } = new AdoProjectAccessModel { Status = Status.Draft };
        //protected bool IsReadOnly => OperationTypeTicket == OperatioType.View; //for testing
        protected bool IsReadOnly => OperationTypeTicket == OperatioType.View || (OperationTypeTicket == OperatioType.Edit && Model.Status > Status.Submitted);
        protected string Title => OperationTypeTicket.ToString() + " Access to Project";

        protected List<string> Organizations = new();
        protected List<string> ProjectNames = new();


        protected async override Task OnInitializedAsync()
        {
            await Load();
        }

        protected override async Task OnParametersSetAsync()
        {            
            await base.OnParametersSetAsync();
            if (string.IsNullOrWhiteSpace(RequestId))
            {
                Model = new();
                return;
            }
            else
                await Load();            
        }

        private async Task Load()
        {
            try
            {
                ShowLoading();
                Organizations.Clear();
                ProjectNames.Clear();

                var orgs = await ProcessAdoServices.GetOrganizationNames();
                if (orgs != null)
                    Organizations = orgs;

                if (OperationTypeTicket != OperatioType.Create)
                {
                    Model.Id = int.Parse(RequestId);
                    var server = await AdoProjectAccessDataService.GetItemByIdAsync(Model.Id);
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

        public async Task OnOrganizationChanged(string value)
        {
            ShowLoading();

            if (value != null)
                Model.Organization = value;

            ProjectNames?.Clear();
            var prs = await ProjectAdoServices.GetProjectNames(Model.Organization);
            if (prs.HasAnyInCollection())
                ProjectNames = prs;

            HideLoading();
        }

        public async Task OnValidSubmit(EditContext context)
        {
            ShowProcessing();
            var valid = context.Validate();
            if (valid)
            {
                try
                {                                       
                    if (OperationTypeTicket == OperatioType.Create)
                    {
                        Model.Status = Status.Submitted;
                        var added = await AdoProjectAccessDataService.AddItemAsync(Model);
                        if (added != null)
                        {
                            await SavedAdoProjectAccess.InvokeAsync(true);
                            Success = true;
                            Model = added;
                        }
                    }
                    else if (OperationTypeTicket == OperatioType.Edit)
                    {
                        var updated = await AdoProjectAccessDataService.UpdateItemAsync(Model);
                        if (updated)
                        {
                            await SavedAdoProjectAccess.InvokeAsync(true);
                            await CsroDialogService.ShowMessage("Success", $"Update Finished", "Refresh");
                            await Load();
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

        public void GoBack()
        {
            NavigationManager.NavigateTo("/ado/AdoRequestsList");
        }

    }
}

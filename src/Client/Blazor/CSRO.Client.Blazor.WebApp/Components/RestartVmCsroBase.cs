using CSRO.Client.Blazor.UI;
using CSRO.Client.Blazor.UI.Services;
using CSRO.Client.Core.Models;
using CSRO.Client.Services;
using CSRO.Client.Services.AzureRestServices;
using CSRO.Client.Services.Models;
using CSRO.Common.AzureSdkServices;
using CSRO.Common.AzureSdkServices.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Client.Blazor.WebApp.Components
{
    public class RestartVmCsroBase : CsroComponentBase
    {
        #region Params and Injects

        [Parameter]
        public string TicketId { get; set; }

        [Parameter]
        public OperatioType OperationTypeTicket { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IVmTicketDataService VmTicketDataService { get; set; }

        [Inject]
        public IVmService VmService { get; set; }

        [Inject]
        public ISubcriptionDataService SubcriptionDataService { get; set; }

        [Inject]
        public IResourceGroupService ResourceGroupervice { get; set; }

        [Inject]
        public ICsroDialogService CsroDialogService { get; set; }

        [Inject]
        public ILocationsService LocationsService { get; set; }

        [Inject]
        public IVmSdkService  AzureSdkService { get; set; }

        [Inject]
        public ILogger<RestartVmCsroBase> Logger { get; set; }

        #endregion

        protected VmTicket Model { get; set; } = new VmTicket();

        protected bool IsReadOnly => OperationTypeTicket == OperatioType.View;
        protected string Title => OperationTypeTicket == OperatioType.Create ? "Request Vm Restart" : $"View {Model.Status} of {Model.VmName}";
        protected List<IdName> Subscripions { get; set; }        
        protected List<string> ResourceGroups { get; set; } = new List<string>();
        protected List<string> Vms { get; set; } = new List<string>();

        protected bool IsRgDisabled => ResourceGroups?.Count == 0;
        protected bool IsVmDisabled => OperationTypeTicket != OperatioType.Create || IsRgDisabled || string.IsNullOrWhiteSpace(Model?.ResorceGroup);

        protected string LastVmStatus = null;

        protected async override Task OnInitializedAsync()
        {            
            await Load();
        }

        async Task LoadRg(string subcriptionId)
        {
            Model.ResorceGroup = null;
            Model.VmName = null;
            ResourceGroups.Clear();
            var rgs = await ResourceGroupervice.GetResourceGroups(subcriptionId);
            if (rgs != null)
            {
                ResourceGroups.AddRange(rgs.Select(a => a.Name));
                StateHasChanged();
            }
        }

        public async Task OnSubscriptionChanged(IdName value)
        {
            if (value != null)
            {
                Model.SubcriptionId = value.Id;
                Model.SubcriptionName = value.Name;                
                LastVmStatus = null;

                ShowLoading();
                await LoadRg(value.Id);
                HideLoading();
            }
        }

        public async Task OnRgChanged(string value)
        {
            if (value != null)
            {
                Model.ResorceGroup = value;
                Model.VmName = null;
                LastVmStatus = null;

                ShowLoading();
                var vms = await VmService.GetVmNames(Model.SubcriptionId, Model.ResorceGroup);
                Vms = vms ?? new List<string>();


                HideLoading();
            }
        }

        public async Task OnVmSelected(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            Model.VmName = value;
            LastVmStatus = "Loading...";
            ShowLoading();
            var status = await AzureSdkService.GetStatus(Model.SubcriptionId, Model.ResorceGroup, Model.VmName).ConfigureAwait(false);
            if (status != null)
                LastVmStatus = status.DisplayStatus;
            HideLoading();
        }

        private async Task Load()
        {
            try
            {
                //var loc = await LocationsService.GetLocations();

                if (OperationTypeTicket != OperatioType.Create)
                {
                    ShowLoading();                    

                    Model.Id = int.Parse(TicketId);
                    var server = await VmTicketDataService.GetItemByIdAsync(Model.Id);
                    await LoadRg(server?.SubcriptionId);
                    //var subName = await SubcriptionService.GetSubcription(server?.SubcriptionId);
                    //var rgName = await ResourceGroupervice.GetResourceGroupsIdName(server?.SubcriptionId);

                    if (server != null)
                    {
                        Model = server;
                        if (OperationTypeTicket == OperatioType.View)
                        {
                            #region Verify old Code
                            //if (Model.VmState == "Restart Started" || !string.Equals(Model.VmState, "VM running"))
                            //{
                            //    //need to create delay to update vm state after restart                                                                       
                            //    LoadingMessage = $"Current state: {Model.VmState}";
                            //    StateHasChanged();

                            //    var running = await VmTicketDataService.VerifyRestartStatusCallback(Model, (status)=> 
                            //    {
                            //        LoadingMessage = $"Current state: {status}";
                            //        StateHasChanged();
                            //    }).ConfigureAwait(false);
                            //    if (running)
                            //    {
                            //        Model = await VmTicketDataService.GetItemByIdAsync(Model.Id);                                    
                            //    }
                            //}
                            #endregion
                        }
                    }
                }
                
                else
                {
                    ShowLoading();
                    Subscripions = await SubcriptionDataService.GetSubcriptions(); 
                    
                    Subscripions = Subscripions ?? new List<IdName>();
#if DEBUG
                    if (Subscripions?.Count == 1)
                    {
                        var id = Subscripions.First().Id;
                        Subscripions.Add(new IdName(id, $"fake-sub-prod"));
                        Subscripions.Add(new IdName(id, $"fake-sub-uat"));
                        Subscripions.Add(new IdName(id, $"fake-sub-dev"));
                        Subscripions.Add(new IdName(id, $"fake-sub-appdev"));
                        Subscripions.Add(new IdName(id, $"fake-sub-test"));
                    }
#endif
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(OnInitializedAsync));                
            }
            finally
            {
                HideLoading();
            }
        }

        public async Task<IEnumerable<IdName>> SearchSubs(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            await Task.Delay(50);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return Subscripions;

            return Subscripions == null ? null : Subscripions.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task OnValidSubmit(EditContext context)
        {
            var valid = context.Validate();
            if (valid)
            {
                try
                {
                    //var testData = await AzureSdkService.TryGetData(Model.SubcriptionId, Model.ResorceGroup, Model.VmName);                                                            
                    //var sdkSubs = await AzureSdkService.GetAllSubcriptions(Model.SubcriptionId);
                    //return;

                    if (OperationTypeTicket == OperatioType.Create)
                    {
                        ShowLoading("Creating request");

                        var added = await VmTicketDataService.AddItemAsync(Model);
                        //var added = await VmTicketDataService.CreateVmTicketAndWaitForConfirmation(Model);
                        if (added != null)
                        {
                            Success = true;
                            Model = added;

                            ShowLoading("Success, please wait");
                            StateHasChanged();
                            await Task.Delay(3 * 1000);
                            HideLoading();

                            NavigationManager.NavigateTo($"vm/restart/view/{Model.Id}");
                        }
                    }
                    else if (OperationTypeTicket == OperatioType.Edit)
                    {
                        ShowLoading("Updating request");

                        var updated = await VmTicketDataService.UpdateItemAsync(Model);
                        if (updated)
                        {
                            Success = true;
                            //throw new Exception("Fake Test exception");
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
                finally
                {
                    HideLoading();
                }
            }
        }

        public void GoBack()
        {
            NavigationManager.NavigateTo("vm");
        }

    }
}

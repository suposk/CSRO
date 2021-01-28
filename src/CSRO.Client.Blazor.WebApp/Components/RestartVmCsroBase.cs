using CSRO.Client.Blazor.UI;
using CSRO.Client.Blazor.UI.Services;
using CSRO.Client.Core.Models;
using CSRO.Client.Services;
using CSRO.Client.Services.Models;
using CSRO.Common.AzureSdkServices;
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
        public ISubscriptionSdkService SubcriptionSdkService { get; set; }

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

        protected async override Task OnInitializedAsync()
        {            
            await Load();
        }

        async Task LoadRg(string subcriptionId)
        {
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
                ShowLoading();
                var vms = await VmService.GetVmNames(Model.SubcriptionId, Model.ResorceGroup);
                Vms = vms ?? new List<string>();


                HideLoading();
            }
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
                            if (Model.VmState == "Restart Started" || !string.Equals(Model.VmState, "VM running"))
                            {
                                //need to create delay to update vm state after restart                                                                       
                                LoadingMessage = $"Current state: {Model.VmState}";
                                StateHasChanged();

                                var running = await VmTicketDataService.VerifyRestartStatusCallback(Model, (status)=> 
                                {
                                    LoadingMessage = $"Current state: {status}";
                                    StateHasChanged();
                                }).ConfigureAwait(false);
                                if (running)
                                {
                                    Model = await VmTicketDataService.GetItemByIdAsync(Model.Id);                                    
                                }
                            }

                        }
                    }
                }
                
                else
                {
                    ShowLoading();
                    Subscripions = await SubcriptionSdkService.GetAllSubcriptions();
                    Subscripions = Subscripions ?? new List<IdName>();

                    #if DEBUG

                    //dubug only
                    //Model.SubcriptionId = "33fb38df-688e-4ca1-8dd8-b46e26262ff8";
                    if (Subscripions?.Count == 1)
                    {
                        for (int i=1; i <= 3; i++)
                        {
                            Subscripions.Add(new IdName(Guid.NewGuid().ToString(), $"fake sub name {i}"));
                        }
                    }
                    //Model.ResorceGroup = "dev-VMS";
                    //Model.VmName = "VmDelete";

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
                        if (added != null)
                        {
                            Success = true;
                            Model = added;

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

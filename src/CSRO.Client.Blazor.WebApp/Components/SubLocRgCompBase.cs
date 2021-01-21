﻿using CSRO.Client.Blazor.UI;
using CSRO.Client.Blazor.UI.Services;
using CSRO.Client.Services;
using CSRO.Client.Services.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using MudBlazor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CSRO.Client.Blazor.WebApp.Components
{ 
    public class SubLocRgCompBase : CsroComponentBase
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
        public ISubcriptionService SubcriptionService { get; set; }

        [Inject]
        public IResourceGroupervice ResourceGroupervice { get; set; }

        [Inject]
        public ICsroDialogService CsroDialogService { get; set; }

        [Inject]
        public ILocationsService LocationsService { get; set; }

        [Inject]
        public ILogger<SubLocRgCompBase> Logger { get; set; }

        #endregion

        protected ResourceGroupModel Model { get; set; } = new ResourceGroupModel();

        protected bool IsReadOnly => OperationTypeTicket == OperatioType.View;

        //protected string Title => OperationTypeTicket == OperatioType.Create ? "Request Vm Restart" : $"View {Model.Status} of {Model.VmName}";

        protected string Title => "Hosting Settings";

        protected List<IdName> Subscripions { get; set; } = new List<IdName>();
        protected List<IdName> Locations { get; set; } = new List<IdName>();
        protected List<string> ResourceGroups { get; set; } = new List<string>();

        
        protected bool IsLocDisabled => string.IsNullOrWhiteSpace(Model?.SubcriptionId) || Locations?.Count == 0;
        protected bool IsRgDisabled => IsLocDisabled | ResourceGroups?.Count == 0;
        protected bool IsNewRgDisabled => IsLocDisabled | string.IsNullOrWhiteSpace(Model.Location);

        protected async override Task OnInitializedAsync()
        {
            await Load();
        }

        public async Task OnSubscriptionChanged(IdName value)
        {
            if (value != null)
            {
                Model.SubcriptionId = value.Id;
                Model.SubcriptionName = value.Name;

                ShowLoading();
                await LoadLocations();
                //var tags = await SubcriptionService.GetTags(Model.SubcriptionId);
                //var defaulTags = await SubcriptionService.GetDefualtTags(Model.SubcriptionId);
                HideLoading();
            }
        }

        public async Task OnLocationChanged(IdName value)
        {
            if (value != null)
            {
                //Model.ResourceGroup.Location = value.Id;                
                Model.LocationIdName = value;

                ShowLoading();
                await LoadRg(Model.SubcriptionId, value.Id);
                HideLoading();
            }
        }

        async Task LoadLocations()
        {
            if (Locations?.Count == 0)
                Locations = await LocationsService.GetLocations();
        }

        async Task LoadRg(string subcriptionId, string location)
        {
            ResourceGroups?.Clear();
            var rgs = await ResourceGroupervice.GetResourceGroups(subcriptionId, location);
            if (rgs != null)
            {
                ResourceGroups.AddRange(rgs.Select(a => a.Name));                
            }
            StateHasChanged();
        }

        private async Task Load()
        {
            try
            {
                //var loc = await LocationsService.GetLocations();

                if (OperationTypeTicket == OperatioType.Create)
                {
                    ShowLoading();
                    Subscripions?.Clear();
                    Subscripions = await SubcriptionService.GetSubcriptions();

#if DEBUG

                    //dubug only
                    //Model.SubcriptionId = "33fb38df-688e-4ca1-8dd8-b46e26262ff8";
                    if (Subscripions?.Count == 1)
                    {
                        for (int i = 1; i <= 3; i++)
                        {
                            Subscripions.Add(new IdName(Guid.NewGuid().ToString(), $"fake sub name {i}"));
                        }
                    }
                    //Model.ResorceGroup = "dev-VMS";
                    //Model.VmName = "VmDelete";

#endif
                }
                else
                {


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
                    if (OperationTypeTicket == OperatioType.Create)
                    {
                        ShowLoading("Creating Resorce Group");

                        var added = await ResourceGroupervice.CreateRgAsync(Model);
                        if (added != null)
                        {
                            //TODO fix workaround
                            string copyAdded = added.ResourceGroup.Name;
                            await Task.Delay(1 * 100);
                            ResourceGroups.Add(copyAdded);
                            Model.ResourceGroup.Name = ResourceGroups.FirstOrDefault();
                            StateHasChanged();
                            await Task.Delay(1 * 10);                            
                            Model.ResourceGroup.Name = copyAdded;
                            await Task.Delay(1 * 10);
                        }
                    }
                    UI.Helpers.EditFormExtensions.ClearValidationMessages(context);
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
            NavigationManager.NavigateTo("/");
        }

    }
}

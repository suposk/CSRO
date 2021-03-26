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

namespace CSRO.Client.Blazor.WebApp.Pages.Customers
{
    public class CustomerCodesBase : CsroComponentBase
    {

        #region Params and Injects

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public ISubscriptionSdkService SubcriptionSdkService { get; set; }

        [Inject]
        public ISubcriptionService SubcriptionService { get; set; }

        [Inject]
        public IResourceGroupService ResourceGroupervice { get; set; }

        [Inject]
        public ICsroDialogService CsroDialogService { get; set; }

        [Inject]
        public ILocationsService LocationsService { get; set; }

        [Inject]
        public ILogger<CustomerCodesBase> Logger { get; set; }

        //[Inject]
        //public IAdService AdService { get; set; }

        #endregion

        protected EditContext editContext { get; private set; }

        protected ResourceGroupModel Model { get; set; } = new ResourceGroupModel();

        //protected string Title => "Hosting Settings";

        protected List<IdNameSdk> Subscripions { get; set; } = new List<IdNameSdk>();
        protected List<IdName> Locations { get; set; } = new List<IdName>();
        protected List<string> ResourceGroups { get; set; } = new List<string>();


        protected bool IsLocDisabled => string.IsNullOrWhiteSpace(Model?.SubcriptionId) || Locations?.Count == 0;
        protected bool IsRgDisabled => IsLocDisabled | ResourceGroups?.Count == 0;
        protected bool IsNewRgDisabled => IsLocDisabled | string.IsNullOrWhiteSpace(Model.Location);

        protected IdNameSdk SelectedSub { get; set; } = new();
        protected HashSet<IdNameSdk> SelectedSubs { get; set; } = new();

        protected async override Task OnInitializedAsync()
        {
            editContext ??= new EditContext(Model);
            await Load();
        }


        public async Task OnSubscriptionValueChanged(IdNameSdk value)
        {
            if (value != null)
            {
                Model.SubcriptionId = value.Id;
                Model.SubcriptionName = value.Name;

                ShowLoading();

                //await SubcriptionIdChanged.InvokeAsync(Model.SubcriptionId);
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
                //await LocationIdNameChanged.InvokeAsync(value);
                await LoadRg(Model.SubcriptionId, value.Id);
                HideLoading();
            }
        }

        async Task LoadLocations()
        {
            if (Locations?.Count == 0)
                Locations = await LocationsService.GetLocations();
        }

        public async Task Search()
        {
            try
            {
                ShowLoading();
                //var tags = await SubcriptionService.GetTags(new List<string> { "33fb38df-688e-4ca1-8dd8-b46e26262ff8" });
                var tags = await SubcriptionService.GetTags(SelectedSubs.Select(a => a.Id).ToList());
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(OnInitializedAsync));
                await CsroDialogService.ShowError("Error", $"Detail error: {ex.Message}");
            }
            HideLoading();
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
                ShowLoading();

                //var tags = SubcriptionSdkService.GetTags(new List<string> { "33fb38df-688e-4ca1-8dd8-b46e26262ff8", "634e6b93-264e-44f0-9e87-3606169fee2f" });
                //var tags = await SubcriptionSdkService.GetTags(new List<string> { "33fb38df-688e-4ca1-8dd8-b46e26262ff8" });
                //var tags = await SubcriptionService.GetTags(new List<string> { "33fb38df-688e-4ca1-8dd8-b46e26262ff8" });

                Subscripions?.Clear();
                Subscripions = await SubcriptionSdkService.GetAllSubcriptions();
                //var restSubs = await SubcriptionService.GetSubcriptions();
                if (Subscripions == null || Subscripions.Count == 0)
                {
                    var other = await SubcriptionService.GetSubcriptions();
                    if (other?.Count > 0)
                    {
                        await CsroDialogService.ShowWarning("Info", "sdk method found no subs");
                        List<IdNameSdk> list = new List<IdNameSdk>();
                        other.ForEach(a => list.Add(new IdNameSdk { Id = a.Id, Name = a.Name }));
                        Subscripions = list;
                    }
                }

#if DEBUG

                //dubug only
                //Model.SubcriptionId = "33fb38df-688e-4ca1-8dd8-b46e26262ff8";
                if (Subscripions?.Count == 1)
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        Subscripions.Add(new IdNameSdk(Guid.NewGuid().ToString(), $"fake sub name {i}"));
                    }
                }
                //Model.ResorceGroup = "dev-VMS";
                //Model.VmName = "VmDelete";

#endif
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

        public async Task<IEnumerable<IdNameSdk>> SearchSubs(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            await Task.Delay(50);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return Subscripions;

            return Subscripions == null ? null : Subscripions.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        public void GoBack()
        {
            NavigationManager.NavigateTo("/");
        }
    }
}

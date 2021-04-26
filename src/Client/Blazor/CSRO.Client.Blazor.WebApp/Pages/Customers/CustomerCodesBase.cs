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
using System.Threading;
using System.Threading.Tasks;

namespace CSRO.Client.Blazor.WebApp.Pages.Customers
{
    public enum CustomerSearchTypeEnum { None, Sub, AtCode, Regions, Env }

    public class CustomerCodesBase : CsroComponentBase
    {

        #region Params and Injects

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public ISubcriptionDataService SubcriptionDataService { get; set; }

        [Inject]
        public ICustomerDataService CustomerDataService { get; set; }

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

        protected ResourceGroupModel Model { get; set; } = new();

        //protected string Title => "Hosting Settings";

        protected List<IdName> Subscripions { get; set; } = new();
        protected List<IdName> SubscripionsFiltered { get; set; } = new();

        protected List<Customer> Customers = new();

        List<Customer> _customersCache = new();


        protected List<IdName> Locations { get; set; } = new();
        protected List<string> ResourceGroups { get; set; } = new();


        protected bool IsLocDisabled => string.IsNullOrWhiteSpace(Model?.SubcriptionId) || Locations?.Count == 0;
        protected bool IsRgDisabled => IsLocDisabled | ResourceGroups?.Count == 0;
        protected bool IsNewRgDisabled => IsLocDisabled | string.IsNullOrWhiteSpace(Model.Location);
                
        private IdName _SelectedSub;

        public IdName SelectedSub
        {
            get { return _SelectedSub; }
            set {
                _SelectedSub = value;
                if (value != null && !string.IsNullOrWhiteSpace(value.Name))
                    SetSearchType(CustomerSearchTypeEnum.Sub);
            }
        }
                
        private HashSet<IdName> _SelectedSubs = new();

        public HashSet<IdName> SelectedSubs
        {
            get { return _SelectedSubs; }
            set 
            {
                _SelectedSubs = value; 
                //if (value.IsNullOrEmptyCollection())
                //    SetSearchType(CustomerSearchTypeEnum.Sub);
            }
        }


        //protected HashSet<IdName> SelectedRegions { get; set; } = new();
        private HashSet<IdName> _SelectedRegions = new();

        public HashSet<IdName> SelectedRegions
        {
            get { return _SelectedRegions; }
            set 
            {
                _SelectedRegions = value;
                if (value.HasAnyInCollection())
                    SetSearchType(CustomerSearchTypeEnum.Regions);
            }
        }


        protected bool IsFilterAutofocused { get; set; } = false;

        protected CustomerSearchTypeEnum CustomerSearchType { get; set; } = CustomerSearchTypeEnum.None;


        //public string AtCode { get; set; } = null;
        private string _AtCode;

        public string AtCode
        {
            get { return _AtCode; }
            set 
            {
                _AtCode = value;
                if (!string.IsNullOrWhiteSpace(value))
                    SetSearchType(CustomerSearchTypeEnum.AtCode);
                InvokeAsync(() => OnAtCodeChanged());
            }
        }


        protected async override Task OnInitializedAsync()
        {
            editContext ??= new EditContext(Model);
            await Load();
        }

        void SetSearchType(CustomerSearchTypeEnum customerSearchType)
        {
            switch (customerSearchType)
            {
                case CustomerSearchTypeEnum.AtCode:
                    SelectedSub = null; 
                    SelectedRegions = new();
                    //AtCode = null;
                    break;
                case CustomerSearchTypeEnum.None:
                    SelectedSub = null;
                    SelectedRegions = new();
                    AtCode = null;
                    break;
                case CustomerSearchTypeEnum.Sub:
                    //SelectedSub = null;
                    SelectedRegions = new();
                    AtCode = null;
                    break;
                case CustomerSearchTypeEnum.Regions:
                    SelectedSub = null;
                    //SelectedRegions = new();
                    AtCode = null;
                    break;
                case CustomerSearchTypeEnum.Env:
                    break;
            }
            CustomerSearchType = customerSearchType;
        }

        //public async Task OnSubscriptionValueChanged(IdName value)
        //{
        //    SelectedSub = value;
        //}

        //public async Task OnLocationChanged(IdName value)
        //{
        //    if (value != null)
        //    {
        //        //Model.ResourceGroup.Location = value.Id;                
        //        Model.LocationIdName = value;

        //        ShowLoading();
        //        //await LocationIdNameChanged.InvokeAsync(value);
        //        await LoadRg(Model.SubcriptionId, value.Id);
        //        HideLoading();
        //    }
        //}

        public Task OnAtCodeChanged()
        {            
            return Task.CompletedTask;
        }

        public Task OnSubscriptionsChanged(HashSet<IdName> values)
        {
            if (values.HasAnyInCollection())
            {
                SelectedRegions?.Clear();
                SelectedSubs = values;
            }
            return Task.CompletedTask;
        }

        public Task OnLocationsChanged(HashSet<IdName> values)
        {
            if (values.HasAnyInCollection())
            {
                SelectedRegions = values;
                SelectedSubs?.Clear();                
            }
            return Task.CompletedTask;
        }

        async Task LoadLocations()
        {
            if (Locations.IsNullOrEmptyCollection())
                Locations = await LocationsService.GetLocations();
        }

        public async Task Search()
        {
            try
            {
                Customers.Clear();
                _customersCache.Clear();
                ShowLoading("Please wait ...");

                await Task.Delay(1);
                List<Customer> customers = null;

                switch (CustomerSearchType)
                {
                    case CustomerSearchTypeEnum.AtCode:
                        customers = await CustomerDataService.GetCustomersByAtCode(AtCode).ConfigureAwait(false);
                        break;
                    case CustomerSearchTypeEnum.Sub:
                        customers = await CustomerDataService.GetCustomersBySubName(SelectedSub?.Name).ConfigureAwait(false);
                        //customers = await CustomerDataService.GetCustomersBySubId(SelectedSub?.Id).ConfigureAwait(false);
                        break;
                    case CustomerSearchTypeEnum.Regions:
                        customers = await CustomerDataService.GetCustomersByRegions(SelectedRegions.Select(a => a.Name).ToList()).ConfigureAwait(false);
                        break;
                    case CustomerSearchTypeEnum.Env:
                        break;
                }

                if (customers.HasAnyInCollection())
                {                    
                    foreach (var cust in customers)
                    {
                        var sub = Subscripions.FirstOrDefault(a => a.Id == cust.SubscriptionId);
                        if (sub != null && string.IsNullOrWhiteSpace(cust.SubscriptionName))
                        {
                            //only if sub name is missing
                            cust.SubscriptionName = sub.Name;
                        }                                                                          
                        _customersCache.Add(cust);
                    }
                    //open first sub
                    var first = _customersCache.FirstOrDefault();
                    if (first != null)
                        first.ShowDetails = true;
                    Customers = _customersCache;
                }
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
                Subscripions?.Clear();
                SubscripionsFiltered?.Clear();
                SelectedSubs?.Clear();
                SelectedRegions?.Clear();
                await LoadLocations();

                //Subscripions = await SubcriptionSdkService.GetAllSubcriptions();   not working properly
                if (Subscripions.IsNullOrEmptyCollection() || Subscripions.Count <= 10)
                {
                    //var restSubs = await SubcriptionService.GetSubcriptions();
                    var restSubs = await SubcriptionDataService.GetSubcriptions();
                    if (restSubs?.Count > 0)                                            
                        Subscripions = restSubs;                    
                }
                SubscripionsFiltered = Subscripions;
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
#endif

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(OnInitializedAsync));
                await CsroDialogService.ShowError("Error", $"Detail error: {ex.Message}");
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
                        
            IsFilterAutofocused = false;
            SubscripionsFiltered = Subscripions.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (SubscripionsFiltered.IsNullOrEmptyCollection())
                SubscripionsFiltered = Subscripions;
            else
                IsFilterAutofocused = true;
            return Subscripions.IsNullOrEmptyCollection() ? null : SubscripionsFiltered;
        }

        public void ShowBtnPress(string id)
        {
            Customer tmpCust = Customers.FirstOrDefault(f => f.SubscriptionId == id);
            if (tmpCust != null)
                tmpCust.ShowDetails = !tmpCust.ShowDetails;
        }

        public void GoBack()
        {
            NavigationManager.NavigateTo("/");
        }
    }
}

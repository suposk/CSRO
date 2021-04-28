﻿using CSRO.Client.Blazor.UI;
using CSRO.Client.Blazor.UI.Services;
using CSRO.Client.Core.Models;
using CSRO.Client.Services;
using CSRO.Client.Services.AzureRestServices;
using CSRO.Client.Services.Models;
using CSRO.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CSRO.Client.Blazor.WebApp.Pages.Customers
{
    public enum CustomerSearchTypeEnum  { None, Regions, SelectedSubs, SelectedAtcodes }

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

        [Inject]
        public ICsvExporter CsvExporter { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        //[Inject]
        //public IAdService AdService { get; set; }

        #endregion


        #region Properties

        protected EditContext editContext { get; private set; }

        protected ResourceGroupModel Model { get; set; } = new();

        //protected string Title => "Hosting Settings";

        protected List<IdName> Subscripions { get; set; } = new();
        protected List<IdName> SubscripionsFiltered { get; set; } = new();

        protected List<Customer> Customers = new();

        List<Customer> _customersCache = new();


        protected List<IdName> Locations { get; set; } = new();

        protected List<string> AtCodesList { get; set; } = new();
        protected List<string> AtCodesListFiltered { get; set; } = new();


        protected List<string> ResourceGroups { get; set; } = new();


        protected bool IsLocDisabled => string.IsNullOrWhiteSpace(Model?.SubcriptionId) || Locations?.Count == 0;
        protected bool IsRgDisabled => IsLocDisabled | ResourceGroups?.Count == 0;
        protected bool IsNewRgDisabled => IsLocDisabled | string.IsNullOrWhiteSpace(Model.Location);
        protected Dictionary<string, string> ColumnsToDisplay = new();
                
        private bool _IsSimpleView = true;

        public bool IsSimpleView
        {
            get { return _IsSimpleView; }
            set 
            {
                _IsSimpleView = value;
                SetColumns(true);
            }
        }
                
        private HashSet<IdName> _SelectedSubs = new();

        public HashSet<IdName> SelectedSubs
        {
            get { return _SelectedSubs; }
            set 
            {
                _SelectedSubs = value;
                if (value.HasAnyInCollection())
                    SetSearchType(CustomerSearchTypeEnum.SelectedSubs);
            }
        }

        private HashSet<string> _SelectedAtCodes = new();

        public HashSet<string> SelectedAtCodes
        {
            get { return _SelectedAtCodes; }
            set 
            {
                _SelectedAtCodes = value;
                if (value.HasAnyInCollection())
                    SetSearchType(CustomerSearchTypeEnum.SelectedAtcodes);
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

                
        //private string _AtCode;

        //public string AtCode
        //{
        //    get { return _AtCode; }
        //    set 
        //    {
        //        _AtCode = value;
        //        if (!string.IsNullOrWhiteSpace(value))
        //            SetSearchType(CustomerSearchTypeEnum.AtCode);
        //        InvokeAsync(() => OnAtCodeChanged());
        //    }
        //}

        private string _SearchSubText;

        public string SearchSubText
        {
            get { return _SearchSubText; }
            set 
            {
                _SearchSubText = value;
                if (!string.IsNullOrWhiteSpace(value))
                    SetSearchType(CustomerSearchTypeEnum.SelectedSubs);                
                InvokeAsync(() => OnSearchSubTextChanged(value));                
            }
        }

        private string _SearchAtCode;

        public string SearchAtCode
        {
            get { return _SearchAtCode; }
            set 
            {
                _SearchAtCode = value;
                if (!string.IsNullOrWhiteSpace(value))
                    SetSearchType(CustomerSearchTypeEnum.SelectedAtcodes);
                InvokeAsync(() => OnSearchAtCodeChanged(value));
            }
        }

        #endregion

        public Task OnSearchSubTextChanged(string value)
        {
            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
            {
                SubscripionsFiltered = Subscripions;
                return Task.CompletedTask;
            }                        
            SubscripionsFiltered = Subscripions.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase)).ToList();
            return Task.CompletedTask;
        }

        public Task OnSearchAtCodeChanged(string value)
        {
            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
            {
                AtCodesListFiltered = AtCodesList;
                return Task.CompletedTask;
            }
            AtCodesListFiltered = AtCodesList.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase)).ToList();
            return Task.CompletedTask;
        }

        protected async override Task OnInitializedAsync()
        {
            editContext ??= new EditContext(Model);
            await LoadAsync();
        }

        void SetSearchType(CustomerSearchTypeEnum customerSearchType)
        {
            ClearSelectedCollections(customerSearchType);
            CustomerSearchType = customerSearchType;
        }

        public Task OnAtCodeChanged()
        {
            //StateHasChanged();
            return Task.CompletedTask;
        }

        void SetColumns(bool refresh)
        {
            ColumnsToDisplay.Clear();
            ColumnsToDisplay.Add(nameof(Customer.AtCode), nameof(Customer.AtCode));
            ColumnsToDisplay.Add(nameof(Customer.AtName), nameof(Customer.AtName));
            ColumnsToDisplay.Add(nameof(Customer.SubscriptionName), nameof(Customer.SubscriptionName));
            ColumnsToDisplay.Add(nameof(Customer.Email), nameof(Customer.Email));            
            if (!IsSimpleView)
            {
                ColumnsToDisplay.Add(nameof(Customer.AtSwc), nameof(Customer.AtSwc));
                ColumnsToDisplay.Add(nameof(Customer.OpEnvironment), nameof(Customer.OpEnvironment));
            }
            if (refresh)
                StateHasChanged();
        }

        public Task OnSubscriptionsChanged(HashSet<IdName> values)
        {
            if (values.HasAnyInCollection())
            {
                ClearSelectedCollections(CustomerSearchTypeEnum.SelectedSubs);
                SelectedSubs = values;
            }
            return Task.CompletedTask;
        }

        public Task OnAtCodesChanged(HashSet<string> values)
        {
            if (values.HasAnyInCollection())
            {
                ClearSelectedCollections(CustomerSearchTypeEnum.SelectedAtcodes);
                SelectedAtCodes = values;
            }
            return Task.CompletedTask;
        }

        public Task OnLocationsChanged(HashSet<IdName> values)
        {
            if (values.HasAnyInCollection())
            {
                ClearSelectedCollections(CustomerSearchTypeEnum.Regions);
                SelectedRegions = values;
            }
            return Task.CompletedTask;
        }

        private void ClearSelectedCollections(CustomerSearchTypeEnum type)
        {
            if (type != CustomerSearchTypeEnum.SelectedSubs)
                SelectedSubs?.Clear();
            if (type != CustomerSearchTypeEnum.SelectedAtcodes)
                SelectedAtCodes?.Clear();
            if (type != CustomerSearchTypeEnum.Regions)
                SelectedRegions?.Clear();
        }

        async Task LoadLocations()
        {
            if (Locations.IsNullOrEmptyCollection())
                Locations = await LocationsService.GetLocations();
        }

        protected async Task Export()
        {
            try
            {
                if (await JSRuntime.InvokeAsync<bool>("confirm", $"Do you want to export this list to Excel?"))
                {
                    var fileBytes = CsvExporter.ExportEventsToCsv(Customers);
                    var fileName = $"MyReport{DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)}.csv";
                    await JSRuntime.InvokeAsync<object>("saveAsFile", fileName, Convert.ToBase64String(fileBytes));
                }
            }
            catch (Exception ex)
            {
                await CsroDialogService.ShowError("Error", ex?.Message);
            }
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
                    case CustomerSearchTypeEnum.SelectedAtcodes:
                        customers = await CustomerDataService.GetCustomersByAtCodes(SelectedAtCodes.ToList()).ConfigureAwait(false);
                        break;
                    case CustomerSearchTypeEnum.SelectedSubs:
                        customers = await CustomerDataService.GetCustomersBySubNames(SelectedSubs.Select(a => a.Name).ToList()).ConfigureAwait(false);
                        break;
                    case CustomerSearchTypeEnum.Regions:
                        customers = await CustomerDataService.GetCustomersByRegions(SelectedRegions.Select(a => a.Name).ToList()).ConfigureAwait(false);
                        break;
                    //case CustomerSearchTypeEnum.Env:
                    //    break;
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
                    ////open first sub
                    //var first = _customersCache.FirstOrDefault();
                    //if (first != null)
                    //    first.ShowDetails = true;
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

        public async override Task LoadAsync()
        {
            try
            {
                ShowLoading();                
                SetColumns(false);


                Subscripions?.Clear();
                SubscripionsFiltered?.Clear();
                SelectedSubs?.Clear();
                SelectedRegions?.Clear();

                await LoadLocations();
                AtCodesList = await CustomerDataService.GetAtCodes();
                AtCodesListFiltered = AtCodesList;

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

        //public void ShowBtnPress(string id)
        //{
        //    Customer tmpCust = Customers.FirstOrDefault(f => f.SubscriptionId == id);
        //    if (tmpCust != null)
        //        tmpCust.ShowDetails = !tmpCust.ShowDetails;
        //}

        public void GoBack()
        {
            NavigationManager.NavigateTo("/");
        }
    }
}

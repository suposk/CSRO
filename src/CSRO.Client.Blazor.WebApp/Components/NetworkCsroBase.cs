using CSRO.Client.Blazor.UI;
using CSRO.Client.Blazor.UI.Services;
using CSRO.Client.Core.Models;
using CSRO.Client.Services;
using CSRO.Client.Services.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Client.Blazor.WebApp.Components
{
    public class NetworkCsroBase : CsroComponentBase
    {

        #region Params and Injects

        [Parameter]
        public bool IsValidationEnabled { get; set; }

        /// <summary>
        /// Passed via callback
        /// </summary>
        [Parameter]
        public string SubcriptionId { get; set; }

        [Parameter]
        public IdName LocationIdName { get; set; }

        [Parameter]
        public EventCallback<DefaultNetwork> OnNetworkSelectedEvent { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public INetworkService NetworkService { get; set; }

        [Inject]
        public ICsroDialogService CsroDialogService { get; set; }

        [Inject]
        public ILogger<NetworkCsroBase> Logger { get; set; }

        #endregion
        protected string _previosSubcriptionId;
        protected IdName _previosLocationIdName;
        protected DefaultNetwork Model { get; set; } = new DefaultNetwork();
        protected Networks Networks { get; set; } = new Networks();
        List<Network> _networksList;

        protected bool IsNetRgDisabled => string.IsNullOrWhiteSpace(SubcriptionId) | string.IsNullOrWhiteSpace(LocationIdName?.Id) | Networks.NetworkResourceGroupList?.Count == 0;
        protected bool IsVNetDisabled => IsNetRgDisabled | (Networks.VirtualNetworkList?.Count == 0);
        protected bool IsSubnetDisabled => IsVNetDisabled | (Networks.SubnetList?.Count == 0);
        protected bool IsReadOnly => false;

        protected string Title => "Select Network Settings";

        //protected async override Task OnInitializedAsync()
        //{
        //    await Load();
        //}

        protected async override Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if ((string.IsNullOrWhiteSpace(SubcriptionId) || string.Equals(SubcriptionId, _previosSubcriptionId)) &&
                (LocationIdName == null || LocationIdName == _previosLocationIdName))
                return;
            else
            {
                _previosSubcriptionId = SubcriptionId;
                _previosLocationIdName = LocationIdName;
            }      
            if (!string.IsNullOrWhiteSpace(SubcriptionId) && !string.IsNullOrWhiteSpace(LocationIdName?.Id))
                await Load();
        }

        private async Task Load()
        {
            try
            {
                ShowLoading();
                Model = new DefaultNetwork();

                _networksList = await NetworkService.GetNetworks(SubcriptionId, LocationIdName.Id);
                Networks = new Networks();
                foreach (var item in _networksList)
                {
                    _networksList.ForEach(a => Networks.NetworkResourceGroupList.Add( a.NetworkResourceGroup));
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

        public async Task OnNetworkResourceGroupChanged(string value)
        {
            if (value != null)
            {
                Model.NetworkResourceGroup = value;
                await OnNetworkSelectedEvent.InvokeAsync(Model);
                var vnets = _networksList.Where(a => a.NetworkResourceGroup == value).Select(a => a.VirtualNetwork);
                if (vnets != null)
                    Networks.VirtualNetworkList.AddRange(vnets);
            }
        }

        public async Task OnVirtualNetworkChanged(string value)
        {
            if (value != null)
            {
                Model.VirtualNetwork = value;
                await OnNetworkSelectedEvent.InvokeAsync(Model);
                var subnets = _networksList.FirstOrDefault(a => a.VirtualNetwork == value);
                if (subnets != null)
                    Networks.SubnetList.AddRange(subnets.Subnets);
            }
        }

        public async Task OnSubnetChanged(string value)
        {
            if (value != null)
            {
                Model.Subnet = value;
                await OnNetworkSelectedEvent.InvokeAsync(Model);
            }
        }

        public async Task OnValidSubmit(EditContext context)
        {
            if (!IsValidationEnabled)
                return;

            var valid = context.Validate();
            if (valid)
            {
                try
                {
                    await OnNetworkSelectedEvent.InvokeAsync(Model);
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

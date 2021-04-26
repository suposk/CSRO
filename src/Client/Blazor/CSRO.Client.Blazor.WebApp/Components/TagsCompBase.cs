using CSRO.Client.Blazor.UI;
using CSRO.Client.Blazor.UI.Services;
using CSRO.Client.Services;
using CSRO.Client.Services.AzureRestServices;
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
    public class TagsCompBase : CsroComponentBase
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
        public bool IsprivilegedMembersVisible { get; set; }

        [Parameter] 
        public EventCallback<DefaultTag> OnTagSelectedEvent { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public ISubcriptionService SubcriptionService { get; set; }

        //[Inject]
        //public ISubcriptionDataService SubcriptionDataService { get; set; }

        [Inject]
        public ICsroDialogService CsroDialogService { get; set; }

        [Inject]
        public ILogger<TagsCompBase> Logger { get; set; }

        #endregion
        protected string _previosSubcriptionId;
        protected DefaultTag Model { get; set; } = new DefaultTag();
        protected DefaultTags Tags { get; set; } = new DefaultTags();

        protected bool IsReadOnly => false;

        protected string Title => "Select Tags";

        //protected async override Task OnInitializedAsync()
        //{
        //    await Load();
        //}

        protected async override Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (string.IsNullOrWhiteSpace(SubcriptionId) || string.Equals(SubcriptionId, _previosSubcriptionId))
                return;
            else
            {
                _previosSubcriptionId = SubcriptionId;
                await Load();
            }
        }

        private async Task Load()
        {
            try
            {
                ShowLoading();
                Model = new DefaultTag();
                
                var tags = await SubcriptionService.GetDefualtTags(SubcriptionId);
                Tags = tags ?? new DefaultTags();
            }
            catch (Exception ex)
            {
                await CsroDialogService.ShowError("Error", $"Detail error: {ex.Message}");
                Logger.LogError(ex, nameof(OnInitializedAsync));
            }
            finally
            {
                HideLoading();
            }
        }

        public async Task OnbillingReferenceChanged(string value)
        {
            if (value != null)
            {
                Model.billingReference = value;
                await OnTagSelectedEvent.InvokeAsync(Model);
            }
        }

        public async Task OnopEnvironmentChanged(string value)
        {
            if (value != null)
            {
                Model.opEnvironment = value;
                await OnTagSelectedEvent.InvokeAsync(Model);
            }
        }

        public async Task OncmdbReferenceChanged(string value)
        {
            if (value != null)
            {
                Model.cmdbReference = value;
                await OnTagSelectedEvent.InvokeAsync(Model);
            }
        }

        //public async Task OnValueChanged(string sender, string value)
        //{
        //    if (sender != null && value != null)
        //    {
        //        switch(sender)
        //        {
        //            case nameof(Model.billingReference):                    
        //                Model.billingReference = value;
        //                break;
        //            case nameof(Model.cmdbReference):
        //                Model.cmdbReference = value;
        //                break;
        //            case nameof(Model.opEnvironment):
        //                Model.opEnvironment = value;
        //                break;
        //        }
                
        //    }
        //}


        public async Task<IEnumerable<string>> SearchBilling(string value)
        {
            if (Tags == null)
                return null;

            // In real life use an asynchronous function for fetching data from an api.
            await Task.Delay(50);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return Tags.BillingReferenceList;

            return Tags.BillingReferenceList == null ? null : Tags.BillingReferenceList.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task<IEnumerable<string>> SearchOpEnv(string value)
        {
            if (Tags == null)
                return null;

            // In real life use an asynchronous function for fetching data from an api.
            await Task.Delay(50);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return Tags.OpEnvironmentList;

            return Tags.OpEnvironmentList == null ? null : Tags.OpEnvironmentList.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task<IEnumerable<string>> SearchCmbdRef(string value)
        {
            if (Tags == null)
                return null;

            // In real life use an asynchronous function for fetching data from an api.
            await Task.Delay(50);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return Tags.CmdbRerenceList;

            return Tags.CmdbRerenceList == null ? null : Tags.CmdbRerenceList.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
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
                    await OnTagSelectedEvent.InvokeAsync(Model);
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

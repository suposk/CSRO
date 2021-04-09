using CSRO.Client.Blazor.UI;
using CSRO.Client.Blazor.UI.Services;
using CSRO.Client.Services;
using CSRO.Client.Services.Models;
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
    public class AdoProjectHistoryBase : CsroComponentBase
    {
        #region Params and Injects

        [Parameter]
        public string RequestId { get; set; }

        [Parameter]
        public OperatioType OperationTypeTicket { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public ICsroDialogService CsroDialogService { get; set; }

        [Inject]
        public IAdoProjectHistoryDataService AdoProjectHistoryDataService { get; set; }

        [Inject]
        public ILogger<AdoProjectHistoryBase> Logger { get; set; }

        #endregion                
        protected bool IsReadOnly => OperationTypeTicket == OperatioType.View;
        protected string Title => OperationTypeTicket.ToString() + " Project";

        protected List<string> Processes = new();

        protected List<string> Organizations = new();

        public List<AdoProjectHistoryModel> History = new();

        protected async override Task OnInitializedAsync()
        {
            await Load();
        }

        private async Task Load()
        {
            //load history                                     
            try
            {
                if (OperationTypeTicket != OperatioType.Create)
                {
                    ShowLoading();
                    History.Clear();
                    if (int.TryParse(RequestId, out int id))
                    {
                        var history = await AdoProjectHistoryDataService.GetItemsByParrentIdAsync(id);
                        if (history.HasAnyInCollection())
                            History = history;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(OnInitializedAsync));
            }
            HideLoading();
        }

        public void GoBack()
        {
            NavigationManager.NavigateTo("/ado/AdoRequestsList");
        }

    }
}

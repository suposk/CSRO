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

namespace CSRO.Client.Blazor.WebApp.Components
{
    public class VmTicketHistoryCompBase : CsroComponentBase
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
        public IBaseDataService<VmTicketHistory> VmTicketHistoryDataService { get; set; }

        [Inject]
        public ILogger<VmTicketHistoryCompBase> Logger { get; set; }

        #endregion                
        protected bool IsReadOnly => OperationTypeTicket == OperatioType.View;
        protected string Title => OperationTypeTicket.ToString() + " Vm Ticket History";

        public List<VmTicketHistory> History = new();

        protected async override Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        protected async override Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (!Refresh)
                return;
            else                            
                await LoadAsync();            
        }

        public async override Task LoadAsync()
        {
            //load history                                     
            try
            {
                if (OperationTypeTicket != OperatioType.Create)
                {
                    ShowLoading();
                    History.Clear();
                    if (!string.IsNullOrWhiteSpace(RequestId))
                    {
                        var history = await VmTicketHistoryDataService.GetItemsByParrentIdAsync(RequestId);
                        if (history.HasAnyInCollection())
                            History = history;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogErrorCsro(ex);
            }
            HideLoading();
        }

        public void GoBack()
        {
            NavigationManager.NavigateTo("/ado/AdoRequestsList");
        }

    }
}

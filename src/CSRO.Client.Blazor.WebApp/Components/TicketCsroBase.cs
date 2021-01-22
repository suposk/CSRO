using CSRO.Client.Blazor.UI.Services;
using CSRO.Client.Services;
using CSRO.Client.Services.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CSRO.Client.Blazor.WebApp.Components
{
    public class TicketCsroBase : ComponentBase
    {
        #region Params and Injects

        [Parameter]
        public string TicketId { get; set; }

        [Parameter]
        public OperatioType OperationTypeTicket { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        IBaseDataService<Ticket> TicketDataService { get; set; }

        [Inject]
        public ICsroDialogService CsroDialogService { get; set; }

        [Inject]
        public ILogger<TicketCsroBase> Logger { get; set; }

        #endregion

        public Ticket Model { get; set; } = new Ticket();

        protected bool Success { get; set; }
        protected bool IsReadOnly => OperationTypeTicket == OperatioType.View;
        protected string Title => OperationTypeTicket.ToString() + " Ticket";

        protected async override Task OnInitializedAsync()
        {
            await Load();
        }

        private async Task Load()
        {
            try
            {
                if (OperationTypeTicket != OperatioType.Create)
                {
                    Model.Id = int.Parse(TicketId);
                    var server = await TicketDataService.GetItemByIdAsync(Model.Id);
                    if (server != null)
                        Model = server;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(OnInitializedAsync));
            }
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
                        var added = await TicketDataService.AddItemAsync(Model);
                        if (added != null)
                        {
                            Success = true;
                            Model = added;
                        }
                    }
                    else if (OperationTypeTicket == OperatioType.Edit)
                    {
                        var updated = await TicketDataService.UpdateItemAsync(Model);
                        if (updated)
                        {
                            Success = true;                            
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
                }
            }
        }

        public void GoBack()
        {
            NavigationManager.NavigateTo("/ticketsview");
        }

    }
}

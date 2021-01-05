using CSRO.Client.Services;
using CSRO.Client.Services.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Client.Blazor.WebApp.Components
{
    public class TicketCsroBase : ComponentBase
    {
        [Parameter]
        public string TicketId { get; set; }

        [Parameter]
        public OperatioType OperationTypeTicket { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        IBaseDataService<Ticket> TicketDataService { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        [Inject]
        public ILogger<TicketCsroBase> Logger { get; set; }


        public Ticket model { get; set; } = new Ticket();

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
                    model.Id = int.Parse(TicketId);
                    var server = await TicketDataService.GetItemByIdAsync(model.Id);
                    if (server != null)
                        model = server;
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
                        var added = await TicketDataService.AddItemAsync(model);
                        if (added != null)
                        {
                            Success = true;
                            model = added;
                        }
                    }
                    else if (OperationTypeTicket == OperatioType.Edit)
                    {
                        var updated = await TicketDataService.UpdateItemAsync(model);
                        if (updated)
                        {
                            Success = true;                            
                        }
                        else
                        {
                            var parameters = new DialogParameters();
                            parameters.Add("ContentText", $"Conflic Detected, Please refresh and try again");
                            parameters.Add("ButtonText", "Refresh");
                            parameters.Add("Color", Color.Error);

                            var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.Small };
                            var userSelect = DialogService.Show<DialogTemplateExample_Dialog>("Update Error", parameters, options);
                            var result = await userSelect.Result;
                            if (!result.Cancelled)
                            {
                                await Load();
                            }
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

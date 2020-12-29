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
    public class TicketCsroBase : ComponentBase
    {
        [Parameter]
        public string TicketId { get; set; }

        [Parameter]
        public OperatioType OperationTypeTicket { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        IBaseDataStore<Ticket> TicketDataStore { get; set; }

        [Inject]
        public ILogger<TicketCsroBase> Logger { get; set; }


        public Ticket model { get; set; } = new Ticket();

        protected bool Success { get; set; }
        protected bool IsReadOnly => OperationTypeTicket == OperatioType.View;

        public string Title => OperationTypeTicket.ToString() + " Ticket";

        //public string Title()
        //{
        //    switch (this.OperationTypeTicket)
        //    {
        //        case OperatioType.View 
        //    }

        //}

        protected async override Task OnInitializedAsync()
        {

            try
            {
                if (OperationTypeTicket != OperatioType.Create)
                {
                    model.Id = int.Parse(TicketId);
                    var server = await TicketDataStore.GetItemByIdAsync(model.Id);
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
                        var added = await TicketDataStore.AddItemAsync(model);
                        if (added != null)
                        {
                            Success = true;
                            model = added;
                        }
                    }
                    else
                    {

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
            NavigationManager.NavigateTo("/");
        }


    }
}

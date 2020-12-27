﻿using CSRO.Client.Services;
using CSRO.Client.Services.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Client.Blazor.WebApp.Pages
{
    public class TicketEditBase : ComponentBase
    {
        [Parameter]
        public string TicketId { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }


        [Inject]
        IBaseDataStore<Ticket> TicketDataStore { get; set; }

        [Inject]
        public ILogger<TicketEditBase> Logger { get; set; }


        public Ticket model { get; set; } = new Ticket();

        protected bool Success { get; set; }
        protected bool IsEdit => !string.IsNullOrWhiteSpace(TicketId);
        //protected bool IsReadOnly => IsEdit;

        protected async override Task OnInitializedAsync()
        {

            try
            {
                if (IsEdit)
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

        public Task EditTicket()
        {
            return Task.CompletedTask;
        }

        public async Task OnValidSubmit(EditContext context)
        {
            var valid = context.Validate();
            if (valid)
            {
                try
                {
                    if (IsEdit)
                    {

                    }
                    else
                    {
                        var added = await TicketDataStore.AddItemAsync(model);
                        if (added != null)
                        {
                            Success = true;
                            model = added;
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
            NavigationManager.NavigateTo("/");
        }


    }
}

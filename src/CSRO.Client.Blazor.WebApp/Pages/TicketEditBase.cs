using CSRO.Client.Services.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
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


        public Ticket model { get; set; } = new Ticket();

        protected bool Success { get; set; }
        protected bool IsEdit => !string.IsNullOrWhiteSpace(TicketId);
        //protected bool IsReadOnly => IsEdit;

        protected async override Task OnInitializedAsync()
        {            
            if (IsEdit)
            {
                model.Id = int.Parse(TicketId);
            }
        }

        public async Task OnValidSubmit(EditContext context)
        {
            Success = true;
            StateHasChanged();
        }

        public void GoBack()
        {
            NavigationManager.NavigateTo("/");
        }


    }
}

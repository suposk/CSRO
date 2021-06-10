using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Client.Blazor.UI
{
    public class CsroComponentBase : ComponentBase
    {
        [Parameter]
        public bool Refresh { get; set; }


        /// <summary>
        /// Callback to other other Components
        /// </summary>
        [Parameter]
        public EventCallback<bool> RefreshChanged { get; set; }


        public const string LOADING_MESSAGE = "Loading...";
        public const string PROCESSING_MESSAGE = "Processing...";


        protected bool IsLoading { get; set; }
        protected string LoadingMessage { get; set; } = LOADING_MESSAGE;

        protected bool IsLoadingSecondary { get; set; }
        protected string LoadingMessageSecondary { get; set; } = LOADING_MESSAGE;

        protected bool Success { get; set; }        
        public async virtual Task RefreshAsync() 
        {
            await RefreshChanged.InvokeAsync(true);
            await LoadAsync();            
        }
        public virtual Task LoadAsync() => Task.CompletedTask;        

        public virtual void ShowLoading(string loadingMessage = LOADING_MESSAGE)
        {
            IsLoading = true;
            LoadingMessage = loadingMessage;
            StateHasChanged();
        }

        public virtual void ShowProcessing(string loadingMessage = PROCESSING_MESSAGE)
        {
            IsLoading = true;
            LoadingMessage = loadingMessage;
            StateHasChanged();
        }

        public virtual void HideLoading()
        {
            IsLoading = false;
            StateHasChanged();
        }

        public virtual void ShowLoadingSecondary(string loadingMessage = LOADING_MESSAGE)
        {
            IsLoadingSecondary = true;
            LoadingMessageSecondary = loadingMessage;
            StateHasChanged();
        }

        public virtual void HideLoadingSecondary()
        {
            IsLoadingSecondary = false;
            StateHasChanged();
        }
    }
}

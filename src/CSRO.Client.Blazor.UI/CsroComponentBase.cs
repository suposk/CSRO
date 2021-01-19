using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Client.Blazor.UI
{
    public class CsroComponentBase : ComponentBase
    {
        public const string LOADING_MESSAGE = "Loading...";


        protected bool IsLoading { get; set; }
        protected string LoadingMessage { get; set; } = LOADING_MESSAGE;
        protected bool Success { get; set; }

        public virtual void ShowLoading(string loadingMessage = LOADING_MESSAGE)
        {
            IsLoading = true;
            LoadingMessage = loadingMessage;
        }

        public virtual void HideLoading()
        {
            IsLoading = false;
        }
    }
}

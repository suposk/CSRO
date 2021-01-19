using CSRO.Client.Blazor.UI.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Client.Blazor.UI.Services
{
    public interface ICsroDialogService
    {        

        Task<bool> ShowDialog(string title, string text, string okText = "Ok");
        Task ShowError(string title, string text, string okText = "Close");
        Task ShowMessage(string title, string text, string okText = "Ok");
        Task<bool> ShowWarning(string title, string text, string okText = "Ok");
    }

    public class CsroDialogService : ICsroDialogService
    {
        public CsroDialogService(IDialogService DialogService)
        {
            this.DialogService = DialogService;
        }

        private IDialogService DialogService { get; }

        public async Task<bool> ShowDialog(string title, string text, string okText = "Ok")
        {
            var parameters = new DialogParameters();
            parameters.Add("ContentText", text);
            parameters.Add("ButtonText", okText);
            parameters.Add("Color", Color.Info);
            //parameters.Add("ShowCancel", false);

            var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.Small };
            var userSelect = DialogService.Show<DialogTemplate>(title, parameters, options);
            var result = await userSelect.Result;

            return !result.Cancelled;
        }

        public async Task<bool> ShowWarning(string title, string text, string okText = "Ok")
        {
            var parameters = new DialogParameters();
            parameters.Add("ContentText", text);
            parameters.Add("ButtonText", okText);
            parameters.Add("Color", Color.Warning);
            //parameters.Add("ShowCancel", false);

            var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.Small };
            var userSelect = DialogService.Show<DialogTemplate>(title, parameters, options);
            var result = await userSelect.Result;

            return !result.Cancelled;
        }

        public async Task ShowMessage(string title, string text, string okText = "Ok")
        {
            var parameters = new DialogParameters();
            parameters.Add("ContentText", text);
            parameters.Add("ButtonText", okText);
            parameters.Add("Color", Color.Info);
            parameters.Add("ShowCancel", false);

            var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.Small };
            var userSelect = DialogService.Show<DialogTemplate>(title, parameters, options);
            var result = await userSelect.Result;
            return;
        }

        public async Task ShowError(string title, string text, string okText = "Close")
        {
            var parameters = new DialogParameters();
            parameters.Add("ContentText", text);
            parameters.Add("ButtonText", okText);
            parameters.Add("Color", Color.Error);
            parameters.Add("ShowCancel", false);

            var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.Small };
            var userSelect = DialogService.Show<DialogTemplate>(title, parameters, options);
            var result = await userSelect.Result;
            return;
        }
    }
}

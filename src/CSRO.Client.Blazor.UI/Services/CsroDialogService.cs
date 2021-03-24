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
        Task<string> ShowDialogWithEntry(string title, string text, string okText = "Ok");
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
        private DialogTemplateBase DialogTemplateBase { get; }

        public async Task<string> ShowDialogWithEntry(string title, string text, string okText = "Ok")
        {
            var parameters = new DialogParameters();
            parameters.Add(nameof(DialogTemplateBase.ContentText), text);
            parameters.Add(nameof(DialogTemplateBase.ButtonText), okText);
            parameters.Add(nameof(DialogTemplateBase.Color), Color.Info);

            string entryVal = null;
            parameters.Add(nameof(DialogTemplateBase.EnteredText), entryVal);
            parameters.Add(nameof(DialogTemplateBase.ShowEntry), true);

            var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.Small };
            var userSelect = DialogService.Show<DialogTemplate>(title, parameters, options);
            var result = await userSelect.Result;
            return result.Data?.ToString();                        
        }

        public async Task<bool> ShowDialog(string title, string text, string okText = "Ok")
        {
            var parameters = new DialogParameters();
            parameters.Add(nameof(DialogTemplateBase.ContentText), text);
            parameters.Add(nameof(DialogTemplateBase.ButtonText), okText);
            parameters.Add(nameof(DialogTemplateBase.Color), Color.Info);            

            var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.Small };
            var userSelect = DialogService.Show<DialogTemplate>(title, parameters, options);
            var result = await userSelect.Result;

            return !result.Cancelled;
        }

        public async Task<bool> ShowWarning(string title, string text, string okText = "Ok")
        {
            var parameters = new DialogParameters();
            parameters.Add(nameof(DialogTemplateBase.ContentText), text);
            parameters.Add(nameof(DialogTemplateBase.ButtonText), okText);
            parameters.Add(nameof(DialogTemplateBase.Color), Color.Warning);            

            var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.Small };
            var userSelect = DialogService.Show<DialogTemplate>(title, parameters, options);
            var result = await userSelect.Result;

            return !result.Cancelled;
        }

        public async Task ShowMessage(string title, string text, string okText = "Ok")
        {
            var parameters = new DialogParameters();
            parameters.Add(nameof(DialogTemplateBase.ContentText), text);
            parameters.Add(nameof(DialogTemplateBase.ButtonText), okText);
            parameters.Add(nameof(DialogTemplateBase.Color), Color.Info);
            parameters.Add(nameof(DialogTemplateBase.ShowCancel), false);

            var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.Small };
            var userSelect = DialogService.Show<DialogTemplate>(title, parameters, options);
            var result = await userSelect.Result;
            return;
        }

        public async Task ShowError(string title, string text, string okText = "Close")
        {
            var parameters = new DialogParameters();
            parameters.Add(nameof(DialogTemplateBase.ContentText), text);
            parameters.Add(nameof(DialogTemplateBase.ButtonText), okText);
            parameters.Add(nameof(DialogTemplateBase.Color), Color.Error);
            parameters.Add(nameof(DialogTemplateBase.ShowCancel), false);

            var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.Small };
            var userSelect = DialogService.Show<DialogTemplate>(title, parameters, options);
            var result = await userSelect.Result;
            return;
        }
    }
}

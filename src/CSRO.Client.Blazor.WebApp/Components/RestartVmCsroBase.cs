﻿using CSRO.Client.Services;
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
    public class RestartVmCsroBase : ComponentBase
    {
        [Parameter]
        public string TicketId { get; set; }

        [Parameter]
        public OperatioType OperationTypeTicket { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        IVmTicketDataService VmTicketDataService { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        [Inject]
        public ILogger<RestartVmCsroBase> Logger { get; set; }

        protected VmTicket Model { get; set; } = new VmTicket();

        protected bool IsLoading { get; set; }
        protected string LoadingMessage { get; set; }
        protected bool Success { get; set; }
        protected bool IsReadOnly => OperationTypeTicket == OperatioType.View;
        protected string Title => OperationTypeTicket == OperatioType.Create ? "Request Vm Restart" : $"View {Model.Status} of {Model.VmName}";

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
                    IsLoading = true;
                    LoadingMessage = "Loading...";

                    Model.Id = int.Parse(TicketId);
                    var server = await VmTicketDataService.GetItemByIdAsync(Model.Id);
                    if (server != null)
                    {
                        Model = server;
                        if (OperationTypeTicket == OperatioType.View)
                        {
                            int i = 0;
                            while (i < 10)
                            {
                                i++;
                                if (Model.VmState == "Restart Started" || !string.Equals(Model.VmState, "VM running"))
                                {
                                    //need to create delay to update vm state after restart                                                                       
                                    LoadingMessage = $"Current state: {Model.VmState}";
                                    StateHasChanged();

                                    await Task.Delay(10 * 1000);                                
                                    
                                    var running = await VmTicketDataService.VerifyRestartStatus(Model).ConfigureAwait(false);
                                    if (running)
                                    {
                                        Model = await VmTicketDataService.GetItemByIdAsync(Model.Id);                                        
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                #if DEBUG
                else
                {
                    //dubug only
                    Model.SubcriptionId = "33fb38df-688e-4ca1-8dd8-b46e26262ff8";
                    Model.ResorceGroup = "dev-VMS";
                    Model.VmName = "VmDelete";
                }
                #endif

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(OnInitializedAsync));                
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task OnValidSubmit(EditContext context)
        {
            var valid = context.Validate();
            if (valid)
            {
                try
                {
                    IsLoading = true;
                    if (OperationTypeTicket == OperatioType.Create)
                    {
                        LoadingMessage = "Creating request";   
                        
                        var added = await VmTicketDataService.AddItemAsync(Model);
                        if (added != null)
                        {
                            Success = true;
                            Model = added;

                            NavigationManager.NavigateTo($"vm/restart/view/{Model.Id}");
                        }
                    }
                    else if (OperationTypeTicket == OperatioType.Edit)
                    {
                        LoadingMessage = "Updating request";

                        var updated = await VmTicketDataService.UpdateItemAsync(Model);
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
                    IsLoading = false;
                    Logger.LogError(ex, nameof(OnValidSubmit));

                    var parameters = new DialogParameters();
                    parameters.Add("ContentText", $"Detail error: {ex.Message}");
                    parameters.Add("ButtonText", "Close");
                    parameters.Add("Color", Color.Error);
                    parameters.Add("ShowCancel", false);

                    var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Small };
                    var userSelect = DialogService.Show<DialogTemplateExample_Dialog>("Update Error", parameters, options);
                    var result = await userSelect.Result;
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        public void GoBack()
        {
            NavigationManager.NavigateTo("vm");
        }

    }
}

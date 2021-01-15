﻿using CSRO.Client.Blazor.UI;
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
    public class RestartVmCsroBase : CsroComponentBase
    {
        [Parameter]
        public string TicketId { get; set; }

        [Parameter]
        public OperatioType OperationTypeTicket { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IVmTicketDataService VmTicketDataService { get; set; }

        [Inject]
        public ISubcriptionService SubcriptionService { get; set; }

        [Inject]
        public IResourceGroupervice ResourceGroupervice { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        [Inject]
        public ILocationsService LocationsService { get; set; }

        [Inject]
        public ILogger<RestartVmCsroBase> Logger { get; set; }

        protected VmTicket Model { get; set; } = new VmTicket();

        protected bool IsReadOnly => OperationTypeTicket == OperatioType.View;
        protected string Title => OperationTypeTicket == OperatioType.Create ? "Request Vm Restart" : $"View {Model.Status} of {Model.VmName}";
        protected List<IdName> Subscripions { get; set; }        
        protected List<string> ResourceGroups { get; set; } = new List<string>();        
        protected bool IsRgDisabled => ResourceGroups?.Count == 0;
        protected bool IsVmDisabled => OperationTypeTicket != OperatioType.Create || IsRgDisabled || string.IsNullOrWhiteSpace(Model?.ResorceGroup);

        protected async override Task OnInitializedAsync()
        {            
            await Load();
        }

        async Task LoadRg(string subcriptionId)
        {
            ResourceGroups.Clear();
            var rgs = await ResourceGroupervice.GetResourceGroups(subcriptionId);
            if (rgs != null)
            {
                ResourceGroups.AddRange(rgs.Select(a => a.Name));
                StateHasChanged();
            }
        }

        public async Task OnChangeCategory(IdName value)
        {
            if (value != null)
            {
                Model.SubcriptionId = value.Id;
                Model.SubcriptionName = value.Name;

                ShowLoading();
                await LoadRg(value.Id);
                HideLoading();
            }
        }

        private async Task Load()
        {
            try
            {
                //var loc = await LocationsService.GetLocations();

                if (OperationTypeTicket != OperatioType.Create)
                {
                    ShowLoading();                    

                    Model.Id = int.Parse(TicketId);
                    var server = await VmTicketDataService.GetItemByIdAsync(Model.Id);
                    await LoadRg(server?.SubcriptionId);
                    //var subName = await SubcriptionService.GetSubcription(server?.SubcriptionId);
                    //var rgName = await ResourceGroupervice.GetResourceGroupsIdName(server?.SubcriptionId);

                    if (server != null)
                    {
                        Model = server;
                        if (OperationTypeTicket == OperatioType.View)
                        {
                            if (Model.VmState == "Restart Started" || !string.Equals(Model.VmState, "VM running"))
                            {
                                //need to create delay to update vm state after restart                                                                       
                                LoadingMessage = $"Current state: {Model.VmState}";
                                StateHasChanged();

                                var running = await VmTicketDataService.VerifyRestartStatusCallback(Model, (status)=> 
                                {
                                    LoadingMessage = $"Current state: {status}";
                                    StateHasChanged();
                                }).ConfigureAwait(false);
                                if (running)
                                {
                                    Model = await VmTicketDataService.GetItemByIdAsync(Model.Id);                                    
                                }
                            }

                        }
                    }
                }
                
                else
                {
                    ShowLoading();
                    Subscripions = await SubcriptionService.GetSubcriptions();                    

                    #if DEBUG

                    //dubug only
                    //Model.SubcriptionId = "33fb38df-688e-4ca1-8dd8-b46e26262ff8";
                    if (Subscripions?.Count == 1)
                    {
                        for (int i=1; i <= 3; i++)
                        {
                            Subscripions.Add(new IdName(Guid.NewGuid().ToString(), $"fake sub name {i}"));
                        }
                    }
                    //Model.ResorceGroup = "dev-VMS";
                    //Model.VmName = "VmDelete";

                    #endif

                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(OnInitializedAsync));                
            }
            finally
            {
                HideLoading();
            }
        }

        public async Task<IEnumerable<IdName>> SearchSubs(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            await Task.Delay(50);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return Subscripions;

            return Subscripions == null ? null : Subscripions.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase));
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
                        ShowLoading("Creating request");   
                        
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
                        ShowLoading("Updating request");

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
                    HideLoading();
                }
            }
        }

        public void GoBack()
        {
            NavigationManager.NavigateTo("vm");
        }

    }
}

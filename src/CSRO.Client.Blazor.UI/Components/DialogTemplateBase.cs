using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Blazor.UI.Components
{
    public class DialogTemplateBase : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        [Parameter] public string ContentText { get; set; }

        [Parameter] public string ButtonText { get; set; }

        [Parameter] public Color Color { get; set; }

        [Parameter] public bool ShowCancel { get; set; } = true;


        public void Submit() => MudDialog.Close(DialogResult.Ok(true));
        public void Cancel() => MudDialog.Cancel();
    }
}

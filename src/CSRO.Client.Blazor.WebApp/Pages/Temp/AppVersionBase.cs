using CSRO.Client.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Client.Blazor.WebApp.Pages
{
    public class AppVersionBase : ComponentBase
    {

        //[Inject]
        //public IConfiguration Configuration { get; set; }

        [Inject]
        public IVersionService VersionService { get; set; }

        public string Result { get; private set; }


        protected async override Task OnInitializedAsync()
        {
            Result = null;
                        
            var v = await VersionService.GetVersion();
            if (v != null)
            {
                //VersionDto add = new VersionDto { Link = "www.bing.com", Version = v.Version + 1, Details= $"Created in client at {DateTime.Now.ToShortTimeString()}" };
                //var r = await AddVersion(add);
                //if (r != null)
                //{
                //    //var v2 = await GetVersion(r.Version);                    
                //    //var deleted = await DeleteVersion(v.Version);
                //}
                var all = await VersionService.GetAllVersion();
                int sec = 1;
                await Task.Delay(sec * 1000);

                Result = $"Found {all.Count} version. Loaded with {sec} second of simulation delay";
                Result += $"\nLatest Version is {v.VersionFull}";
            }
        }


    }
}

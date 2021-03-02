using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSRO.Common.AdoServices.Models
{
    /// <summary>
    /// Wrapper around TeamProject
    /// </summary>
    public class ProjectAdo : TeamProject
    {
        public string Organization { get; set; }
    }
}

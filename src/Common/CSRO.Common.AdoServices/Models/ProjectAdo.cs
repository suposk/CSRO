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
        public string ProcessName { get; set; }
        public Guid? AdoId { get; set; }

        public new int Id { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public string ModifiedBy { get; set; }

        public byte[] RowVersion { get; set; }
    }
}

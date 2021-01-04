using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Models
{
    public class AppVersion : ModelBase
    {
        public int Id { get; set; }

        public RecomendedAction RecomendedAction { get; set; }

        public int VersionValue { get; set; }

        public string VersionFull { get; set; }

        public string Link { get; set; }

        public string Details { get; set; }

        public string DetailsFormat { get; set; }

        public DateTime? ReleasedAt { get; set; }
    }
}

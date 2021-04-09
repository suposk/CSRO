using CSRO.Server.Entities.Enums;
using CSRO.Server.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSRO.Server.Entities.Entity
{
    public class AppVersion: EntityBase
    {
        public RecomendedAction RecomendedAction { get; set; }

        public int VersionValue { get; set; }

        public string VersionFull { get; set; }

        public string Link { get; set; }

        public string Details { get; set; }

        public string DetailsFormat { get; set; }

        public DateTime? ReleasedAt { get; set; }
    }
}

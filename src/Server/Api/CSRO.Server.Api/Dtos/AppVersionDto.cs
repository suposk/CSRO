using CSRO.Server.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSRO.Server.Api.Dtos
{
    public class AppVersionDto : DtoBase
    {
        public RecomendedActionEnum RecomendedAction { get; set; }

        public int VersionValue { get; set; }

        public string VersionFull { get; set; }

        public string Link { get; set; }

        public string Details { get; set; }

        public string DetailsFormat { get; set; }

        public DateTime? ReleasedAt { get; set; }
    }
}

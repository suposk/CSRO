using CSRO.Server.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Server.Ado.Api.Dtos
{
    public class AdoProjectAccessDto : DtoBase
    {
        public Status Status { get; set; }
        public string Organization { get; set; }
        public string Name { get; set; }
        public string Justification { get; set; }
    }
}

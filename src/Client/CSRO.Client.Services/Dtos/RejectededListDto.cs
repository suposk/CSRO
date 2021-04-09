using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Dtos
{
    public class RejectededListDto
    {
        [Required]
        public List<int> ToReject { get; set; }

        [Required]
        [MinLength(4)]
        public string Reason { get; set; }
    }
}

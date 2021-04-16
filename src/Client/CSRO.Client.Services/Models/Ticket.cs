using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Models
{

    public class Ticket : ModelBase
    {
        [Required]
        public string Description { get; set; }

        [Required]
        public string RequestedFor { get; set; }        

        public bool IsOnBehalf { get; set; }        

    }

}

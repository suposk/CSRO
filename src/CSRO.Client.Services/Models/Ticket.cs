using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Models
{
    public enum OperatioType { Create, Edit, View }

    public class Ticket
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string RequestedFor { get; set; }

        public string CreatedBy { get; set; }

        public bool IsOnBehalf { get; set; }        

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

    }

}

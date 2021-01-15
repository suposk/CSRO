using System.ComponentModel.DataAnnotations;

namespace CSRO.Client.Services.Models
{
    public class VmTicket : ModelBase
    {
        public VmTicket()
        {
            _SubscripionIdName = new IdName();
        }

        private string _SubcriptionId;

        public string SubcriptionId
        {
            get { return _SubcriptionId; }
            set { _SubcriptionId = value; _SubscripionIdName.Id = value; }
        }

        private string _SubcriptionName;

        public string SubcriptionName
        {
            get { return _SubcriptionName; }
            set { _SubcriptionName = value;  _SubscripionIdName.Name = value; }
        }


        public string Note { get; set; }
        
        //public string SubcriptionId { get; set; }

        //public string SubcriptionName { get; set; }
        
        public string ResorceGroup { get; set; }

        //using FluentValidation [Required]
        public string VmName { get; set; }

        public string Status { get; set; }

        public string VmState { get; set; }


        IdName _SubscripionIdName;
        public IdName SubscripionIdName => _SubscripionIdName;
    }

}

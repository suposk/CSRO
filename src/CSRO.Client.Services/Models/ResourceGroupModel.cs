using System;

namespace CSRO.Client.Services.Models
{
    public class ResourceGroupModel : ModelBase
    {
        public ResourceGroupModel()
        {
            ResourceGroup = new ResourceGroup();

            _SubscripionIdName = new IdName();
            _LocationIdName = new IdName();            
        }

        public bool IsNewRg { get; set; }
        public string NewRgName { get; set; }        

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
            set { _SubcriptionName = value; _SubscripionIdName.Name = value; }
        }

        //public string SubcriptionId { get; set; }

        IdName _SubscripionIdName;
        public IdName SubscripionIdName => _SubscripionIdName;

        public ResourceGroup ResourceGroup { get; set; }
        public string Location => ResourceGroup?.Location;

        IdName _LocationIdName;
        public IdName LocationIdName
        {
            get => _LocationIdName;
            set 
            {
                _LocationIdName = value;
                ResourceGroup.Location = value.Id;
            }
        }        
    }
}

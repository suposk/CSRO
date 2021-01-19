namespace CSRO.Client.Services.Models
{
    public class ResourceGroupModel : ModelBase
    {
        public ResourceGroupModel()
        {
            _SubscripionIdName = new IdName();
            ResourceGroup = new ResourceGroup();
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
            set { _SubcriptionName = value; _SubscripionIdName.Name = value; }
        }

        IdName _SubscripionIdName;
        public IdName SubscripionIdName => _SubscripionIdName;


        //public string ResorceGroup { get; set; }                
        //public string Location { get; set; }        

        public ResourceGroup ResourceGroup { get; set; }
        
        public IdName ResourceGroupIdName => new IdName { Id = ResourceGroup.Location, Name = ResourceGroup.Name };

    }
}

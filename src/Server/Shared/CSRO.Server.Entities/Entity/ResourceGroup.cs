namespace CSRO.Server.Entities.Entity
{
    public class ResourceGroup
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public Properties Properties { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class Properties
    {
        public string ProvisioningState { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Dtos.AzureDtos
{
    public class VirtualMachinesDto
    {
        public List<VirtualMachineDto> Value { get; set; }
    }

    public class VirtualMachineDto
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public TagDto Tags { get; set; }
        public IdentityDto Identity { get; set; }
        public VmPropertiesDto Properties { get; set; }
        //public List<long> Zones { get; set; }
        public List<string> Zones { get; set; }
    }

    public class IdentityDto
    {
        public string Type { get; set; }
        public Guid PrincipalId { get; set; }
        public Guid TenantId { get; set; }
    }

    public class VmPropertiesDto
    {
        public Guid VmId { get; set; }
        //public HardwareProfile HardwareProfile { get; set; }
        //public StorageProfile StorageProfile { get; set; }
        //public OsProfile OsProfile { get; set; }
        //public NetworkProfile NetworkProfile { get; set; }
        public string LicenseType { get; set; }
        public string ProvisioningState { get; set; }
    }

    //public class HardwareProfile
    //{
    //    public string VmSize { get; set; }
    //}

    //public class NetworkProfile
    //{
    //    public List<NetworkInterface> NetworkInterfaces { get; set; }
    //}

    //public class NetworkInterface
    //{
    //    public string Id { get; set; }
    //}

    //public class OsProfile
    //{
    //    public string ComputerName { get; set; }
    //    public string AdminUsername { get; set; }
    //    public WindowsConfiguration WindowsConfiguration { get; set; }
    //    public List<object> Secrets { get; set; }
    //    public bool AllowExtensionOperations { get; set; }
    //    public bool RequireGuestProvisionSignal { get; set; }
    //}

    //public class WindowsConfiguration
    //{
    //    public bool ProvisionVmAgent { get; set; }
    //    public bool EnableAutomaticUpdates { get; set; }
    //    public PatchSettings PatchSettings { get; set; }
    //}

    //public class PatchSettings
    //{
    //    public string PatchMode { get; set; }
    //}

    //public class StorageProfile
    //{
    //    public ImageReference ImageReference { get; set; }
    //    public OsDisk OsDisk { get; set; }
    //    public List<object> DataDisks { get; set; }
    //}

    //public class ImageReference
    //{
    //    public string Publisher { get; set; }
    //    public string Offer { get; set; }
    //    public string Sku { get; set; }
    //    public string Version { get; set; }
    //    public string ExactVersion { get; set; }
    //}

    //public class OsDisk
    //{
    //    public string OsType { get; set; }
    //    public string Name { get; set; }
    //    public string CreateOption { get; set; }
    //    public string Caching { get; set; }
    //    public NetworkInterface ManagedDisk { get; set; }
    //}

    //public partial class Tags
    //{
    //    public string OpEnvironment { get; set; }
    //}
}

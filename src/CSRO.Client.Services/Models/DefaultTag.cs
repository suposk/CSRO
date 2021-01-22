using System.ComponentModel.DataAnnotations;

namespace CSRO.Client.Services.Models
{
    /// <summary>
    /// only to look for prop tag names
    /// </summary>
    public class DefaultTag
    {
        public string opEnvironment { get; set; }
                
        public string cmdbRerence { get; set; }
                
        public string billingReference { get; set; }
    }
}

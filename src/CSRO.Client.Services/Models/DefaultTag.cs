using System.ComponentModel.DataAnnotations;

namespace CSRO.Client.Services.Models
{
    /// <summary>
    /// only to look for prop tag names
    /// </summary>
    public class DefaultTag
    {
        public string opEnvironment { get; set; }

        /// <summary>
        /// bug it was cmdbRerence, not cmdbReference
        /// </summary>
        public string cmdbReference { get; set; }
                
        public string billingReference { get; set; }
    }
}

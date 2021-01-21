using System.ComponentModel.DataAnnotations;

namespace CSRO.Client.Services.Models
{
    /// <summary>
    /// only to look for prop tag names
    /// </summary>
    public class DefaultTag
    {
        [Required]
        public string opEnvironment { get; set; }

        [Required]
        public string cmdbRerence { get; set; }

        [Required]
        public string billingReference { get; set; }
    }
}

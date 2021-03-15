using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Server.Services
{
    //public record EmailConfig(string SmtpHost, int SmtpPort, string SmtpUser, string SmtpPass) 
    //{
    //    public bool HasPassword => !string.IsNullOrWhiteSpace(SmtpPass);
    //};

    public class EmailConfig 
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }

        public string SmtpUser { get; set; }

        public string SmtpPass { get; set; }

        public bool HasPassword => !string.IsNullOrWhiteSpace(SmtpPass);
    }
}

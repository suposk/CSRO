using CSRO.Server.Infrastructure;

namespace CSRO.Server.Entities.Entity
{
    public enum ApproverType
    {
        /// <summary>
        /// Admin or primary 
        /// </summary>
        Primary = 1, 

        /// <summary>
        /// Secondary person can approve
        /// </summary>
        Deputy = 2,
    }

    public class AdoProjectApprover : EntitySoftDeleteBase
    {
        public string UserId { get; set; }        
        public string Email { get; set; }
        public bool IsEnabled { get; set; }
        public ApproverType ApproverType { get; set; }
    }
}

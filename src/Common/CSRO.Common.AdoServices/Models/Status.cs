namespace CSRO.Common.AdoServices.Models
{
    /// <summary>
    /// Status of Ado Ticket
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// Saved only ticket, not submited
        /// </summary>
        Draft = 10,

        /// <summary>
        /// Awaiting Approval 
        /// </summary>
        Submitted = 20,

        /// <summary>
        /// Rejected, ticket must be edited and resubmited
        /// </summary>
        Rejected = 30,

        /// <summary>
        /// Approved. Can be executed
        /// </summary>
        Approved = 40,

        Completed = 50,

        Deleted = 60
    }
}

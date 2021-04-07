namespace CSRO.Client.Services.Models
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

        Deleted = 30,

        /// <summary>
        /// Rejected, ticket must be edited and resubmited
        /// </summary>
        Rejected = 40,

        /// <summary>
        /// Some error during process.
        /// </summary>
        Failed = 50,

        /// <summary>
        /// Approved. Can be executed
        /// </summary>
        Approved = 60,

        Completed = 70,
    }
}

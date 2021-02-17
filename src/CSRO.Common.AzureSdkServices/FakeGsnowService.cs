using Azure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSRO.Common.AzureSdkServices
{
    public interface IGsnowService
    {
        Task<bool> IsValidGsnowTicket(string gsnowTicketNumber, CancellationToken cancelToken = default);
    }

    public class FakeGsnowService : IGsnowService
    {
        private readonly ICsroTokenCredentialProvider _csroTokenCredentialProvider;
        private readonly TokenCredential _tokenCredential;

        public FakeGsnowService(ICsroTokenCredentialProvider csroTokenCredentialProvider)
        {
            _csroTokenCredentialProvider = csroTokenCredentialProvider;
            _tokenCredential = _csroTokenCredentialProvider.GetCredential();
        }

        public async Task<bool> IsValidGsnowTicket(string gsnowTicketNumber, CancellationToken cancelToken = default)
        {
            if (string.IsNullOrWhiteSpace(gsnowTicketNumber))            
                throw new ArgumentException($"'{nameof(gsnowTicketNumber)}' cannot be null or whitespace", nameof(gsnowTicketNumber));            

            await Task.Delay(10);
            return true;
        }
    }

}

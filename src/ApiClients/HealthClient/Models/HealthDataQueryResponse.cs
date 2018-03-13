using System;

namespace Clinician.ApiClients.HealthClient.Models
{
    public class HealthDataQueryResponse
    {
        public int NumberOfResults { get; set; }
        
        public DateTimeOffset ExpirationDate { get; set; }

        public string QueryKey { get; set; }

        public bool IsValid()
        {
            return DateTimeOffset.UtcNow < this.ExpirationDate && !string.IsNullOrWhiteSpace(this.QueryKey);
        }
    }
}
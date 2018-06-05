using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using Refit;

namespace Clinician.ApiClients.AgencyClient
{
    public class AgencySubjectClaim
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }

    public class HealthScopeItem
    {
        public string Name { get; }
        public string FriendlyName => this.Name;

        public HealthScopeItem(string name)
        {
            
            Name = name;
        }
    }

    public class AgencySubject : List<AgencySubjectClaim>
    {
        public string GivenName => this.FirstOrDefault(c => c.Type == JwtClaimTypes.GivenName)?.Value;
        public string FamilyName => this.FirstOrDefault(c => c.Type == JwtClaimTypes.FamilyName)?.Value;
        public string Name => this.FirstOrDefault(c => c.Type == JwtClaimTypes.Name)?.Value;
        public string Subject => this.FirstOrDefault(c => c.Type == JwtClaimTypes.Subject)?.Value;

        public IList<HealthScopeItem> HealthDataScopes => this.Where(x => x.Type == "scope" && x.Value.StartsWith("read.")).Select(s => new HealthScopeItem(s.Value)).ToList();
    }

    public class AgencySubjectQueryTokenRequest
    {
        public AgencySubjectQueryTokenRequest(bool includeAll, IEnumerable<Guid> included = null, IEnumerable<Guid> excluded = null)
        {
            this.IncludeAll = includeAll;
            this.SpecificallyIncludedSubs = included ?? Enumerable.Empty<Guid>();
            this.SpecificallyExcludedSubs = excluded?? Enumerable.Empty<Guid>();
        }

        public bool IncludeAll { get; set; }
        public IEnumerable<Guid> SpecificallyIncludedSubs { get; set; }
        public IEnumerable<Guid> SpecificallyExcludedSubs { get; set; }
    }

    public class AgencySubjectQueryTokenResponse
    {
        public string Value { get; set; }
    }

    [Headers("Authorization: Bearer")]
    public interface IAgencyApiClient
    {
        [Get("/agency/claims/v1")]
        Task<IEnumerable<AgencySubject>> ResolveClaimsAsync();

        [Post("/agency/querytoken/v1")]
        Task<AgencySubjectQueryTokenResponse> CreateQueryAsync([Body(BodySerializationMethod.Json)] AgencySubjectQueryTokenRequest request);
    }
}
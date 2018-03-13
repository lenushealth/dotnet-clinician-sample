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
        public string Subject => this.FirstOrDefault(c => c.Type == "pseudo_sub")?.Value;

        public IList<HealthScopeItem> HealthDataScopes => this.Where(x => x.Type == "scope" && x.Value.StartsWith("read.")).Select(s => new HealthScopeItem(s.Value)).ToList();
    }

    public class AgencySubjectQueryTokenRequest
    {
        public AgencySubjectQueryTokenRequest(bool includeAll, IEnumerable<Guid> included = null, IEnumerable<Guid> excluded = null)
        {
            this.IncludeAll = includeAll;
            this.SpecificallyIncludedPseudoSubs = included ?? Enumerable.Empty<Guid>();
            this.SpecificallyExcludedPseudoSubs = excluded?? Enumerable.Empty<Guid>();
        }

        public bool IncludeAll { get; set; }
        public IEnumerable<Guid> SpecificallyIncludedPseudoSubs { get; set; }
        public IEnumerable<Guid> SpecificallyExcludedPseudoSubs { get; set; }
    }

    public class AgencySubjectQueryTokenResponse
    {
        public string Value { get; set; }
    }

    [Headers("Authorization: Bearer")]
    public interface IAgencyApiClient
    {
        [Get("/claims")]
        Task<IEnumerable<AgencySubject>> ResolveClaimsAsync();

        [Post("/querytoken")]
        Task<AgencySubjectQueryTokenResponse> CreateQueryAsync([Body(BodySerializationMethod.Json)] AgencySubjectQueryTokenRequest request);
    }
}
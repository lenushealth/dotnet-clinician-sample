using System.Collections.Generic;
using System.Linq;
using IdentityModel;

namespace Clinician.ApiClients.AgencyClient
{
    public class AgencySubject : List<AgencySubjectClaim>
    {
        public string GivenName => this.FirstOrDefault(c => c.Type == JwtClaimTypes.GivenName)?.Value;
        public string FamilyName => this.FirstOrDefault(c => c.Type == JwtClaimTypes.FamilyName)?.Value;
        public string Name => this.FirstOrDefault(c => c.Type == JwtClaimTypes.Name)?.Value;
        public string Subject => this.FirstOrDefault(c => c.Type == JwtClaimTypes.Subject)?.Value;

        public IList<HealthScopeItem> HealthDataScopes => this.Where(x => x.Type == "scope" && x.Value.StartsWith("read.")).Select(s => new HealthScopeItem(s.Value)).ToList();
    }
}
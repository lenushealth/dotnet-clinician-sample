using System.Collections.Generic;
using Clinician.ApiClients.AgencyClient;

namespace Clinician.Models
{
    public class AgencySubjectViewModel
    {
        public string GivenName { get; set; }
        public string FamilyName { get; set; }

        public string Name { get; set; }
        public string Subject { get; set; }

        public IEnumerable<HealthScopeItem> HealthDataScopes { get; set; }
    }
}
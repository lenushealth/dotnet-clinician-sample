using System;
using System.Collections.Generic;
using System.Linq;

namespace Clinician.ApiClients.AgencyClient
{
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
}
using System;
using System.Collections.Generic;
using Clinician.ApiClients.AgencyClient;

namespace Clinician.Models
{
    public class AgencySubjectQueryDateFilterViewModel
    {
        public AgencySubjectQueryDateFilterViewModel(AgencySubject agencySubject)
        {
            this.AgencySubject = agencySubject;
        }

        public AgencySubject AgencySubject { get; set; }
        public IDictionary<string, (DateTimeOffset From, DateTimeOffset To)> Filters { get; set; } = new Dictionary<string, (DateTimeOffset From, DateTimeOffset To)>();
    }
}

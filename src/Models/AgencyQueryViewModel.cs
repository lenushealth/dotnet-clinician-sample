using System.Collections.Generic;
using Clinician.ApiClients.AgencyClient;
using Clinician.Models;

namespace Clinician.Controllers
{
    public class AgencyQueryViewModel
    {
        public AgencyQueryViewModel(AgencySubject agencySubject)
        {
            AgencySubject = agencySubject;
        }

        public AgencySubject AgencySubject { get; set; }
        public IEnumerable<SampleDataTypes> Types { get; set; }
    }
}

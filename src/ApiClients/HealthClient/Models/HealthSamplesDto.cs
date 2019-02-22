using System.Collections.Generic;

namespace Clinician.ApiClients.HealthClient.Models
{
    public class HealthSamplesDto
    {
        public int TotalAvailable { get; set; }

        public int Count { get; set; }

        public IList<HealthSample> Datas { get; set; }
    }
}

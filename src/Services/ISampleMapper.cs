using System.Collections.Generic;
using Clinician.ApiClients.HealthClient.Models;
using Clinician.Models;

namespace Clinician.Services
{
    public interface ISampleMapper
    {
        IEnumerable<ISampleModel> Map(IEnumerable<HealthSample> samples, SampleDataTypes type);
    }
}
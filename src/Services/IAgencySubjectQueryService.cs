using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinician.ApiClients.AgencyClient;
using Clinician.Controllers;
using Clinician.Models;

namespace Clinician.Services
{
    public interface IAgencySubjectQueryService
    {
        Task<AgencySubject> GetAgencySubjectAsync(Guid subject);
        Task<IEnumerable<AgencySubject>> GetAgencySubjectsAsync();
        Task<IEnumerable<ISampleModel>> QueryAsync(SampleDataTypes type, AgencySubjectQueryParameters parameters);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace Clinician.ApiClients.AgencyClient
{
    [Headers("Authorization: Bearer")]
    public interface IAgencyApiClient
    {
        [Get("/claims")]
        Task<IEnumerable<AgencySubject>> ResolveClaimsAsync();

        [Post("/querytoken")]
        Task<AgencySubjectQueryTokenResponse> CreateQueryAsync([Body(BodySerializationMethod.Json)] AgencySubjectQueryTokenRequest request);
    }
}
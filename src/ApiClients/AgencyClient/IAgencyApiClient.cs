using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace Clinician.ApiClients.AgencyClient
{
    [Headers("Authorization: Bearer")]
    public interface IAgencyApiClient
    {
        [Get("/v1/claims")]
        Task<IEnumerable<AgencySubject>> ResolveClaimsAsync();

        [Post("/v1/querytoken")]
        Task<AgencySubjectQueryTokenResponse> CreateQueryAsync([Body(BodySerializationMethod.Json)] AgencySubjectQueryTokenRequest request);
    }
}
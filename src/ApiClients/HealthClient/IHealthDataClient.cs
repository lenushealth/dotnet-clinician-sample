using System.Collections.Generic;
using System.Threading.Tasks;
using Clinician.ApiClients.HealthClient.Models;
using Refit;

namespace Clinician.ApiClients.HealthClient
{
    [Headers("api-version: 2.0", "Authorization: Bearer")]
    public interface IHealthDataClient
    {
        [Post("/api/query")]
        Task<HealthDataQueryResponse> CreateQueryAsync([Body(BodySerializationMethod.Json)] HealthDataQueryRequest request, [Header("agency-query-token")] string agencyQueryToken = null);

        [Get("/api/query")]
        Task<IEnumerable<HealthSample>> ExecuteQueryAsync([Query] string querykey, [Query] int take = 100, [Query] int? skip = null);

        [Post("/api/sample")]
        Task SubmitBloodPressureMeasurementAsync([Body(BodySerializationMethod.Json)] params HealthSample[] samples);
    }
}
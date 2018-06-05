using System.Collections.Generic;
using System.Threading.Tasks;
using Clinician.ApiClients.HealthClient.Models;
using Refit;

namespace Clinician.ApiClients.HealthClient
{
    [Headers("Authorization: Bearer")]
    public interface IHealthDataClient
    {
        [Post("/query/v1")]
        Task<HealthDataQueryResponse> CreateQueryAsync([Body(BodySerializationMethod.Json)] HealthDataQueryRequest request, [Header("agency-query-token")] string agencyQueryToken = null);

        [Get("/query/v1")]
        Task<IEnumerable<HealthSample>> ExecuteQueryAsync([Query] string querykey, [Query] int take = 100, [Query] int? skip = null);

        [Post("/sample/v1")]
        Task SubmitBloodPressureMeasurementAsync([Body(BodySerializationMethod.Json)] params HealthSample[] samples);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinician.ApiClients.HealthClient.Models;
using Refit;

namespace Clinician.ApiClients.HealthClient
{
    [Headers("Authorization: Bearer", "api-version: 2.0")]
    public interface IHealthDataClient
    {
        [Get("/api/sample")]
        Task<HealthSamplesDto> ExecuteQueryAsync(
            [Header("agency-query-token")] string agencyQueryToken,
            [Query(CollectionFormat.Multi)] IEnumerable<string> types,
            [Query(Format = "O")] Dictionary<string, object> queryDateParams,
            [Query] HealthDataQueryRequest.OrderPropertyOptions orderProperty,
            [Query] HealthDataQueryRequest.OrderDirectionOptions orderDirection,
            [Query] int take,
            [Query] int? skip = null);
    }
}
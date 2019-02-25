using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Clinician.ApiClients.AgencyClient;
using Clinician.ApiClients.HealthClient;
using Clinician.ApiClients.HealthClient.Models;
using Clinician.Models;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Clinician.Services.Impl
{
    public class AgencySubjectQueryService : IAgencySubjectQueryService
    {
        private readonly IAgencyApiClient agencyApiClient;
        private readonly IHealthDataClient healthDataClient;
        private readonly ISampleMapper sampleMapper;
        private readonly ISampleDataTypeMapper sampleDataTypeMapper;
        private readonly IMemoryCache memoryCache;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AgencySubjectQueryService(IAgencyApiClient agencyApiClient, IHealthDataClient healthDataClient, ISampleMapper sampleMapper, ISampleDataTypeMapper sampleDataTypeMapper, IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor)
        {
            this.agencyApiClient = agencyApiClient;
            this.healthDataClient = healthDataClient;
            this.sampleMapper = sampleMapper;
            this.sampleDataTypeMapper = sampleDataTypeMapper;
            this.memoryCache = memoryCache;
            this.httpContextAccessor = httpContextAccessor;
        }

        public Task<AgencySubject> GetAgencySubjectAsync(Guid subject)
        {
            var agencySubject = RetrieveFromCache(subject);

            return Task.FromResult(agencySubject);
        }

        private string GetAgencySubjectCacheKey(string subject)
        {
            var identity = (this.httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity);
            var currentSubject = identity.FindFirst(JwtRegisteredClaimNames.Sid).Value;
            return $"{JwtClaimTypes.Subject}-{currentSubject}-{subject}";
        }

        public async Task<IEnumerable<AgencySubject>> GetAgencySubjectsAsync()
        {
            var claimsResponse = await this.agencyApiClient.ResolveClaimsAsync().ConfigureAwait(false);

            this.CacheAgencySubjects(claimsResponse);

            return claimsResponse;
        }

        private AgencySubject RetrieveFromCache(Guid subject)
        {
            if (this.memoryCache.TryGetValue(GetAgencySubjectCacheKey(subject.ToString()),
                out AgencySubject agencySubject))
            {
                return agencySubject;
            }

            return null;
        }

        private void CacheAgencySubjects(IEnumerable<AgencySubject> claimsResponse)
        {
            foreach (var subject in claimsResponse)
            {
                this.memoryCache.Set(this.GetAgencySubjectCacheKey(subject.Subject), subject, TimeSpan.FromMinutes(10));
            }
        }

        public async Task<IEnumerable<ISampleModel>> QueryAsync(SampleDataTypes type, AgencySubjectQueryParameters parameters)
        {
            var request = new AgencySubjectQueryTokenRequest(false, included: new[] {parameters.Subject});
            var response = await this.agencyApiClient.CreateQueryAsync(request);

            var agencyQueryToken = response.Value;

            if (string.IsNullOrWhiteSpace(agencyQueryToken))
            {
                throw new InvalidOperationException("Unable to complete health data query, agency-query-token is missing");
            }

            var requestDateParams = new Dictionary<string, object>
            {
                {"RangeOfStartDate", new HealthSample.HealthSampleDateRange(parameters.From, parameters.To)},
            };

            var queryResult = await this.healthDataClient.ExecuteQueryAsync(
                    agencyQueryToken,
                    this.sampleDataTypeMapper.GetHealthQueryTypesFor(type).ToArray(), 
                    requestDateParams,
                    HealthDataQueryRequest.OrderPropertyOptions.EndDate,
                    HealthDataQueryRequest.OrderDirectionOptions.Descending,
                    parameters.Take)
                .ConfigureAwait(false);

            var model = this.sampleMapper.Map(queryResult.Datas, type);

            return model;
        }
    }
}
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Clinician.ApiClients.HealthClient.Models
{
    public class HealthDataQueryRequest
    {
        [Required]
        [UIHint("CorrelationTypes")]
        public IEnumerable<string> Types { get; set; }
        
        [Required]
        public HealthSample.HealthSampleDateRange RangeOfStartDate { get; set; }
        public HealthSample.HealthSampleDateRange RangeOfEndDate { get; set; }

        public enum OrderPropertyOptions
        {
            StartDate,
            EndDate
        }

        public enum OrderDirectionOptions
        {
            Ascending,
            Descending
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public OrderPropertyOptions? OrderProperty { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public OrderDirectionOptions? OrderDirection { get; set; }

        public HealthDataQueryRequest()
        {
            
        }
        public HealthDataQueryRequest(HealthSample.HealthSampleDateRange startDateRange, HealthSample.HealthSampleDateRange endDateRange, params string[] types)
        {
            this.Types = types;
            this.RangeOfStartDate = startDateRange;
            this.RangeOfEndDate = endDateRange;
        }

        public HealthDataQueryRequest(HealthSample.HealthSampleDateRange dateRange, params string[] types)
        {
            this.Types = types;
            this.RangeOfStartDate = dateRange;
        }

        public HealthDataQueryRequest(DateTimeOffset lowerBound, DateTimeOffset upperBound, params string[] types) : this(new HealthSample.HealthSampleDateRange(lowerBound, upperBound), types)
        {
            
        }
    }
}
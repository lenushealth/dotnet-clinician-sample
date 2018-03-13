using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Converters;

namespace Clinician.ApiClients.HealthClient.Models
{
    public class HealthSample
    {
        public class HealthSampleDateRange
        {
            public HealthSampleDateRange()
            {
                
            }

            public HealthSampleDateRange(DateTimeOffset lowerBound, DateTimeOffset upperBound)
            {
                this.LowerBound = lowerBound;
                this.UpperBound = upperBound;
            }

            [Required]
            [DataType(DataType.DateTime)]
            [JsonConverter(typeof(IsoDateTimeConverter))]
            public DateTimeOffset LowerBound { get; set; }
            
            [Required]
            [DataType(DataType.DateTime)]
            [JsonConverter(typeof(IsoDateTimeConverter))]
            public DateTimeOffset UpperBound { get; set; }
        }

        public string Device { get; set; }
        public string ClientAssignedId { get; set; }
        public string Subject { get; set; }
        public string Type { get; set; }
        public decimal QuantityValue { get;set; }
        public string CategoryValue { get; set; }
        public IEnumerable<HealthSample> CorrelationObjects { get; set; } = new List<HealthSample>();

        public HealthSampleDateRange DateRange { get; set; }
    }
}
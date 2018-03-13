using System;
using System.Collections.Generic;

namespace Clinician.ApiClients.HealthClient.Models
{
    public class ValidationProblemDetails : ProblemDetails
    {
        public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>(StringComparer.Ordinal);
    }
}

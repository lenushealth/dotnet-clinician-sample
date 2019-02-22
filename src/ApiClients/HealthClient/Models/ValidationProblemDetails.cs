using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Clinician.ApiClients.HealthClient.Models
{
    public class ValidationProblemDetails : ProblemDetails
    {
        public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>(StringComparer.Ordinal);
    }
}

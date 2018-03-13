using System;
using System.Collections.Generic;

namespace Clinician.Models
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class RequiresScopeAttribute : Attribute
    {
        public RequiresScopeAttribute(params string[] scopes)
        {
            Scopes = scopes;
        }

        public IEnumerable<string> Scopes { get; }
    }
}

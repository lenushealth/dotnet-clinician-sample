using System;
using System.Collections.Generic;

namespace Clinician.Models
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class SampleDataTypeAttribute : Attribute
    {
        public IEnumerable<string> TypeNames { get; set; }

        public SampleDataTypeAttribute(params string[] typeNames)
        {
            this.TypeNames = typeNames;
        }
    }
}
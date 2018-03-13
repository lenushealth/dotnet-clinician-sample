using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Clinician.ApiClients.AgencyClient;
using Clinician.Models;
using Humanizer;

namespace Clinician.Services.Impl
{
    public class SampleDataTypeMapper : ISampleDataTypeMapper
    {
        private static IEnumerable<T> GetAttributesOn<T>(SampleDataTypes enumVal) where T : Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes<T>(false);
            return attributes;
        }

        public SampleDataTypes? ConvertToSampleDataType(string type)
        {
            if (Enum.TryParse<SampleDataTypes>(type.Dehumanize(), out var enumType))
            {
                return enumType;
            }

            return null;
        }

        public (string Name, SampleDataTypes Type, string Slug)? ConvertToSampleDataTypeModel(string type)
        {
            if (Enum.TryParse<SampleDataTypes>(type.Dehumanize(), out var enumType))
            {
                var name = GetAttributesOn<DescriptionAttribute>(enumType).FirstOrDefault()?.Description ?? enumType.Humanize();
                return (name, enumType, enumType.ToString("G").Kebaberize());
            }

            return null;
        }

        public IEnumerable<(string Name, SampleDataTypes Type, string Slug)> GetAvailableSampleDataTypes(IEnumerable<string> scopes = null)
        {
            return Enum.GetNames(typeof(SampleDataTypes)).Select(Enum.Parse<SampleDataTypes>)
                .Where(s => scopes != null && HasRequiredScopes(scopes, s))
                .Select(s =>
                {
                    var name = GetAttributesOn<DescriptionAttribute>(s).FirstOrDefault()?.Description ?? s.Humanize();
                    return (name, s, s.ToString().Kebaberize());
                })
                .ToList();
        }

        public IEnumerable<string> GetHealthQueryTypesFor(SampleDataTypes type)
        {
            var types = GetAttributesOn<SampleDataTypeAttribute>(type).SelectMany(s => s.TypeNames);
            if (types.Any())
            {
                return types;
            }

            return new [] { type.ToString("G") };
        }

        public bool HasRequiredScopes(IEnumerable<string> scopes, SampleDataTypes type)
        {
            var sampleScopes = GetAttributesOn<RequiresScopeAttribute>(type).SelectMany(s => s.Scopes);
            return sampleScopes.All(s => scopes.Contains(s));
        }
    }
}
using System.Collections.Generic;
using Clinician.ApiClients.AgencyClient;
using Clinician.Models;

namespace Clinician.Services.Impl
{
    public interface ISampleDataTypeMapper
    {
        IEnumerable<string> GetHealthQueryTypesFor(SampleDataTypes type);

        IEnumerable<(string Name, SampleDataTypes Type, string Slug)> GetAvailableSampleDataTypes(IEnumerable<string> scopes = null);
        (string Name, SampleDataTypes Type, string Slug)? ConvertToSampleDataTypeModel(string type);

        SampleDataTypes? ConvertToSampleDataType(string type);

        bool HasRequiredScopes(IEnumerable<string> scopes, SampleDataTypes type);
    }
}
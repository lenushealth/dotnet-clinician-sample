using System;
using System.Collections.Generic;
using System.Linq;
using Clinician.ApiClients.HealthClient.Models;
using Clinician.Models;

namespace Clinician.Services.Impl
{
    public class SampleMapper : ISampleMapper
    {
        private readonly ISampleDataTypeMapper sampleDataTypeMapper;

        public SampleMapper(ISampleDataTypeMapper sampleDataTypeMapper)
        {
            this.sampleDataTypeMapper = sampleDataTypeMapper;
        }

        private IEnumerable<BloodPressureSampleModel> BloodPressure(IEnumerable<HealthSample> samples)
        {
            var types = sampleDataTypeMapper.GetHealthQueryTypesFor(SampleDataTypes.BloodPressure).ToList();

            string TypeContaining(string s) => types.Single(t => t.Contains(s.ToLowerInvariant()));
            
            var correlationType = types.First();
            var diastolicType = TypeContaining(nameof(BloodPressureSampleModel.Diastolic));
            var systolicType = TypeContaining(nameof(BloodPressureSampleModel.Systolic));
            
            decimal QuantityValue(IEnumerable<HealthSample> healthSamples, string type) => 
                healthSamples.Single(hs => hs.Type == type).QuantityValue; 
            
            return ConstructModel(samples, correlationType, hs =>
            {
                if (hs.CorrelationObjects?.Count() != 2)
                {
                    return null;
                }

                return new BloodPressureSampleModel
                {
                    From = hs.DateRange.LowerBound,
                    Diastolic = QuantityValue(hs.CorrelationObjects, diastolicType),
                    Systolic = QuantityValue(hs.CorrelationObjects, systolicType),
                };
            });
        }

        private IEnumerable<BodyMassSampleModel> BodyMass(IEnumerable<HealthSample> samples)
        {
            return ConstructModel(samples, FirstType(SampleDataTypes.BodyMass), hs => new BodyMassSampleModel
            {
                From = hs.DateRange.LowerBound,
                Kg = hs.QuantityValue
            });
        }

        private IEnumerable<HeartRateSampleModel> HeartRate(IEnumerable<HealthSample> samples)
        {
            return ConstructModel(samples, FirstType(SampleDataTypes.HeartRate), hs => new HeartRateSampleModel
            {
                From = hs.DateRange.LowerBound,
                BeatsPerMinute = hs.QuantityValue
            });
        }

        private IEnumerable<HeightSampleModel> Height(IEnumerable<HealthSample> samples)
        {
            
            return ConstructModel(samples, FirstType(SampleDataTypes.Height), hs => new HeightSampleModel
            {
                From = hs.DateRange.LowerBound,
                Metres = hs.QuantityValue
            });
        }

        private IEnumerable<ISampleModel> PhysicalActivity(IEnumerable<HealthSample> samples)
        {
            return ConstructModel(samples, FirstType(SampleDataTypes.PhysicalActivity), 
                hs => new PhysicalActivitySampleModel
            {
                From = hs.DateRange.LowerBound,
                Steps = (int) Math.Round(hs.QuantityValue)
            });
        }

        private string FirstType(SampleDataTypes sampleDataTypes)
        {
            return sampleDataTypeMapper.GetHealthQueryTypesFor(sampleDataTypes).First();
        }

        private static IEnumerable<TModel> ConstructModel<TModel>(IEnumerable<HealthSample> samples, string type, 
            Func<HealthSample, TModel> factory) where TModel: ISampleModel
        {
            return samples
                .Where(s => s.Type == type)
                .Select(factory)
                .Where(s => s != null)
                .OrderByDescending(s => s.From)
                .ToList();
        }

        public IEnumerable<ISampleModel> Map(IEnumerable<HealthSample> samples, SampleDataTypes type)
        {
            switch (type)
            {
                case SampleDataTypes.BloodPressure:
                    return BloodPressure(samples);
                case SampleDataTypes.BodyMass:
                    return BodyMass(samples);
                case SampleDataTypes.HeartRate:
                    return HeartRate(samples);
                case SampleDataTypes.Height:
                    return Height(samples);
                case SampleDataTypes.PhysicalActivity:
                    return PhysicalActivity(samples);
                default:
                    throw new NotSupportedException($"Unable to map samples for {type}");
            }
        }
    }
}
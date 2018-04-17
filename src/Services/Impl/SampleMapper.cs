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
            var type = this.sampleDataTypeMapper.GetHealthQueryTypesFor(SampleDataTypes.BloodPressure)?.FirstOrDefault();
            var diastolic = this.sampleDataTypeMapper.GetHealthQueryTypesFor(SampleDataTypes.BloodPressure).FirstOrDefault(t => t.Contains("diastolic"));
            var systolic = this.sampleDataTypeMapper.GetHealthQueryTypesFor(SampleDataTypes.BloodPressure).FirstOrDefault(t => t.Contains("systolic"));

            var model = samples
                .Where(s => s.Type == type)
                .Select(s =>
                {
                    if (s.CorrelationObjects != null && s.CorrelationObjects.Count() == 2)
                    {
                        var reading = new BloodPressureSampleModel()
                        {
                            From = s.DateRange.LowerBound,
                            Diastolic = s.CorrelationObjects.SingleOrDefault(x => x.Type == diastolic)
                                .QuantityValue,
                            Systolic = s.CorrelationObjects.SingleOrDefault(x => x.Type == systolic)
                                .QuantityValue
                        };
                        return reading;
                    }

                    return null;
                })
                .Where(s => s != null)
                .OrderByDescending(s => s.From)
                .ToList();

            return model;
        }

        private IEnumerable<BodyMassSampleModel> BodyMass(IEnumerable<HealthSample> samples)
        {
            var type = this.sampleDataTypeMapper.GetHealthQueryTypesFor(SampleDataTypes.BodyMass)?.FirstOrDefault();

            var model = samples
                .Where(s => s.Type == type)
                .Select(s =>
                {
                    var reading = new BodyMassSampleModel
                    {
                        From = s.DateRange.LowerBound,
                        Kg = s.QuantityValue
                    };
                    return reading;
                })
                .Where(s => s != null)
                .OrderByDescending(s => s.From)
                .ToList();

            return model;
        }

        private IEnumerable<HeartRateSampleModel> HeartRate(IEnumerable<HealthSample> samples)
        {
            var type = this.sampleDataTypeMapper.GetHealthQueryTypesFor(SampleDataTypes.HeartRate)?.FirstOrDefault();

            var model = samples
                .Where(s => s.Type == type)
                .Select(s =>
                {
                    var reading = new HeartRateSampleModel
                    {
                        From = s.DateRange.LowerBound,
                        BeatsPerSecond = s.QuantityValue
                    };
                    return reading;
                })
                .Where(s => s != null)
                .OrderByDescending(s => s.From)
                .ToList();

            return model;
        }

        private IEnumerable<HeightSampleModel> Height(IEnumerable<HealthSample> samples)
        {
            var type = this.sampleDataTypeMapper.GetHealthQueryTypesFor(SampleDataTypes.Height)?.FirstOrDefault();

            var model = samples
                .Where(s => s.Type == type)
                .Select(s =>
                {
                    var reading = new HeightSampleModel
                    {
                        From = s.DateRange.LowerBound,
                        Metres = s.QuantityValue
                    };
                    return reading;
                })
                .Where(s => s != null)
                .OrderByDescending(s => s.From)
                .ToList();

            return model;
        }

        private IEnumerable<PhysicalActivitySampleModel> PhysicalActivity(IEnumerable<HealthSample> samples)
        {
            var type = this.sampleDataTypeMapper.GetHealthQueryTypesFor(SampleDataTypes.PhysicalActivity)?.FirstOrDefault();

            var model = samples
                .Where(s => s.Type == type)
                .Select(s =>
                {
                    var reading = new PhysicalActivitySampleModel
                    {
                        From = s.DateRange.LowerBound,
                        Steps = (int)Math.Round(s.QuantityValue)
                    };
                    return reading;
                })
                .Where(s => s != null)
                .OrderByDescending(s => s.From)
                .ToList();

            return model;
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
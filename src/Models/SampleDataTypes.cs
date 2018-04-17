using System.ComponentModel;

namespace Clinician.Models
{

    public enum SampleDataTypes
    {
        [SampleDataType("blood_pressure", "blood_pressure_systolic", "blood_pressure_diastolic")]
        [RequiresScope("read.blood_pressure", "read.blood_pressure.blood_pressure_systolic", "read.blood_pressure.blood_pressure_diastolic")]
        [Description("Blood Pressure")]
        BloodPressure,
        
        [SampleDataType("body_mass")] 
        [Description("BodyMass (Weight in KG)")]
        [RequiresScope("read.body_mass")]
        BodyMass,
        
        [SampleDataType("height")] 
        [Description("Height in Metres")]
        [RequiresScope("read.height")]
        Height,
        
        [SampleDataType("step_count")] 
        [RequiresScope("read.step_count")]
        PhysicalActivity,
        
        [SampleDataType("heart_rate")] 
        [RequiresScope("read.heart_rate")]
        HeartRate
    }
}

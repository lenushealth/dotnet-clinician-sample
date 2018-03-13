using System.ComponentModel;

namespace Clinician.Models
{

    public enum SampleDataTypes
    {
        [SampleDataType("BloodPressure", "BloodPressureSystolic", "BloodPressureDiastolic")]
        [RequiresScope("read.blood_pressure", "read.blood_pressure.blood_pressure_systolic", "read.blood_pressure.blood_pressure_diastolic")]
        [Description("Blood Pressure")]
        BloodPressure,
        
        [SampleDataType("BodyMass")] 
        [Description("BodyMass (Weight in KG)")]
        [RequiresScope("read.body_mass")]
        BodyMass,
        
        [SampleDataType("Height")] 
        [Description("Height in Metres")]
        [RequiresScope("read.height")]
        Height,
        
        [SampleDataType("StepCount")] 
        [RequiresScope("read.step_count")]
        PhysicalActivity,
        
        [SampleDataType("HeartRate")] 
        [RequiresScope("read.heart_rate")]
        HeartRate
    }
}

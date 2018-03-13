using System;
using System.ComponentModel.DataAnnotations;
using Clinician.Units;

namespace Clinician.Models
{

    public class BloodPressureSampleModel : ISampleModel
    {
        [DisplayFormat(DataFormatString = "{0:G}")]
        [DataType(DataType.DateTime)]
        [Required]
        public DateTimeOffset From { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:mmHg}")]
        [Required]
        public BloodPressureUnit Systolic { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:mmHg}")]
        [Required]
        public BloodPressureUnit Diastolic { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace Clinician.Models
{
    public class PhysicalActivitySampleModel : ISampleModel
    {
        [DisplayFormat(DataFormatString = "{0:G}")]
        [DataType(DataType.DateTime)]
        [Required]
        public DateTimeOffset From { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:D} Steps")]
        [Required]
        public int Steps { get; set; }
    }
}

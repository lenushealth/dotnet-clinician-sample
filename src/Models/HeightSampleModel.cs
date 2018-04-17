using System;
using System.ComponentModel.DataAnnotations;

namespace Clinician.Models
{
    public class HeightSampleModel : ISampleModel
    {
        [DisplayFormat(DataFormatString = "{0:G}")]
        [DataType(DataType.DateTime)]
        [Required]
        public DateTimeOffset From { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:D} Metres")]
        [Required]
        public decimal Metres { get; set; }
    }
}

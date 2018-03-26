using System;
using System.ComponentModel.DataAnnotations;

namespace Clinician.Models
{
    public class HeartRateSampleModel : ISampleModel
    {
        [DisplayFormat(DataFormatString = "{0:G}")]
        [DataType(DataType.DateTime)]
        [Required]
        public DateTimeOffset From { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:D} Beats per Minute")]
        [Required]
        public decimal BeatsPerSecond { get; set; }
    }
}

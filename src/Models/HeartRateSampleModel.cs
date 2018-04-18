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
        
<<<<<<< HEAD
        [DisplayFormat(DataFormatString = "{0:D} Beats per Minute")]
=======
        [DisplayFormat(DataFormatString = "{0} Beats per Minute", NullDisplayText = "N/A")]
>>>>>>> develop
        [Required]
        public decimal BeatsPerMinute { get; set; }
    }
}

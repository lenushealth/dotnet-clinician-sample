using System;
using System.ComponentModel.DataAnnotations;

namespace Clinician.Models
{
    public class BodyMassSampleModel : ISampleModel
    {
        [DisplayFormat(DataFormatString = "{0:G}")]
        [DataType(DataType.DateTime)]
        [Required]
        public DateTimeOffset From { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:N} Kg")]
        [Required]
        public decimal Kg { get; set; }
    }
}

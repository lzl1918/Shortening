using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shortening.Models
{

    public class MappedItem
    {
        [Required, Key]
        [MaxLength(16)]
        public string Alias { get; set; }

        [Required]
        [MaxLength(128)]
        public string Value { get; set; }


        [Required]
        public DateTime CreatedTime { get; set; }
    }
}

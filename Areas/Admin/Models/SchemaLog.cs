using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shivyshine.Areas.Admin.Models
{
    public abstract class SchemaLog
    {
        [Display(Order = 101)]
        [Column(TypeName = "nvarchar(100)")]
        public string CreatedBy { get; set; }

        [Display(Order = 102)]
        public DateTime CreatedDate { get; set; }

        [Display(Order = 103)]
        [Column(TypeName = "nvarchar(100)")]
        public string UpdatedBy { get; set; }

        [Display(Order = 104)]
        public DateTime? UpdatedDate { get; set; }

        [NotMapped]
        public string ReturnUrl { get; set; }

        [NotMapped]
        public string LogMessage { get; set; }
    }

}
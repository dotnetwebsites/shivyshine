using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shivyshine.Areas.Admin.Models;

namespace Shivyshine.Models
{
    public class ProductReview : SchemaLog
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UnitId { get; set; }
        public int ShadeId { get; set; }

        [Required(ErrorMessage = "Please enter name")]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter email address")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [DataType(DataType.EmailAddress)]
        [Column(TypeName = "nvarchar(100)")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter ReviewText")]
        [EmailAddress(ErrorMessage = "Invalid ReviewText")]
        [DataType(DataType.Text)]
        [Column(TypeName = "nvarchar(1000)")]
        public string ReviewText { get; set; }

        public int Rating { get; set; }
    }

    public class ProductReviewView
    {
        public int ProductId { get; set; }
        public int UnitId { get; set; }
        public int ShadeId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string ReviewText { get; set; }

        public int Rating { get; set; }
    }
}
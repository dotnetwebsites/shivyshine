using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Shivyshine.Areas.Admin.Models
{
    public class City : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please choose State")]
        public int StateId { get; set; }

        [Display(Name = "City Code")]
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please enter City Code")]
        public string CityCode { get; set; }

        [Display(Name = "City")]
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please enter City")]
        public string CityName { get; set; }

        [Display(Name = "Short Name")]
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please enter Short Name")]
        public string ShortName { get; set; }
        public State State { get; set; }
    }

    public class ExportCity
    {
        public int Id { get; set; }
        public string StateName { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public string ShortName { get; set; }        
    }

    public class CityView : City
    {
        [Display(Name = "State")]
        public string StateName { get; set; }
    }
}
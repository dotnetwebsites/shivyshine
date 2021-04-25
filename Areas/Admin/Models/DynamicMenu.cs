using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shivyshine.Areas.Admin.Models
{
    public class DynamicMenu : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter Menu name")]
        [Column(TypeName = "nvarchar(100)")]
        [DataType(DataType.Text)]
        [Display(Name = "Menu Name")]
        public string MenuName { get; set; }

        [Display(Name = "Parent Menu")]
        public int? ParentId { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [DataType(DataType.Text)]
        [Display(Name = "Area Section")]
        public string Area { get; set; }

        [Required(ErrorMessage = "Please enter Menu URL")]
        [DataType(DataType.Text)]
        [Column(TypeName = "nvarchar(500)")]
        [Display(Name = "Menu URL")]
        public string MenuUrl { get; set; }

        [Display(Name = "Order")]
        public int? MenuOrder { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [DataType(DataType.Text)]
        [Display(Name = "Menu Icon")]
        public string MenuIcon { get; set; }

        public bool IsActive { get; set; }

        public bool IsAuth { get; set; }
    }

    public class DynamicMenuView : DynamicMenu
    {
        public string ParentMenuName { get; set; }
    }
}
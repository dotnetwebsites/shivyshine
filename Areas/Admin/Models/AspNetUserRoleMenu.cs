using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shivyshine.Areas.Admin.Models
{
    public class AspNetUserRoleMenu
    {
        [Key]
        [Column(Order = 1)]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(900)", Order = 2)]
        public string RoleId { get; set; }

        [Column(TypeName = "nvarchar(900)", Order = 3)]
        public int MenuId { get; set; }

    }
}
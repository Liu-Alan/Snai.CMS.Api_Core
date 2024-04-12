using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snai.CMS.Api_Core.Entities.CMS
{
    [Table("role_module")]
    public class RoleModule
    {
        public RoleModule()
        {
            ID = 0;
            RoleID = 0;
            ModuleID = 0;
        }

        [Key]
        [Column("id")]
        public int ID { get; set; }

        [Column("role_id")]
        public int RoleID { get; set; }

        [Column("module_id")]
        public int ModuleID { get; set; }
    }
}

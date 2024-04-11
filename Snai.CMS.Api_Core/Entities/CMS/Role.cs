using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snai.CMS.Api_Core.Entities.CMS
{
    [Table("roles")]
    public class Role
    {
        public Role()
        {
            ID = 0;
            Title = "";
            State = 1;
        }

        [Key]
        [Column("id")]
        public int ID { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("state")]
        public short State { get; set; }
    }
}

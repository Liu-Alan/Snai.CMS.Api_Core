using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snai.CMS.Api_Core.Entities.CMS
{
    [Table("tokens")]
    public class Token
    {
        public Token()
        {
            ID = 0;
            TokenStr = "";
            UserID = "";
            State = 1;
            CreateTime = 0;
        }

        [Key]
        [Column("id")]
        public int ID { get; set; }

        [Column("token")]
        public string TokenStr { get; set; }

        [Column("user_id")]
        public string UserID { get; set; }

        [Column("state")]
        public short State { get; set; }

        [Column("create_time")]
        public int CreateTime { get; set; }
    }
}

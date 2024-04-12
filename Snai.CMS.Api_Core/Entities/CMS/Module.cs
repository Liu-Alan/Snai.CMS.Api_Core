using Microsoft.AspNetCore.Components.Routing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snai.CMS.Api_Core.Entities.CMS
{
    [Table("modules")]
    public class Module
    {
        public Module()
        {
            ID = 0;
            ParentID = 0;
            Title = "";
            Router = "";
            Sort = 0;
            State = 1;
        }

        [Key]
        [Column("id")]
        public int ID { get; set; }

        [Column("parent_id")]
        public int ParentID { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("router")]
        public string Router { get; set; }

        [Column("sort")]
        public int Sort { get; set; }

        [Column("state")]
        public short State { get; set; }
    }
}

﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snai.CMS.Api_Core.Entities.CMS
{
    [Table("admins")]
    public class Admin
    {
        public Admin()
        {
            ID = 0;
            UserName = "";
            Password = "";
            RoleID = 0;
            State = 1;
            CreateTime = 0;
            UpdateTime = 0;
            LastLogonTime = 0;
            LastLogonIP = "";
            ErrorLogonTime = 0;
            ErrorLogonCount = 0;
            LockTime = 0;
        }

        [Key]
        [Column("id")]
        public int ID { get; set; }

        [Column("user_name")]
        public string UserName { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("role_id")]
        public int RoleID { get; set; }

        [Column("state")]
        public short State { get; set; }

        [Column("create_time")]
        public int CreateTime { get; set; }

        [Column("update_time")]
        public int UpdateTime { get; set; }

        [Column("last_logon_time")]
        public int LastLogonTime { get; set; }

        [Column("last_logon_ip")]
        public string LastLogonIP { get; set; }

        [Column("error_logon_time")]
        public int ErrorLogonTime { get; set; }

        [Column("error_logon_count")]
        public int ErrorLogonCount { get; set; }

        [Column("lock_time")]
        public int LockTime { get; set; }
    }
}

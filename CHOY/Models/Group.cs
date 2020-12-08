using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Models
{
    public class Group
    {
        [Key]
        [Column(Order = 1)]
        [DisplayName("群組編號")]
        [Required]
        [RegularExpression("G[0-9]{4}")]
        public string GroupID { get; set; } = "G0000";

        [DisplayName("群組名稱")]
        [Required(ErrorMessage = "請輸入群組名稱")]
        public string GroupName { get; set; }

        [Key]
        [Column(Order = 2)]
        [ForeignKey("Member")]
        [DisplayName("群組擁有者")]
        public string MemberID { get; set; }

        
        public virtual Member Member { get; set; }
        public virtual ICollection<GroupMember> GroupMember { get; set; }



    }
}


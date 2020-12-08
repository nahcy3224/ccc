using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHOY.Models
{
    public class GroupMember
    {
        [Key]
        [Column(Order = 1)]
        [DisplayName("群組編號")]
        [ForeignKey("Group")]
        [Required]
        [RegularExpression("G[0-9]{4}")]
        public string GroupID { get; set; } = "G0000";

        [Key]
        [Column(Order = 3)]
        [DisplayName("群組內的成員")]
        [ForeignKey("Member")]
        [Required]
        [RegularExpression("M[0-9]{4}")]
        public string MemberIDInGroup { get; set; }

        [Key]        
        [Column(Order = 2)]
        [DisplayName("群組擁有者")]
        [ForeignKey("Group")]
        [Required]
        [RegularExpression("M[0-9]{4}")]
        public string MemberIDOwner { get; set; }

        public virtual Group Group { get; set; }
        public virtual Member Member { get; set; }


    }
}
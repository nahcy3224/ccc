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
    public class MemberJoinProject
    {
        [Key, Column(Order = 2)]
        [ForeignKey("Project")]
        [DisplayName("專案擁有者")]
        [Required]
        [RegularExpression("M[0-9]{4}")]
        public string MemberIDOwner { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Project")]
        [DisplayName("專案編號")]
        [Required]
        [RegularExpression("P[0-9]{4}")]
        public string ProjectID { get; set; }

        [Key, Column(Order = 3)]
        [DisplayName("專案參與者")]
        [ForeignKey("Member")]
        [Required]
        [RegularExpression("M[0-9]{4}")]
        public string MemberIDJoin { get; set; }

        [DisplayName("專案權限代碼")]
        public Share SharePerID { get; set; }

        public virtual Member Member { get; set; }
        public virtual Project Project { get; set; }
    }
}

[Flags]
public enum Share
{
    [Display(Name = "無權限 ")]
    No = 0,  // 0b_0000_0000

    [Display(Name = "編輯 ")]
    Edit = 1,  //  0b_0000_0001

    [Display(Name = "分享 ")]
    Share = 2,  // 0b_0000_0010

}
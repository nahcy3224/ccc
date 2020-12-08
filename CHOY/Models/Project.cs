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
    public class Project
    {
        [Key]
        [DisplayName("專案編號")]
        [Column(Order = 1)]
        [Required]
        [RegularExpression("P[0-9]{4}", ErrorMessage = "專案編號格式有誤")]
        public string ProjectID { get; set; } = "P0000";

        [DisplayName("專案名稱")]
        [Required(ErrorMessage = "請輸入專案名稱")]
        [StringLength(15, ErrorMessage = "專案名稱最多15個字")]
        public string ProjectName { get; set; }

        [DisplayName("建立時間")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = false)]
        public System.DateTime CreateAt { get; set; } = DateTime.Now;


        [DisplayName("刪除時間")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{00:00:00:yyyy/MM/dd}", ApplyFormatInEditMode = false)]
        public Nullable<System.DateTime> DeleteAt { get; set; }

        [Key]
        [Column(Order = 2)]
        [ForeignKey("Member")]
        [DisplayName("會員編號")]
        [Required]
        [RegularExpression("M[0-9]{4}")]
        public string MemberID { get; set; }

        public virtual ICollection<Board> Board { get; set; }
        public virtual Member Member { get; set; }
        public virtual ICollection<MemberJoinProject> MemberJoinProject { get; set; }
        public virtual ICollection<Vote> Vote { get; set; }
    }
}
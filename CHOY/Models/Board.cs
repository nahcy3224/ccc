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
    public class Board
    {
        [Key]
        [DisplayName("白板編號")]
        [Required]
        [RegularExpression("B[0-9]{4}")]
        public string BoardID { get; set; } = "B0000";

        [ForeignKey("Project")]
        [Column(Order = 1)]
        [DisplayName("專案編號")]
        [Required]
        [RegularExpression("P[0-9]{4}")]
        public string ProjectID { get; set; }

        [ForeignKey("Project")]
        [Column(Order = 2)]
        [DisplayName("專案擁有者")]
        [Required]
        [RegularExpression("M[0-9]{4}")]
        public string MemberIDOwner { get; set; }

        [DisplayName("刪除時間")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{00:00:00:yyyy/MM/dd}", ApplyFormatInEditMode = false)]
        public Nullable<System.DateTime> DeleteAt { get; set; }


        [DisplayName("白板內容")]
        public string Code { get; set; }


        public virtual Project Project { get; set; }

    }
}
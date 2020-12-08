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
    public class Vote
    {

        [Key]
        [DisplayName("投票編號")]
        [Required]
        [RegularExpression("V[0-9]{4}")]
        public string VoteID { get; set; } = "V0000";

        [DisplayName("投票名稱")]
        [Required]
        [StringLength(20, ErrorMessage = "投票名稱最多20個字")]
        public string VoteName { get; set; }

        [DisplayName("投票結果")]
        public string Result { get; set; }

        [DisplayName("總投票數")]
        public Nullable<int> VoteCount { get; set; }


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

        public virtual Project Project { get; set; }
        public virtual ICollection<VoteRecords> VoteRecords { get; set; }
    }
}
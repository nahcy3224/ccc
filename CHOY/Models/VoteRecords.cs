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
    public class VoteRecords
    {
        [ForeignKey("Vote")]
        [Key]
        [DisplayName("投票編號")]
        [Column(Order = 1)]
        [Required]
        [RegularExpression("V[0-9]{4}")]
        public string VoteID { get; set; }

        [Key]
        [DisplayName("選項編號")]
        [Column(Order = 2)]
        [Required]
        [RegularExpression("C[0-9]{4}")]
        public string ChoiceID { get; set; } = "C0000";

        [DisplayName("選項")]
        [Required(ErrorMessage = "請輸入選項名稱")]
        [StringLength(20, ErrorMessage = "投票選項最多20個字")]
        public string Choice { get; set; }

        [DisplayName("投票數")]
        public Nullable<int> VoteCounts { get; set; }

        public virtual Vote Vote { get; set; }

    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHOY.Models
{
    public class VoteRecords
    {
        [ForeignKey("Vote")]
        [DisplayName("投票編號")]
        public int VoteID { get; set; }

        [Key]
        [DisplayName("選項編號")]
        public int ChoiceID { get; set; }

        [DisplayName("選項")]
        [Required(ErrorMessage = "請輸入選項名稱")]
        [StringLength(20, ErrorMessage = "投票選項最多20個字")]
        public string Choice { get; set; }

        [DisplayName("投票數")]
        public Nullable<int> VoteCounts { get; set; } = 0;

        public virtual Vote Vote { get; set; }

    }
}
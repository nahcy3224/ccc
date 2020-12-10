using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace CHOY.Models
{
    public class MemberExcel
    {
        //[DefaultValue("M0000")]
        [Key]
        [DisplayName("會員編號")]
        // [RegularExpression("M[0-9]{4}", ErrorMessage = "會員編號格式有誤")]
        public string MemberID { get; set; } = "M0000";

        
        [DisplayName("帳號")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }


        [DisplayName("暱稱")]
        [StringLength(15, ErrorMessage = "暱稱最多15個字")]
        public string NickName { get; set; }

        [DisplayName("性別")]
        public string Gender { get; set; }


        [DisplayName("生日")]
        public System.DateTime?Birthday { get; set; }

        [DisplayName("備用電子郵件")]
        [DataType(DataType.EmailAddress)]
        public string ContactEmail { get; set; }

        [DisplayName("建立的時間")]
        [DataType(DataType.Date)]

        public System.DateTime?CreateAt { get; set; } = DateTime.Now;

        [DisplayName("權限代碼")]
        public Permissions PerCode { get; set; }

        [DisplayName("是否停權")]
        public string IsSuspended { get; set; } 



    }
}
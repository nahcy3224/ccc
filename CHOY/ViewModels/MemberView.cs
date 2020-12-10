using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CHOY.ViewModels
{
    public class MemberView
    {
        [DisplayName("會員編號")]
        public string MemberID { get; set; } 

        [DisplayName("帳號")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DisplayName("暱稱")]
        [StringLength(15, ErrorMessage = "暱稱最多15個字")]
        public string NickName { get; set; }
    }
}
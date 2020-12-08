using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Demo.Models
{
    public class Bulletin: IValidatableObject
    {
        [Key]
        [DisplayName("公告編號")]
        [Required]
        [RegularExpression("N[0-9]{4}")]
        public string BulletinID { get; set; } = "N0000";

        [DisplayName("編輯時間")]
        [DataType(DataType.DateTime)]
        //[DisplayFormat(DataFormatString = "{00:00:00:yyyy/MM/dd}", ApplyFormatInEditMode = false)]
        public System.DateTime EditTime { get; set; } = DateTime.Now;

        [DisplayName("發佈開始時間")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:00:00:yyyy/MM/dd}", ApplyFormatInEditMode = false)]
        public System.DateTime PublishStart { get; set; }

        [DisplayName("發佈結束時間")]
        [DataType(DataType.DateTime)]
       [DisplayFormat(DataFormatString = "{0:00:00:yyyy/MM/dd}", ApplyFormatInEditMode = false)]
        public System.DateTime PublishEnd { get; set; }

        [DisplayName("公告內容")]
        public string Content { get; set; }



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> results = new List<ValidationResult>();

            if (PublishStart < DateTime.Now)
            {
                results.Add(new ValidationResult("發佈開始時間必須大於現在時間", new[] { "PublishStart" }));
            }

            if (PublishEnd <= PublishStart)
            {
                results.Add(new ValidationResult("發佈結束時間必須大於發佈開始時間", new[] { "PublishEnd" }));
            }

            return results;
        }


    }
}
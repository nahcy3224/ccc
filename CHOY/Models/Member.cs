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
    public class Member
    {
        //[DefaultValue("M0000")]
        [Key]
        [DisplayName("會員編號")]
        // [RegularExpression("M[0-9]{4}", ErrorMessage = "會員編號格式有誤")]
        public string MemberID { get; set; } = "M0000";

        
        [DisplayName("帳號")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DisplayName("密碼")]
        [StringLength(64)]
        [DataType(DataType.Password)]
        public string Psw { get; set; }

        [DisplayName("暱稱")]
        [StringLength(15, ErrorMessage = "暱稱最多15個字")]
        public string NickName { get; set; }

        [DisplayName("性別")]
        public bool Gender { get; set; }


        [DisplayName("生日")]
        [DataType(DataType.Date)]
        public System.DateTime Bday { get; set; }

        [DisplayName("備用電子郵件")]
        [DataType(DataType.EmailAddress)]
        public string ContactEmail { get; set; }

        [DisplayName("建立的時間")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = false)]
        public System.DateTime CreateAt { get; set; } = DateTime.Now;

        [DisplayName("大頭照")]
        public byte[] ProfilePic { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string ImageMimeType { get; set; }

        [DisplayName("權限代碼")]
        public Permissions PerCode { get; set; }

        [DisplayName("是否停權")]
        public bool IsSuspended { get; set; } = false;

        [DisplayName("最後上線時間")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{00:00:00:yyyy/MM/dd}", ApplyFormatInEditMode = false)]
        public Nullable<System.DateTime> LastLogInTime { get; set; }


        public virtual ICollection<Group> Group { get; set; }
        public virtual ICollection<MemberJoinProject> MemberJoinProject { get; set; }
        public virtual ICollection<Project> Project { get; set; }



    }
}
public static class BoolExtensions
{
    public static string ToGender(this bool value)
    {
        return value ? "男" : "女";
    }
}

[Flags]
public enum Permissions
{
    [Display(Name = "一般會員 ")]
    General = 0,  // 0b_0000_0000

    [Display(Name = "管理公告 ")]
    Bulletin = 1,  //  0b_0000_0001

    [Display(Name = "管理會員權限 ")]
    Suspension =2 ,  // 0b_0000_0010

    [Display(Name = "會員資料下載 ")]
    Download = 4,  // 0b_0000_0100

    [Display(Name = "管理權限 ")]
    Manager =8  //  0b_0000_1000
                            //Create = 0b_0001_0000,  // 16
                            //Saturday = 0b_0010_0000,  // 32
                            //Sunday = 0b_0100_0000,  // 64
                            //All = Layout | Member | Download | Permit | Create

}
//public static class EnumExtensions
//{
//    public static string GetDisplayName(this Enum enumValue)
//    {
//        return enumValue.GetType()
//                        .GetMember(enumValue.ToString())
//                        .First()
//                        .GetCustomAttribute<DisplayAttribute>()
//                        .GetName();
//    }
//}
public static class EnumExtensions
{
    public static string GetDisplayName(this Enum enumValue)
    {
        var enumType = enumValue.GetType();
        var names = new List<string>();
        foreach (var e in Enum.GetValues(enumType))
        {
            var flag = (Enum)e;
            if (enumValue.HasFlag(flag))
            {
                names.Add(GetSingleDisplayName(flag));
            }
        }
        if (names.Count <= 0) throw new ArgumentException();
        if (names.Count == 1) return names.First();
        return string.Join(",", names);
    }
    public static string GetSingleDisplayName(this Enum flag)
    {
        try
        {
            return flag.GetType()
                    .GetMember(flag.ToString())
                    .First()
                    .GetCustomAttribute<DisplayAttribute>()
                    .Name;
        }
        catch
        {
            return flag.ToString();
        }
    }
}


public class FlagsEnumExample
{
    public static void Main()
    {
        Permissions all = Permissions.General | Permissions.Bulletin | Permissions.Suspension | Permissions.Download | Permissions.Manager;
        Console.WriteLine(all);

        Permissions noLayout = Permissions.General | Permissions.Suspension | Permissions.Download | Permissions.Manager;
        Console.WriteLine(noLayout);

        Permissions noMember = Permissions.General | Permissions.Bulletin |  Permissions.Download | Permissions.Manager;
        Console.WriteLine(noMember);

        Permissions noDownload = Permissions.General | Permissions.Bulletin | Permissions.Suspension | Permissions.Manager;
        Console.WriteLine(noDownload);

        Permissions noPermit = Permissions.General | Permissions.Bulletin | Permissions.Suspension |  Permissions.Download;
        Console.WriteLine(noPermit);

        Permissions ten = Permissions.General | Permissions.Suspension | Permissions.Manager;
        Console.WriteLine(ten);

        Permissions nine = Permissions.General | Permissions.Bulletin | Permissions.Manager;
        Console.WriteLine(nine);

        Permissions senven = Permissions.General | Permissions.Bulletin | Permissions.Suspension | Permissions.Download;
        Console.WriteLine(senven);

        Permissions six = Permissions.General | Permissions.Suspension | Permissions.Download;
        Console.WriteLine(six);

        Permissions five = Permissions.General | Permissions.Bulletin | Permissions.Download;
        Console.WriteLine(five);

        Permissions three = Permissions.General | Permissions.Bulletin | Permissions.Suspension;
        Console.WriteLine(three);
    }
}
    // Output:
    // Layout | Member | Download | Permit

//        Permissions workingFromHomeDays = Permissions.Thursday | Permissions.Friday;
//        Console.WriteLine($"Join a meeting by phone on {meetingDays & workingFromHomeDays}");
//        // Output:
//        // Join a meeting by phone on Friday

//        bool isMeetingOnTuesday = (meetingDays & Permissions.Tuesday) == Permissions.Tuesday;
//        Console.WriteLine($"Is there a meeting on Tuesday: {isMeetingOnTuesday}");
//        // Output:
//        // Is there a meeting on Tuesday: False

//        var a = (Permissions)37;
//        Console.WriteLine(a);
//        // Output:
//        // Monday, Wednesday, Saturday
//    }
//}

using System;

namespace CHOY.Models.ModelBinders
{
    public class ApiAuthVerifyEmailAddress
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool Gender { get; set; }
        public DateTime Birthday { get; set; }
    }
}
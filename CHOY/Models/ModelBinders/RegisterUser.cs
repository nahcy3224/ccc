using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CHOY.Models.ModelBinders
{
    public class RegisterUser
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool Gender { get; set; }
        public DateTime Birthday { get; set; }
    }
}
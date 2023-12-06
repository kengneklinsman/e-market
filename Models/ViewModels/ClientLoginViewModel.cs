using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Emarketing.ViewModels
{
    public class ClientLoginViewModel
    {
        [Required(ErrorMessage = "Username or Email is required.")]
        public string name { get; set; }

        [Required(ErrorMessage = "username or email is required.")]
        public string email { get; set; }
    }
}
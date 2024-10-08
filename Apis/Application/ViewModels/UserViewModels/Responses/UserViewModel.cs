﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.UserViewModels.Responses
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Fullname { get; set; }
        public string? Avatar { get; set; }
        public DateTime? Birthday { get; set; }
        public IList<string> Roles { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsLockout { get; set; } = false;
    }
}

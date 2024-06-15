using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.UserViewModels.Requests
{
    public class UserRequestModel
    {
        public string Username { get; set; }
        public string Fullname { get; set; }
        public DateTime? Birthday { get; set; }
        [NotMapped]
        public IFormFile? Avatar { get; set; }
        public string PhoneNumber { get; set; }
    }
}

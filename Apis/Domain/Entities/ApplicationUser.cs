
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ApplicationUser: IdentityUser
    {
        public string Fullname { get; set; }
        public string? Avatar { get; set; }
        public DateTime? Birthday { get; set; }
        [JsonIgnore]
        public virtual Mentor? Mentor { get; set; }
        public virtual Learner? Learner { get; set; }
    }
}

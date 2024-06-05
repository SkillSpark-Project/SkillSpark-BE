using Application.Interfaces;
using System.Security.Claims;

namespace WebAPI.Services
{
    public class ClaimsService : IClaimsService
    {
        public ClaimsService(IHttpContextAccessor httpContextAccessor)
        {
            // todo implementation to get the current userId
            var Id = httpContextAccessor.HttpContext?.User?.FindFirstValue("userId");
            var isAdmin = httpContextAccessor.HttpContext?.User?.FindFirstValue("isAdmin");
            var isMentor = httpContextAccessor.HttpContext?.User?.FindFirstValue("isMentor");
            var isLearner = httpContextAccessor.HttpContext?.User?.FindFirstValue("isLearner");


            GetCurrentUserId = string.IsNullOrEmpty(Id) ? Guid.Empty : Guid.Parse(Id);
            if (string.IsNullOrEmpty(isAdmin))
                GetIsAdmin = false;
            else if (isAdmin.Equals("True")) GetIsAdmin = true;
            else GetIsAdmin = false;

            if (string.IsNullOrEmpty(isMentor))
                GetIsMentor = false;
            else if (isMentor.Equals("True")) GetIsMentor = true;
            else GetIsMentor = false;

            if (string.IsNullOrEmpty(isLearner))
                GetIsLearner = false;
            else if (isLearner.Equals("True")) GetIsLearner = true;
            else GetIsLearner = false;
        }

        public Guid GetCurrentUserId { get; }
        public bool GetIsAdmin { get; }
        public bool GetIsMentor { get; }
        public bool GetIsLearner { get; }
    }
}

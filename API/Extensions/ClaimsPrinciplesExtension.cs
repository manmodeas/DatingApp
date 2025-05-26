using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrinciplesExtension
    {
        public static string GetUserName(this ClaimsPrincipal user)
        {
            var username = user.FindFirstValue(ClaimTypes.NameIdentifier) 
                ?? throw new Exception("Canot get username from token");

            return username;
        }
         
    }
}

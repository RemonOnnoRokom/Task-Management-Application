using Microsoft.AspNetCore.Identity;

namespace SimpleTaskManagementWebApplication.Models
{
    public class AppUser : IdentityUser<int>
    {
        public string FirstName { get; set; } = "";
        public string? LastName { get; set; }
    }
}

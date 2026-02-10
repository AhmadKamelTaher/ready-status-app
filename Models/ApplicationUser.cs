using Microsoft.AspNetCore.Identity;

namespace ReadyStatusApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public bool IsReady { get; set; } = false;
        public DateTime? LastStatusChange { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}

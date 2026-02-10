namespace ReadyStatusApp.ViewModels
{
    public class UserStatusViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsReady { get; set; }
        public DateTime? LastStatusChange { get; set; }
    }
}

namespace ReadyStatusApp.Models
{
    public class StatusLog
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        public bool IsReady { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}

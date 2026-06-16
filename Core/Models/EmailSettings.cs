namespace Core.Models
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = null!;
        public int SmtpPort { get; set; }
        public string SenderName { get; set; } = null!;
        public string SenderEmail { get; set; } = null!;
        public string SenderLogin { get; set; } = null!;
        public string SenderPassword { get; set; } = null!;
    }
}
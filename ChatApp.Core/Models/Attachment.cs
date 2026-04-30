namespace ChatApp.Core.Models
{
    public class Attachment
    {
        public Guid Id { get; private set; }
        public Guid MessageId { get; private set; }
        public Message Message { get; private set; } = null!;
        public string FileUrl { get; private set; } = string.Empty;
        public string FileName { get; private set; } = string.Empty;
        public string FileType { get; private set; } = string.Empty;
        public long FileSize { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Attachment() { }

        public Attachment(Guid messageId, string fileUrl, string fileName, string fileType, long fileSize)
        {
            this.Id = Guid.NewGuid();
            this.MessageId = messageId;
            this.FileUrl = fileUrl;
            this.FileName = fileName;
            this.FileType = fileType;
            this.FileSize = fileSize;
            this.CreatedAt = DateTime.UtcNow;
        }
    }
}
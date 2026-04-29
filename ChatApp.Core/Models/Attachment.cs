using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Core.Models
{
    public class Attachment
    {
        public Guid Id { get; private set; }
        public Guid MessageId { get; private set; }
        public Message Message { get; private set; } = null!;
        public string FileUrl { get; private set; }
        public string FileName { get; private set; }
        public string FileType { get; private set; }
        public string FileSize { get; private set; }
        public DateTime CreatedAt { get; private set; }
    }
}

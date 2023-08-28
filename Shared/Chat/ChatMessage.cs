using Microsoft.AspNetCore.Identity;

namespace Shared.Chat
{
    public class ChatMessage
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public string UserId { get; set; } // Foreign key
        public IdentityUser User { get; set; } // Navigation property
        public Guid ChatRoomId { get; set; }
        public ChatRoom ChatRoom { get; set; }
    }
}
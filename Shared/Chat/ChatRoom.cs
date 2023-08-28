using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Chat
{
    public class ChatRoom
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set;}
        public string UserIdCreator { get; set; }
        public List<ChatMessage> ChatMessages { get; set; }
    }
}

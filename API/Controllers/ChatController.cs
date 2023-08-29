using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Chat;
using Web.Data;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class ChatController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("{idChatRoom}")]
        public IActionResult GetMessages(Guid idChatRoom)
        {
            var messages = _context.ChatMessages
                                   .Where(chat => chat.ChatRoomId == idChatRoom)
                                   .OrderByDescending(m => m.Created)
                                   .Take(50);

            return Ok(messages);
        }

        [HttpPost]
        public IActionResult PostMessage(ChatMessage message)
        {
            if (message == null)
                return BadRequest();

            _context.ChatMessages.Add(message);
            _context.SaveChanges();

            return Ok();
        }
    }
}

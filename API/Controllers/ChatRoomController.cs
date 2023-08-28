using API.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Chat;
using Web.Data;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class ChatRoomController : ControllerBase
    {
        public readonly ApplicationDbContext _context;

        public ChatRoomController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult GetChatRooms()
        {
            List<ChatRoom> chatRooms = _context.ChatRooms.ToList();
            return Ok(chatRooms);
        }

        [HttpGet("{id}")]
        public IActionResult GetChatRoom(Guid id)
        {
            var chatRoom = _context.ChatRooms.FirstOrDefault(cr => cr.Id == id);
            if (chatRoom == null)
            {
                return NotFound();
            }

            return Ok(chatRoom);
        }

        [HttpPost]
        public IActionResult PostChatRoom(ChatRoom chatRoom) {
            if (chatRoom == null)
                return BadRequest();

            _context.ChatRooms.Add(chatRoom);
            _context.SaveChanges();

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = AuthConstants.ChatRoomCreatorPolicy)]
        public IActionResult DeleteChatRoom(Guid id){
            if (id == null)
                return NotFound();

            var existingChatRoom = _context.ChatRooms.FirstOrDefault(cr => cr.Id == id);
            if (existingChatRoom != null)
            {
                _context.ChatRooms.Remove(existingChatRoom);
                _context.SaveChanges();
                return Ok();
            }

            return NotFound();
        }
    }
}

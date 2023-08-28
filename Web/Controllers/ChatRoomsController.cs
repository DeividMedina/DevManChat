using Microsoft.AspNetCore.Mvc;
using Shared.Chat;
using System.Security.Claims;

namespace Web.Controllers
{
    [Route("[controller]")]
    public class ChatRoomsController : Controller
    {
        private readonly HttpClient _httpClient;

        public ChatRoomsController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {

            var response = await _httpClient.GetAsync("https://localhost:7106/api/chatroom");

            if (response.IsSuccessStatusCode)
            {
                var rooms = await response.Content.ReadFromJsonAsync<List<ChatRoom>>();
                return View(rooms);
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                // Handle error
                Console.WriteLine($"Bad Request Response Content: {responseContent}");
                return BadRequest();
            }
        }

        [HttpGet("{idChatRoom}")]
        public async Task<IActionResult> GetChatRoom(Guid idChatRoom)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7106/api/chatroom/{idChatRoom}");

            if (response.IsSuccessStatusCode)
            {
                var room = await response.Content.ReadFromJsonAsync<ChatRoom>();
                ViewBag.RoomDetails = room; // Set the room details in ViewBag
                return View(room);
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                // Handle error
                Console.WriteLine($"Bad Request Response Content: {responseContent}");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(ChatRoom chatRoom)
        {

            chatRoom.UserIdCreator = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            List<ChatMessage> messages = new List<ChatMessage>();
            chatRoom.ChatMessages = messages;
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7106/api/chatroom", chatRoom);

            if (response.IsSuccessStatusCode)
            {
                // Chat room created successfully, redirect to the chat room index view
                return RedirectToAction("Index");
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                // Handle error
                Console.WriteLine($"Bad Request Response Content: {responseContent}");
                return BadRequest();
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Chat;
using System.Net.Http;
using System.Security.Claims;

namespace Web.Controllers
{
    [Route("[controller]")]
    [Authorize]
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
        public async Task<IActionResult> ChatRoom(Guid idChatRoom)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7106/api/chatroom/{idChatRoom}");

            if (response.IsSuccessStatusCode)
            {
                var room = await response.Content.ReadFromJsonAsync<ChatRoom>();
                return View("ChatRoom", room); // Redirect to the ChatRoom view with the 'room' object
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

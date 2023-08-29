using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Shared.Chat;

namespace Web.Hubs
{
    public class ChatHub :Hub
    {
        private readonly HttpClient _httpClient;
        private readonly UserManager<IdentityUser> _userManager;
        public ChatHub(HttpClient httpClient, UserManager<IdentityUser> userManager)
        {
            _httpClient = httpClient;
            _userManager = userManager;
        }

        public async Task SendMessage(string chatRoomId, string message)
        {
            var user = await _userManager.GetUserAsync(Context.User);
            var chatroom = Guid.Parse(chatRoomId);

            var chatMessage = new ChatMessage
            {
                UserId = user.Id,
                Text = message,
                Created = DateTime.UtcNow,
                ChatRoomId = chatroom
            };
            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7106/api/chat", chatMessage);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP request error: {ex.Message}");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return;
            }

            try
            {
                await Clients.All.SendAsync("ReceiveMessage", user, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR send error: {ex.Message}");
            }
        }
        public async Task<List<ChatMessage>> GetChatHistory(string idChatRoom)
        {
            Console.WriteLine("GetChatHistory called.");
            List<ChatMessage> chatMessages = new List<ChatMessage>();
            try
            {

            var response = await _httpClient.GetAsync($"https://localhost:7106/api/chat/{idChatRoom}");
            response.EnsureSuccessStatusCode();
            chatMessages = await response.Content.ReadFromJsonAsync<List<ChatMessage>>();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bad Request Response Content: {ex.Message}");
            };
            return chatMessages;
        }
    }
}

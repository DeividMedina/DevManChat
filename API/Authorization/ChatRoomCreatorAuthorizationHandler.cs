using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Web.Data;

namespace API.Authorization
{
    public class ChatRoomCreatorAuthorizationHandler : AuthorizationHandler<ChatRoomCreatorRequirement>
    {
        private readonly ApplicationDbContext _context;

        public ChatRoomCreatorAuthorizationHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ChatRoomCreatorRequirement requirement)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.Name);
            var chatRoomId = context.Resource as Guid?;

            if (userIdClaim != null && chatRoomId != null)
            {
                var isCreator = _context.ChatRooms.Any(cr => cr.Id == chatRoomId && cr.UserIdCreator == userIdClaim.Value);

                if (isCreator)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}

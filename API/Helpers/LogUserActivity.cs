using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //before Action executed

            var resutContext = await next();
            //After Action executed
            if (context.HttpContext.User.Identity?.IsAuthenticated != true) return;

            var userId = resutContext.HttpContext.User.GetUserId();

            var repo = resutContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

            var user = await repo.GetUserByIdAsync(userId);

            if (user is null) return;

            user.LastActive = DateTime.UtcNow;

            await repo.SaveAllAsync();

        }
    }
}

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApp.webapi.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace DatingApp.webapi.Helpers
{
    public class LogUserLastActivity : IAsyncActionFilter
    {
        public async Task  OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            int userId = int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var repo = resultContext.HttpContext.RequestServices.GetService<IDatingRepository>();
            var user = await repo.GetUser(userId) ;
            user.LastActiveDate = DateTime.Now;

            await repo.SaveAll();
        }
    }
}
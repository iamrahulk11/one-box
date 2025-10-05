using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace api.Controllers
{
    public abstract class baseController : Controller
    {
        protected string username { get; private set; } = string.Empty;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            FetchClaims();
        }


        private void FetchClaims()
        {
            //Ensuring only to fetch the claims if authentication is done.
            if (User.Identity?.IsAuthenticated == true)
            {
                //Fetching the claims value from the wrapper
                username = User.FindFirstValue("username");
            }
        }
    }
}

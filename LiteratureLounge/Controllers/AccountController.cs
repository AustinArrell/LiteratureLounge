using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class AccountController : Controller
{
    public async Task Login(string returnUrl = "/")
    {
        
   
    }
     
    [Authorize]
    public IActionResult Profile()
    {
        return View(new
        {
            Name = User.Identity.Name,
            UserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
        });
    }

    [Authorize]
    public async Task Logout()
    {

    }
}
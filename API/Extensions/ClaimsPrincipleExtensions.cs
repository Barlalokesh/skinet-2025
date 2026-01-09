using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using core.Entities; // <-- for AppUser
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;

namespace API.Extensions;

public static class ClaimsPrincipleExtensions
{
    public static async Task<AppUser>GetUserByEmail(this UserManager<AppUser> userManager, 
    ClaimsPrincipal user)
    {
        var usertoReturn = await userManager.Users.FirstOrDefaultAsync(x =>
        x.Email == user.GetEmail());

        if(usertoReturn == null) throw new AuthenticationException("user not found");

        return usertoReturn;
    }

    public static async Task<AppUser>GetUserByEmailWithAddress(this UserManager<AppUser> userManager, 
    ClaimsPrincipal user)
    {
        var usertoReturn = await userManager.Users
        .Include(x => x.Address)
        .FirstOrDefaultAsync(x => x.Email == user.GetEmail());

        if(usertoReturn == null) throw new AuthenticationException("user not found");

        return usertoReturn;
    }



public static string GetEmail(this ClaimsPrincipal user)
    {
        var email = user.FindFirstValue(ClaimTypes.Email)  // ✅ correct
 
        ?? throw new AuthenticationException("Email claim not found");
        return email;
    }

}
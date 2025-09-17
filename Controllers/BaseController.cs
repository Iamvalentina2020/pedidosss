using GestionPedidos.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GestionPedidos.Controllers
{
    public class BaseController : Controller
    {
        protected User? GetCurrentUser()
        {
            var userJson = HttpContext.Session.GetString("CurrentUser");
            if (string.IsNullOrEmpty(userJson))
                return null;

            try
            {
                var userData = JsonSerializer.Deserialize<JsonElement>(userJson);
                return new User
                {
                    Id = userData.GetProperty("Id").GetInt32(),
                    Name = userData.GetProperty("Name").GetString() ?? "",
                    Email = userData.GetProperty("Email").GetString() ?? "",
                    Role = (UserRole)userData.GetProperty("Role").GetInt32()
                };
            }
            catch
            {
                return null;
            }
        }

        protected bool IsUserInRole(params UserRole[] roles)
        {
            var user = GetCurrentUser();
            return user != null && roles.Contains(user.Role);
        }

        protected bool IsAuthenticated()
        {
            return GetCurrentUser() != null;
        }
    }
}
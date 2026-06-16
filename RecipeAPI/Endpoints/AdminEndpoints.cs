using RecipeAPI.DTOs;
using RecipeAPI.Entities;
using RecipeAPI.Services;

namespace RecipeAPI.Endpoints
{
    public static class AdminEndpoints
    {
        public static void MapAdminEndpoints(this WebApplication app)
        {
            var admin=app.MapGroup("/admin").RequireAuthorization("AdminOnly");

            admin.MapGet("/users", (Role? role, string? search, AdminService adminService) =>
            {
                var users = adminService.GetUsers(role, search);

                if (users.Count == 0)
                {
                    return Results.NotFound();
                }

                return Results.Ok(users);
            });
            admin.MapGet("/users/{id}", (int id, AdminService adminService) =>
            {
                var user = adminService.GetUserById(id);

                if (user == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(user);
            });
            admin.MapDelete("/users/{id}", (int id,AdminService adminService) =>
            {
                bool success= adminService.DeleteUser(id);

                if (!success)
                {
                    return Results.NotFound();
                }

                return Results.NoContent();
            });
            admin.MapPatch("/users/{id}/role", (int id, RoleChangeRequest req, AdminService adminService) =>
            {
                bool success = adminService.ChangeRole(id, req);

                if (!success)
                {
                    return Results.NotFound();
                }
                return Results.NoContent();

            });
            
        }
    }
}

using RecipeAPI.DTOs;
using RecipeAPI.Services;
using System.Net.WebSockets;

namespace RecipeAPI.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this WebApplication app)
        {
            app.MapPost("/auth/register", (RegisterRequest req,AuthService authService,ILogger<Program> logger) =>
            {
                logger.LogInformation("Registration attempt");
                var errors = req.ValidateRegisterRequest();
                if (errors.Count > 0)
                {
                    logger.LogWarning("Registration attempt failed validation.");
                    return Results.BadRequest(errors);
                }

                var registered = authService.Register(req);
                if (!registered)
                {
                    logger.LogWarning("Registration attempt with already taken username.");
                    return Results.Conflict("Username already taken.");
                }

                logger.LogInformation("User registered successefully");
                return Results.Ok("Username registered successefully.");
            });

            app.MapPost("/auth/login",(LoginRequest req, AuthService authService, ILogger<Program> logger) =>
            {
                logger.LogInformation("Login attempt");
                var result = authService.Login(req);

                if (result == null)
                {
                    logger.LogWarning("Login attempt failed");
                    return Results.BadRequest("Username or Password invalid");
                }
                logger.LogInformation("Login successeful.");
                return Results.Ok(result);
            });

        }
    }
}

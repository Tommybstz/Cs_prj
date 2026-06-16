using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeAPI.Data;
using RecipeAPI.DTOs;
using RecipeAPI.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecipeAPI.Services
{
    public class AdminService
    {
        public AppDbContext _db { get; set; }
        public ILogger<AdminService> _logger { get; set; }
        public AdminService(AppDbContext db, ILogger<AdminService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public List<User> GetUsers([FromQuery]Role? role, string? search)
        {
            var users = _db.Users.Where(u => (string.IsNullOrEmpty(search) || u.Username.StartsWith(search)) && (!role.HasValue || u.Role == role.Value)).ToList();

            return users;
        }
        public User? GetUserById(int id)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == id);

            return user;
        }
        public bool ChangeRole(int id, RoleChangeRequest req)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == id);

            if (user == null) 
            {
                _logger.LogWarning("Attempted to change the role in a non-existend User with ID:{Id}",id);
                return false;
            }

            _logger.LogInformation("Attempting to change role of user ID: {UserId}", user.Id);
            user.Role = req.Role;
            _logger.LogInformation("User ID: {UserId} role changed in memory", user.Id);

            try
            {
                _db.SaveChanges();
                _logger.LogInformation("User ID: {UserId} role changed in database", user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to change user role ID:{UserId} in database", user.Id);
                throw;
            }
            return true;
        }
        public bool DeleteUser(int id)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                _logger.LogWarning("Attempted to remove a non-existend User with ID:{Id}", id);
                return false;
            }

            _logger.LogInformation("Attempting to remove user ID: {UserId}", user.Id);
            _db.Users.Remove(user);
            _logger.LogInformation("User ID: {UserId} removed in memory",user.Id);

            try
            {
                _db.SaveChanges();
                _logger.LogInformation("User ID: {UserId} removed in database", user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove user ID:{UserId} in database", user.Id);
                throw;
            }

            return true;
        }
    }
}

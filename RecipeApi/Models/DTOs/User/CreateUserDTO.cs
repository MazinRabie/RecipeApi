﻿namespace RecipeApi.Models.DTOs.User
{
    public class CreateUserDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Gender { get; set; }
        public List<string>? Fridgeingredients { get; set; } = new List<string>();
    }
}

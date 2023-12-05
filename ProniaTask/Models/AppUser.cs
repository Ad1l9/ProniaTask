﻿using Microsoft.AspNetCore.Identity;
using ProniaTask.Enumerations;

namespace ProniaTask.Models
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public Gender Gender { get; set; }
    }
}

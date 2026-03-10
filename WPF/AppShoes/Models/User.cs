using System;
using System.Collections.Generic;

namespace AppShoes.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? UserRole { get; set; }

    public string? UserFirstname { get; set; }

    public string? UserName { get; set; }

    public string? UserLastname { get; set; }

    public string? UserLogin { get; set; }

    public string? UserPassword { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    
    public string UserFullName => $"{UserFirstname} {UserName} {UserLastname} - {UserRole}";
}

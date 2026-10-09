using System;

namespace MyHttpServer;

[Controller("auth")]
public class AuthController
{
    [HttpPost("login")]
    public void Login(string email, string password)
    {
        Console.WriteLine($"Email: {email}");
        Console.WriteLine($"Password: {password}");
    }
}
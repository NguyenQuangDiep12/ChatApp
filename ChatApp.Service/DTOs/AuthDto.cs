namespace ChatApp.Service.DTOs
{
    public record RegisterRequest(string UserName, string Email, string Password);
    public record LoginRequest(string Email, string Password, string? DeviceInfo);
    public record AuthResponse(string AccessToken, DateTime ExpiresAt, UserDto User);
}

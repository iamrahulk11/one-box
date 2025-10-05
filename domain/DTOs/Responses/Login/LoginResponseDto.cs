namespace domain.DTOs.Responses.Login
{
    public record loginResponseDto
    {
        public tokenResponseDto Tokens { get; set; } = new();
    }
}

using helpers;
using System.ComponentModel.DataAnnotations;

namespace domain.DTOs
{
    public record baseResponseDto<T>
    {
        public resultResponseDto Result { get; set; } = new();
        public T? Data { get; set; }
    }
    public record resultResponseDto
    {
        public int flag { get; set; } = 0;
        public string flag_message { get; set; } = string.Empty;
    }
    public record tokenResponseDto
    {
        public string access_token { get; set; } = string.Empty;
        public string refresh_token { get; set; } = string.Empty;
    }
    public record tokenInfoResponseDto : tokenResponseDto
    {
        public DateTime access_token_expirty { get; set; } 
        public DateTime refresh_token_expirty { get; set; }
        public string issuer { get; set; } = string.Empty;
        public string audience { get; set; } = string.Empty;
    }
    public record baseListDetails<T> where T : class
    {
        public int total_records { get; set; }
        public List<T> applications { get; set; } = new();
    }
    public record nameAndAbbreviationDto
    {
        public string name { get; set; } = string.Empty;
        public string abbrevation { get; set; } = string.Empty;
    }
    public record usernameResponseDto
    {
        public string username { get; set; } = string.Empty;
    }
}

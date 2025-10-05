namespace domain.DTOs.Responses.User
{
    public record userProfileResponseDto : tokenResponseDto
    {
        public string username { get; set; } = string.Empty;
        public string gender { get; set; } = string.Empty;
        public string language { get; set; } = string.Empty;
        public string active_service { get; set; } = string.Empty;
    }
}

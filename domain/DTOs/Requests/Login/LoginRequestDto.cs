using System.Text.Json.Serialization;

namespace domain.DTOs.Requests.Login
{    
    public record userCreationRequestDto: passwordRequestDto
    {
        [JsonIgnore]
        public string ukey { get; set; } = Guid.NewGuid().ToString();

        [JsonIgnore]
        public string encrypted_password { get; set; } = string.Empty;
    }

    public record LoginRequestDto : passwordRequestDto
    {

    }        
}

using helpers;
using System.ComponentModel.DataAnnotations;

namespace domain.DTOs
{
    public record basePaginationRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = dataAnnotationMessages.PAGE_NUMBER_REQUIRED)]
        [Range(1, 10000, ErrorMessage = $"page_number {dataAnnotationMessages.RANGE_PAGE_NUMBER}")]
        public int page_number { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = dataAnnotationMessages.PAGE_SIZE_REQUIRED)]
        [Range(1, 50, ErrorMessage = $"page_size {dataAnnotationMessages.RANGE_PAGE_SIZE}")]
        public int page_size { get; set; }
        [LengthIfNotNull(30, ErrorMessage = $"search_value {dataAnnotationMessages.SEARCH_LENGTH}")]
        public string search_value { get; set; } = string.Empty;

    }

    public record basePaginationWithoutSearchValueRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = dataAnnotationMessages.PAGE_NUMBER_REQUIRED)]
        [Range(1, 10000, ErrorMessage = $"page_number {dataAnnotationMessages.RANGE_PAGE_NUMBER}")]
        public int page_number { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = dataAnnotationMessages.PAGE_SIZE_REQUIRED)]
        [Range(1, 50, ErrorMessage = $"page_size {dataAnnotationMessages.RANGE_PAGE_SIZE}")]
        public int page_size { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = dataAnnotationMessages.SORT_COLUMN_REQUIRED)]
        [MaxLength(30, ErrorMessage = $"sort_column {dataAnnotationMessages.INPUT_LENGTH_EXCEEDS}")]
        public string sort_column { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = dataAnnotationMessages.SORT_DIRECTION_REQUIRED)]
        [AllowedValuesForStringIfNotNull(typeof(allowedSortingDirectionValue))]
        public string sort_direction { get; set; } = string.Empty;
    }

    public record passwordRequestDto : usernameRequestDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = dataAnnotationMessages.PASSWORD_REQUIRED)]
        [MaxLength(20, ErrorMessage = $"encrypted_password {dataAnnotationMessages.INPUT_LENGTH_EXCEEDS}")]
        public string password { get; set; } = string.Empty;
    }

    public record usernameRequestDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = dataAnnotationMessages.USERNAME_REQUIRED)]
        [MaxLength(20, ErrorMessage = $"username {dataAnnotationMessages.INPUT_LENGTH_EXCEEDS}")]
        public string username { get; set; } = string.Empty;
    }

    public record refreshTokenRequestDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = dataAnnotationMessages.REFRESH_TOKEN_REQUIRED)]
        public string refresh_token { get; set; } = string.Empty;
    }

}

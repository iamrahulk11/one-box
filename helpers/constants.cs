namespace helpers
{
    public record responseMessages
    {
        public const string DATA_NOT_FOUND = "Data not found.";
        public const string SOMETHING_WENT_WRONG = "Something went wrong.";
        public const string DATA_FOUND = "Data is found.";
        public const string TOKEN_INVALID = "Invalid token.";

        //Refresh Token
        public const string TOKEN_EXPIRE = "Session expired ! Please login again.";
        public const string TOKEN_REFRESHED_SUCCESSFULLY = "Token refreshed successfully.";

        public const string EMPLOYEE_IN_ACTIVE = "Employee is in-active.";
        public const string COMMUNICATION_SENT_SUCCESSFULLY = "sent successfully.";

        //Bloom Filter
        public const string USERNAME_ALREADY_EXISTS = "Username already taken, try something different.";
        public const string USERNAME_AVAILABLE = "Username is available to use.";

        //User
        public const string USER_CREATED_SUCCESSFULLY = "User created successfully.";
        public const string WRONG_PASSWORD_ENTERED = "Wrong password entered!";
    }

    public record dataAnnotationMessages
    {
        public const string INVALID_INPUT = "Invalid input.";
        public const string INVALID_INPUT_LENGTH = "Invalid input length.";
        public const string INPUT_LENGTH_EXCEEDS = "exceeds the maximum allowed length.";
        public const string FILE_SIZE_EXCEEDS = "File size should not be greater than";
        public const string INVALID_DOCUMENT_TYPE = "Invalid document type.";
        public const string FILE_NOT_ALLOWED = "File not allowed.";
        public const string INVALID_SEGMENTS = "Invalid segment name.";

        //LOGIN
        public const string USERNAME_REQUIRED = "Username cannot be blank.";
        public const string PASSWORD_REQUIRED = "Password cannot be blank.";
        public const string APPLICATION_NUMBER_REQUIRED = "Application number cannot be blank.";
        public const string UCC_REQUIRED = "UCC Code cannot be blank.";
        public const string PAN_REQUIRED = "PAN cannot be blank.";
        public const string OTP_REQUIRED = "Otp cannot be blank";
        public const string MODULE_NAME_REQUIRED = "Module name cannot be blank";
        public const string REFRESH_TOKEN_REQUIRED = "Refresh token cannot be blank";

        //CHANGE-PASSWORD
        public const string OLD_PASSWORD_REQUIRED = "Old password cannot be blank.";
        public const string NEW_PASSWORD_REQUIRED = "New password cannot be blank.";
        public const string CONFIRM_NEW_PASSWORD_REQUIRED = "Confirm New password cannot be blank.";
        public const string CONFIRM_PASSWORD_NEW_PASSWORD_NOT_MATCH = "Confirm password and new password should be same.";
        public const string PASSWORD_ONE_UPPERCASE_REQUIRED = "One uppercase charater required.";
        public const string PASSWORD_ONE_SPECIAL_CHARACTER_REQUIRED = "One special character required.";
        public const string PASSWORD_MINLENTH_REQUIRED = "Password should be minimum 8 characters.";
        public const string PASSWORD_MAXLENTH_REQUIRED = "Password should be maximum 25 characters.";

        //User management
        public const string EMPLOYEE_ID_REQUIRED = "Employee Id cannot be blank.";
        public const string EMPLOYEE_NAME_REQUIRED = "Employee Name cannot be blank.";
        public const string EMPLOYEE_MOBILE_REQUIRED = "Employee Mobile cannot be blank.";
        public const string EMPLOYEE_EMAIL_REQUIRED = "Employee Email cannot be blank.";
        public const string EMPLOYEE_GENDER_REQUIRED = "Employee Gender cannot be blank.";
        public const string IS_ACTIVE_REQUIRED = "Active status cannot be blank.";

        //Pagination 
        public const string PAGE_NUMBER_REQUIRED = "Page number cannot be blank.";
        public const string PAGE_SIZE_REQUIRED = "Page size cannot be blank.";
        public const string RANGE_PAGE_NUMBER = "Number of pages must be less than 10,000 pages.";
        public const string RANGE_PAGE_SIZE = "Number of items per page should be in between 1 to 50";
        public const string SEARCH_LENGTH = "Minimum 3 letters are required to search.";
        public const string STATUS_TYPE_REQUIRED = "Please select at-least 1 type of status to proceed.";

        public const string SORT_COLUMN_REQUIRED = "Sort column cannot be blank.";
        public const string SORT_DIRECTION_REQUIRED = "Sort direction cannot be blank.";


    }


    public record generalMessages
    {
    }

    public record emailSubjects
    {
        public const string GENERAL_OTP = "Your one time password(OTP)";
    }
}
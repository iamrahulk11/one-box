using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace helpers
{

    public class LengthIfNotNullAttribute : ValidationAttribute
    {
        private readonly int _maxLength;

        public LengthIfNotNullAttribute(int maxLength)
        {
            _maxLength = maxLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validation_context)
        {
            // Check if the value is null
            if (value == null)
            {
                // If the value is null, consider it valid (no need to check length)
                return ValidationResult.Success;
            }

            // Check if the value is a string
            if (value is string stringValue)
            {
                // Validate the length of the string
                if (stringValue.Length <= _maxLength)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult($"{validation_context.DisplayName} {dataAnnotationMessages.INPUT_LENGTH_EXCEEDS}");
                }
            }

            // If the value is not a string, it's an invalid type for this attribute
            return new ValidationResult($"The field {validation_context.DisplayName} must be a string.");
        }
    }

    public class FileValidationIfNotNullAttribute : ValidationAttribute
    {
        private readonly string[] _allowedExtensions;
        private readonly long _maxFileSize;
        private readonly int _maxSizeToPrint;

        public FileValidationIfNotNullAttribute(string[] allowed_extensions, long max_file_size, int max_size_to_print)
        {
            _allowedExtensions = allowed_extensions;
            _maxFileSize = max_file_size;
            _maxSizeToPrint = max_size_to_print;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validation_context)
        {
            // Check if the value is null
            if (value == null)
            {
                // If the value is null, consider it valid (no need to check anything)
                return ValidationResult.Success;
            }

            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(extension))
                {
                    return new ValidationResult(dataAnnotationMessages.FILE_NOT_ALLOWED);
                }

                if (file.Length > _maxFileSize)
                {
                    return new ValidationResult($"{dataAnnotationMessages.FILE_SIZE_EXCEEDS} {_maxSizeToPrint}MB.");
                }
            }

            return ValidationResult.Success;
        }

    }


    public class AllowedValuesForListOfStringIfNotNullAttribute : ValidationAttribute
    {
        private readonly Type _enumType;

        public AllowedValuesForListOfStringIfNotNullAttribute(Type enum_type)
        {
            if (!enum_type.IsEnum)
            {
                throw new ArgumentException("The type must be an enumeration.");
            }
            _enumType = enum_type;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validation_context)
        {
            // Check if the value is null
            if (value == null)
            {
                // If the value is null, consider it valid (no need to check anything)
                return ValidationResult.Success;
            }

            if (value is not IEnumerable<string> list)
            {
                return new ValidationResult("The property must be a list of strings.");
            }

            var enumNames = Enum.GetNames(_enumType);
            var invalidValues = list.Where(item => !enumNames.Contains(item)).ToList();

            if (invalidValues.Any())
            {
                return new ValidationResult(dataAnnotationMessages.INVALID_INPUT);
            }

            return ValidationResult.Success;
        }
    }

    public class AllowedValuesForStringIfNotNullAttribute : ValidationAttribute
    {
        private readonly Type _enumType;

        public AllowedValuesForStringIfNotNullAttribute(Type enum_type)
        {
            if (!enum_type.IsEnum)
            {
                throw new ArgumentException("The type must be an enumeration.");
            }
            _enumType = enum_type;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validation_context)
        {
            // Check if the value is null
            if (value == null)
            {
                // If the value is null, consider it valid (no need to check anything)
                return ValidationResult.Success;
            }

            if (value is not string stringValue)
            {
                return new ValidationResult("The property must be a string.");
            } 

            var enumNames = Enum.GetNames(_enumType);
            if (!enumNames.Contains(stringValue))
            {
                return new ValidationResult(dataAnnotationMessages.INVALID_INPUT);
            }

            return ValidationResult.Success;
        }
    }

    //This is a check which is used, when we want to make a field mandatory if other field have some specific value.
    //For eg. In case of nominee, if the minor flag is Y then Guardian details should be mandatory(Guardian Address line 1) etc.
    public class FieldRequiredIfAttribute : ValidationAttribute
    {
        private readonly string _propertyToCheck;
        private readonly string _valueToMatch;
        private readonly string _errorMessage;

        public FieldRequiredIfAttribute(string property, string value_to_match, string error_message)
        {
            _propertyToCheck = property;
            _valueToMatch = value_to_match;
            _errorMessage = error_message;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validation_context)
        {
            var _propertyToCheckInfo = validation_context.ObjectType.GetProperty(_propertyToCheck);

            if (_propertyToCheckInfo == null)
            {
                return new ValidationResult($"Property '{_propertyToCheck}' not found.");
            }

            string _valueOfParentProperty = (string)_propertyToCheckInfo.GetValue(validation_context.ObjectInstance);

            if (_valueOfParentProperty == _valueToMatch && string.IsNullOrWhiteSpace(value?.ToString()))
            {
                return new ValidationResult(_errorMessage);
            }

            return ValidationResult.Success;
        }
    }

    public class FieldRequiredIfParentPropertyIsNotNullAttribute : ValidationAttribute
    {
        private readonly string _propertyToCheck;
        private readonly string _errorMessage;

        public FieldRequiredIfParentPropertyIsNotNullAttribute(string property,  string error_message)
        {
            _propertyToCheck = property;
            _errorMessage = error_message;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validation_context)
        {
            var _propertyToCheckInfo = validation_context.ObjectType.GetProperty(_propertyToCheck);

            if (_propertyToCheckInfo == null)
            {
                return new ValidationResult($"Property '{_propertyToCheck}' not found.");
            }

            string _valueOfParentProperty = (string)_propertyToCheckInfo.GetValue(validation_context.ObjectInstance);
            
            //Parent property is not null but child value is null
            //Send Error
            if (!string.IsNullOrEmpty(_valueOfParentProperty) &&
                string.IsNullOrWhiteSpace(value?.ToString()))
            {
                return new ValidationResult(_errorMessage);
            }

            return ValidationResult.Success;
        }
    }

    public class PasswordValidationAttribute : ValidationAttribute
    {
        private readonly int _minLength;
        private readonly int _maxLength;
        private readonly bool _requireUppercase;
        private readonly bool _requireSpecialChar;

        public PasswordValidationAttribute(int minLength,
                                           int maxLength,
                                           bool requireUppercase,
                                           bool requireSpecialChar)
        {
            _minLength = minLength;
            _maxLength = maxLength;
            _requireUppercase = requireUppercase;
            _requireSpecialChar = requireSpecialChar;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult(dataAnnotationMessages.NEW_PASSWORD_REQUIRED);
            }

            var input = value as string;

            if (string.IsNullOrEmpty(input))
            {
                return new ValidationResult(dataAnnotationMessages.NEW_PASSWORD_REQUIRED);
            }

            // Check length
            if (input.Length < _minLength)
            {
                return new ValidationResult(dataAnnotationMessages.PASSWORD_MINLENTH_REQUIRED);
            }

            if (input.Length > _maxLength)
            {
                return new ValidationResult(dataAnnotationMessages.PASSWORD_MAXLENTH_REQUIRED);
            }

            // Check for uppercase letters
            if (_requireUppercase && !Regex.IsMatch(input, @"[A-Z]"))
            {
                return new ValidationResult(dataAnnotationMessages.PASSWORD_ONE_UPPERCASE_REQUIRED);
            }

            // Check for special characters
            if (_requireSpecialChar && !Regex.IsMatch(input, @"[\W_]"))
            {
                return new ValidationResult(dataAnnotationMessages.PASSWORD_ONE_SPECIAL_CHARACTER_REQUIRED);
            }

            return ValidationResult.Success;
        }
    }
}

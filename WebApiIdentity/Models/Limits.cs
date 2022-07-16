namespace WebApiIdentity.Models
{
    public static class Limits
    {
        public static class StringIdentifier
        {
            public const int MaxLength = 150;
        }
        
        // see. https://www.itu.int/rec/dologin_pub.asp?lang=e&id=T-REC-E.164-201011-I!!PDF-R&type=items
        public static class Phone
        {
            public const int MaxLength = 15;    
            public const int MinLength = 7;
            public const string Pattern = @"^\d{7,15}$";
        }

        public static class Email
        {
            /// see. http://stackoverflow.com/questions/386294/what-is-the-maximum-length-of-a-valid-email-address
            public const int MaxLength = 254;

            // see. https://stackoverflow.com/questions/1423195/what-is-the-actual-minimum-length-of-an-email-address-as-defined-by-the-ietf
            public const int MinLength = 3;
        }

        public static class Url
        {
            // see. https://stackoverflow.com/questions/417142/what-is-the-maximum-length-of-a-url-in-different-browsers
            public const int MaxLength = 2000;    
        }

        public static class Password
        {
            public const int MaxLength = 32;
            public const int MinLength = 6;
            public const string Pattern = @"^(\w|~|!|@|#|\$|%|\^|&|\*|_|\+|`|-|=){6,32}$";
        }

        public static class PasswordHash
        {
            // 64 bytes allows you to store a hash with a length of 512 bits, which is sufficient for SHA-2 and SHA-3.
            public const int MaxLength = 64;
        }

        public static class PromoCode
        {
            public const int MaxLength = 12;
            public const int MinLength = 4;
            public const string Pattern = @"^([0-9]|[A-Z]){4,12}$";            
        }

        public static class ConfirmationCode
        {
            public const int MaxLength = 6;
            public const int MinLength = 4;
            public const string Pattern = @"^\d{4,6}$";
        }
        
        public static class LicensePlateNumber
        {
            public const int MaxLength = 16;
            public const int MinLength = 5;
            public const string Pattern = @"^.{7,15}$";
        } 
        
        public static class FirstName
        {
            public const int MaxLength = 50;
            public const int MinLength = 1;
            public const string Pattern = @"^.{1,50}$";
        }
        
        public static class MiddleName
        {
            public const int MaxLength = 50;
            public const string Pattern = @"^.{0,50}$";
        }

        public static class LastName 
        {
            public const int MaxLength = 50;
            public const int MinLength = 1;
            public const string Pattern = @"^.{1,50}$";
        }

        public static class CompanyName
        {
            public const int MaxLength = 150;
        }

        public static class AnyShortName 
        {
            public const int MaxLength = 50;
        }

        public static class AnyFullName 
        {
            public const int MaxLength = 150;
        }

        public static class Address 
        {
            public const int MaxLength = 250;
        }

        public static class Entrance 
        {
            public const int MaxLength = 5;
        }

        public static class Floor 
        {
            public const int MaxLength = 5;
        }

        public static class Apartment 
        {
            public const int MaxLength = 5;
        }

        public static class Comment
        {
            public const int MaxLength = 2000;
        }

        public static class CultureName
        {
            public const int MaxLength = 7;
        }

        public static class Title
        {
            public const int MaxLength = 150;
        }

        public static class Brief
        {
            public const int MaxLength = 2000;
        }

        public static class Content
        {
            public const int MaxLength = 4000;
        }

        public static class LongContent
        {
            public const int MaxLength = 8000;
        }
    }
}
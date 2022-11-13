namespace TaskTronic.Common
{
    using System;
    using System.Linq;

    public static class Guard
    {
        private static readonly char[] ForbiddenCharacters = new char[] { '\\', '/', ':', '*', '?', '<', '>', '|', '"' };
        private const string VALUE_NAME = "Value";
        private const string OBJECT_NAME = "Object";

        public static void AgainstEmptyString<TException>(string value, string name = VALUE_NAME)
            where TException : BaseDomainException, new()
        {
            if (!string.IsNullOrEmpty(value))
            {
                return;
            }

            ThrowException<TException>($"{name} cannot be null or empty.");
        }

        public static void AgainstLessThanZero<TException>(int value, string name = VALUE_NAME)
            where TException : BaseDomainException, new()
        {
            if (value >= 0)
            {
                return;
            }

            ThrowException<TException>($"{name} cannot be less than zero.");
        }

        public static void AgainstLessThanOne<TException>(int value, string name = VALUE_NAME)
            where TException : BaseDomainException, new()
        {
            if (value > 0)
            {
                return;
            }

            ThrowException<TException>($"{name} cannot be less than one.");
        }

        public static void AgainstNullObject<TException>(object obj, string name = OBJECT_NAME)
            where TException : BaseDomainException, new()
        {
            if (!(obj is null))
            {
                return;
            }

            ThrowException<TException>($"{name} cannot be null.");
        }

        public static void AgainstInvalidWindowsCharacters<TException>(string value, string name = VALUE_NAME)
            where TException : BaseDomainException, new()
        {
            if (value.Any(c => ForbiddenCharacters.Contains(c)))
            {
                ThrowException<TException>($"Invalid characters found in the name: {name}");
            }
        }

        private static void ThrowException<TException>(string message)
            where TException : BaseDomainException, new()
        {
            var exception = new TException { Message = message };

            throw exception;
        }
    }
}

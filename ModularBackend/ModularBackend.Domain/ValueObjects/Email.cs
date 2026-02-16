using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Domain.ValueObjects
{
    public readonly record struct Email
    {
        public string Value { get; }

        public Email(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));

            email = email.Trim().ToLowerInvariant();

            if (!IsValidEmail(email))
                throw new ArgumentException("Invalid email format.", nameof(email));

            Value = email.Trim();
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public override string ToString() => Value;
    }
}

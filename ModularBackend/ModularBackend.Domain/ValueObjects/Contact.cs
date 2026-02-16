using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Domain.ValueObjects
{
    public sealed record Contact
    {
        public Email? Email { get; }
        public string? Phone { get; }

        public Contact(Email? email, string? phone)
        {
            if (email is null && string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("At least one contact method is required.");

            if (!string.IsNullOrEmpty(phone) && !IsValidPhone(phone))
                throw new ArgumentException("Invalid phone number.", nameof(phone));

            Email = email;
            Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
        }

        private static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            phone = phone.Trim();

            return phone.Length >= 7 && phone.Length <= 15;
        }
    }
}

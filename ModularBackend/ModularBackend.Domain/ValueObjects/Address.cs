using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Text;

namespace ModularBackend.Domain.ValueObjects
{
    public sealed record Address
    {
        public string Street { get; }
        public string City { get; }
        public string PostalCode { get; }
        public string Country { get; }

        public Address(string street, string city, string postalCode, string country)
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new ArgumentException("Street is required.");

            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City is required.");

            if (string.IsNullOrWhiteSpace(postalCode))
                throw new ArgumentException("Postal code is required.");

            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentException("Country is required.");

            Street = street.Trim();
            City = city.Trim();
            PostalCode = postalCode.Trim();
            Country = country.Trim();
        }
    }
}

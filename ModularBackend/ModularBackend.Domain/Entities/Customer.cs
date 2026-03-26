using ModularBackend.Domain.Abstractions;
using ModularBackend.Domain.Exceptions;
using ModularBackend.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Domain.Entities
{
    public sealed class Customer : AggregateRoot
    {
        public string Name { get; private set; }
        public Contact Contact { get; private set; }
        public TaxId TaxId { get; private set; }
        public Address BillingAddress { get; private set; }
        public Address InstallationAddress { get; private set; }
        public bool IsDeleted { get; private set; }

        public Customer()
        {
            
        }

        public Customer(string name, TaxId taxId, Contact contact, Address billingAddress, Address? installationAddress = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Customer name cannot be empty.", nameof(name));

            if (contact is null)
                throw new ArgumentNullException(nameof(contact), "Contact is required.");

            if (billingAddress is null)
                throw new ArgumentNullException(nameof(billingAddress), "Billing address is required.");

            Name = name.Trim();
            TaxId = taxId;
            Contact = contact;
            BillingAddress = billingAddress;
            InstallationAddress = installationAddress == null ? billingAddress : installationAddress;
        }

        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Customer name cannot be empty.", nameof(newName));

            Name = newName.Trim();
        }

        public void UpdateContact(Contact newContact)
        {
            if (newContact is null)
                throw new ArgumentNullException(nameof(newContact), "Contact is required.");

            Contact = newContact;
        }

        public void UpdateTaxId(TaxId newTaxId)
            => TaxId = newTaxId;

        public void UpdateBillingAddress(Address newBillingAddress)
        {
            if (newBillingAddress is null)
                throw new ArgumentNullException(nameof(newBillingAddress), "Billing address is required.");

            var installationFollowsBilling = InstallationAddress.Equals(BillingAddress);

            BillingAddress = newBillingAddress;

            if (installationFollowsBilling)
                InstallationAddress = newBillingAddress;
        }

        public void DeleteCustomer()
            => IsDeleted = true;

        public void UpdateInstallationAddress(Address? newInstallationAddress)
            => InstallationAddress = newInstallationAddress ?? BillingAddress;
    }
}

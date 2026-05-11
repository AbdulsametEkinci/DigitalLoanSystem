using System;
using System.Collections.Generic;

namespace DigitalLoanSystem.Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string IdentityNumber { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
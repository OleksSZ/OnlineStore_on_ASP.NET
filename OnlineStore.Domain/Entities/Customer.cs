using OnlineStore.Domain.Common;

namespace OnlineStore.Domain.Entities;

public class Customer : Entity
{
    public string Name { get; private set; } = null!;
    public string Phone { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string Address { get; private set; } = null!;

    private Customer() { } // для EF Core

    public Customer(string name, string phone, string email, string address)
    {
        Update(name, phone, email, address);
    }

    public void Update(string name, string phone, string email, string address)
    {
        Name = Guard.NotEmpty(name, nameof(name));
        Phone = Guard.NotEmpty(phone, nameof(phone));
        Email = Guard.NotEmpty(email, nameof(email));
        Address = Guard.NotEmpty(address, nameof(address));
    }
}
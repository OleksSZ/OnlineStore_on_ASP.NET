using OnlineStore.Domain.Common;

namespace OnlineStore.Domain.Entities;

public class Courier : Entity
{
    public string Name { get; private set; } = null!;
    public string Phone { get; private set; } = null!;
    public bool IsAvailable { get; private set; } = true;

    private Courier() { }

    public Courier(string name, string phone)
    {
        Name = Guard.NotEmpty(name, nameof(name));
        Phone = Guard.NotEmpty(phone, nameof(phone));
    }

    internal void MarkBusy()
    {
        if (!IsAvailable) throw new InvalidOperationException("Кур'єр зайнятий");
        IsAvailable = false;
    }

    internal void MarkFree() => IsAvailable = true;
    public void Update(string name, string phone)
    {
        Name = Guard.NotEmpty(name, nameof(name));
        Phone = Guard.NotEmpty(phone, nameof(phone));
    }
}
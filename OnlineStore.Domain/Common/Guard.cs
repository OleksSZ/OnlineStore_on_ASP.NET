namespace OnlineStore.Domain.Common;

internal static class Guard
{
    public static string NotEmpty(string? value, string name) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException($"{name} не може бути порожнім")
            : value.Trim();

    public static void Positive(decimal value, string name)
    {
        if (value <= 0) throw new ArgumentException($"{name} має бути більше 0");
    }
}
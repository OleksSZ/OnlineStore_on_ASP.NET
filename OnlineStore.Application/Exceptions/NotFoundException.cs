namespace OnlineStore.Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string entity, int id)
        : base($"{entity} з Id={id} не знайдено") { }
}
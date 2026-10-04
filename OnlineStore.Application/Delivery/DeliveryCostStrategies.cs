using OnlineStore.Domain.Entities;

namespace OnlineStore.Application.Delivery;

public interface IDeliveryCostStrategy
{
    decimal Calculate(Order order);
}

// Фиксированная цена
public class FlatRateDeliveryCost : IDeliveryCostStrategy
{
    private const decimal Fee = 50m;
    public decimal Calculate(Order order) => Fee;
}

// Бесплатно от порога, иначе фиксированная цена
public class FreeOverThresholdDeliveryCost : IDeliveryCostStrategy
{
    private const decimal Threshold = 1000m;
    private const decimal Fee = 70m;
    public decimal Calculate(Order order) => order.Total >= Threshold ? 0m : Fee;
}
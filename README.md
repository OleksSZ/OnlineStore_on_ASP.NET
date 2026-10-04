# Online Store Management API

Система керування інтернет-магазином: клієнти, товари, замовлення, кур'єри, маршрути доставки.
Стек: C# / ASP.NET Core Web API (.NET 10), EF Core, PostgreSQL, Docker.

## Архітектура

- `Domain` — сутності та правила (Order, Customer, Courier, Route, Product), інтерфейси репозиторіїв
- `Application` — DTO, сервіси, стратегії розрахунку доставки
- `Infrastructure` — EF Core, репозиторії, міграції
- `Api` — контролери, middleware, Program.cs

Залежності йдуть всередину: Api → Application → Domain, Infrastructure → Domain.

## Запуск у Docker

```bash
cp .env.example .env        # задати DB_PASSWORD
docker compose up -d --build
```

Документація API (Scalar): http://localhost:8080/scalar/v1
Сам міні-сайт http://localhost:8080/
## Локальна розробка

```bash
docker compose up -d db
dotnet user-secrets init --project OnlineStore.Api
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5433;Database=onlinestore;Username=store;Password=ПАРОЛЬ" --project OnlineStore.Api
dotnet run --project OnlineStore.Api
```

## Корисні команди

```bash
docker compose logs -f api       # логи API
docker compose down              # зупинити (дані БД зберігаються)
docker compose down -v           # зупинити і видалити дані БД

dotnet ef migrations add Назва -p OnlineStore.Infrastructure -s OnlineStore.Api
```

## Життєвий цикл замовлення

New → Confirmed → Packed → InDelivery → Delivered (скасувати можна до InDelivery).
Доставка йде через маршрут: курʼєр займається при старті маршруту і звільняється при завершенні.

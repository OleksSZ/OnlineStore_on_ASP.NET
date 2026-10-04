using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Application.Exceptions;

namespace OnlineStore.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _log;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> log)
    {
        _next = next;
        _log = log;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (Exception ex)
        {
            var (status, title, detail) = ex switch
            {
                NotFoundException => (404, "Не знайдено", ex.Message),
                ArgumentException => (400, "Некоректні дані", ex.Message),
                InvalidOperationException => (409, "Операція неможлива", ex.Message),
                DbUpdateException => (409, "Конфлікт даних", "Операція порушує зв'язки в БД (є пов'язані записи)"),
                _ => (500, "Внутрішня помилка сервера", "Щось пішло не так")
            };

            if (status == 500) _log.LogError(ex, "Необроблена помилка");

            ctx.Response.StatusCode = status;
            await ctx.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail
            });
        }
    }
}
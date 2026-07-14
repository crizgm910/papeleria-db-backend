using System.Security.Claims;
using PapeleriaDB.Application.Services;

namespace PapeleriaDB.Api.Middleware;

public class AuditMiddleware
{
    private readonly RequestDelegate _next;
    public AuditMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, IAuditoriaService auditoriaService)
    {
        await _next(context);
        if (context.User.Identity?.IsAuthenticated != true || context.Request.Method is not ("POST" or "PUT" or "PATCH" or "DELETE") || context.Response.StatusCode is < 200 or >= 400) return;
        var userIdValue = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdValue, out var userId)) return;
        var segments = context.Request.Path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries) ?? [];
        var resource = segments.Length > 1 ? segments[1] : "Sistema";
        var resourceId = context.Request.RouteValues.TryGetValue("id", out var id) ? id?.ToString() : null;
        var action = context.Request.Method switch { "POST" when context.Request.Path.Value?.Contains("devolucion") == true => "Devolución", "POST" when context.Request.Path.Value?.Contains("abrir") == true => "Apertura", "POST" when context.Request.Path.Value?.Contains("cerrar") == true => "Cierre", "POST" => "Creación", "PUT" or "PATCH" => "Actualización", "DELETE" => "Eliminación", _ => "Cambio" };
        await auditoriaService.RecordAsync(userId, context.User.Identity.Name ?? $"Usuario {userId}", action, resource, resourceId, context.Request.Method, context.Request.Path, context.Response.StatusCode);
    }
}

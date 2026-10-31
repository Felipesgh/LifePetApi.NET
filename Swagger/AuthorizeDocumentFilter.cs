using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LifePetApi.Swagger;

public sealed class AuthorizeDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument document, DocumentFilterContext context)
    {
        foreach (var description in context.ApiDescriptions)
        {
            if (description.ActionDescriptor is not ControllerActionDescriptor action)
                continue;

            var method = action.MethodInfo;
            var controller = action.ControllerTypeInfo;
            if (method.IsDefined(typeof(AllowAnonymousAttribute), inherit: true)
                || controller.IsDefined(typeof(AllowAnonymousAttribute), inherit: true))
                continue;

            if (!method.IsDefined(typeof(AuthorizeAttribute), inherit: true)
                && !controller.IsDefined(typeof(AuthorizeAttribute), inherit: true))
                continue;

            if (string.IsNullOrWhiteSpace(description.HttpMethod)
                || !document.Paths.TryGetValue($"/{description.RelativePath}", out var path)
                || path is null
                || !path.Operations.TryGetValue(new HttpMethod(description.HttpMethod), out var operation))
                continue;

            operation.Security ??= [];
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });
        }
    }
}

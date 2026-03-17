using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ModularBackend.Api.Swagger;

public sealed class AuthorizationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var metadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;

        var hasAllowAnonymous = metadata.OfType<AllowAnonymousAttribute>().Any();
        if (hasAllowAnonymous)
            return;

        var hasAuthorize = metadata.OfType<IAuthorizeData>().Any();
        if (!hasAuthorize)
            return;

        operation.Security ??= new List<OpenApiSecurityRequirement>();

        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer")] = []
        });
    }
}

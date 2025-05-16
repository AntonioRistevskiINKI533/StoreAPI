/*using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class SwaggerNullableParameterFilter : IParameterFilter
{
	public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
	{
		if (!parameter.Schema.Nullable &&
			(context.ApiParameterDescription.Type.IsNullable() || !context.ApiParameterDescription.Type.IsValueType))
		{
			parameter.Schema.Nullable = true;
		}
	}
}*/
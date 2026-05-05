//---------------------------------------------------------------------------------
// Copyright (c) May 2026, devMobile Software
//
// http://www.apache.org/licenses/LICENSE-2.0
//
/*
   <PropertyGroup>
      <InterceptorsNamespaces>$(InterceptorsNamespaces);Microsoft.AspNetCore.Http.Validation.Generated</InterceptorsNamespaces>
   </PropertyGroup>
*/
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddValidation();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   app.MapOpenApi();
   app.MapScalarApiReference();
}

app.UseHttpsRedirection();

//app.MapPost(pattern: "/Neighborhoods/", async ([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] NeighborhoodDto? neighborhood) =>
//app.MapPost(pattern: "/Neighborhoods/", async (NeighborhoodDto? neighborhood) =>
app.MapPost(pattern: "/Neighborhoods", async (NeighborhoodDto? neighborhood) =>
{
   if (neighborhood is null)
   {
      return Results.BadRequest();
   }

   return Results.NoContent();
}).WithName("NeighborhoodUpdate").AddEndpointFilter<RequireJsonBodyFilter<NeighborhoodDto>>();

app.MapPost(pattern: "/Neighborhoods", async (HttpContext http) =>
{
   if (http.Request.ContentLength is null or 0)
   {
      return Results.BadRequest("Request body is required.");
   }

   /*
   var neighborhood = await JsonSerializer.DeserializeAsync<NeighborhoodDto>(http.Request.Body);
   if (neighborhood is null)
   {
      return Results.BadRequest();
   }
   */
   /*
   var neighborhood = await http.Request.ReadFromJsonAsync<NeighborhoodDto>();
   if (neighborhood is null)
   {
      return Results.BadRequest();
   }
   */

   /*   
   try
   {
      // Force JSON read to get a clean error instead of crashing pipeline
      //var neighborhood = await http.Request.ReadFromJsonAsync<NeighborhoodDto>();
      var neighborhood = await JsonSerializer.DeserializeAsync<NeighborhoodDto>(http.Request.Body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
      if (neighborhood is null)
      {
         return Results.BadRequest();
      }
   }
   catch (JsonException)
   {
      return Results.BadRequest("Invalid JSON payload.");
   }
   */
   return Results.NoContent();
}).WithName("NeighborhoodUpdate");//.AddEndpointFilter<RequireJsonBodyFilter<NeighborhoodDto>>();

app.Run();


public sealed class RequireJsonBodyFilter<T> : IEndpointFilter
{
   public async ValueTask<object?> InvokeAsync(
       EndpointFilterInvocationContext context,
       EndpointFilterDelegate next)
   {
      var http = context.HttpContext.Request;

      if (http.ContentLength is null or 0)
      {
         return Results.BadRequest("Request body is required.");
      }

      return await next(context);
   }
}

internal record NeighborhoodDto
{
   [Range(0, 100)]
   public long Id { get; init; }

   [MinLength(1), MaxLength(15)]
   public string Url { get; init; } = string.Empty;

   [MinLength(1), MaxLength(10)]
   public string Name { get; init; } = string.Empty;

   [Range(0.0, 50.0)]
   public float AreaInSquareKilometers { get; init; }
}


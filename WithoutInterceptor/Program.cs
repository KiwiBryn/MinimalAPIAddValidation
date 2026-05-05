//---------------------------------------------------------------------------------
// Copyright (c) May 2026, devMobile Software
//
// http://www.apache.org/licenses/LICENSE-2.0
//
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
}).WithName("NeighborhoodUpdate").AddEndpointFilter<RequireJsonBodyFilter<NeighborhoodDto>>() ;

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
   [Range(0,100)]
   public long Id { get; init; }

   [MinLength(1), MaxLength(15)]
   public string Url { get; init; } = string.Empty;

   [MinLength(1), MaxLength(10)]
   public string Name { get; init; } = string.Empty;

   [Range(0.0, 50.0)]
   public float AreaInSquareKilometers { get; init; }
}


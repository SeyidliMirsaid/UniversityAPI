using Education.API;
using Education.Business;
using Education.Infrastructure;
using Education.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// ====================== Start Services =====================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// ====================== Api Services =====================

builder.Services.InjectApiLayer(builder.Configuration);

// ====================== Business Services =====================
builder.Services.InjectBusinessLayer(builder.Configuration);

// ====================== Domain Services =====================


// ====================== Infrastructure Services =====================

builder.Services.InjectInfraStructure(builder.Configuration);


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

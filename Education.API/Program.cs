using Education.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ====================== Start Services =====================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ====================== Api Services =====================


// ====================== Business Services =====================


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

app.UseAuthorization();

app.MapControllers();

app.Run();

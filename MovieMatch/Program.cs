using MovieMatch.Hubs;
using MovieMatch.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IRoomStore, InMemoryRoomStore>();
builder.Services.AddHttpClient<IMovieApiService, MovieApiService>();

var app = builder.Build();

app.MapHub<RoomHub>("/hub");

if (app.Environment.IsDevelopment())
{
    app.UseCors(x => x
        .AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed(origin => true) // Allow any origin
        .AllowCredentials() // Allow credentials
    );
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();


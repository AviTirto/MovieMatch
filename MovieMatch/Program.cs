using MovieMatch.Hubs;
using MovieMatch.Services;
using MovieMatch.Services.Game;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IRoomStore, InMemoryRoomStore>();
builder.Services.AddHttpClient<IMovieApiService, MovieApiService>();
builder.Services.AddScoped<IMovieFetchService, MovieFetchService>();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("MyCorsPolicy");

app.MapHub<RoomHub>("/hub");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
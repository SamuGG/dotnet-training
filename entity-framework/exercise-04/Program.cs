using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqlite<GameStore.Api.Data.GameStoreContext>(builder.Configuration.GetConnectionString("GameStore"));
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGamesEndpoints();
app.Run();

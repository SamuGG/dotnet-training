using GameStore.Api.Data;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    public static RouteGroupBuilder MapGamesEndpoints(this IEndpointRouteBuilder routes)
    {
        const string GetGameEndpointName = "GetGame";
        RouteGroupBuilder group = routes.MapGroup("/games");

        group.MapGet("/", async (GameStoreContext db)
            => await db.Games.AsNoTracking().ToListAsync());

        group
            .MapGet("/{id:int}", async (int id, GameStoreContext db)
                => await db.Games.FindAsync(id) is Game game ? Results.Ok(game) : Results.NotFound())
            .WithName(GetGameEndpointName);

        group.MapPost("/", async (Game game, GameStoreContext db) =>
        {
            db.Games.Add(game);
            await db.SaveChangesAsync();
            return Results.CreatedAtRoute(GetGameEndpointName, new { game.Id }, game);
        });

        group.MapPut("/{id:int}", async (int id, Game game, GameStoreContext db) =>
        {
            if (await db.Games.FindAsync(id) is not Game existingGame)
                return Results.NotFound();

            db.Entry(existingGame).CurrentValues.SetValues(game);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapDelete("/{id:int}", async (int id, GameStoreContext db) =>
        {
            await db.Games.Where(game => game.Id == id).ExecuteDeleteAsync();
            return Results.NoContent();
        });

        return group;
    }
}
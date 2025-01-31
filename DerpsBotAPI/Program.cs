
using BadgerClan.Logic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<movingClass>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/change/{input}", (string input, movingClass mc) => {
    string logic = input;
    mc.changeStrat(input);

});

app.MapPost("/", (MoveRequest request, movingClass mc) => {

    List<Move> moves = new();
    MoveResponse response = new MoveResponse(mc.MakeMoves(request));
    return response;
}).WithName("SendMoveRequest");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


public class movingClass {
    private static string _moveStrategy = "HOLD"; // Default move strategy

    public List<Move> MakeMoves(MoveRequest request) {
        List<Move> moves = new();

        foreach (var unit in request.Units) {
            Coordinate newLocation = _moveStrategy switch {
                "NW" => unit.Location.MoveNorthWest(1),
                "NE" => unit.Location.MoveNorthEast(1),
                "W" => unit.Location.MoveWest(1),
                "E" => unit.Location.MoveEast(1),
                "SW" => unit.Location.MoveSouthWest(1),
                "SE" => unit.Location.MoveSouthEast(1),
                "CORNER" => MoveToClosestCorner(unit, request.BoardSize, request.Units),
                _ => unit.Location // Default: Hold position
            };

            // Validate move within board boundaries
            if (newLocation.Q >= 0 && newLocation.R >= 0 &&
                newLocation.Q < request.BoardSize && newLocation.R < request.BoardSize) {
                moves.Add(new Move(MoveType.Walk, unit.Id, newLocation));
            }

            // Check if enemy is in range and attack
            var enemies = request.Units.Where(u => u.Team != unit.Team);
            var closestEnemy = enemies.OrderBy(e => e.Location.Distance(unit.Location)).FirstOrDefault();

            if (closestEnemy != null && closestEnemy.Location.Distance(unit.Location) <= unit.AttackDistance) {
                moves.Add(new Move(MoveType.Attack, unit.Id, closestEnemy.Location));
            }
        }

        return moves;
    }

    public void changeStrat(string strat) {
        _moveStrategy = strat;
    }

    private static Coordinate MoveToClosestCorner(UnitDto unit, int boardSize, IEnumerable<UnitDto> allUnits) {
        List<Coordinate> corners = new()
        {
            new Coordinate(0, 0),                    // Top-left
            new Coordinate(boardSize - 1, 0),        // Top-right
            new Coordinate(0, boardSize - 1),        // Bottom-left
            new Coordinate(boardSize - 1, boardSize - 1) // Bottom-right
        };

        // Find the closest corner
        var closestCorner = corners.OrderBy(c => c.Distance(unit.Location)).First();

        // Move knights just outside the corner
        if (unit.Type == "Knight") {
            return unit.Location.Toward(closestCorner).Away(closestCorner);
        }

        // Move archers into the corner
        return unit.Location.Toward(closestCorner);
    }
}

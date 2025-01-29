
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
    private static int moveNum = 1;

    public List<Move> MakeMoves(MoveRequest request) {
        List<Move> moves = new();

        if (_moveStrategy == "Kata")
        {
            foreach (var unit in request.Units)
            {
                switch (moveNum)
                {
                    case 1:
                        unit.Location.MoveNorthWest(1);
                        break;
                    case 2:
                        unit.Location.MoveWest(1);
                        break;
                    case 3:
                        unit.Location.MoveSouthWest(1);
                        break;
                    case 4:
                        unit.Location.MoveSouthEast(1);
                        break;
                    case 5:
                        unit.Location.MoveEast(1);
                        break;
                    case 6:
                        unit.Location.MoveNorthEast(1);
                        break;
                    default: 
                        throw new ArgumentException("moveNum was an unexpected value.");
                }
                
            }

            if (moveNum >= 5)
            {
                moveNum++;

            }
            else
            {
                moveNum = 1;
            }
        }

        foreach (var unit in request.Units) {
            Coordinate newLocation = _moveStrategy switch {
                "NW" => unit.Location.MoveNorthWest(1),
                "NE" => unit.Location.MoveNorthEast(1),
                "W" => unit.Location.MoveWest(1),
                "E" => unit.Location.MoveEast(1),
                "SW" => unit.Location.MoveSouthWest(1),
                "SE" => unit.Location.MoveSouthEast(1),
                _ => unit.Location
            };

            if (newLocation.Q >= 0 && newLocation.R >= 0 &&
                newLocation.Q < request.BoardSize && newLocation.R < request.BoardSize) {
                moves.Add(new Move(MoveType.Walk, unit.Id, newLocation));
            }
        }

        return moves;
    }

    public void changeStrat(string strat) {
        _moveStrategy = strat;
    }

    

}

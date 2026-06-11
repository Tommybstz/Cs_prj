using TodoApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

var todos = new List<Todo>();

app.MapGet("/todos", () => todos);
app.MapGet("/todos/completed", () => todos.Where(t=>t.Completed));
app.MapPost("/todos", (Todo req) =>
{
    var todo = new Todo
    {
        Title = req.Title,
        Description = req.Description,
        Completed = req.Completed
    };
    todos.Add(todo);
    return Results.Created($"/todos/{todo.Id}", todo);
});
app.MapDelete("/todos/{id}", (int id) => { 
    todos.Remove(todos.FirstOrDefault(t=>t.Id==id));
    return Results.NoContent;
});



app.Run();
var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseRouting();

app.MapGet("/", () => "DMS Configuration Service");
app.MapGet("/ping", () => Results.Ok(DateTime.Now));

app.Run();

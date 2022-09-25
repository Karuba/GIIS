List<Contact> contacts = new List<Contact>()
{
    new Contact(){ Name = "Alex", Address="Kolotushkino" },
    new Contact(){ Name = "Dranik", Address="Bombass" }
};

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/contacts", () => contacts);
app.MapGet("/api/contacts/{name}", (string name) =>
{

});
app.MapGet("/api/contacts/{address}", (string address) =>
{

});
app.MapPut("/api/contacts", (Contact contact) =>
{

});
app.MapPost("/api/contacts", (Contact contact) =>
{

});
app.MapDelete("/api/contacts/{name}/{address}", (string name, string address) =>
{

});

app.Run();

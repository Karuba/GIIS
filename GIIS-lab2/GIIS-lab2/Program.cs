List<Contact> contacts = new List<Contact>()
{
    new Contact(){ Id = Guid.NewGuid(),  Name = "Alex", Address="Kolotushkino" },
    new Contact(){ Id = Guid.NewGuid(), Name = "Dranik", Address="Bombass" }
};

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/contacts", () => contacts);
app.MapGet("/api/contact/{id}", (Guid id) =>
{
    Contact? contact = contacts.FirstOrDefault(c => c.Id.Equals(id));

    if (contact == null)
        return Results.NotFound(new { message = "Contact not found"});
    
    return Results.Json(contact);
});
app.MapPut("/api/contacts", (Contact contactData) =>
{
    Contact? contact = contacts.FirstOrDefault(c => c.Id.Equals(contactData.Id));
    if (contact == null)
        return Results.NotFound(new { message = "Contact not found" });

    contact.Name = contactData.Name;
    contact.Address = contactData.Address;

    return Results.Json(contact);
});
app.MapPost("/api/contacts", (Contact contactData) =>
{
    Contact? contact = contacts.FirstOrDefault(c => c.Id.Equals(contactData.Id));

    if (contact == null)
    {
        contacts.Add(contactData);
    }
    else
    {
        return Results.Conflict(new { message = "Contact already exist" });
    }
    return Results.Json(contactData);
});
app.MapDelete("/api/contact/{id}", (Guid id) =>
{
    Contact? contact = contacts.FirstOrDefault(c => c.Id.Equals(id));

    if (contact == null)
        return Results.NotFound(new { message = "Contact not found" });
    
    contacts.Remove(contact);

    return Results.Json(contact);
});

app.Run();

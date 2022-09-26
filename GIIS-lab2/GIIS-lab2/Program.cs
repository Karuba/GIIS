using GIIS_lab2.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<RepositoryContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MsSQLConnection")));

var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/contacts", (RepositoryContext db) => db.Contacts);
app.MapGet("/api/contact/{id}", async (Guid id, RepositoryContext db) =>
{
    Contact? contact = await db.Contacts.FirstOrDefaultAsync(c => c.Id.Equals(id));

    if (contact == null)
        return Results.NotFound(new { message = "Contact not found"});
    
    return Results.Json(contact);
});
app.MapGet("/api/contacts/{name}", async (string name, RepositoryContext db) =>
{
    List<Contact>? contacts = await db.Contacts.Where(c => c.Name.Equals(name)).ToListAsync();

    if (contacts == null)
        return Results.NotFound(new { message = "No Contact with this name was found" });

    return Results.Json(contacts);
});
app.MapPut("/api/contacts", async (Contact contactData, RepositoryContext db) =>
{
    Contact? contact = await db.Contacts.FirstOrDefaultAsync(c => c.Id.Equals(contactData.Id));

    if (contact == null)
        return Results.NotFound(new { message = "Contact not found" });

    contact.Name = contactData.Name;
    contact.Address = contactData.Address;

    await db.SaveChangesAsync();

    return Results.Json(contact);
});
app.MapPost("/api/contacts", async (Contact contactData, RepositoryContext db) =>
{
    Contact? contact = await db.Contacts.FirstOrDefaultAsync(c => c.Id.Equals(contactData.Id));

    if (contact == null)
    {
        await db.Contacts.AddAsync(contactData);
        await db.SaveChangesAsync();
    }
    else
    {
        return Results.Conflict(new { message = "Contact already exist" });
    }
    return Results.Json(contactData);
});
app.MapDelete("/api/contact/{id}", async (Guid id, RepositoryContext db) =>
{
    Contact? contact = await db.Contacts.FirstOrDefaultAsync(c => c.Id.Equals(id));

    if (contact == null)
        return Results.NotFound(new { message = "Contact not found" });

    db.Contacts.Remove(contact);
    await db.SaveChangesAsync();

    return Results.Json(contact);
});

app.Run();

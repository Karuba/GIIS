using GIIS_lab2.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

var server = Configuration["DBServer"] ?? "localhost";
var port = Configuration["DBPort"] ?? "1433";
var user = Configuration["DBUser"] ?? "SA";
var password = Configuration["DBPassword"] ?? "Pa55w0rd";
var database = Configuration["Database"] ?? "ContactsManager";

builder.Services.AddDbContext<RepositoryContext>(options => options.UseSqlServer($"Server={server}, {port};Initial Catalog={database};User ID={user};Password={password}" /*Configuration.GetConnectionString("SqlServer")*/));

var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();


app.MapGet("/api/contacts", async (RepositoryContext db) => await db.Contacts.ToListAsync());
app.MapGet("/api/contacts/file", async (RepositoryContext db) =>
{
    string path = @"C:\WorkPlace\University-4-7\labs\\GIIS\GIIS-lab2\GIIS-lab2\wwwroot\Files\Contacts.txt";

    File.Delete(path);
    StreamWriter writer = new StreamWriter(path, true);

    var contacts = await db.Contacts.ToListAsync();
    contacts.ForEach(c => writer.WriteLine($"Name: {c.Name}, Address: {c.Address};"));
    writer.Close();


    return Results.File(File.ReadAllBytes(path), "text/plain");
});
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

    Contact? contactComp = await db.Contacts.FirstOrDefaultAsync(c => c.Name.Equals(contactData.Name) && c.Address.Equals(contactData.Address));
    if (contactComp != null)
        return Results.Conflict(new { message = "Contact already exist" });

    contact.Name = contactData.Name;
    contact.Address = contactData.Address;

    await db.SaveChangesAsync();

    return Results.Json(contact);
});
app.MapPost("/api/contacts", async (Contact contactData, RepositoryContext db) =>
{
    Contact? contact = await db.Contacts.FirstOrDefaultAsync(c => c.Id.Equals(contactData.Id));
    Contact? contactComp = await db.Contacts.FirstOrDefaultAsync(c => c.Name.Equals(contactData.Name) && c.Address.Equals(contactData.Address));

    if (contact == null && contactComp == null)
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

app.MapPost("/upload", async (HttpContext context, RepositoryContext db) =>
{
    var response = context.Response;
    var request = context.Request;

    response.ContentType = "text/html; charset=utf-8";

    if (request.Path == "/upload" && request.Method == "POST")
    {
        IFormFileCollection files = request.Form.Files;
        // путь к папке, где будут храниться файлы
        var uploadPath = $"{Directory.GetCurrentDirectory()}/uploads";
        // создаем папку для хранения файлов
        Directory.CreateDirectory(uploadPath);

        try
        {
            foreach (var file in files)
            {
                // путь к папке uploads
                string fullPath = $"{uploadPath}/{file.FileName}";

                // сохраняем файл в папку uploads
                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                using (var reader = new StreamReader(fullPath))
                {
                    var line = new List<string>();
                    while (!reader.EndOfStream)
                    {
                        string str = reader.ReadLine() ?? "";
                        str = str.Replace("Name: ", "");
                        str = str.Replace("Address: ", "");
                        str = str.Replace(";", "");
                        string[] strings = str.Split(", ");


                        Contact? contactComp = await db.Contacts.FirstOrDefaultAsync(c => c.Name.Equals(strings[0]) && c.Address.Equals(strings[1]));

                        if (contactComp == null)
                        {
                            var contactData = new Contact() { Id = Guid.NewGuid(), Name = strings[0], Address = strings[1] };
                            await db.Contacts.AddAsync(contactData);
                            await db.SaveChangesAsync();
                        }

                    }
                }
            }
        }
        catch (Exception)
        {
            await response.WriteAsync("Файл не соответсвует <button onclick=\"window.location.href=\'/\';\">return</button>");
            throw;
        }
        
        await response.WriteAsync("Файлы успешно загружены <button onclick=\"window.location.href=\'/\';\">return</button>");
    }
    else
    {
        await response.SendFileAsync("wwwroot/index.html");
    }
});

app.Run();
//[Index(nameof(Name), (nameof(Address)))]
public class Contact
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
}
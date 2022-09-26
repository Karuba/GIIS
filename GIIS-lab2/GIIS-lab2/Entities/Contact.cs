using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

[Index(nameof(Name), (nameof(Address)))]
public class Contact
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    [MinLength(3)]
    public string Name { get; set; }
    [Required]
    [MinLength(3)]
    public string Address { get; set; }
}
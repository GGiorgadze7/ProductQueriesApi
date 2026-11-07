using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductQueriesApi.Entities;

[Table("Categories")]
public class Category
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }
    public List<Product> Products { get; set; } = new();



}

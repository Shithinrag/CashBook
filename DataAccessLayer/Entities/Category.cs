
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities
{
    public class Category
    {
     
        public int Id { get; set; }
        [StringLength(20)]
        public string Name { get; set; } = "";
        [StringLength(10)]
        public string Type { get; set; } = "";
        public bool IsDelete { get; set; }
    }
    public class CategoryList
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

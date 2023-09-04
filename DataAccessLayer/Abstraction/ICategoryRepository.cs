using DataAccessLayer.Entities;

namespace DataAccessLayer.Abstraction
{
    public interface ICategoryRepository
    {
        public void AddCategory(Category category);
        public void UpdateCategory(Category category);
        public void ToggleStatus(int id);
        public Category GetCategoryDetails(int id);
        public IEnumerable<Category> GetAllCategory();
        public bool CheckDuplicateCategory(string name,string type, int? id = null);
        public IEnumerable<CategoryList> GetActiveCategory(string type);
    }
       
}
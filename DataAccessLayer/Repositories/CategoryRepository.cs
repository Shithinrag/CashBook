using DataAccessLayer.Abstraction;
using DataAccessLayer.Entities;
using Logger;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class CategoryRepository: ICategoryRepository
    {
        private readonly AppDb _appDb;
        public CategoryRepository(AppDb appDb)
        {
            _appDb = appDb;
        }
        public void AddCategory(Category category)
        {
            SqlParameter[] parameters = new SqlParameter[3];
            parameters[0] = new SqlParameter("@name", category.Name);
            parameters[1] = new SqlParameter("@type", category.Type);
            parameters[2] = new SqlParameter("@isD", category.IsDelete);
            try
            {
                _appDb.Database.ExecuteSqlRaw("Add_Category @name,@type,@isD", parameters);
            }
            catch (Exception ex)
            {
                CashBookLogger.LogError(ex, "Inside CategoryRepository- Add_Category");
                Console.Error.WriteLine(ex.Message);
            }
        }
        public void UpdateCategory(Category category)
        {
            SqlParameter[] parameters = new SqlParameter[4];
            parameters[0] = new SqlParameter("@id", category.Id);
            parameters[1] = new SqlParameter("@name", category.Name);
            parameters[2] = new SqlParameter("@type", category.Type);
            parameters[3] = new SqlParameter("@isD", category.IsDelete);
            try
            {
                _appDb.Database.ExecuteSqlRaw("Update_Category @id,@name,@type,@isD", parameters);
            }
            catch (Exception ex)
            {
                CashBookLogger.LogError(ex, "Inside CategoryRepository-UpdateCategory");
                Console.Error.WriteLine(ex.Message);
            }
        }
        public void ToggleStatus(int id)
        {
            Category category = GetCategoryDetails(id);
            category.IsDelete = !category.IsDelete;
            UpdateCategory(category);
        }
        public Category GetCategoryDetails(int id)
        {
            SqlParameter[] parameters = new SqlParameter[1];
            parameters[0] = new SqlParameter("@id", id);
            try
            {
                var result = _appDb.Category.FromSqlRaw<Category>("GetDetails_Category @id", parameters).AsEnumerable().FirstOrDefault();
                result ??= new Category();
                return (Category)result;
            }
            catch (Exception ex)
            {
                CashBookLogger.LogError(ex, "Inside CategoryRepository-GetCategoryDetails");
                Console.Error.WriteLine(ex.Message);
                throw;
            }
        }
        public IEnumerable<Category> GetAllCategory()
        {
            try
            {
                var result = _appDb.Category.FromSqlRaw<Category>("GetAll_Category");
                return (IEnumerable<Category>)result;
            }
            catch (Exception ex)
            {
                CashBookLogger.LogError(ex, "Inside CategoryRepository- GetAllCategory");
                Console.Error.WriteLine(ex.Message);
                throw;
            }
        }
        public bool CheckDuplicateCategory(string name, string type, int? id = null)
        {
            if (id == null)
            {
                if (_appDb.Category.Any(p =>p.Type == type && p.Name == name))
                {
                    return false;
                }
                return true;
            }
            if (_appDb.Category.Any(p => p.Id != id && p.Type == type &&  p.Name == name))
            {
                return false;
            }
            return true;

        }
        public IEnumerable<CategoryList> GetActiveCategory(string type)
        {
            try
            {
                var result = _appDb.Category
                    .Where(c=>c.Type == type && c.IsDelete == false).Select(c=> new CategoryList { Id = c.Id, Name = c.Name }).ToList();

                return (IEnumerable<CategoryList>)result;
            }
            catch (Exception ex)
            {
                CashBookLogger.LogError(ex, "Inside CategoryRepository- GetActiveCategory");
                Console.Error.WriteLine(ex.Message);
                throw;
            }
        }
    }
}


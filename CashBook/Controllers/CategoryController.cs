using DataAccessLayer.Abstraction;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CashBook.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public IActionResult Index()
        {
            ViewBag.Options = new List<SelectListItem>
                            {
                                new SelectListItem { Value = "false", Text = "Active" },
                                new SelectListItem { Value = "true", Text = "Inactive" }
            };
            IEnumerable<Category> categoryDetails = _categoryRepository.GetAllCategory();
            return PartialView("Category/_categoryTable", categoryDetails);
        }
        
        [HttpPost]
        public IActionResult Create([FromBody] Category category)
        {
            if (_categoryRepository.CheckDuplicateCategory(category.Name, category.Type))
            {
                _categoryRepository.AddCategory(category);
                return Json(new { success = true, message = "Category added successfully" });
            }
            return Json(new { success = false, message = "Name already exists" });
        }
        public IActionResult Update(int id)
        {
            Category details = _categoryRepository.GetCategoryDetails(id);
            return Json(details);
        }
        [HttpPost]
        public IActionResult Update([FromBody] Category category)
        {
            if (_categoryRepository.CheckDuplicateCategory(category.Name, category.Type, category.Id))
            {
                _categoryRepository.UpdateCategory(category);
                return Json(new { success = true, message = "Category updated successfully" });
            }
            return Json(new { success = false, message = "Name already exists" });
        }
        public void ToggleStatus(int id)
        {
            _categoryRepository.ToggleStatus(id);
        }
    }
}

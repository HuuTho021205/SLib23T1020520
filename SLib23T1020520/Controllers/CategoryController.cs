using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SLib23T1020520.BusinessLayer;
using SLib23T1020520.Models.Entities;

namespace SLib23T1020520.WebApp.Controllers
{
    public class CategoryController(CategoryService _categoryService) : Controller
    {
        // hiển thị danh sách
        public async Task<IActionResult> Index()
        {
            try
            {
                var model = await _categoryService.ListAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                //ghi lại log lỗi
                return Content("Hệ thống hiện đang quá tải. Vui lòng thử lại sau vài phút");
            }
        }

        // giao diện để nhập phân loại sách cần tạo
        public IActionResult Create()
        {
            ViewBag.title = "Bổ sung phân loại sách mới";
            var model = new Category();
            {
                model.CategoryId = 0;
            }
            ;
            return View("Edit", model);
        }

        // giao diện để nhập phân loại sách cần sửa
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.title = "Cập nhật thông tin phân loại sách";
            var model = await _categoryService.GetAsync(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> SaveData(Category data)
        {
            //TODO: phải kiểm tra dữ liệu đầu vào
            if (data.CategoryId == 0)
            {
                await _categoryService.AddAsync(data);
            }
            else
            {
                await _categoryService.UpdateAsync(data);
            }
            return RedirectToAction("Index");
        }

        // giao diện để hỏi xác nhận xóa một phân loại sách
        public async Task<IActionResult> Delete(int id)
        {
            if (Request.Method == "POST")
            {
                await _categoryService.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            var model = await _categoryService.GetAsync(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        /// <summary>
        /// Lấy phân loại sách theo mã; trả về <c>null</c> nếu mã không hợp lệ hoặc phân loại sách không tồn tại.
        /// </summary>
        private async Task<Category?> FindAsync(int id)
        {
            try
            {
                return await _categoryService.GetAsync(id);
            }
            catch (ValidationException)
            {
                return null;
            }
        }
    }
}
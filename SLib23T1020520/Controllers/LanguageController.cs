using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SLib23T1020520.BusinessLayer;
using SLib23T1020520.Models.Entities;

namespace SLib23T1020520.WebApp.Controllers
{
    public class LanguageController(LanguageService _languageService) : Controller
    {
        // hiển thị danh sách
        public async Task<IActionResult> Index()
        {
            try
            {
                var model = await _languageService.ListAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                //ghi lại log lỗi
                return Content("Hệ thống hiện đang quá tải. Vui lòng thử lại sau vài phút");
            }
        }

        // giao diện để nhập ngôn ngữ cần tạo
        public IActionResult Create()
        {
            ViewBag.title = "Bổ sung ngôn ngữ mới";
            var model = new Language();
            {
                model.LanguageId = 0;
            }
            ;
            return View("Edit", model);
        }

        // giao diện để nhập ngôn ngữ cần sửa
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.title = "Cập nhật thông tin ngôn ngữ";
            var model = await _languageService.GetAsync(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> SaveData(Language data)
        {
            //TODO: phải kiểm tra dữ liệu đầu vào
            if (data.LanguageId == 0)
            {
                await _languageService.AddAsync(data);
            }
            else
            {
                await _languageService.UpdateAsync(data);
            }
            return RedirectToAction("Index");
        }

        // giao diện để hỏi xác nhận xóa một ngôn ngữ
        public async Task<IActionResult> Delete(int id)
        {
            if (Request.Method == "POST")
            {
                await _languageService.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            var model = await _languageService.GetAsync(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        /// <summary>
        /// Lấy ngôn ngữ theo mã; trả về <c>null</c> nếu mã không hợp lệ hoặc ngôn ngữ không tồn tại.
        /// </summary>
        private async Task<Language?> FindAsync(int id)
        {
            try
            {
                return await _languageService.GetAsync(id);
            }
            catch (ValidationException)
            {
                return null;
            }
        }
    }
}
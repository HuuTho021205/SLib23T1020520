using Microsoft.AspNetCore.Mvc;
using SLib23T1020520.BusinessLayer;
using SLib23T1020520.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace SLib23T1020520.WebApp.Controllers
{
    public class PublisherController(PublisherService _publisherService) : Controller
    {
        // hiển thị danh sách
        public async Task<IActionResult> Index()
        {
            try
            {
                var model = await _publisherService.ListAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                //ghi lại log lỗi
                return Content("Hệ thống hiện đang quá tải. Vui lòng thử lại sau vài phút");
            }
        }

        public IActionResult Create()
        {
            ViewBag.title = "Bổ sung nhà xuất bản mới";
            var model = new Publisher();
            {
                model.PublisherId = 0;
            }
            ;
            return View("Edit", model);
        }

        // giao diện để nhập nhà xuất bản cần sửa
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.title = "Cập nhật thông tin nhà xuất bản";
            var model = await _publisherService.GetAsync(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> SaveData(Publisher data)
        {
            //TODO: phải kiểm tra dữ liệu đầu vào
            if (data.PublisherId == 0)
            {
                await _publisherService.AddAsync(data);
            }
            else
            {
                await _publisherService.UpdateAsync(data);
            }
            return RedirectToAction("Index");
        }
        // giao diện để hỏi xác nhận xóa một nhà xuất bản
        public async Task<IActionResult> Delete(int id)
        {
            if (Request.Method == "POST")
            {
                await _publisherService.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            var model = await _publisherService.GetAsync(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        /// <summary>
        /// Lấy nhà xuất bản theo mã; trả về <c>null</c> nếu mã không hợp lệ hoặc nhà xuất bản không tồn tại.
        /// </summary>
        private async Task<Publisher?> FindAsync(int id)
        {
            try
            {
                return await _publisherService.GetAsync(id);
            }
            catch (ValidationException)
            {
                return null;
            }
        }
    }
}

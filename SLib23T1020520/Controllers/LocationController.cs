using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SLib23T1020520.BusinessLayer;
using SLib23T1020520.Models.Entities;

namespace SLib23T1020520.WebApp.Controllers
{
    public class LocationController(LocationService _locationService) : Controller
    {
        // hiển thị danh sách
        public async Task<IActionResult> Index()
        {
            try
            {
                var model = await _locationService.ListAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                //ghi lại log lỗi
                return Content("Hệ thống hiện đang quá tải. Vui lòng thử lại sau vài phút");
            }
        }

        // giao diện để nhập vị trí lưu trữ cần tạo
        public IActionResult Create()
        {
            ViewBag.title = "Bổ sung vị trí lưu trữ mới";
            var model = new Location();
            {
                model.LocationId = 0;
            }
            ;
            return View("Edit", model);
        }

        // giao diện để nhập vị trí lưu trữ cần sửa
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.title = "Cập nhật thông tin vị trí lưu trữ";
            var model = await _locationService.GetAsync(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> SaveData(Location data)
        {
            //TODO: phải kiểm tra dữ liệu đầu vào
            if (data.LocationId == 0)
            {
                await _locationService.AddAsync(data);
            }
            else
            {
                await _locationService.UpdateAsync(data);
            }
            return RedirectToAction("Index");
        }

        // giao diện để hỏi xác nhận xóa một vị trí lưu trữ
        public async Task<IActionResult> Delete(int id)
        {
            if (Request.Method == "POST")
            {
                await _locationService.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            var model = await _locationService.GetAsync(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        /// <summary>
        /// Lấy vị trí lưu trữ theo mã; trả về <c>null</c> nếu mã không hợp lệ hoặc vị trí không tồn tại.
        /// </summary>
        private async Task<Location?> FindAsync(int id)
        {
            try
            {
                return await _locationService.GetAsync(id);
            }
            catch (ValidationException)
            {
                return null;
            }
        }
    }
}
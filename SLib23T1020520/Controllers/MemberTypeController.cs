using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SLib23T1020520.BusinessLayer;
using SLib23T1020520.Models.Entities;

namespace SLib23T1020520.WebApp.Controllers
{
    public class MemberTypeController(MemberTypeService _memberTypeService) : Controller
    {
        // hiển thị danh sách
        public async Task<IActionResult> Index()
        {
            try
            {
                var model = await _memberTypeService.ListAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                //ghi lại log lỗi
                return Content("Hệ thống hiện đang quá tải. Vui lòng thử lại sau vài phút");
            }
        }

        // giao diện để nhập loại thành viên cần tạo
        public IActionResult Create()
        {
            ViewBag.title = "Bổ sung phân loại độc giả mới";
            var model = new MemberType
            {
                MemberTypeId = 0,
                MaxActiveLoans = 3,
                LoanPeriodDays = 14,
                MaxRenewals = 0,
                IsActive = true
            };
            return View("Edit", model);
        }

        // giao diện để nhập loại thành viên cần sửa
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.title = "Cập nhật thông tin phân loại độc giả";
            var model = await _memberTypeService.GetAsync(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> SaveData(MemberType data)
        {
            //TODO: phải kiểm tra dữ liệu đầu vào
            if (data.MemberTypeId == 0)
            {
                await _memberTypeService.AddAsync(data);
            }
            else
            {
                await _memberTypeService.UpdateAsync(data);
            }
            return RedirectToAction("Index");
        }

        // giao diện để hỏi xác nhận xóa một loại thành viên
        public async Task<IActionResult> Delete(int id)
        {
            if (Request.Method == "POST")
            {
                await _memberTypeService.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            var model = await _memberTypeService.GetAsync(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        /// <summary>
        /// Lấy loại thành viên theo mã; trả về <c>null</c> nếu mã không hợp lệ hoặc loại thành viên không tồn tại.
        /// </summary>
        private async Task<MemberType?> FindAsync(int id)
        {
            try
            {
                return await _memberTypeService.GetAsync(id);
            }
            catch (ValidationException)
            {
                return null;
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using SLib23T1020520.BusinessLayer;
using SLib23T1020520.Models.Entities;
using System.Threading.Tasks;

namespace SLib23T1020520.WebApp.Controllers
{
    public class AuthorController(AuthorService _authorService) : Controller
    {   
        // hiển thị danh sách
        public async Task<IActionResult> Index()
        {
            try
            {
                var model = await _authorService.ListAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                //ghi lại log lỗi
                return Content("Hệ thống hiện đang quá tải. Vui lòng thử lại sau vài phút");
            }
        }
        // giao diện để nhập tác giả cần tạo
        public IActionResult Create()
        {
            ViewBag.title = "Bổ sung tác giả mới";
            var model = new Author();
            {
                model.AuthorId = 0;
            };
            return View("Edit", model);
        }

        // giao diện để nhập tác giả cần sửa
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.title = "Cập nhật thông tin tác giả";
            var model = await _authorService.GetAsync(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }    
            return View(model);
        }

        public async Task<IActionResult> SaveData(Author data)
        {
            //TODO: phải kiểm tra dữ liệu đầu vào
            if (data.AuthorId == 0)
            {
                await _authorService.AddAsync(data);
            }
            else
            {
                await _authorService.UpdateAsync(data);
            }
            return RedirectToAction("Index");
        }

        // giao diện để hỏi xác nhận xóa một tác giả
        public async Task<IActionResult> Delete(int id)
        {
            if(Request.Method == "POST")
            {
                await _authorService.DeleteAsync(id);
                return RedirectToAction("Index");
            }    
            var model = await _authorService.GetAsync(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

    }
}
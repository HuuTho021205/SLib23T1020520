using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SLib23T1020520.BusinessLayer;
using SLib23T1020520.Models.Entities;
using System.Threading.Tasks;

namespace SLib23T1020520.WebApp.Controllers
{
    public class AuthorController(AuthorService _authorService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var authors = await _authorService.ListAsync();
            return View(authors);
        }

        /// <summary>
        /// Hiển thị thông tin chi tiết của một tác giả.
        /// </summary>
        /// <param name="id">Mã tác giả.</param>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var author = await FindAsync(id);
            if (author == null)
                return NotFound();

            return View(author);
        }

        /// <summary>
        /// Hiển thị form nhập thông tin tác giả mới.
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Author());
        }

        /// <summary>
        /// Nhận dữ liệu từ form, nhờ <see cref="AuthorService"/> kiểm tra và lưu tác giả mới.
        /// Thành công thì chuyển về danh sách; dữ liệu sai thì hiển thị lại form kèm thông báo lỗi.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Author data)
        {
            // Tên tác giả do AuthorService kiểm tra (thông báo tiếng Việt). Bỏ lỗi "required"
            // mặc định (tiếng Anh) mà model binding tự sinh ra cho thuộc tính string không cho null.
            ModelState.Remove(nameof(Author.AuthorName));

            if (!ModelState.IsValid)
                return View(data);

            try
            {
                await _authorService.AddAsync(data);
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(data);
            }
        }

        /// <summary>
        /// Hiển thị form chỉnh sửa thông tin của một tác giả.
        /// </summary>
        /// <param name="id">Mã tác giả cần chỉnh sửa.</param>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var author = await FindAsync(id);
            if (author == null)
                return NotFound();

            return View(author);
        }

        /// <summary>
        /// Nhận dữ liệu từ form chỉnh sửa, nhờ <see cref="AuthorService"/> kiểm tra và cập nhật.
        /// Thành công thì chuyển về danh sách; dữ liệu sai thì hiển thị lại form kèm thông báo lỗi.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Author data)
        {
            // Xem giải thích ở action Create (POST).
            ModelState.Remove(nameof(Author.AuthorName));

            if (!ModelState.IsValid)
                return View(data);

            try
            {
                // false: không tìm thấy tác giả cần cập nhật (có thể đã bị xóa)
                if (!await _authorService.UpdateAsync(data))
                    return NotFound();

                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(data);
            }
        }

        /// <summary>
        /// Hiển thị trang xác nhận xóa một tác giả. Nếu tác giả đang được sử dụng
        /// (gắn với đầu sách) thì trang chỉ hiện cảnh báo và không cho xóa.
        /// </summary>
        /// <param name="id">Mã tác giả cần xóa.</param>
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var author = await FindAsync(id);
            if (author == null)
                return NotFound();

            ViewBag.InUsed = await _authorService.InUsedAsync(id);
            return View(author);
        }

        /// <summary>
        /// Thực hiện xóa tác giả sau khi người dùng xác nhận.
        /// Thành công thì chuyển về danh sách; không xóa được thì hiển thị lại trang xác nhận kèm lỗi.
        /// </summary>
        /// <param name="id">Mã tác giả cần xóa.</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // false: không tìm thấy tác giả cần xóa (có thể đã bị xóa trước đó)
                if (!await _authorService.DeleteAsync(id))
                    return NotFound();

                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException ex)
            {
                var author = await FindAsync(id);
                if (author == null)
                    return NotFound();

                ViewBag.InUsed = await _authorService.InUsedAsync(id);
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(author);
            }
        }

        /// <summary>
        /// Lấy tác giả theo mã; trả về <c>null</c> nếu mã không hợp lệ hoặc tác giả không tồn tại.
        /// </summary>
        private async Task<Author?> FindAsync(int id)
        {
            try
            {
                return await _authorService.GetAsync(id);
            }
            catch (ValidationException)
            {
                return null;
            }
        }
    }
}
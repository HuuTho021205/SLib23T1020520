using System.ComponentModel.DataAnnotations;
using SLib23T1020520.DataLayer.Interfaces;
using SLib23T1020520.Models.Entities;

namespace SLib23T1020520.BusinessLayer;

/// <summary>
/// Cung cấp các chức năng nghiệp vụ liên quan đến danh mục sách (<see cref="Category"/>):
/// kiểm tra tính hợp lệ của dữ liệu và điều phối các thao tác xử lý dữ liệu với
/// <see cref="ICategoryRepository"/>.
/// <para>
/// Quy ước xử lý lỗi: dữ liệu không hợp lệ hoặc vi phạm quy tắc nghiệp vụ thì ném
/// <see cref="ValidationException"/> với thông điệp tiếng Việt có thể hiển thị trực tiếp cho người dùng;
/// tham số bắt buộc là <c>null</c> thì ném <see cref="ArgumentNullException"/>; các lỗi hệ thống từ tầng dữ liệu
/// được để nguyên. Khi không tìm thấy bản ghi cần cập nhật hoặc cần xóa, <see cref="UpdateAsync(Category)"/>
/// và <see cref="DeleteAsync(int)"/> không ném exception mà trả về <c>false</c>.
/// </para>
/// </summary>
public class CategoryService
{
    // Độ dài tối đa của các cột trong bảng Categories
    private const int CategoryNameMaxLength = 150;
    private const int DescriptionMaxLength = 500;

    private readonly ICategoryRepository _repository;

    /// <summary>
    /// Khởi tạo dịch vụ nghiệp vụ danh mục sách.
    /// </summary>
    /// <param name="repository">Đối tượng thực hiện các phép xử lý dữ liệu trên bảng danh mục sách.</param>
    /// <exception cref="ArgumentNullException"><paramref name="repository"/> là <c>null</c>.</exception>
    public CategoryService(ICategoryRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    /// <summary>
    /// Lấy danh sách toàn bộ danh mục sách.
    /// </summary>
    /// <returns>Tập hợp các danh mục sách; tập hợp rỗng nếu chưa có dữ liệu.</returns>
    public async Task<IEnumerable<Category>> ListAsync()
    {
        return await _repository.ListAsync();
    }

    /// <summary>
    /// Lấy thông tin của một danh mục sách theo mã.
    /// </summary>
    /// <param name="id">Mã danh mục sách (phải lớn hơn 0).</param>
    /// <returns>Bản ghi tìm được; <c>null</c> nếu không tồn tại bản ghi có mã tương ứng.</returns>
    /// <exception cref="ValidationException"><paramref name="id"/> không hợp lệ.</exception>
    public async Task<Category?> GetAsync(int id)
    {
        ValidateId(id);
        return await _repository.GetAsync(id);
    }

    /// <summary>
    /// Bổ sung một danh mục sách mới.
    /// </summary>
    /// <param name="data">
    /// Thông tin cần bổ sung. Dữ liệu được chuẩn hóa trước khi lưu (đối tượng <paramref name="data"/> bị thay đổi
    /// trực tiếp). Bản ghi mới luôn được tạo ở trạng thái đang sử dụng (<c>IsActive = true</c>).
    /// </param>
    /// <returns>Mã của bản ghi vừa được bổ sung.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> là <c>null</c>.</exception>
    /// <exception cref="ValidationException">Dữ liệu không hợp lệ.</exception>
    public async Task<int> AddAsync(Category data)
    {
        ArgumentNullException.ThrowIfNull(data);

        Normalize(data);
        await ValidateDataAsync(data);
        data.IsActive = true;

        return await _repository.AddAsync(data);
    }

    /// <summary>
    /// Cập nhật thông tin của một danh mục sách đã có.
    /// </summary>
    /// <param name="data">
    /// Thông tin mới; bản ghi cần cập nhật được xác định bằng <c>CategoryId</c>.
    /// Dữ liệu được chuẩn hóa trước khi lưu (đối tượng <paramref name="data"/> bị thay đổi trực tiếp).
    /// </param>
    /// <returns>
    /// <c>true</c> nếu cập nhật thành công; <c>false</c> nếu không tìm thấy bản ghi cần cập nhật.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> là <c>null</c>.</exception>
    /// <exception cref="ValidationException">Mã không hợp lệ hoặc dữ liệu không hợp lệ.</exception>
    public async Task<bool> UpdateAsync(Category data)
    {
        ArgumentNullException.ThrowIfNull(data);

        ValidateId(data.CategoryId);
        Normalize(data);
        await ValidateDataAsync(data);

        return await _repository.UpdateAsync(data);
    }

    /// <summary>
    /// Xóa một danh mục sách theo mã. Chỉ được xóa khi bản ghi chưa được sử dụng ở nơi khác (gắn với đầu sách nào hoặc có danh mục con).
    /// </summary>
    /// <param name="id">Mã cần xóa (phải lớn hơn 0).</param>
    /// <returns>
    /// <c>true</c> nếu xóa thành công; <c>false</c> nếu không tìm thấy bản ghi cần xóa.
    /// </returns>
    /// <exception cref="ValidationException">Mã không hợp lệ hoặc bản ghi đang được sử dụng.</exception>
    public async Task<bool> DeleteAsync(int id)
    {
        ValidateId(id);

        if (await _repository.InUsedAsync(id))
            throw new ValidationException("Không thể xóa danh mục này vì đang có sách thuộc danh mục hoặc có danh mục con.");

        return await _repository.DeleteAsync(id);
    }

    /// <summary>
    /// Kiểm tra xem một danh mục sách có đang được sử dụng (gắn với đầu sách nào hoặc có danh mục con) hay không.
    /// </summary>
    /// <param name="id">Mã cần kiểm tra (phải lớn hơn 0).</param>
    /// <returns><c>true</c> nếu đang được sử dụng (không thể xóa); ngược lại là <c>false</c>.</returns>
    /// <exception cref="ValidationException"><paramref name="id"/> không hợp lệ.</exception>
    public async Task<bool> InUsedAsync(int id)
    {
        ValidateId(id);
        return await _repository.InUsedAsync(id);
    }

    // ---------------------------------------------------------------------
    // Các hàm hỗ trợ kiểm tra và chuẩn hóa dữ liệu
    // ---------------------------------------------------------------------

    /// <summary>Kiểm tra mã danh mục sách hợp lệ (lớn hơn 0).</summary>
    /// <exception cref="ValidationException">Mã không hợp lệ.</exception>
    private static void ValidateId(int id)
    {
        if (id <= 0)
            throw new ValidationException("Mã danh mục sách không hợp lệ.");
    }

    /// <summary>
    /// Chuẩn hóa dữ liệu trước khi kiểm tra và lưu: cắt khoảng trắng thừa,
    /// mô tả rỗng được đổi thành <c>null</c>.
    /// </summary>
    private static void Normalize(Category data)
    {
        data.CategoryName = (data.CategoryName ?? string.Empty).Trim();
        data.Description = NullIfEmpty(data.Description);
    }

    /// <summary>Cắt khoảng trắng thừa; chuỗi rỗng hoặc chỉ gồm khoảng trắng được đổi thành <c>null</c>.</summary>
    private static string? NullIfEmpty(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    /// <summary>
    /// Kiểm tra tính hợp lệ của dữ liệu danh mục (đã được chuẩn hóa): độ dài các trường, danh mục cha phải tồn tại
    /// và không được tạo thành vòng lặp trong cây danh mục.
    /// </summary>
    /// <exception cref="ValidationException">Dữ liệu không hợp lệ.</exception>
    private async Task ValidateDataAsync(Category data)
    {
        if (data.CategoryName.Length == 0)
            throw new ValidationException("Tên danh mục không được để trống.");

        if (data.CategoryName.Length > CategoryNameMaxLength)
            throw new ValidationException($"Tên danh mục không được vượt quá {CategoryNameMaxLength} ký tự.");

        if (data.Description != null && data.Description.Length > DescriptionMaxLength)
            throw new ValidationException($"Mô tả không được vượt quá {DescriptionMaxLength} ký tự.");

        if (!data.ParentCategoryId.HasValue)
            return;

        int parentId = data.ParentCategoryId.Value;

        if (parentId <= 0)
            throw new ValidationException("Mã danh mục cha không hợp lệ.");

        if (parentId == data.CategoryId)
            throw new ValidationException("Danh mục cha không được là chính danh mục này.");

        var all = (await _repository.ListAsync()).ToDictionary(c => c.CategoryId);

        if (!all.ContainsKey(parentId))
            throw new ValidationException("Danh mục cha không tồn tại.");

        // Chống vòng lặp: đi ngược lên các danh mục tổ tiên của danh mục cha được chọn;
        // nếu gặp lại chính danh mục đang sửa thì danh mục cha thực chất là danh mục con của nó.
        if (data.CategoryId > 0)
        {
            var visited = new HashSet<int>();
            int? current = parentId;
            while (current.HasValue && visited.Add(current.Value))
            {
                if (current.Value == data.CategoryId)
                    throw new ValidationException("Danh mục cha không được là danh mục con (trực tiếp hoặc gián tiếp) của chính danh mục này.");

                current = all.TryGetValue(current.Value, out var node) ? node.ParentCategoryId : null;
            }
        }
    }
}

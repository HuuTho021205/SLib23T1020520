using System.ComponentModel.DataAnnotations;
using SLib23T1020520.DataLayer.Interfaces;
using SLib23T1020520.Models.Entities;

namespace SLib23T1020520.BusinessLayer;

/// <summary>
/// Cung cấp các chức năng nghiệp vụ liên quan đến tác giả (<see cref="Author"/>):
/// kiểm tra tính hợp lệ của dữ liệu và điều phối các thao tác xử lý dữ liệu với
/// <see cref="IAuthorRepository"/>.
/// <para>
/// Quy ước xử lý lỗi của lớp này:
/// </para>
/// <list type="bullet">
/// <item><description>
/// Dữ liệu không hợp lệ hoặc vi phạm quy tắc nghiệp vụ (mã không hợp lệ, tên để trống,
/// bản ghi đang được sử dụng...) thì ném <see cref="ValidationException"/>
/// với thông điệp tiếng Việt có thể hiển thị trực tiếp cho người dùng.
/// </description></item>
/// <item><description>
/// Khi không tìm thấy bản ghi cần cập nhật hoặc cần xóa, <see cref="UpdateAsync(Author)"/> và
/// <see cref="DeleteAsync(int)"/> không ném exception mà trả về <c>false</c>.
/// </description></item>
/// <item><description>
/// Tham số bắt buộc là <c>null</c> (lỗi lập trình) thì ném <see cref="ArgumentNullException"/>.
/// </description></item>
/// <item><description>
/// Các lỗi hệ thống phát sinh từ tầng dữ liệu (ví dụ mất kết nối CSDL) không bị che giấu,
/// được để nguyên cho nơi gọi xử lý.
/// </description></item>
/// </list>
/// </summary>
public class AuthorService
{
    /// <summary>Độ dài tối đa của tên tác giả, theo cột <c>Authors.AuthorName</c> (<c>nvarchar(200)</c>).</summary>
    private const int AuthorNameMaxLength = 200;

    private readonly IAuthorRepository _repository;

    /// <summary>
    /// Khởi tạo dịch vụ nghiệp vụ tác giả.
    /// </summary>
    /// <param name="repository">Đối tượng thực hiện các phép xử lý dữ liệu trên bảng tác giả.</param>
    /// <exception cref="ArgumentNullException"><paramref name="repository"/> là <c>null</c>.</exception>
    public AuthorService(IAuthorRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    /// <summary>
    /// Lấy danh sách toàn bộ tác giả.
    /// </summary>
    /// <returns>Tập hợp các tác giả; tập hợp rỗng nếu chưa có dữ liệu.</returns>
    public async Task<IEnumerable<Author>> ListAsync()
    {
        return await _repository.ListAsync();
    }

    /// <summary>
    /// Lấy thông tin của một tác giả theo mã.
    /// </summary>
    /// <param name="id">Mã tác giả (phải lớn hơn 0).</param>
    /// <returns>Tác giả tìm được; <c>null</c> nếu không tồn tại tác giả có mã tương ứng.</returns>
    /// <exception cref="ValidationException"><paramref name="id"/> không hợp lệ.</exception>
    public async Task<Author?> GetAsync(int id)
    {
        ValidateId(id);
        return await _repository.GetAsync(id);
    }

    /// <summary>
    /// Bổ sung một tác giả mới.
    /// </summary>
    /// <param name="data">
    /// Thông tin tác giả cần bổ sung. Các chuỗi sẽ được cắt khoảng trắng thừa, ngày sinh/ngày mất
    /// được chuẩn hóa về phần ngày (đối tượng <paramref name="data"/> bị thay đổi trực tiếp).
    /// Tác giả mới luôn được tạo ở trạng thái đang sử dụng (<c>IsActive = true</c>).
    /// </param>
    /// <returns>Mã của tác giả vừa được bổ sung.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> là <c>null</c>.</exception>
    /// <exception cref="ValidationException">Dữ liệu của tác giả không hợp lệ.</exception>
    public async Task<int> AddAsync(Author data)
    {
        ArgumentNullException.ThrowIfNull(data);

        Normalize(data);
        ValidateData(data);
        data.IsActive = true;

        return await _repository.AddAsync(data);
    }

    /// <summary>
    /// Cập nhật thông tin của một tác giả đã có.
    /// </summary>
    /// <param name="data">
    /// Thông tin mới của tác giả; tác giả cần cập nhật được xác định bằng <c>AuthorId</c>.
    /// Các chuỗi sẽ được cắt khoảng trắng thừa, ngày sinh/ngày mất được chuẩn hóa về phần ngày
    /// (đối tượng <paramref name="data"/> bị thay đổi trực tiếp).
    /// </param>
    /// <returns>
    /// <c>true</c> nếu cập nhật thành công; <c>false</c> nếu không tìm thấy tác giả cần cập nhật.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> là <c>null</c>.</exception>
    /// <exception cref="ValidationException">Mã tác giả không hợp lệ hoặc dữ liệu không hợp lệ.</exception>
    public async Task<bool> UpdateAsync(Author data)
    {
        ArgumentNullException.ThrowIfNull(data);

        ValidateId(data.AuthorId);
        Normalize(data);
        ValidateData(data);

        return await _repository.UpdateAsync(data);
    }

    /// <summary>
    /// Xóa một tác giả theo mã. Chỉ được xóa khi tác giả chưa được sử dụng ở nơi khác
    /// (chưa gắn với đầu sách nào).
    /// </summary>
    /// <param name="id">Mã tác giả cần xóa (phải lớn hơn 0).</param>
    /// <returns>
    /// <c>true</c> nếu xóa thành công; <c>false</c> nếu không tìm thấy tác giả cần xóa.
    /// </returns>
    /// <exception cref="ValidationException">Mã không hợp lệ hoặc tác giả đang được sử dụng.</exception>
    public async Task<bool> DeleteAsync(int id)
    {
        ValidateId(id);

        if (await _repository.InUsedAsync(id))
            throw new ValidationException("Không thể xóa tác giả này vì đang có sách.");

        return await _repository.DeleteAsync(id);
    }

    /// <summary>
    /// Kiểm tra xem một tác giả có đang được sử dụng (gắn với đầu sách) hay không.
    /// </summary>
    /// <param name="id">Mã tác giả cần kiểm tra (phải lớn hơn 0).</param>
    /// <returns><c>true</c> nếu tác giả đang được sử dụng (không thể xóa); ngược lại là <c>false</c>.</returns>
    /// <exception cref="ValidationException"><paramref name="id"/> không hợp lệ.</exception>
    public async Task<bool> InUsedAsync(int id)
    {
        ValidateId(id);
        return await _repository.InUsedAsync(id);
    }

    // ---------------------------------------------------------------------
    // Các hàm hỗ trợ kiểm tra và chuẩn hóa dữ liệu
    // ---------------------------------------------------------------------

    /// <summary>Kiểm tra mã tác giả hợp lệ (lớn hơn 0).</summary>
    /// <exception cref="ValidationException">Mã không hợp lệ.</exception>
    private static void ValidateId(int id)
    {
        if (id <= 0)
            throw new ValidationException("Mã tác giả không hợp lệ.");
    }

    /// <summary>
    /// Chuẩn hóa dữ liệu trước khi kiểm tra và lưu: cắt khoảng trắng thừa của chuỗi,
    /// chuỗi rỗng ở các trường tùy chọn được đổi thành <c>null</c>, ngày chỉ giữ phần ngày.
    /// </summary>
    private static void Normalize(Author data)
    {
        data.AuthorName = (data.AuthorName ?? string.Empty).Trim();
        data.Biography = string.IsNullOrWhiteSpace(data.Biography) ? null : data.Biography.Trim();
        data.BirthDate = data.BirthDate?.Date;
        data.DeathDate = data.DeathDate?.Date;
    }

    /// <summary>
    /// Kiểm tra tính hợp lệ của dữ liệu tác giả (đã được chuẩn hóa), phù hợp với các ràng buộc của CSDL.
    /// </summary>
    /// <exception cref="ValidationException">Dữ liệu không hợp lệ.</exception>
    private static void ValidateData(Author data)
    {
        if (data.AuthorName.Length == 0)
            throw new ValidationException("Tên tác giả không được để trống.");

        if (data.AuthorName.Length > AuthorNameMaxLength)
            throw new ValidationException($"Tên tác giả không được vượt quá {AuthorNameMaxLength} ký tự.");

        DateTime today = DateTime.Today;

        if (data.BirthDate.HasValue && data.BirthDate.Value > today)
            throw new ValidationException("Ngày sinh không được lớn hơn ngày hiện tại.");

        if (data.DeathDate.HasValue && data.DeathDate.Value > today)
            throw new ValidationException("Ngày mất không được lớn hơn ngày hiện tại.");

        if (data.BirthDate.HasValue && data.DeathDate.HasValue && data.DeathDate.Value < data.BirthDate.Value)
            throw new ValidationException("Ngày mất không được sớm hơn ngày sinh.");
    }
}
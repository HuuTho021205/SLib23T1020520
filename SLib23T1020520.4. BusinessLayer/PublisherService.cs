using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using SLib23T1020520.DataLayer.Interfaces;
using SLib23T1020520.Models.Entities;

namespace SLib23T1020520.BusinessLayer;

/// <summary>
/// Cung cấp các chức năng nghiệp vụ liên quan đến nhà xuất bản (<see cref="Publisher"/>):
/// kiểm tra tính hợp lệ của dữ liệu và điều phối các thao tác xử lý dữ liệu với
/// <see cref="IPublisherRepository"/>.
/// <para>
/// Quy ước xử lý lỗi: dữ liệu không hợp lệ hoặc vi phạm quy tắc nghiệp vụ thì ném
/// <see cref="ValidationException"/> với thông điệp tiếng Việt có thể hiển thị trực tiếp cho người dùng;
/// tham số bắt buộc là <c>null</c> thì ném <see cref="ArgumentNullException"/>; các lỗi hệ thống từ tầng dữ liệu
/// được để nguyên. Khi không tìm thấy bản ghi cần cập nhật hoặc cần xóa, <see cref="UpdateAsync(Publisher)"/>
/// và <see cref="DeleteAsync(int)"/> không ném exception mà trả về <c>false</c>.
/// </para>
/// </summary>
public class PublisherService
{
    // Độ dài tối đa của các cột trong bảng Publishers
    private const int PublisherNameMaxLength = 200;
    private const int AddressMaxLength = 300;
    private const int PhoneMaxLength = 20;
    private const int EmailMaxLength = 254;
    private const int WebsiteMaxLength = 300;

    private readonly IPublisherRepository _repository;

    /// <summary>
    /// Khởi tạo dịch vụ nghiệp vụ nhà xuất bản.
    /// </summary>
    /// <param name="repository">Đối tượng thực hiện các phép xử lý dữ liệu trên bảng nhà xuất bản.</param>
    /// <exception cref="ArgumentNullException"><paramref name="repository"/> là <c>null</c>.</exception>
    public PublisherService(IPublisherRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    /// <summary>
    /// Lấy danh sách toàn bộ nhà xuất bản.
    /// </summary>
    /// <returns>Tập hợp các nhà xuất bản; tập hợp rỗng nếu chưa có dữ liệu.</returns>
    public async Task<IEnumerable<Publisher>> ListAsync()
    {
        return await _repository.ListAsync();
    }

    /// <summary>
    /// Lấy thông tin của một nhà xuất bản theo mã.
    /// </summary>
    /// <param name="id">Mã nhà xuất bản (phải lớn hơn 0).</param>
    /// <returns>Bản ghi tìm được; <c>null</c> nếu không tồn tại bản ghi có mã tương ứng.</returns>
    /// <exception cref="ValidationException"><paramref name="id"/> không hợp lệ.</exception>
    public async Task<Publisher?> GetAsync(int id)
    {
        ValidateId(id);
        return await _repository.GetAsync(id);
    }

    /// <summary>
    /// Bổ sung một nhà xuất bản mới.
    /// </summary>
    /// <param name="data">
    /// Thông tin cần bổ sung. Dữ liệu được chuẩn hóa trước khi lưu (đối tượng <paramref name="data"/> bị thay đổi
    /// trực tiếp). Bản ghi mới luôn được tạo ở trạng thái đang sử dụng (<c>IsActive = true</c>).
    /// </param>
    /// <returns>Mã của bản ghi vừa được bổ sung.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> là <c>null</c>.</exception>
    /// <exception cref="ValidationException">Dữ liệu không hợp lệ.</exception>
    public async Task<int> AddAsync(Publisher data)
    {
        ArgumentNullException.ThrowIfNull(data);

        Normalize(data);
        ValidateData(data);
        data.IsActive = true;

        return await _repository.AddAsync(data);
    }

    /// <summary>
    /// Cập nhật thông tin của một nhà xuất bản đã có.
    /// </summary>
    /// <param name="data">
    /// Thông tin mới; bản ghi cần cập nhật được xác định bằng <c>PublisherId</c>.
    /// Dữ liệu được chuẩn hóa trước khi lưu (đối tượng <paramref name="data"/> bị thay đổi trực tiếp).
    /// </param>
    /// <returns>
    /// <c>true</c> nếu cập nhật thành công; <c>false</c> nếu không tìm thấy bản ghi cần cập nhật.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> là <c>null</c>.</exception>
    /// <exception cref="ValidationException">Mã không hợp lệ hoặc dữ liệu không hợp lệ.</exception>
    public async Task<bool> UpdateAsync(Publisher data)
    {
        ArgumentNullException.ThrowIfNull(data);

        ValidateId(data.PublisherId);
        Normalize(data);
        ValidateData(data);

        return await _repository.UpdateAsync(data);
    }

    /// <summary>
    /// Xóa một nhà xuất bản theo mã. Chỉ được xóa khi bản ghi chưa được sử dụng ở nơi khác (gắn với đầu sách nào).
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
            throw new ValidationException("Không thể xóa nhà xuất bản này vì đang có sách.");

        return await _repository.DeleteAsync(id);
    }

    /// <summary>
    /// Kiểm tra xem một nhà xuất bản có đang được sử dụng (gắn với đầu sách nào) hay không.
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

    /// <summary>Kiểm tra mã nhà xuất bản hợp lệ (lớn hơn 0).</summary>
    /// <exception cref="ValidationException">Mã không hợp lệ.</exception>
    private static void ValidateId(int id)
    {
        if (id <= 0)
            throw new ValidationException("Mã nhà xuất bản không hợp lệ.");
    }

    /// <summary>
    /// Chuẩn hóa dữ liệu trước khi kiểm tra và lưu: cắt khoảng trắng thừa,
    /// chuỗi rỗng ở các trường tùy chọn được đổi thành <c>null</c>.
    /// </summary>
    private static void Normalize(Publisher data)
    {
        data.PublisherName = (data.PublisherName ?? string.Empty).Trim();
        data.Address = NullIfEmpty(data.Address);
        data.Phone = NullIfEmpty(data.Phone);
        data.Email = NullIfEmpty(data.Email);
        data.Website = NullIfEmpty(data.Website);
    }

    /// <summary>Cắt khoảng trắng thừa; chuỗi rỗng hoặc chỉ gồm khoảng trắng được đổi thành <c>null</c>.</summary>
    private static string? NullIfEmpty(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    /// <summary>
    /// Kiểm tra tính hợp lệ của dữ liệu nhà xuất bản (đã được chuẩn hóa), phù hợp với các ràng buộc của CSDL.
    /// </summary>
    /// <exception cref="ValidationException">Dữ liệu không hợp lệ.</exception>
    private static void ValidateData(Publisher data)
    {
        if (data.PublisherName.Length == 0)
            throw new ValidationException("Tên nhà xuất bản không được để trống.");

        if (data.PublisherName.Length > PublisherNameMaxLength)
            throw new ValidationException($"Tên nhà xuất bản không được vượt quá {PublisherNameMaxLength} ký tự.");

        if (data.Address != null && data.Address.Length > AddressMaxLength)
            throw new ValidationException($"Địa chỉ không được vượt quá {AddressMaxLength} ký tự.");

        if (data.Phone != null)
        {
            if (data.Phone.Length > PhoneMaxLength)
                throw new ValidationException($"Số điện thoại không được vượt quá {PhoneMaxLength} ký tự.");

            if (!Regex.IsMatch(data.Phone, @"^\+?[0-9][0-9 ().-]*$"))
                throw new ValidationException("Số điện thoại không hợp lệ.");
        }

        if (data.Email != null)
        {
            if (data.Email.Length > EmailMaxLength)
                throw new ValidationException($"Email không được vượt quá {EmailMaxLength} ký tự.");

            if (!new EmailAddressAttribute().IsValid(data.Email))
                throw new ValidationException("Email không hợp lệ.");
        }

        if (data.Website != null)
        {
            if (data.Website.Length > WebsiteMaxLength)
                throw new ValidationException($"Website không được vượt quá {WebsiteMaxLength} ký tự.");

            if (!Uri.TryCreate(data.Website, UriKind.Absolute, out Uri? uri)
                || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                throw new ValidationException("Website không hợp lệ (phải bắt đầu bằng http:// hoặc https://).");
        }
    }
}

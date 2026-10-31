using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using SLib23T1020520.DataLayer.Interfaces;
using SLib23T1020520.Models.Entities;

namespace SLib23T1020520.BusinessLayer;

/// <summary>
/// Cung cấp các chức năng nghiệp vụ liên quan đến vị trí lưu trữ sách (<see cref="Location"/>):
/// kiểm tra tính hợp lệ của dữ liệu và điều phối các thao tác xử lý dữ liệu với
/// <see cref="ILocationRepository"/>.
/// <para>
/// Quy ước xử lý lỗi: dữ liệu không hợp lệ hoặc vi phạm quy tắc nghiệp vụ thì ném
/// <see cref="ValidationException"/> với thông điệp tiếng Việt có thể hiển thị trực tiếp cho người dùng;
/// tham số bắt buộc là <c>null</c> thì ném <see cref="ArgumentNullException"/>; các lỗi hệ thống từ tầng dữ liệu
/// được để nguyên. Khi không tìm thấy bản ghi cần cập nhật hoặc cần xóa, <see cref="UpdateAsync(Location)"/>
/// và <see cref="DeleteAsync(int)"/> không ném exception mà trả về <c>false</c>.
/// </para>
/// </summary>
public class LocationService
{
    // Độ dài tối đa của các cột trong bảng Locations
    private const int LocationCodeMaxLength = 30;
    private const int LocationNameMaxLength = 150;
    private const int FloorMaxLength = 30;
    private const int RoomMaxLength = 50;
    private const int ShelfMaxLength = 50;
    private const int DescriptionMaxLength = 300;

    private readonly ILocationRepository _repository;

    /// <summary>
    /// Khởi tạo dịch vụ nghiệp vụ vị trí lưu trữ sách.
    /// </summary>
    /// <param name="repository">Đối tượng thực hiện các phép xử lý dữ liệu trên bảng vị trí lưu trữ sách.</param>
    /// <exception cref="ArgumentNullException"><paramref name="repository"/> là <c>null</c>.</exception>
    public LocationService(ILocationRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    /// <summary>
    /// Lấy danh sách toàn bộ vị trí lưu trữ sách.
    /// </summary>
    /// <returns>Tập hợp các vị trí lưu trữ sách; tập hợp rỗng nếu chưa có dữ liệu.</returns>
    public async Task<IEnumerable<Location>> ListAsync()
    {
        return await _repository.ListAsync();
    }

    /// <summary>
    /// Lấy thông tin của một vị trí lưu trữ sách theo mã.
    /// </summary>
    /// <param name="id">Mã vị trí lưu trữ sách (phải lớn hơn 0).</param>
    /// <returns>Bản ghi tìm được; <c>null</c> nếu không tồn tại bản ghi có mã tương ứng.</returns>
    /// <exception cref="ValidationException"><paramref name="id"/> không hợp lệ.</exception>
    public async Task<Location?> GetAsync(int id)
    {
        ValidateId(id);
        return await _repository.GetAsync(id);
    }

    /// <summary>
    /// Bổ sung một vị trí lưu trữ sách mới.
    /// </summary>
    /// <param name="data">
    /// Thông tin cần bổ sung. Dữ liệu được chuẩn hóa trước khi lưu (đối tượng <paramref name="data"/> bị thay đổi
    /// trực tiếp). Bản ghi mới luôn được tạo ở trạng thái đang sử dụng (<c>IsActive = true</c>).
    /// </param>
    /// <returns>Mã của bản ghi vừa được bổ sung.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> là <c>null</c>.</exception>
    /// <exception cref="ValidationException">Dữ liệu không hợp lệ.</exception>
    public async Task<int> AddAsync(Location data)
    {
        ArgumentNullException.ThrowIfNull(data);

        Normalize(data);
        await ValidateDataAsync(data);
        data.IsActive = true;

        return await _repository.AddAsync(data);
    }

    /// <summary>
    /// Cập nhật thông tin của một vị trí lưu trữ sách đã có.
    /// </summary>
    /// <param name="data">
    /// Thông tin mới; bản ghi cần cập nhật được xác định bằng <c>LocationId</c>.
    /// Dữ liệu được chuẩn hóa trước khi lưu (đối tượng <paramref name="data"/> bị thay đổi trực tiếp).
    /// </param>
    /// <returns>
    /// <c>true</c> nếu cập nhật thành công; <c>false</c> nếu không tìm thấy bản ghi cần cập nhật.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> là <c>null</c>.</exception>
    /// <exception cref="ValidationException">Mã không hợp lệ hoặc dữ liệu không hợp lệ.</exception>
    public async Task<bool> UpdateAsync(Location data)
    {
        ArgumentNullException.ThrowIfNull(data);

        ValidateId(data.LocationId);
        Normalize(data);
        await ValidateDataAsync(data);

        return await _repository.UpdateAsync(data);
    }

    /// <summary>
    /// Xóa một vị trí lưu trữ sách theo mã. Chỉ được xóa khi bản ghi chưa được sử dụng ở nơi khác (có bản sao sách nào đang xếp ở vị trí này).
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
            throw new ValidationException("Không thể xóa vị trí này vì đang có bản sao sách được xếp ở đây.");

        return await _repository.DeleteAsync(id);
    }

    /// <summary>
    /// Kiểm tra xem một vị trí lưu trữ sách có đang được sử dụng (có bản sao sách nào đang xếp ở vị trí này) hay không.
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

    /// <summary>Kiểm tra mã vị trí lưu trữ sách hợp lệ (lớn hơn 0).</summary>
    /// <exception cref="ValidationException">Mã không hợp lệ.</exception>
    private static void ValidateId(int id)
    {
        if (id <= 0)
            throw new ValidationException("Mã vị trí lưu trữ sách không hợp lệ.");
    }

    /// <summary>
    /// Chuẩn hóa dữ liệu trước khi kiểm tra và lưu: cắt khoảng trắng thừa,
    /// chuỗi rỗng ở các trường tùy chọn được đổi thành <c>null</c>;
    /// mã vị trí được viết hoa toàn bộ (ví dụ "loc-f1-r1-s1" thành "LOC-F1-R1-S1").
    /// </summary>
    private static void Normalize(Location data)
    {
        data.LocationCode = (data.LocationCode ?? string.Empty).Trim().ToUpperInvariant();
        data.LocationName = (data.LocationName ?? string.Empty).Trim();
        data.Floor = NullIfEmpty(data.Floor);
        data.Room = NullIfEmpty(data.Room);
        data.Shelf = NullIfEmpty(data.Shelf);
        data.Description = NullIfEmpty(data.Description);
    }

    /// <summary>Cắt khoảng trắng thừa; chuỗi rỗng hoặc chỉ gồm khoảng trắng được đổi thành <c>null</c>.</summary>
    private static string? NullIfEmpty(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    /// <summary>
    /// Kiểm tra tính hợp lệ của dữ liệu vị trí (đã được chuẩn hóa), phù hợp với các ràng buộc của CSDL.
    /// Việc kiểm tra trùng mã chỉ để báo lỗi sớm bằng thông điệp thân thiện; ràng buộc UNIQUE
    /// trong CSDL vẫn là chốt chặn cuối cùng.
    /// </summary>
    /// <exception cref="ValidationException">Dữ liệu không hợp lệ.</exception>
    private async Task ValidateDataAsync(Location data)
    {
        if (data.LocationCode.Length == 0)
            throw new ValidationException("Mã vị trí không được để trống.");

        if (data.LocationCode.Length > LocationCodeMaxLength)
            throw new ValidationException($"Mã vị trí không được vượt quá {LocationCodeMaxLength} ký tự.");

        // Cột LocationCode kiểu varchar (không lưu được chữ có dấu) nên chỉ cho phép ký tự ASCII.
        if (!Regex.IsMatch(data.LocationCode, "^[A-Z0-9_.-]+$"))
            throw new ValidationException("Mã vị trí chỉ được gồm chữ cái không dấu, chữ số và các ký tự - _ .");

        if (data.LocationName.Length == 0)
            throw new ValidationException("Tên vị trí không được để trống.");

        if (data.LocationName.Length > LocationNameMaxLength)
            throw new ValidationException($"Tên vị trí không được vượt quá {LocationNameMaxLength} ký tự.");

        if (data.Floor != null && data.Floor.Length > FloorMaxLength)
            throw new ValidationException($"Tầng không được vượt quá {FloorMaxLength} ký tự.");

        if (data.Room != null && data.Room.Length > RoomMaxLength)
            throw new ValidationException($"Phòng không được vượt quá {RoomMaxLength} ký tự.");

        if (data.Shelf != null && data.Shelf.Length > ShelfMaxLength)
            throw new ValidationException($"Kệ không được vượt quá {ShelfMaxLength} ký tự.");

        if (data.Description != null && data.Description.Length > DescriptionMaxLength)
            throw new ValidationException($"Mô tả không được vượt quá {DescriptionMaxLength} ký tự.");

        var all = await _repository.ListAsync();
        bool duplicated = all.Any(x => x.LocationId != data.LocationId
                                       && string.Equals(x.LocationCode, data.LocationCode, StringComparison.OrdinalIgnoreCase));
        if (duplicated)
            throw new ValidationException("Mã vị trí đã tồn tại.");
    }
}

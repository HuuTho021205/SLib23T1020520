using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SLib23T1020520.DataLayer.SQLServer;

/// <summary>
/// Lớp hỗ trợ truy cập cơ sở dữ liệu SQL Server: đọc chuỗi kết nối từ cấu hình
/// và cung cấp các kết nối mới cho các lớp cài đặt Repository.
/// </summary>
public class DbHelper
{
    private readonly string _connectionString;

    /// <summary>
    /// Khởi tạo <see cref="DbHelper"/> bằng cách đọc chuỗi kết nối có tên
    /// <c>DefaultConnection</c> trong phần <c>ConnectionStrings</c> của cấu hình ứng dụng
    /// (ví dụ: <c>appsettings.json</c>).
    /// </summary>
    /// <param name="configuration">Cấu hình của ứng dụng, được cung cấp bởi cơ chế Dependency Injection.</param>
    public DbHelper(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    /// <summary>
    /// Tạo một kết nối mới đến cơ sở dữ liệu. Kết nối chưa được mở; nơi gọi chịu trách nhiệm
    /// giải phóng kết nối sau khi dùng xong (ví dụ bằng câu lệnh <c>using</c>).
    /// </summary>
    /// <returns>Đối tượng <see cref="SqlConnection"/> mới.</returns>
    public SqlConnection GetConnection() => new SqlConnection(_connectionString);
}
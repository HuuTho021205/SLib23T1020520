using Dapper;
using SLib23T1020520.DataLayer.Interfaces;
using SLib23T1020520.Models.Entities;

namespace SLib23T1020520.DataLayer.SQLServer;

/// <summary>
/// Cài đặt các phép xử lý dữ liệu trên bảng <c>Locations</c> (vị trí lưu trữ sách) bằng Dapper và SQL Server.
/// </summary>
public class LocationRepository : ILocationRepository
{
    private readonly DbHelper _db;

    /// <summary>
    /// Khởi tạo repository.
    /// </summary>
    /// <param name="db">Đối tượng cung cấp kết nối đến cơ sở dữ liệu SQL Server.</param>
    public LocationRepository(DbHelper db)
    {
        _db = db;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Location>> ListAsync()
    {
        const string sql = @"SELECT * FROM dbo.Locations ORDER BY LocationName";

        using var connection = _db.GetConnection();
        return await connection.QueryAsync<Location>(sql);
    }

    /// <inheritdoc/>
    public async Task<Location?> GetAsync(int id)
    {
        const string sql = @"SELECT * FROM dbo.Locations WHERE LocationId = @id";

        using var connection = _db.GetConnection();
        return await connection.QueryFirstOrDefaultAsync<Location>(sql, new { id });
    }

    /// <inheritdoc/>
    public async Task<int> AddAsync(Location data)
    {
        const string sql = @"
            INSERT INTO dbo.Locations (LocationCode, LocationName, [Floor], Room, Shelf, Description, IsActive)
            VALUES (@LocationCode, @LocationName, @Floor, @Room, @Shelf, @Description, @IsActive);

            SELECT CAST(SCOPE_IDENTITY() AS int);";

        using var connection = _db.GetConnection();
        return await connection.ExecuteScalarAsync<int>(sql, data);
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateAsync(Location data)
    {
        const string sql = @"
            UPDATE dbo.Locations
            SET LocationCode = @LocationCode,
                LocationName = @LocationName,
                [Floor] = @Floor,
                Room = @Room,
                Shelf = @Shelf,
                Description = @Description,
                IsActive = @IsActive
            WHERE LocationId = @LocationId;";

        using var connection = _db.GetConnection();
        return await connection.ExecuteAsync(sql, data) > 0;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = @"DELETE FROM dbo.Locations WHERE LocationId = @id";

        using var connection = _db.GetConnection();
        return await connection.ExecuteAsync(sql, new { id }) > 0;
    }

    /// <inheritdoc/>
    public async Task<bool> InUsedAsync(int id)
    {
        const string sql = @"
            SELECT CASE
                       WHEN EXISTS (SELECT 1 FROM dbo.BookCopies WHERE LocationId = @id)
                       THEN 1 ELSE 0
                   END;";

        using var connection = _db.GetConnection();
        return await connection.ExecuteScalarAsync<bool>(sql, new { id });
    }
}

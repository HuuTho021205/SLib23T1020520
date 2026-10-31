using Dapper;
using SLib23T1020520.DataLayer.Interfaces;
using SLib23T1020520.Models.Entities;

namespace SLib23T1020520.DataLayer.SQLServer;

/// <summary>
/// Cài đặt các phép xử lý dữ liệu trên bảng <c>MemberTypes</c> (loại thành viên) bằng Dapper và SQL Server.
/// </summary>
public class MemberTypeRepository : IMemberTypeRepository
{
    private readonly DbHelper _db;

    /// <summary>
    /// Khởi tạo repository.
    /// </summary>
    /// <param name="db">Đối tượng cung cấp kết nối đến cơ sở dữ liệu SQL Server.</param>
    public MemberTypeRepository(DbHelper db)
    {
        _db = db;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<MemberType>> ListAsync()
    {
        const string sql = @"SELECT * FROM dbo.MemberTypes ORDER BY TypeName";

        using var connection = _db.GetConnection();
        return await connection.QueryAsync<MemberType>(sql);
    }

    /// <inheritdoc/>
    public async Task<MemberType?> GetAsync(int id)
    {
        const string sql = @"SELECT * FROM dbo.MemberTypes WHERE MemberTypeId = @id";

        using var connection = _db.GetConnection();
        return await connection.QueryFirstOrDefaultAsync<MemberType>(sql, new { id });
    }

    /// <inheritdoc/>
    public async Task<int> AddAsync(MemberType data)
    {
        const string sql = @"
            INSERT INTO dbo.MemberTypes (TypeName, MaxActiveLoans, LoanPeriodDays, MaxRenewals, IsActive)
            VALUES (@TypeName, @MaxActiveLoans, @LoanPeriodDays, @MaxRenewals, @IsActive);

            SELECT CAST(SCOPE_IDENTITY() AS int);";

        using var connection = _db.GetConnection();
        return await connection.ExecuteScalarAsync<int>(sql, data);
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateAsync(MemberType data)
    {
        const string sql = @"
            UPDATE dbo.MemberTypes
            SET TypeName = @TypeName,
                MaxActiveLoans = @MaxActiveLoans,
                LoanPeriodDays = @LoanPeriodDays,
                MaxRenewals = @MaxRenewals,
                IsActive = @IsActive
            WHERE MemberTypeId = @MemberTypeId;";

        using var connection = _db.GetConnection();
        return await connection.ExecuteAsync(sql, data) > 0;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = @"DELETE FROM dbo.MemberTypes WHERE MemberTypeId = @id";

        using var connection = _db.GetConnection();
        return await connection.ExecuteAsync(sql, new { id }) > 0;
    }

    /// <inheritdoc/>
    public async Task<bool> InUsedAsync(int id)
    {
        const string sql = @"
            SELECT CASE
                       WHEN EXISTS (SELECT 1 FROM dbo.Members WHERE MemberTypeId = @id)
                       THEN 1 ELSE 0
                   END;";

        using var connection = _db.GetConnection();
        return await connection.ExecuteScalarAsync<bool>(sql, new { id });
    }
}

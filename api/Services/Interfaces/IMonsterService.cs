public interface IMonsterService
{
    Task<IEnumerable<MonsterSummaryDto>> GetAllAsync(decimal? maxCr, int? creatureTypeId, int? requestingUserId = null, CancellationToken ct = default);
    Task<MonsterDetailDto> GetByIdAsync(int id, int? requestingUserId = null, CancellationToken ct = default);
    Task<MonsterDetailDto> CreateAsync(CreateMonsterRequest request, int userId, CancellationToken ct = default);
    Task<MonsterDetailDto> UpdateAsync(int id, CreateMonsterRequest request, int userId, CancellationToken ct = default);
    Task DeleteAsync(int id, int userId, CancellationToken ct = default);
}

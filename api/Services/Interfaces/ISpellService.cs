public interface ISpellService
{
    Task<IEnumerable<SpellSummaryDto>> GetAllAsync(int? level, int? schoolId, int? classId, int? requestingUserId = null, CancellationToken ct = default);
    Task<SpellDetailDto> GetByIdAsync(int id, int? requestingUserId = null, CancellationToken ct = default);
    Task<SpellDetailDto> CreateAsync(CreateSpellRequest request, int userId, CancellationToken ct = default);
    Task<SpellDetailDto> UpdateAsync(int id, CreateSpellRequest request, int userId, CancellationToken ct = default);
    Task DeleteAsync(int id, int userId, CancellationToken ct = default);
}

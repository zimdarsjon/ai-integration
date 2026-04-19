public interface IReferenceService
{
    Task<IEnumerable<AbilityScoreDto>> GetAbilityScoresAsync(int? requestingUserId = null, CancellationToken ct = default);
    Task<AbilityScoreDto> CreateCustomAbilityScoreAsync(CreateAbilityScoreRequest request, int userId, CancellationToken ct = default);
    Task<IEnumerable<SkillDto>> GetSkillsAsync(CancellationToken ct = default);
    Task<IEnumerable<ReferenceItemDto>> GetDamageTypesAsync(CancellationToken ct = default);
    Task<IEnumerable<ReferenceItemDto>> GetConditionsAsync(CancellationToken ct = default);
    Task<IEnumerable<ReferenceItemDto>> GetMagicSchoolsAsync(CancellationToken ct = default);
    Task<IEnumerable<ReferenceItemDto>> GetLanguagesAsync(CancellationToken ct = default);
    Task<IEnumerable<ReferenceItemDto>> GetAlignmentsAsync(CancellationToken ct = default);
    Task<IEnumerable<ReferenceItemDto>> GetCreatureTypesAsync(CancellationToken ct = default);
    Task<IEnumerable<RaceDto>> GetRacesAsync(int? requestingUserId = null, CancellationToken ct = default);
    Task<RaceDto> GetRaceByIdAsync(int id, int? requestingUserId = null, CancellationToken ct = default);
    Task<RaceDto> CreateCustomRaceAsync(CreateRaceRequest request, int userId, CancellationToken ct = default);
    Task<IEnumerable<ClassDto>> GetClassesAsync(int? requestingUserId = null, CancellationToken ct = default);
    Task<ClassDto> GetClassByIdAsync(int id, int? requestingUserId = null, CancellationToken ct = default);
    Task<ClassDto> CreateCustomClassAsync(CreateClassRequest request, int userId, CancellationToken ct = default);
    Task<IEnumerable<ItemSummaryDto>> GetItemsAsync(ItemType? type, int? requestingUserId = null, CancellationToken ct = default);
    Task<ItemDetailDto> GetItemByIdAsync(int id, int? requestingUserId = null, CancellationToken ct = default);
    Task<ItemDetailDto> CreateCustomItemAsync(CreateItemRequest request, int userId, CancellationToken ct = default);
}

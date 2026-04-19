public interface ICharacterService
{
    Task<IEnumerable<CharacterSummaryDto>> GetUserCharactersAsync(int userId, CancellationToken ct = default);
    Task<CharacterDetailDto> GetByIdAsync(int id, int requestingUserId, CancellationToken ct = default);
    Task<CharacterDetailDto> CreateAsync(CreateCharacterRequest request, int userId, CancellationToken ct = default);
    Task<CharacterDetailDto> UpdateAsync(int id, UpdateCharacterRequest request, int userId, CancellationToken ct = default);
    Task DeleteAsync(int id, int userId, CancellationToken ct = default);
    Task<CharacterDetailDto> ApplyDamageAsync(int id, int damage, int requestingUserId, CancellationToken ct = default);
    Task<CharacterDetailDto> HealAsync(int id, int amount, int requestingUserId, CancellationToken ct = default);
    Task UpdateSpellSlotsAsync(int id, UpdateSpellSlotsRequest request, int userId, CancellationToken ct = default);
    Task RecalculateSpellSlotsAsync(int id, int userId, CancellationToken ct = default);
    Task UpdateCurrencyAsync(int id, UpdateCurrencyRequest request, int userId, CancellationToken ct = default);
    Task AddInventoryItemAsync(int id, AddInventoryItemRequest request, int userId, CancellationToken ct = default);
    Task RemoveInventoryItemAsync(int id, int inventoryItemId, int userId, CancellationToken ct = default);
    Task ApplyConditionAsync(int id, ApplyConditionRequest request, int userId, CancellationToken ct = default);
    Task RemoveConditionAsync(int id, int conditionId, int userId, CancellationToken ct = default);
    Task AddSpellAsync(int id, int spellId, int userId, CancellationToken ct = default);
    Task RemoveSpellAsync(int id, int spellId, int userId, CancellationToken ct = default);
    Task PrepareSpellAsync(int id, int spellId, bool isPrepared, int userId, CancellationToken ct = default);
    Task<CharacterDetailDto> LevelUpAsync(int id, LevelUpRequest request, int userId, CancellationToken ct = default);
}

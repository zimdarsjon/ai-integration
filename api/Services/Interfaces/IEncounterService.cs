public interface IEncounterService
{
    Task<IEnumerable<EncounterSummaryDto>> GetCampaignEncountersAsync(int campaignId, int requestingUserId, CancellationToken ct = default);
    Task<EncounterDetailDto> GetByIdAsync(int id, int requestingUserId, CancellationToken ct = default);
    Task<EncounterDetailDto> CreateAsync(int campaignId, CreateEncounterRequest request, int userId, CancellationToken ct = default);
    Task DeleteAsync(int id, int userId, CancellationToken ct = default);
    Task<EncounterDetailDto> AddParticipantAsync(int id, AddParticipantRequest request, int userId, CancellationToken ct = default);
    Task<EncounterDetailDto> RemoveParticipantAsync(int id, int participantId, int userId, CancellationToken ct = default);
    Task<EncounterDetailDto> ApplyDamageAsync(int id, int participantId, ApplyEncounterDamageRequest request, int userId, CancellationToken ct = default);
    Task<EncounterDetailDto> ApplyConditionAsync(int id, int participantId, int conditionId, int userId, CancellationToken ct = default);
    Task<EncounterDetailDto> RemoveConditionAsync(int id, int participantId, int conditionId, int userId, CancellationToken ct = default);
    Task<EncounterDetailDto> AdvanceTurnAsync(int id, int userId, CancellationToken ct = default);
    Task<EncounterDetailDto> NextRoundAsync(int id, int userId, CancellationToken ct = default);
}

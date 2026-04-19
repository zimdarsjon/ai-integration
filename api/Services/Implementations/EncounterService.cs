using Microsoft.EntityFrameworkCore;

public class EncounterService : IEncounterService
{
    private readonly AppDbContext _db;
    private readonly ICharacterService _characterService;

    public EncounterService(AppDbContext db, ICharacterService characterService)
    {
        _db = db;
        _characterService = characterService;
    }

    public async Task<IEnumerable<EncounterSummaryDto>> GetCampaignEncountersAsync(int campaignId, int requestingUserId, CancellationToken ct = default)
    {
        await VerifyCampaignAccessAsync(campaignId, requestingUserId, ct);
        return await _db.Encounters
            .Where(e => e.CampaignId == campaignId)
            .Select(e => new EncounterSummaryDto
            {
                Id = e.Id,
                Name = e.Name,
                Round = e.Round,
                IsActive = e.IsActive,
                ParticipantCount = e.Participants.Count
            }).ToListAsync(ct);
    }

    public async Task<EncounterDetailDto> GetByIdAsync(int id, int requestingUserId, CancellationToken ct = default)
    {
        var encounter = await LoadFullEncounterAsync(id, ct)
            ?? throw new NotFoundException($"Encounter {id} not found.");
        await VerifyCampaignAccessAsync(encounter.CampaignId, requestingUserId, ct);
        return MapToDto(encounter);
    }

    public async Task<EncounterDetailDto> CreateAsync(int campaignId, CreateEncounterRequest request, int userId, CancellationToken ct = default)
    {
        await VerifyDMAsync(campaignId, userId, ct);
        var encounter = new Encounter
        {
            CampaignId = campaignId,
            SessionId = request.SessionId,
            Name = request.Name,
            Description = request.Description,
            IsActive = true,
            Round = 1
        };
        _db.Encounters.Add(encounter);
        await _db.SaveChangesAsync(ct);
        return await GetByIdAsync(encounter.Id, userId, ct);
    }

    public async Task DeleteAsync(int id, int userId, CancellationToken ct = default)
    {
        var encounter = await _db.Encounters.FindAsync([id], ct)
            ?? throw new NotFoundException($"Encounter {id} not found.");
        await VerifyDMAsync(encounter.CampaignId, userId, ct);
        _db.Encounters.Remove(encounter);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<EncounterDetailDto> AddParticipantAsync(int id, AddParticipantRequest request, int userId, CancellationToken ct = default)
    {
        var encounter = await _db.Encounters.FindAsync([id], ct)
            ?? throw new NotFoundException($"Encounter {id} not found.");
        await VerifyDMAsync(encounter.CampaignId, userId, ct);

        int maxHP = request.MaxHP ?? 0;
        int ac = request.AC ?? 0;
        string displayName = request.DisplayName ?? "";

        if (request.ParticipantType == ParticipantType.Character && request.CharacterId.HasValue)
        {
            var character = await _db.Characters.FindAsync([request.CharacterId.Value], ct)
                ?? throw new NotFoundException($"Character not found.");
            maxHP = character.MaxHP;
            ac = character.AC;
            displayName = string.IsNullOrWhiteSpace(displayName) ? character.Name : displayName;
        }
        else if (request.ParticipantType == ParticipantType.Monster && request.MonsterId.HasValue)
        {
            var monster = await _db.Monsters.FindAsync([request.MonsterId.Value], ct)
                ?? throw new NotFoundException($"Monster not found.");
            maxHP = request.MaxHP ?? monster.MaxHP;
            ac = request.AC ?? monster.AC;
            displayName = string.IsNullOrWhiteSpace(displayName) ? monster.Name : displayName;
        }

        var turnOrder = await _db.EncounterParticipants
            .Where(p => p.EncounterId == id)
            .CountAsync(ct);

        _db.EncounterParticipants.Add(new EncounterParticipant
        {
            EncounterId = id,
            ParticipantType = request.ParticipantType,
            CharacterId = request.CharacterId,
            MonsterId = request.MonsterId,
            DisplayName = displayName,
            Initiative = request.Initiative,
            CurrentHP = maxHP,
            MaxHP = maxHP,
            AC = ac,
            TurnOrder = turnOrder,
            IsActive = true
        });

        await _db.SaveChangesAsync(ct);
        return await GetByIdAsync(id, userId, ct);
    }

    public async Task<EncounterDetailDto> RemoveParticipantAsync(int id, int participantId, int userId, CancellationToken ct = default)
    {
        var encounter = await _db.Encounters.FindAsync([id], ct)
            ?? throw new NotFoundException($"Encounter {id} not found.");
        await VerifyDMAsync(encounter.CampaignId, userId, ct);

        var participant = await _db.EncounterParticipants.FindAsync([participantId], ct)
            ?? throw new NotFoundException("Participant not found.");

        _db.EncounterParticipants.Remove(participant);
        await _db.SaveChangesAsync(ct);
        return await GetByIdAsync(id, userId, ct);
    }

    public async Task<EncounterDetailDto> ApplyDamageAsync(int id, int participantId, ApplyEncounterDamageRequest request, int userId, CancellationToken ct = default)
    {
        var encounter = await _db.Encounters.FindAsync([id], ct)
            ?? throw new NotFoundException($"Encounter {id} not found.");
        await VerifyDMAsync(encounter.CampaignId, userId, ct);

        var participant = await _db.EncounterParticipants.FindAsync([participantId], ct)
            ?? throw new NotFoundException("Participant not found.");

        participant.CurrentHP = Math.Max(0, participant.CurrentHP - request.Amount);

        // If linked character, sync damage back
        if (request.SyncToCharacter && participant.CharacterId.HasValue)
        {
            await _characterService.ApplyDamageAsync(participant.CharacterId.Value, request.Amount, userId, CancellationToken.None);
        }

        await _db.SaveChangesAsync(ct);
        return await GetByIdAsync(id, userId, ct);
    }

    public async Task<EncounterDetailDto> ApplyConditionAsync(int id, int participantId, int conditionId, int userId, CancellationToken ct = default)
    {
        var encounter = await _db.Encounters.FindAsync([id], ct)
            ?? throw new NotFoundException($"Encounter {id} not found.");
        await VerifyDMAsync(encounter.CampaignId, userId, ct);

        var already = await _db.EncounterConditions.AnyAsync(ec => ec.EncounterParticipantId == participantId && ec.ConditionId == conditionId, ct);
        if (!already)
        {
            _db.EncounterConditions.Add(new EncounterCondition { EncounterParticipantId = participantId, ConditionId = conditionId });
            await _db.SaveChangesAsync(ct);
        }

        return await GetByIdAsync(id, userId, ct);
    }

    public async Task<EncounterDetailDto> RemoveConditionAsync(int id, int participantId, int conditionId, int userId, CancellationToken ct = default)
    {
        var encounter = await _db.Encounters.FindAsync([id], ct)
            ?? throw new NotFoundException($"Encounter {id} not found.");
        await VerifyDMAsync(encounter.CampaignId, userId, ct);

        var condition = await _db.EncounterConditions
            .FirstOrDefaultAsync(ec => ec.EncounterParticipantId == participantId && ec.ConditionId == conditionId, ct);
        if (condition != null)
        {
            _db.EncounterConditions.Remove(condition);
            await _db.SaveChangesAsync(ct);
        }

        return await GetByIdAsync(id, userId, ct);
    }

    public async Task<EncounterDetailDto> AdvanceTurnAsync(int id, int userId, CancellationToken ct = default)
    {
        var encounter = await _db.Encounters
            .Include(e => e.Participants)
            .FirstOrDefaultAsync(e => e.Id == id, ct)
            ?? throw new NotFoundException($"Encounter {id} not found.");
        await VerifyDMAsync(encounter.CampaignId, userId, ct);

        var active = encounter.Participants.Where(p => p.IsActive).OrderBy(p => p.TurnOrder).ToList();
        if (active.Count == 0) return await GetByIdAsync(id, userId, ct);

        var currentMax = active.Max(p => p.TurnOrder);
        var next = active.FirstOrDefault(p => p.TurnOrder > currentMax);
        if (next == null)
        {
            encounter.Round++;
            foreach (var p in active) p.TurnOrder = active.IndexOf(p);
        }

        await _db.SaveChangesAsync(ct);
        return await GetByIdAsync(id, userId, ct);
    }

    public async Task<EncounterDetailDto> NextRoundAsync(int id, int userId, CancellationToken ct = default)
    {
        var encounter = await _db.Encounters.FindAsync([id], ct)
            ?? throw new NotFoundException($"Encounter {id} not found.");
        await VerifyDMAsync(encounter.CampaignId, userId, ct);

        encounter.Round++;
        await _db.SaveChangesAsync(ct);
        return await GetByIdAsync(id, userId, ct);
    }

    private async Task VerifyCampaignAccessAsync(int campaignId, int userId, CancellationToken ct)
    {
        var isMember = await _db.CampaignMembers.AnyAsync(m => m.CampaignId == campaignId && m.UserId == userId, ct);
        var isDM = await _db.Campaigns.AnyAsync(c => c.Id == campaignId && c.DMUserId == userId, ct);
        if (!isMember && !isDM) throw new UnauthorizedException("Not a member of this campaign.");
    }

    private async Task VerifyDMAsync(int campaignId, int userId, CancellationToken ct)
    {
        var isDM = await _db.Campaigns.AnyAsync(c => c.Id == campaignId && c.DMUserId == userId, ct);
        if (!isDM) throw new UnauthorizedException("Only the DM can perform this action.");
    }

    private async Task<Encounter?> LoadFullEncounterAsync(int id, CancellationToken ct) =>
        await _db.Encounters
            .Include(e => e.Participants).ThenInclude(p => p.Conditions).ThenInclude(c => c.Condition)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    private static EncounterDetailDto MapToDto(Encounter e) => new()
    {
        Id = e.Id,
        CampaignId = e.CampaignId,
        Name = e.Name,
        Description = e.Description,
        Round = e.Round,
        IsActive = e.IsActive,
        Participants = e.Participants.OrderByDescending(p => p.Initiative).Select(p => new ParticipantDto
        {
            Id = p.Id,
            ParticipantType = p.ParticipantType,
            CharacterId = p.CharacterId,
            MonsterId = p.MonsterId,
            DisplayName = p.DisplayName,
            Initiative = p.Initiative,
            CurrentHP = p.CurrentHP,
            MaxHP = p.MaxHP,
            AC = p.AC,
            TurnOrder = p.TurnOrder,
            IsActive = p.IsActive,
            Conditions = p.Conditions.Select(c => c.Condition.Name).ToList()
        }).ToList()
    };
}

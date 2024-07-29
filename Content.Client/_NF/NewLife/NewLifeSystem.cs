using Content.Shared._NF.NewLife;
using Content.Shared.Preferences;

namespace Content.Client._NF.NewLife;

public sealed class NewLifeSystem : EntitySystem
{
    private HashSet<HumanoidCharacterProfile> _usedProfiles = [];

    public override void Initialize()
    {
        SubscribeNetworkEvent<UsedProfilesEvent>(OnUsedCharacters);
    }

    private void OnUsedCharacters(UsedProfilesEvent e)
    {
        _usedProfiles = e.UsedProfiles;
    }

    public bool IsProfileUsed(HumanoidCharacterProfile profile)
    {
        return _usedProfiles.Contains(profile);
    }
}

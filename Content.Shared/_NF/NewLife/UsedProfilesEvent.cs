using Content.Shared.Preferences;
using Robust.Shared.Serialization;

namespace Content.Shared._NF.NewLife;

[Serializable, NetSerializable]
public sealed class UsedProfilesEvent(HashSet<HumanoidCharacterProfile> usedProfiles) : EntityEventArgs
{
    public HashSet<HumanoidCharacterProfile> UsedProfiles = usedProfiles;
}

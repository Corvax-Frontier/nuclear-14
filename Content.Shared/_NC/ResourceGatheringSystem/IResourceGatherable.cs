namespace Content.Shared._NC.ResourceGatheringSystem;


public interface IResourceGatherable
{

    void OnGathered(EntityUid byUser);
}

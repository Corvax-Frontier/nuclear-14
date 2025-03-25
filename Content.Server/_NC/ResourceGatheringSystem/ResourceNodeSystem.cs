using Content.Shared._NC.ResourceGatheringSystem;

namespace Content.Server._NC.ResourceGatheringSystem;


public sealed class ResourceNodeSystem : EntitySystem
{
    private float _updateTimer;
    private const float UpdateInterval = 1f;

    private static readonly ISawmill Sawmill = Logger.GetSawmill("resourceNode");

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        _updateTimer += frameTime;

        if (_updateTimer < UpdateInterval)
        {
            return;
        }

        _updateTimer = 0f;

        foreach (var node in EntityQuery<ResourceNodeComponent>())
        {
            if (node.TimeBeforeNextGather > 0)
            {
                node.TimeBeforeNextGather = Math.Max(0, node.TimeBeforeNextGather - UpdateInterval);
                Sawmill.Debug($"[ResourceNodeSystem] Node cooldown tick: Remaining {node.TimeBeforeNextGather} sec");
            }
        }
    }
}

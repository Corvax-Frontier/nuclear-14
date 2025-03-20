using Content.Shared._NC.ResourceGatheringSystem;

namespace Content.Server._NC.ResourceGatheringSystem;
public sealed class ResourceNodeSystem : EntitySystem
{
    private float _updateTimer;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        _updateTimer += frameTime;

        if (_updateTimer < 1f)
            return;

        _updateTimer = 0f;


        foreach (var node in EntityQuery<ResourceNode>())
        {
            if (node.TimeBeforeNextGather > 0)
            {
                node.TimeBeforeNextGather = Math.Max(0, node.TimeBeforeNextGather - 1f);
            }
        }
    }
}

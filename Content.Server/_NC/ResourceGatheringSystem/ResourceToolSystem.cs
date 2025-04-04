using Content.Shared._NC.ResourceGatheringSystem;

namespace Content.Server._NC.ResourceGatheringSystem;

public sealed class ResourceToolSystem : EntitySystem
{
    public bool IsAllowedTool(EntityUid toolEntity, List<string> allowedToolIds)
    {
        if (allowedToolIds.Count == 0)
        {
            return true;
        }

        if (!TryComp<SharedResourceToolComponent>(toolEntity, out var toolComp))
        {
            return false;
        }

        return allowedToolIds.Contains(toolComp.ToolId);
    }
}

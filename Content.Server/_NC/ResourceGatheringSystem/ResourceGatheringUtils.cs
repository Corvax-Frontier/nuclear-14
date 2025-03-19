namespace Content.Server._NC.ResourceGatheringSystem;

/// <summary>
/// Система для проверки, является ли предмет подходящим инструментом.
/// </summary>
public sealed class ResourceToolSystem : EntitySystem
{
    /// <summary>
    /// Проверяет, соответствует ли данный entity (инструмент) списку инструментов.
    /// </summary>
    public bool IsAllowedTool(EntityUid toolEntity, List<string> allowedToolIds)
    {
        if (allowedToolIds.Count == 0)
            return true; // если пусто, подойдёт любой инструмент

        if (!TryComp<ResourceToolComponent>(toolEntity, out var toolComp))
            return false;

        return allowedToolIds.Contains(toolComp.ToolId);
    }
}

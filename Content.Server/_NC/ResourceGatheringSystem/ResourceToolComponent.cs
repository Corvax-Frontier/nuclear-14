using Content.Server._NC.AdvancedSpawner;

namespace Content.Server._NC.ResourceGatheringSystem;
[RegisterComponent]
public sealed partial class ResourceToolComponent : Component
{
    [DataField] public string ToolId = string.Empty;

    // Модификаторы веса на категории
    [DataField] public Dictionary<string, int> WeightModifiers = new();

    // Добавляемые прототипы в категории
    [DataField] public Dictionary<string, List<SpawnEntry>> ExtraPrototypes = new();

    // Удаляемые прототипы из категорий
    [DataField] public Dictionary<string, List<string>> RemovePrototypes = new();

    // Добавляемые новые категории (с весом)
    [DataField] public Dictionary<string, int> AddCategories = new();

    // Полностью удаляемые категории
    [DataField] public List<string> RemoveCategories = new();
}

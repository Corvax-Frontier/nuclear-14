namespace Content.Server._NC.AdvancedSpawner;

public sealed class AdvancedRandomSpawnerConfig
{
    private readonly Dictionary<string, int> _categoryWeights;
    private readonly Dictionary<string, List<SpawnEntry>> _prototypes;
    private readonly List<SpawnCategory> _categories;

    public float Offset { get; }
    public bool DeleteAfterSpawn { get; }
    public int MaxSpawnCount { get; }

    private static readonly ISawmill Sawmill = Logger.GetSawmill("advancedSpawner");

    private AdvancedRandomSpawnerConfig(AdvancedRandomSpawnerComponent comp)
    {
        _categoryWeights = new(comp.CategoryWeights);
        _prototypes = new();
        _categories = new();

        foreach (var category in comp.CategoryWeights.Keys)
        {
            var entries = comp.PrototypeLists.GetValueOrDefault(category, new List<SpawnEntry>());
            _prototypes[category] = new(entries);
            _categories.Add(new SpawnCategory(category, _categoryWeights[category], _prototypes[category]));
        }

        Offset = comp.Offset;
        DeleteAfterSpawn = comp.DeleteAfterSpawn;
        MaxSpawnCount = comp.MaxSpawnCount;

        Sawmill.Debug("[AdvancedSpawnerConfig] Initialized from component.");
    }

    public static AdvancedRandomSpawnerConfig FromComponent(AdvancedRandomSpawnerComponent comp)
    {
        return new AdvancedRandomSpawnerConfig(comp);
    }

    public int GetCategoryWeight(string category)
    {
        return _categoryWeights.TryGetValue(category, out var weight) ? weight : 0;
    }

    private void ModifyCategoryWeight(string category, int delta)
    {
        if (_categoryWeights.ContainsKey(category))
            _categoryWeights[category] = Math.Max(1, _categoryWeights[category] + delta);
        else
            _categoryWeights[category] = Math.Max(1, delta);

        Sawmill.Debug($"[AdvancedSpawnerConfig] Modified weight of '{category}' by {delta}, new weight: {_categoryWeights[category]}");
    }

    public void AddPrototype(string category, SpawnEntry entry)
    {
        if (!_prototypes.ContainsKey(category))
        {
            _prototypes[category] = new List<SpawnEntry>();
            _categoryWeights[category] = 1;
        }
        _prototypes[category].Add(entry);
        Sawmill.Debug($"[AdvancedSpawnerConfig] Added prototype '{entry.PrototypeId}' to category '{category}'");
    }

    public void ClearCategory(string category)
    {
        _prototypes.Remove(category);
        _categoryWeights.Remove(category);
        Sawmill.Debug($"[AdvancedSpawnerConfig] Cleared category '{category}'");
    }

    public void ApplyModifiers(Dictionary<string, int> weightModifiers, Dictionary<string, List<SpawnEntry>>? extraPrototypes = null)
    {
        foreach (var (category, modifier) in weightModifiers)
        {
            ModifyCategoryWeight(category, modifier);
        }

        if (extraPrototypes != null)
        {
            foreach (var (category, entries) in extraPrototypes)
            {
                foreach (var entry in entries)
                {
                    AddPrototype(category, entry);
                }
            }
        }
    }

    public void RebuildCategories()
    {
        _categories.Clear();
        foreach (var category in _categoryWeights.Keys)
        {
            var entries = _prototypes.GetValueOrDefault(category, new List<SpawnEntry>());
            _categories.Add(new SpawnCategory(category, _categoryWeights[category], entries));
        }
        Sawmill.Debug("[AdvancedSpawnerConfig] Rebuilt Categories after modifiers.");
    }

    public IReadOnlyList<SpawnCategory> GetCategories()
    {
        return _categories;
    }

    public bool TryRemovePrototypes(string category, List<string> prototypeIds, out int removedCount)
    {
        removedCount = 0;
        if (!_prototypes.TryGetValue(category, out var entries))
            return false;

        var before = entries.Count;
        entries.RemoveAll(entry => prototypeIds.Contains(entry.PrototypeId));
        removedCount = before - entries.Count;
        return removedCount > 0;
    }
}


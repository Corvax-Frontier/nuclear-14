namespace Content.Server._NC.AdvancedSpawner;

public static class SpawnerExtensions
{
    public static int GetCategoryModifier(this AdvancedRandomSpawnerConfig config, string category)
    {
        return config.GetCategoryWeight(category);
    }
}

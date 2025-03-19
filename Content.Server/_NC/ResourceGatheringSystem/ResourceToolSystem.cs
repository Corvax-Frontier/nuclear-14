namespace Content.Server._Stalker.ResourceGathering;

public static class ResourceGatheringUtils
{
    [Obsolete("Пример для отладки или расширений")]
    public static int GetWeightModifier(string category, Dictionary<string, int> weightModifiers)
    {
        var modifier = weightModifiers.GetValueOrDefault(category, 0);
        Logger.Info($"[ResourceGatheringUtils] GetWeightModifier: {category} = {modifier}");
        return modifier;
    }
}

using Content.Shared.Popups;
using Robust.Shared.Random;
using Robust.Shared.Map;
using Content.Server._NC.AdvancedSpawner;
using Robust.Shared.Audio;
using Robust.Shared.Timing;
using Robust.Server.Audio;
using Robust.Server.GameObjects;

namespace Content.Server._NC.ResourceGatheringSystem;

public enum ResourceVisuals
{
    CurrentAnimation,
    Richness,
    Cooldown
}

public sealed class ResourceGatheringSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedPopupSystem _popupSystem = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;

    private static readonly ISawmill Sawmill = Logger.GetSawmill("resourceGathering");

    public void ProcessGathering(EntityUid actor, EntityCoordinates location, AdvancedRandomSpawnerConfig config)
    {
        Sawmill.Debug($"[ResourceGathering] Processing gathering action by {actor} at {location}");

        var spawner = AdvancedEntitySpawner.Create(_random, _entityManager, _popupSystem, config.Categories, config.MaxSpawnCount);
        var spawnedItems = spawner.SpawnEntities(actor, location, config.Offset, config);

        if (spawnedItems.Count == 0)
            _popupSystem.PopupEntity("Ты ничего не нашёл.", actor);
        else
            _popupSystem.PopupEntity($"Ты нашёл: {string.Join(", ", spawnedItems)}", actor);
    }

    public AdvancedRandomSpawnerConfig CreateDefaultMiningConfig()
    {
        var dummyComp = new AdvancedRandomSpawnerComponent
        {
            MaxSpawnCount = 2,
            Offset = 0.0f,
            DeleteAfterSpawn = false,
            CategoryWeights = new Dictionary<string, int>
            {
                { "ore", 10 },
                { "trash", 3 }
            },
            PrototypeLists = new Dictionary<string, List<SpawnEntry>>
            {
                {
                    "ore",
                    new List<SpawnEntry>
                    {
                        new("iron_ore", 8, 1),
                        new("gold_ore", 3, 1),
                        new("platinum_ore", 1, 1)
                    }
                },
                {
                    "trash",
                    new List<SpawnEntry>
                    {
                        new("rusty_can", 5, 1),
                        new("broken_tool", 3, 1)
                    }
                }
            }
        };

        return AdvancedRandomSpawnerConfig.FromComponent(dummyComp);
    }

    public void TryGatherFromNode(EntityUid actor, EntityUid node, EntityCoordinates location)
    {
        if (!TryComp<ResourceNodeComponent>(node, out var nodeComp))
            return;

        if (nodeComp.IsOnCooldown)
        {
            _popupSystem.PopupEntity("Узел отдыхает...", actor);
            return;
        }

        PlayAnimationByRichness(node, nodeComp);
        _entityManager.System<AudioSystem>().PlayPvs(nodeComp.GatherSound, node);
        ProcessGathering(actor, location, nodeComp.GatherConfig);

        nodeComp.CurrentUses++;
        nodeComp.IsOnCooldown = true;
        UpdateNodeAppearance(node, nodeComp);

        Timer.Spawn(TimeSpan.FromSeconds(nodeComp.CooldownTime), () =>
        {
            nodeComp.IsOnCooldown = false;
            RecalculateRichness(nodeComp); // Тут мы убираем параметр node
            UpdateNodeAppearance(node, nodeComp);
            _popupSystem.PopupEntity("Узел восстановился!", node);
        });
    }

    // Убираем лишний параметр 'node', раз не используется
    private void PlayAnimationByRichness(EntityUid node, ResourceNodeComponent nodeComp)
    {
        if (!nodeComp.AnimationStates.TryGetValue(nodeComp.Richness, out var anim))
            return;

        _entityManager.System<AppearanceSystem>().SetData(node, ResourceVisuals.CurrentAnimation, anim);
    }

    private void UpdateNodeAppearance(EntityUid node, ResourceNodeComponent nodeComp)
    {
        _entityManager.System<AppearanceSystem>().SetData(node, ResourceVisuals.Richness, nodeComp.Richness);
        _entityManager.System<AppearanceSystem>().SetData(node, ResourceVisuals.Cooldown, nodeComp.IsOnCooldown);
    }

    // Избавились от параметра node
    private void RecalculateRichness(ResourceNodeComponent nodeComp)
    {
        var roll = _random.Next(0, 100);
        nodeComp.Richness = roll switch
        {
            <= 20 => NodeRichness.Rich,
            <= 70 => NodeRichness.Medium,
            _ => NodeRichness.Poor
        };
    }
}

[RegisterComponent]
public sealed partial class ResourceNodeComponent : Component
{
    [DataField("maxUses")]
    public int MaxUses = 5;

    // Default(0) для int нет смысла хранить
    [DataField("currentUses")]
    public int CurrentUses; // убрали '= 0;'

    [DataField("gatherConfig")]
    public AdvancedRandomSpawnerConfig GatherConfig = default!;

    [DataField("cooldownTime")]
    public float CooldownTime = 60f;

    // Default(false) для bool нет смысла хранить
    [DataField("isOnCooldown")]
    public bool IsOnCooldown; // убрали '= false;'

    [DataField("richness")]
    public NodeRichness Richness = NodeRichness.Medium;

    [DataField("animationStates")]
    public Dictionary<NodeRichness, string> AnimationStates = new();

    [DataField("gatherSound")]
    public SoundSpecifier GatherSound = new SoundPathSpecifier("/Audio/Effects/mine_hit.ogg");
}

public enum NodeRichness
{
    Poor,
    Medium,
    Rich
}

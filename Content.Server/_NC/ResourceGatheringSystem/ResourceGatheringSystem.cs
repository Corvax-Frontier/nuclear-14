using System.Numerics;
using Content.Server.Popups;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.Map;
using Content.Server._NC.AdvancedSpawner;
using Content.Shared._NC.ResourceGatheringSystem;
using Content.Shared.Tag;
using Robust.Shared.Random;

namespace Content.Server._NC.ResourceGatheringSystem;

public sealed class ResourceGatheringSystem : EntitySystem
{
    [Dependency] private readonly PopupSystem _popupSystem = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly AdvancedRandomSpawnerSystem _spawnerSystem = default!;
    [Dependency] private readonly TagSystem _tag = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    private const float SearchRadius = 1.0f;
    private const int AngleStep = 30;
    private const int FullCircle = 360;
    private static readonly ISawmill Sawmill = Logger.GetSawmill("resourceGathering");

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SharedResourceGatheringComponent, BeforeRangedInteractEvent>(OnUseTool);
        SubscribeLocalEvent<SharedResourceGatheringComponent, GatherResourceDoAfterEvent>(OnDoAfter);
    }

    private void OnUseTool(EntityUid uid, SharedResourceGatheringComponent comp, BeforeRangedInteractEvent args)
    {
        if (!args.CanReach || args.Target == null)
            return;

        if (!ValidateGathering(uid, args.Target.Value, args.User))
            return;

        // Блок мини-игры перед началом добычи
        if (!PlayMiniGame(args.User))
        {
            _popupSystem.PopupEntity(Loc.GetString("nc-resource-minigame-fail"), args.User, PopupType.MediumCaution);
            return;
        }

        StartGathering(uid, comp, args.Target.Value, args.User);
    }

    private bool ValidateGathering(EntityUid tool, EntityUid target, EntityUid user)
    {
        if (!TryComp<ResourceNodeComponent>(target, out var node))
        {
            _popupSystem.PopupEntity(Loc.GetString("nc-resource-invalid-target"), user, PopupType.LargeCaution);
            return false;
        }

        if (!_tag.HasTag(target, "Gatherable"))
        {
            _popupSystem.PopupEntity(Loc.GetString("nc-resource-invalid-target"), user, PopupType.LargeCaution);
            return false;
        }

        if (!TryComp<SharedResourceToolComponent>(tool, out var toolComp) || !TryComp<SharedResourceGatheringComponent>(tool, out _))
        {
            _popupSystem.PopupEntity(Loc.GetString("nc-resource-tool-error"), user, PopupType.LargeCaution);
            return false;
        }

        if (node.AllowedTools.Count > 0 && !node.AllowedTools.Contains(toolComp.ToolId))
        {
            _popupSystem.PopupEntity(Loc.GetString("nc-resource-tool-not-suitable"), user, PopupType.LargeCaution);
            return false;
        }

        if (node.TimeBeforeNextGather > 0f)
        {
            _popupSystem.PopupEntity(Loc.GetString("nc-resource-empty"), user, PopupType.LargeCaution);
            return false;
        }

        return true;
    }

    private void StartGathering(EntityUid tool, SharedResourceGatheringComponent comp, EntityUid target, EntityUid user)
    {
        var doAfterArgs = new DoAfterArgs(
            EntityManager, user, TimeSpan.FromSeconds(comp.GatherTime),
            new GatherResourceDoAfterEvent(), tool, target: target, used: tool)
        {
            BreakOnDamage = true,
            NeedHand = true,
            DistanceThreshold = 2f
        };

        _doAfterSystem.TryStartDoAfter(doAfterArgs);
    }

    private void OnDoAfter(EntityUid uid, SharedResourceGatheringComponent comp, GatherResourceDoAfterEvent args)
    {
        if (args.Handled || args.Cancelled || args.Args.Target == null)
            return;

        if (!TryComp<ResourceNodeComponent>(args.Args.Target.Value, out var node) || string.IsNullOrEmpty(node.LootSpawner))
            return;

        HandleGathering(uid, comp, node, args.Args.User);
        ApplyNodeCooldown(node);

        _popupSystem.PopupEntity(Loc.GetString("nc-resource-gather-success"), args.Args.User, PopupType.Large);
        args.Handled = true;
    }

    private void HandleGathering(EntityUid tool, SharedResourceGatheringComponent comp, ResourceNodeComponent node, EntityUid user)
    {
        var spawnCoords = FindFreePosition(user);
        var spawnerUid = EntityManager.SpawnEntity(node.LootSpawner, spawnCoords);

        if (!TryComp<AdvancedRandomSpawnerComponent>(spawnerUid, out var spawner))
        {
            Sawmill.Warning($"[ResourceGathering] Failed to get AdvancedRandomSpawnerComponent on {spawnerUid}, deleting.");
            EntityManager.DeleteEntity(spawnerUid);
            return;
        }

        var config = new AdvancedRandomSpawnerConfig(spawner);
        if (!TryComp(tool, out SharedResourceToolComponent? toolComp))
        {
            Sawmill.Warning($"[ResourceGathering] Tool {tool} lost SharedResourceToolComponent during gather.");
            return;
        }

        var totalModifiers = CalculateTotalModifiers(node, comp, toolComp);
        ApplyToolPrototypeEffects(toolComp, config);
        config.ApplyModifiers(totalModifiers);

        config.RebuildCategories();

        _spawnerSystem.SpawnEntitiesUsingSpawner(spawnerUid, config);
    }

    private void ApplyNodeCooldown(ResourceNodeComponent node)
    {
        node.TimeBeforeNextGather = node.CooldownAfterGather;
        Sawmill.Debug($"[ResourceGathering] Node cooldown applied: {node.CooldownAfterGather} sec");
    }

    private Dictionary<string, int> CalculateTotalModifiers(ResourceNodeComponent node, SharedResourceGatheringComponent comp, SharedResourceToolComponent toolComp)
    {
        var modifiers = new Dictionary<string, int>(comp.WeightModifiers);

        if (node.ResourceRichness != ResourceRichness.Medium)
        {
            var richnessMod = node.ResourceRichness switch
            {
                ResourceRichness.Rich => 5,
                ResourceRichness.Poor => -3,
                _ => 0
            };
            modifiers["richness"] = modifiers.GetValueOrDefault("richness") + richnessMod;
        }

        foreach (var (category, mod) in toolComp.WeightModifiers)
            modifiers[category] = modifiers.GetValueOrDefault(category) + mod;

        foreach (var (category, weight) in toolComp.AddCategories)
            modifiers[category] = modifiers.GetValueOrDefault(category) + weight;

        return modifiers;
    }

    private void ApplyToolPrototypeEffects(SharedResourceToolComponent toolComp, AdvancedRandomSpawnerConfig config)
    {
        // Удаление целых категорий вместе с весами и прототипами
        foreach (var category in toolComp.RemoveCategories)
        {
            var weightRemoved = config.CategoryWeights.Remove(category);
            var prototypesRemoved = config.Prototypes.Remove(category);
            Sawmill.Debug($"[ResourceGathering] Removed category '{category}' -> Weights: {weightRemoved}, Prototypes: {prototypesRemoved}");
        }

        // Удаление конкретных прототипов внутри категории
        foreach (var (category, toRemove) in toolComp.RemovePrototypes)
        {
            if (config.Prototypes.TryGetValue(category, out var list))
            {
                var before = list.Count;
                list.RemoveAll(entry => toRemove.Contains(entry.PrototypeId));
                var removed = before - list.Count;
                if (removed > 0)
                    Sawmill.Debug($"[ResourceGathering] Removed {removed} prototype(s) from category '{category}'");
            }
        }

        // Добавление дополнительных прототипов в категории
        foreach (var (category, protoList) in toolComp.ExtraPrototypeIds)
        {
            foreach (var protoId in protoList)
            {
                config.AddPrototype(category, new SpawnEntry { PrototypeId = protoId });
                Sawmill.Debug($"[ResourceGathering] Added extra prototype '{protoId}' to category '{category}'");
            }
        }
    }


    private EntityCoordinates FindFreePosition(EntityUid user)
    {
        if (!EntityManager.TryGetComponent(user, out TransformComponent? userTransform))
            return new EntityCoordinates(user, Vector2.Zero);

        var origin = userTransform.Coordinates;

        for (var i = 0; i < FullCircle; i += AngleStep)
        {
            var angle = i * (float)(Math.PI / 180.0);
            var offset = new Vector2(MathF.Cos(angle) * SearchRadius, MathF.Sin(angle) * SearchRadius);
            var testCoords = new EntityCoordinates(origin.EntityId, origin.Position + offset);

            // TODO: Проверку коллизий сделать тут через карту
            return testCoords;
        }

        return origin;
    }

    /// <summary>
    /// Мини-игра (QTE заглушка): шанс успеха. В будущем сюда нормальный интерактив.
    /// </summary>
    private bool PlayMiniGame(EntityUid user)
    {
        var success = _random.Prob(0.75f); // 75% успеха, можно менять
        if (success)
            _popupSystem.PopupEntity(Loc.GetString("nc-resource-minigame-success"), user, PopupType.Medium);
        return success;
    }
}

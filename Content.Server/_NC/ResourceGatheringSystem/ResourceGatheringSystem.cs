using System.Numerics;
using Content.Server.Popups;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Content.Server._NC.AdvancedSpawner;
using GatherResourceDoAfterEvent = Content.Shared._NC.ResourceGatheringSystem.GatherResourceDoAfterEvent;

namespace Content.Server._NC.ResourceGatheringSystem;

public sealed class ResourceGatheringSystem : EntitySystem
{
    [Dependency] private readonly PopupSystem _popupSystem = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] internal new readonly IEntityManager EntityManager = default!;
    [Dependency] private readonly AdvancedRandomSpawnerSystem _spawnerSystem = default!;

    private const float SearchRadius = 1.0f;
    private const int AngleStep = 30;
    private const int FullCircle = 360;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ResourceGatheringComponent, BeforeRangedInteractEvent>(OnUseTool);
        SubscribeLocalEvent<ResourceGatheringComponent, GatherResourceDoAfterEvent>(OnDoAfter);
    }

    private void OnUseTool(EntityUid uid, ResourceGatheringComponent comp, BeforeRangedInteractEvent args)
    {
        if (!args.CanReach)
            return;

        StartGathering(uid, comp, args.Target, args.User);
    }

    private void StartGathering(EntityUid uid, ResourceGatheringComponent comp, EntityUid? target, EntityUid user)
    {
        if (target == null || !TryComp<ResourceNodeComponent>(target.Value, out var node))
            return;

        if (!TryComp<ResourceToolComponent>(uid, out var toolComp))
        {
            _popupSystem.PopupEntity("Инструмент неисправен!", user, PopupType.LargeCaution);
            return;
        }

        if (node.AllowedTools.Count > 0 && !node.AllowedTools.Contains(toolComp.ToolId))
        {
            _popupSystem.PopupEntity("Этот инструмент не подходит!", user, PopupType.LargeCaution);
            return;
        }

        if (node.TimeBeforeNextGather > 0f)
        {
            _popupSystem.PopupEntity("Здесь пока нечего собирать.", user, PopupType.LargeCaution);
            return;
        }

        var doAfterArgs = new DoAfterArgs(EntityManager, user, TimeSpan.FromSeconds(comp.GatherTime),
            new GatherResourceDoAfterEvent(), uid, target: target.Value, used: uid)
        {
            BreakOnDamage = true,
            NeedHand = true,
            DistanceThreshold = 2f
        };

        _doAfterSystem.TryStartDoAfter(doAfterArgs);
    }

    private void OnDoAfter(EntityUid uid, ResourceGatheringComponent comp, GatherResourceDoAfterEvent args)
    {
        if (args.Handled || args.Cancelled || args.Args.Target == null)
            return;

        if (!TryComp<ResourceNodeComponent>(args.Args.Target.Value, out var node))
            return;

        var spawnerPrototype = node.LootSpawner;
        if (string.IsNullOrEmpty(spawnerPrototype))
            return;

        var spawnCoords = FindFreePosition(args.Args.User);
        var spawnerUid = EntityManager.SpawnEntity(spawnerPrototype, spawnCoords);

        if (!TryComp<AdvancedRandomSpawnerComponent>(spawnerUid, out var spawner))
        {
            EntityManager.DeleteEntity(spawnerUid);
            return;
        }

        var config = new AdvancedRandomSpawnerConfig(spawner);

        ApplyNodeRichness(node, comp);

        // Важное исправление - ApplyToolEffects принимает готовый toolComp
        if (TryComp<ResourceToolComponent>(uid, out var toolComp))
        {
            ApplyToolEffects(toolComp, comp, config);
        }

        config.ApplyModifiers(comp.WeightModifiers, comp.ExtraPrototypes);

        node.TimeBeforeNextGather = node.CooldownAfterGather;

        _spawnerSystem.SpawnEntitiesUsingSpawner(spawnerUid, config);

        _popupSystem.PopupEntity("Вы успешно собрали ресурсы!", uid, PopupType.LargeCaution);
        args.Handled = true;
    }

    private void ApplyNodeRichness(ResourceNodeComponent node, ResourceGatheringComponent comp)
    {
        var richnessModifier = node.ResourceRichness switch
        {
            "rich" => 5,
            "poor" => -3,
            _ => 0
        };

        comp.WeightModifiers["rare"] = comp.WeightModifiers.GetValueOrDefault("rare") + richnessModifier;
    }

    // ✅ toolComp передаётся сюда напрямую
    private void ApplyToolEffects(ResourceToolComponent toolComp, ResourceGatheringComponent comp, AdvancedRandomSpawnerConfig config)
    {
        // Весовые модификаторы
        foreach (var (category, modifier) in toolComp.WeightModifiers)
        {
            comp.WeightModifiers[category] = comp.WeightModifiers.GetValueOrDefault(category) + modifier;
        }

        // Добавление прототипов
        foreach (var (category, entries) in toolComp.ExtraPrototypes)
        {
            if (!comp.ExtraPrototypes.ContainsKey(category))
                comp.ExtraPrototypes[category] = new();

            comp.ExtraPrototypes[category].AddRange(entries);
        }

        // Добавление новых категорий
        foreach (var (category, weight) in toolComp.AddCategories)
        {
            comp.WeightModifiers.TryAdd(category, weight);
        }

        // Удаление категорий
        foreach (var category in toolComp.RemoveCategories)
        {
            config.CategoryWeights.Remove(category);
            config.Prototypes.Remove(category);
        }

        // Удаление отдельных прототипов
        foreach (var (category, protosToRemove) in toolComp.RemovePrototypes)
        {
            if (config.Prototypes.TryGetValue(category, out var list))
                list.RemoveAll(entry => protosToRemove.Contains(entry.PrototypeId));
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

            if (!EntityManager.TryGetComponent(testCoords.EntityId, out PhysicsComponent? physics) || !physics.CanCollide)
                return testCoords;
        }

        return origin;
    }
}

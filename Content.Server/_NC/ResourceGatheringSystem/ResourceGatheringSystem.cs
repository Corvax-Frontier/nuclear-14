using System.Numerics;
using Content.Server.Popups;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Content.Server._NC.AdvancedSpawner;
using Content.Shared._NC.ResourceGatheringSystem;

namespace Content.Server._NC.ResourceGatheringSystem;

public sealed class ResourceGatheringSystem : EntitySystem
{
    [Dependency] private readonly PopupSystem _popupSystem = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly AdvancedRandomSpawnerSystem _spawnerSystem = default!;

    private const float SearchRadius = 1.0f;
    private const int AngleStep = 30;
    private const int FullCircle = 360;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SharedResourceGatheringComponent, BeforeRangedInteractEvent>(OnUseTool);
        SubscribeLocalEvent<SharedResourceGatheringComponent, GatherResourceDoAfterEvent>(OnDoAfter);
    }

    private void OnUseTool(EntityUid uid, SharedResourceGatheringComponent comp, BeforeRangedInteractEvent args)
    {
        if (!args.CanReach)
            return;

        StartGathering(uid, comp, args.Target, args.User);
    }

    private void StartGathering(EntityUid uid, SharedResourceGatheringComponent comp, EntityUid? target, EntityUid user)
    {
        if (target == null || !TryComp<ResourceNode>(target.Value, out var node))
            return;

        if (!TryComp<SharedResourceToolComponent>(uid, out var toolComp))
        {
            _popupSystem.PopupEntity(
                Loc.GetString("nc-resource-tool-error"),
                user,
                PopupType.LargeCaution
            );
            return;
        }

        if (node.AllowedTools.Count > 0 && !node.AllowedTools.Contains(toolComp.ToolId))
        {
            _popupSystem.PopupEntity(
                Loc.GetString("nc-resource-tool-not-suitable"),
                user,
                PopupType.LargeCaution
            );
            return;
        }

        if (node.TimeBeforeNextGather > 0f)
        {
            _popupSystem.PopupEntity(
                Loc.GetString("nc-resource-empty"),
                user,
                PopupType.LargeCaution
            );
            return;
        }

        var doAfterArgs = new DoAfterArgs(
            EntityManager,
            user,
            TimeSpan.FromSeconds(comp.GatherTime),
            new GatherResourceDoAfterEvent(),
            uid,
            target: target.Value,
            used: uid
        )
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

        if (!TryComp<ResourceNode>(args.Args.Target.Value, out var node))
            return;

        if (string.IsNullOrEmpty(node.LootSpawner))
            return;

        var spawnCoords = FindFreePosition(args.Args.User);
        var spawnerUid = EntityManager.SpawnEntity(node.LootSpawner, spawnCoords);

        if (!TryComp<AdvancedRandomSpawnerComponent>(spawnerUid, out var spawner))
        {
            EntityManager.DeleteEntity(spawnerUid);
            return;
        }

        var config = new AdvancedRandomSpawnerConfig(spawner);

        ApplyNodeRichness(node, comp);
        ProcessToolEffects(uid, comp, config);
        config.ApplyModifiers(comp.WeightModifiers);

        node.TimeBeforeNextGather = node.CooldownAfterGather;

        _spawnerSystem.SpawnEntitiesUsingSpawner(spawnerUid, config);

        _popupSystem.PopupEntity(
            Loc.GetString("nc-resource-gather-success"),
            uid,
            PopupType.LargeCaution
        );
        args.Handled = true;
    }

    private void ProcessToolEffects(EntityUid uid, SharedResourceGatheringComponent comp, AdvancedRandomSpawnerConfig config)
    {
        if (!TryComp<SharedResourceToolComponent>(uid, out var toolComp))
            return;

        ApplyToolEffects(toolComp, comp, config);

        foreach (var (category, protoList) in toolComp.ExtraPrototypeIds)
        {
            foreach (var protoId in protoList)
            {
                config.AddPrototype(category, new SpawnEntry { PrototypeId = protoId });
            }
        }
    }

    private void ApplyNodeRichness(ResourceNode node, SharedResourceGatheringComponent comp)
    {
        var richnessModifier = node.ResourceRichness switch
        {
            ResourceRichness.Rich => 5,
            ResourceRichness.Poor => -3,
            _ => 0
        };

        comp.WeightModifiers["rare"] = comp.WeightModifiers.GetValueOrDefault("rare") + richnessModifier;
    }

    private void ApplyToolEffects(SharedResourceToolComponent toolComp, SharedResourceGatheringComponent comp, AdvancedRandomSpawnerConfig config)
    {
        foreach (var (category, modifier) in toolComp.WeightModifiers)
        {
            comp.WeightModifiers[category] = comp.WeightModifiers.GetValueOrDefault(category) + modifier;
        }

        foreach (var (category, weight) in toolComp.AddCategories)
        {
            comp.WeightModifiers.TryAdd(category, weight);
        }

        foreach (var category in toolComp.RemoveCategories)
        {
            config.CategoryWeights.Remove(category);
            config.Prototypes.Remove(category);
        }

        foreach (var (category, prototypesToRemove) in toolComp.RemovePrototypes)
        {
            if (config.Prototypes.TryGetValue(category, out var list))
                list.RemoveAll(entry => prototypesToRemove.Contains(entry.PrototypeId));
        }
    }

    private EntityCoordinates FindFreePosition(EntityUid user)
    {
        if (!EntityManager.TryGetComponent(user, out TransformComponent? userTransform))
            return new EntityCoordinates(user, Vector2.Zero);

        var origin = userTransform.Coordinates;

        for (var i = 0; i < FullCircle; i += AngleStep)
        {
            var angle = i * (float) (Math.PI / 180.0);
            var offset = new Vector2(MathF.Cos(angle) * SearchRadius, MathF.Sin(angle) * SearchRadius);
            var testCoords = new EntityCoordinates(origin.EntityId, origin.Position + offset);

            if (!EntityManager.TryGetComponent(testCoords.EntityId, out PhysicsComponent? physics) || !physics.CanCollide)
                return testCoords;
        }

        return origin;
    }
}

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
        {
            return;
        }

        TryStartGathering(uid, comp, args.Target, args.User);
    }

    private void TryStartGathering(EntityUid tool, SharedResourceGatheringComponent comp, EntityUid? target, EntityUid user)
    {
        if (!ValidateGathering(tool, target, user))
        {
            return;
        }

        var doAfterArgs = new DoAfterArgs(
            EntityManager, user, TimeSpan.FromSeconds(comp.GatherTime),
            new GatherResourceDoAfterEvent(), tool, target: target!.Value, used: tool)
        {
            BreakOnDamage = true,
            NeedHand = true,
            DistanceThreshold = 2f
        };

        _doAfterSystem.TryStartDoAfter(doAfterArgs);
    }

    private bool ValidateGathering(EntityUid tool, EntityUid? target, EntityUid user)
    {
        if (target == null || !TryComp<ResourceNode>(target.Value, out var node))
        {
            return false;
        }

        if (!TryComp<SharedResourceToolComponent>(tool, out var toolComp))
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

    private void OnDoAfter(EntityUid uid, SharedResourceGatheringComponent comp, GatherResourceDoAfterEvent args)
    {
        if (args.Handled || args.Cancelled || args.Args.Target == null)
        {
            return;
        }

        if (!TryComp<ResourceNode>(args.Args.Target.Value, out var node) || string.IsNullOrEmpty(node.LootSpawner))
        {
            return;
        }

        SpawnLoot(uid, comp, node, args.Args.User);
        ApplyNodeCooldown(node);

        _popupSystem.PopupEntity(Loc.GetString("nc-resource-gather-success"), uid, PopupType.LargeCaution);
        args.Handled = true;
    }

    private void SpawnLoot(EntityUid tool, SharedResourceGatheringComponent comp, ResourceNode node, EntityUid user)
    {
        var spawnCoords = FindFreePosition(user);
        var spawnerUid = EntityManager.SpawnEntity(node.LootSpawner, spawnCoords);

        if (!TryComp<AdvancedRandomSpawnerComponent>(spawnerUid, out var spawner))
        {
            EntityManager.DeleteEntity(spawnerUid);
            return;
        }

        var config = new AdvancedRandomSpawnerConfig(spawner);
        var toolComp = EnsureComp<SharedResourceToolComponent>(tool);
        var totalModifiers = CalculateTotalModifiers(node, comp, toolComp);

        ApplyToolPrototypeEffects(toolComp, config);
        config.ApplyModifiers(totalModifiers);

        _spawnerSystem.SpawnEntitiesUsingSpawner(spawnerUid, config);
    }

    private void ApplyNodeCooldown(ResourceNode node)
    {
        node.TimeBeforeNextGather = node.CooldownAfterGather;
    }

    private Dictionary<string, int> CalculateTotalModifiers(ResourceNode node, SharedResourceGatheringComponent comp, SharedResourceToolComponent toolComp)
    {
        var modifiers = new Dictionary<string, int>(comp.WeightModifiers);

        var richnessMod = node.ResourceRichness switch
        {
            ResourceRichness.Rich => 5,
            ResourceRichness.Poor => -3,
            _ => 0
        };

        if (richnessMod != 0)
        {
            modifiers["rare"] = modifiers.GetValueOrDefault("rare") + richnessMod;
        }

        foreach (var (category, mod) in toolComp.WeightModifiers)
        {
            modifiers[category] = modifiers.GetValueOrDefault(category) + mod;
        }

        foreach (var (category, weight) in toolComp.AddCategories)
        {
            modifiers.TryAdd(category, weight);
        }

        return modifiers;
    }

    private void ApplyToolPrototypeEffects(SharedResourceToolComponent toolComp, AdvancedRandomSpawnerConfig config)
    {
        foreach (var category in toolComp.RemoveCategories)
        {
            config.CategoryWeights.Remove(category);
            config.Prototypes.Remove(category);
        }

        foreach (var (category, toRemove) in toolComp.RemovePrototypes)
        {
            if (config.Prototypes.TryGetValue(category, out var list))
            {
                list.RemoveAll(entry => toRemove.Contains(entry.PrototypeId));
            }
        }

        foreach (var (category, protoList) in toolComp.ExtraPrototypeIds)
        {
            foreach (var protoId in protoList)
            {
                config.AddPrototype(category, new SpawnEntry { PrototypeId = protoId });
            }
        }
    }

    private EntityCoordinates FindFreePosition(EntityUid user)
    {
        if (!EntityManager.TryGetComponent(user, out TransformComponent? userTransform))
        {
            return new EntityCoordinates(user, Vector2.Zero);
        }

        var origin = userTransform.Coordinates;

        for (var i = 0; i < FullCircle; i += AngleStep)
        {
            var angle = i * (float)(Math.PI / 180.0);
            var offset = new Vector2(MathF.Cos(angle) * SearchRadius, MathF.Sin(angle) * SearchRadius);
            var testCoords = new EntityCoordinates(origin.EntityId, origin.Position + offset);

            if (!EntityManager.TryGetComponent(testCoords.EntityId, out PhysicsComponent? physics) || !physics.CanCollide)
            {
                return testCoords;
            }
        }

        return origin;
    }
}

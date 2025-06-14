using Content.Shared.Store;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server._NC.Trade;

/// <summary>
/// Условие для продажи, которое уничтожает один предмет заданного типа из инвентаря, рук или контейнеров.
/// </summary>
public sealed partial class SellConsumesItemCondition : ListingCondition
{
    [DataField("protoId", required: true, customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string ProtoId = null!;

    public override bool Condition(ListingConditionArgs args)
    {
        var entMan = args.EntityManager;
        var uid = args.Buyer;

        return TryConsumeFromHands(uid, entMan)
            || TryConsumeFromInventory(uid, entMan)
            || TryConsumeFromContainers(uid, entMan);
    }

    private bool TryConsumeFromHands(EntityUid uid, IEntityManager entMan)
    {
        if (!entMan.TryGetComponent(uid, out HandsComponent? hands))
            return false;

        var handsSys = entMan.System<SharedHandsSystem>();
        foreach (var held in handsSys.EnumerateHeld(uid, hands))
            if (IsTarget(held, entMan))
            {
                entMan.DeleteEntity(held);
                return true;
            }

        return false;
    }

    private bool TryConsumeFromInventory(EntityUid uid, IEntityManager entMan)
    {
        if (!entMan.TryGetComponent(uid, out InventoryComponent? inventory))
            return false;

        var invSys = entMan.System<InventorySystem>();
        foreach (var slot in inventory.Slots)
        {
            var slotId = slot.Name;

            if (invSys.TryGetSlotEntity(uid, slotId, out var item) && IsTarget(item.Value, entMan))
            {
                entMan.DeleteEntity(item.Value);
                return true;
            }
        }

        return false;
    }

    private bool TryConsumeFromContainers(EntityUid uid, IEntityManager entMan)
    {
        if (!entMan.TryGetComponent(uid, out ContainerManagerComponent? containers))
            return false;

        foreach (var container in containers.Containers.Values)
        {
            foreach (var ent in container.ContainedEntities)
                if (IsTarget(ent, entMan))
                {
                    entMan.DeleteEntity(ent);
                    return true;
                }
        }

        return false;
    }

    private bool IsTarget(EntityUid uid, IEntityManager entMan) =>
        entMan.TryGetComponent(uid, out MetaDataComponent? meta) &&
        meta.EntityPrototype?.ID == ProtoId;
}

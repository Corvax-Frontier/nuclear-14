using System.Linq;
using Content.Shared._NC.Trade;
using Content.Shared.Hands.EntitySystems;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Containers;
using Content.Server.Stack;
using Content.Shared.Stacks;

namespace Content.Server._NC.Trade;

public sealed class NcStoreLogicSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entMan = default!;
    [Dependency] private readonly IPrototypeManager _prototypes = default!;
    [Dependency] private readonly SharedTransformSystem _xform = default!;
    [Dependency] private readonly SharedHandsSystem _hands = default!;
    [Dependency] private readonly StackSystem _stack = default!;

    public int GetBalance(EntityUid user, string currencyId)
    {
        var total = 0;

        if (!_prototypes.TryIndex<NcCurrencyPrototype>(currencyId, out var currencyProto)
            || !_prototypes.TryIndex<EntityPrototype>(currencyProto.Entity, out var entityProto)
            || !entityProto.TryGetComponent(out StackComponent? protoStack, IoCManager.Resolve<IComponentFactory>()))
        {
            return 0;
        }

        var expectedStackType = protoStack.StackTypeId;

        foreach (var item in GetAllInventoryAndHands(user))
        {
            if (_entMan.TryGetComponent(item, out CurrencyItemComponent? currencyComp) && currencyComp.Currency == currencyId)
            {
                total += currencyComp.Amount;
            }
            else if (_entMan.TryGetComponent(item, out StackComponent? stack) && stack.StackTypeId == expectedStackType)
            {
                total += stack.Count;
            }
        }

        return total;
    }

    public bool TryPurchase(string listingId, EntityUid machine, NcStoreComponent? store, EntityUid user)
    {
        if (store == null || store.Listings.Count == 0)
            return false;

        var listing = store.Listings.FirstOrDefault(x => x.ID == listingId);
        if (listing == null || listing.Cost.Count == 0 || string.IsNullOrEmpty(listing.ProductEntity))
            return false;

        var price = (int) listing.Cost.First().Value;
        var isSell = price < 0;

        if (store.CurrencyWhitelist.Count == 0)
            return false;

        var currency = store.CurrencyWhitelist.First();

        if (!isSell)
        {
            if (GetBalance(user, currency) < price)
                return false;

            RemoveCurrency(user, currency, price);
            SpawnProduct(listing.ProductEntity, user);
            return true;
        }

        if (!RemoveItem(user, listing.ProductEntity))
            return false;

        AddCurrency(user, currency, -price);
        return true;
    }

    private void AddCurrency(EntityUid user, string currencyId, int amount)
    {
        if (amount <= 0)
            return;

        if (!_prototypes.TryIndex<NcCurrencyPrototype>(currencyId, out var currencyProto))
            return;

        var entityProtoId = currencyProto.Entity;
        var coords = Transform(user).Coordinates;

        if (_prototypes.TryIndex<EntityPrototype>(entityProtoId, out var proto) &&
            proto.TryGetComponent(out StackComponent? protoStack, IoCManager.Resolve<IComponentFactory>()) &&
            _prototypes.TryIndex<StackPrototype>(protoStack.StackTypeId, out var stackProto))
        {
            _stack.Spawn(amount, stackProto, coords);
            return;
        }

        for (int i = 0; i < amount; i++)
        {
            var ent = _entMan.SpawnEntity(entityProtoId, coords);
            _hands.PickupOrDrop(user, ent);
        }
    }

    private void RemoveCurrency(EntityUid user, string currencyId, int amount)
    {
        if (amount <= 0)
            return;

        if (!_prototypes.TryIndex<NcCurrencyPrototype>(currencyId, out var currencyProto)
            || !_prototypes.TryIndex<EntityPrototype>(currencyProto.Entity, out var proto)
            || !proto.TryGetComponent(out StackComponent? protoStack, IoCManager.Resolve<IComponentFactory>()))
        {
            return;
        }

        var expectedStackType = protoStack.StackTypeId;

        foreach (var item in GetAllInventoryAndHands(user))
        {
            if (amount <= 0)
                break;

            if (_entMan.TryGetComponent(item, out CurrencyItemComponent? currencyComp)
                && currencyComp.Currency == currencyId)
            {
                if (currencyComp.Amount <= amount)
                {
                    amount -= currencyComp.Amount;
                    _entMan.DeleteEntity(item);
                }
                else
                {
                    currencyComp.Amount -= amount;
                    amount = 0;
                }
            }
            else if (_entMan.TryGetComponent(item, out StackComponent? stack)
                && stack.StackTypeId == expectedStackType)
            {
                if (stack.Count <= amount)
                {
                    amount -= stack.Count;
                    _entMan.DeleteEntity(item);
                }
                else
                {
                    stack.Count -= amount;
                    amount = 0;
                }
            }
        }
    }

    private bool RemoveItem(EntityUid user, string protoId)
    {
        foreach (var item in GetAllInventoryAndHands(user))
        {
            if (_entMan.GetComponent<MetaDataComponent>(item).EntityPrototype?.ID == protoId)
            {
                _entMan.DeleteEntity(item);
                return true;
            }
        }

        return false;
    }

    private IEnumerable<EntityUid> GetAllInventoryAndHands(EntityUid user)
    {
        var result = new HashSet<EntityUid>();

        if (!_entMan.TryGetComponent(user, out ContainerManagerComponent? containerManager))
            return result;

        foreach (var container in containerManager.Containers.Values)
        {
            foreach (var entity in container.ContainedEntities)
                AddWithNestedContainers(entity, result);
        }

        return result;
    }

    private void AddWithNestedContainers(EntityUid entity, HashSet<EntityUid> result)
    {
        if (!result.Add(entity))
            return;

        if (!_entMan.TryGetComponent(entity, out ContainerManagerComponent? containerManager))
            return;

        foreach (var container in containerManager.Containers.Values)
        {
            foreach (var inner in container.ContainedEntities)
                AddWithNestedContainers(inner, result);
        }
    }

    private void SpawnProduct(string protoId, EntityUid user)
    {
        var coords = Transform(user).Coordinates;
        var ent = _entMan.SpawnEntity(protoId, coords);
        _hands.PickupOrDrop(user, ent);
    }
}

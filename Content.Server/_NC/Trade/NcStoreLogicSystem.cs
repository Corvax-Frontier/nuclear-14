using System.Linq;
using Content.Shared._NC.Trade;
using Content.Shared.Hands.EntitySystems;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Containers;


namespace Content.Server._NC.Trade;

public sealed class NcStoreLogicSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entMan = null!;
    [Dependency] private readonly IPrototypeManager _prototypes = null!;
    [Dependency] private readonly SharedTransformSystem _xform = null!;


    public int GetBalance(EntityUid user, string currencyId)
    {
        var total = 0;

        foreach (var item in GetAllInventoryAndHands(user))
        {
            if (!_entMan.TryGetComponent<CurrencyItemComponent>(item, out var currencyComp))
                continue;

            if (currencyComp.Currency != currencyId)
                continue;

            total += currencyComp.Amount;
        }

        return total;
    }

    public bool TryPurchase(string listingId, EntityUid machine, NcStoreComponent? store, EntityUid user)
    {
        if (store == null)
            return false;

        if (store.Listings.Count == 0)
            return false;

        var listing = store.Listings.FirstOrDefault(x => x.ID == listingId);
        if (listing == null || listing.Cost.Count == 0 || string.IsNullOrEmpty(listing.ProductEntity))
            return false;

        var price = (int) listing.Cost.First().Value;
        var isSell = price < 0;

        if (store.CurrencyWhitelist.Count == 0)
            return false;

        var currency = store.CurrencyWhitelist.First();

        if (!isSell) // Покупка
        {
            if (GetBalance(user, currency) < price)
                return false;

            RemoveCurrency(user, currency, price);
            SpawnProduct(listing.ProductEntity, _xform.GetMapCoordinates(machine));
            return true;
        }

        // Продажа
        if (!RemoveItem(user, listing.ProductEntity))
            return false;

        AddCurrency(user, currency, -price);
        return true;
    }

    private void AddCurrency(EntityUid user, string currency, int amount)
    {
        var proto = _prototypes.Index<NcCurrencyPrototype>(currency);

        for (var i = 0; i < amount; i++)
        {
            var ent = _entMan.SpawnEntity(proto.Entity, _xform.GetMapCoordinates(user));
            _entMan.System<SharedHandsSystem>().PickupOrDrop(user, ent);
        }
    }

    private void RemoveCurrency(EntityUid user, string currency, int amount)
    {
        foreach (var item in GetAllInventoryAndHands(user))
        {
            if (!_entMan.TryGetComponent<CurrencyItemComponent>(item, out var currencyComp))
                continue;

            if (currencyComp.Currency != currency)
                continue;

            if (amount <= 0)
                break;

            _entMan.DeleteEntity(item);
            amount -= currencyComp.Amount;
        }
    }

    private bool RemoveItem(EntityUid user, string protoId)
    {
        foreach (var item in GetAllInventoryAndHands(user))
            if (_entMan.GetComponent<MetaDataComponent>(item).EntityPrototype?.ID == protoId)
            {
                _entMan.DeleteEntity(item);
                return true;
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
                AddWithNestedContainers(inner, result); // рекурсия
        }
    }

    private void SpawnProduct(string protoId, MapCoordinates coords) => _entMan.SpawnEntity(protoId, coords);
}


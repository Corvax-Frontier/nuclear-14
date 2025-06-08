using Content.Shared._NC.Trade;
using Robust.Shared.Prototypes;

namespace Content.Server._NC.Trade;

public sealed class StoreSystemStructuredLoader : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypes = null!;

    public override void Initialize() => SubscribeLocalEvent<NcStoreComponent, MapInitEvent>(OnMapInit);

    private void OnMapInit(EntityUid uid, NcStoreComponent comp, MapInitEvent args)
    {
        if (!_prototypes.TryIndex<StorePresetStructuredPrototype>(comp.Preset, out var preset))
            return;

        comp.CurrencyWhitelist.Clear();
        comp.CurrencyWhitelist.Add(preset.Currency);

        comp.Categories.Clear();
        comp.Listings.Clear();

        foreach (var (mode, categories) in preset.Catalog)
        {
            foreach (var (category, entries) in categories)
            {
                if (!comp.Categories.Contains(category))
                    comp.Categories.Add(category);

                foreach (var entry in entries)
                {
                    var listing = new StoreListingPrototype
                    {
                        ID = $"{mode}_{category}_{entry.Proto}",
                        ProductEntity = entry.Price > 0 ? entry.Proto : null,
                        Name = entry.Name,
                        Description = entry.Description,
                        Icon = entry.Icon,
                        Cost = new() { [preset.Currency] = entry.Price, },
                        Categories = [category,],
                        Conditions = new()
                    };

                    if (entry.Price < 0)
                    {
                        listing.Conditions.Add(
                            new ListingConditionPrototype
                        {
                            Condition = new SellConsumesItemCondition { ProtoId = entry.Proto, }
                        });
                    }

                    comp.Listings.Add(listing);
                }
            }
        }
    }
}

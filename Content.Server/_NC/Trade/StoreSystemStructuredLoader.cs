using Content.Shared._NC.Trade;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._NC.Trade;

public sealed class StoreSystemStructuredLoader : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypes = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize() =>
        // Используем Startup вместо MapInit
        SubscribeLocalEvent<NcStoreComponent, ComponentStartup>(OnStartup);

    private void OnStartup(EntityUid uid, NcStoreComponent comp, ComponentStartup args)
    {
        if (string.IsNullOrWhiteSpace(comp.Preset))
        {
            Log.Warning($"[NcStore] Нет указанного прсета у магазина {ToPrettyString(uid)}");
            return;
        }

        if (!_prototypes.TryIndex<StorePresetStructuredPrototype>(comp.Preset, out var preset))
        {
            Log.Error($"[NcStore] Прсет '{comp.Preset}' не найден для магазина {ToPrettyString(uid)}");
            return;
        }

        Log.Info($"[NcStore] Загружается прсет '{comp.Preset}' для {ToPrettyString(uid)}");

        comp.CurrencyWhitelist.Clear();
        comp.CurrencyWhitelist.Add(preset.Currency);

        comp.Categories.Clear();
        comp.Listings.Clear();

        var total = 0;

        foreach (var (mode, categories) in preset.Catalog)
        {
            foreach (var (category, entries) in categories)
            {
                if (!comp.Categories.Contains(category))
                {
                    comp.Categories.Add(category);
                    Log.Debug($"[NcStore] Добавлена категория: {category}");
                }

                foreach (var entry in entries)
                {
                    var id = $"{mode}_{category}_{entry.Proto}_{_random.Next(100000)}";
                    var listing = new StoreListingPrototype
                    {
                        ID = id,
                        ProductEntity = entry.Proto,
                        Name = entry.Name,
                        Description = entry.Description,
                        Icon = entry.Icon,
                        Cost = new() { [preset.Currency] = entry.Price },
                        Categories = [category],
                        Conditions = new()
                    };

                    Log.Debug($"[NcStore] Добавление товара: {id}, цена: {entry.Price}");

                    if (entry.Price < 0)
                    {
                        listing.Conditions.Add(new ListingConditionPrototype
                        {
                            Condition = new SellConsumesItemCondition
                            {
                                ProtoId = entry.Proto
                            }
                        });

                        Log.Debug($"[NcStore] Добавлено условие продажи для {entry.Proto}");
                    }

                    comp.Listings.Add(listing);
                    total++;
                }
            }
        }

        Log.Info($"[NcStore] Загружено товаров: {total}");
    }
}

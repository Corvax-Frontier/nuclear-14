using Content.Shared.Store;


namespace Content.Shared._NC.Trade;

public sealed class ListingConditionPrototype
{
    [DataField("condition")]
    public ListingCondition Condition = null!;
}

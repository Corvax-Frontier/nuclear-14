ent-N14FurnitureWashingmachine = washing machine
    .desc = wishy washy.
ent-N14FurnitureWashingmachineIndustrial = { ent-N14FurnitureWashingmachine }
    .suffix = Industrial
    .desc = { ent-N14FurnitureWashingmachine.desc }
ent-N14ClosetSafe = safe
    .desc = Might be filled with valuables.
ent-N14LootClosetSafe = { ent-['N14ClosetSafe', 'N14BaseMilitaryStorageFill'] }

  .suffix = Loot, Random Military
  .desc = { ent-['N14ClosetSafe', 'N14BaseMilitaryStorageFill'].desc }
ent-N14LootClosetSafeCurrency = { ent-['N14ClosetSafe', 'N14StorageFillCurrencyFill'] }

  .suffix = Loot, Currency one stack random
  .desc = { ent-['N14ClosetSafe', 'N14StorageFillCurrencyFill'].desc }
ent-N14LootClosetSafePrewar = { ent-['N14ClosetSafe', 'N14StorageFillSafePrewarFill'] }

  .suffix = Loot, safe pre-war
  .desc = { ent-['N14ClosetSafe', 'N14StorageFillSafePrewarFill'].desc }
ent-N14ClosetSafeSpinner = { ent-N14ClosetSafe }
    .suffix = spinner
    .desc = { ent-N14ClosetSafe.desc }

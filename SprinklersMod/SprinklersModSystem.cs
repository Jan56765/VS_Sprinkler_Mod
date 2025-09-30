using System;
using Vintagestory.API.Common;
using SprinklersMod.BlockEntities;
using SprinklersMod.Blocks;

namespace SprinklersMod;

public class SprinklersModSystem : ModSystem
{

    public static ModConfig config;

    private const String CONFIG_LOCATION = "sprinklersConfig.json";

    public override void Start(ICoreAPI api)
    {

        loadConfig(api);

        base.Start(api);
        //api.RegisterBlockClass(Mod.Info.ModID + ".slidepole", typeof(BlockSlidepole));
        api.RegisterBlockClass(Mod.Info.ModID + ".sprinkler", typeof(BlockSprinkler));
        //api.RegisterBlockClass(Mod.Info.ModID + ".waterspreader", typeof(BlockWaterspreader));
        api.RegisterBlockEntityClass(Mod.Info.ModID + ".blockEntitySprinkler", typeof(BlockEntitySprinkler));
        //api.RegisterBlockEntityClass(Mod.Info.ModID + ".blockEntityWaterspreader", typeof(BlockEntityWaterspreader));
    }

    private void loadConfig(ICoreAPI api)
    {
        try
        {
            config = api.LoadModConfig<ModConfig>(CONFIG_LOCATION);
            if (config == null)
            {
                config = new ModConfig();
            }
            api.StoreModConfig<ModConfig>(config, CONFIG_LOCATION);
        }
        catch (Exception e)
        {
            Mod.Logger.Error("Could not load config! Loading default settings instead.");
            Mod.Logger.Error(e);
            config = new ModConfig();
        }

    }

}

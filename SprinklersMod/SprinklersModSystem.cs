using System;
using Vintagestory.API.Common;
using SprinklersMod.BlockEntities;
using SprinklersMod.Blocks;

namespace SprinklersMod;

public class SprinklersModSystem : ModSystem
{

    public static ModConfig config;

    private const string CONFIG_LOCATION = "sprinklersConfig.json";

    public override void Start(ICoreAPI api)
    {

        loadConfig(api);

        base.Start(api);
        api.RegisterBlockClass(Mod.Info.ModID + ".sprinkler", typeof(BlockSprinkler));
        api.RegisterBlockEntityClass(Mod.Info.ModID + ".blockEntitySprinkler", typeof(BlockEntitySprinkler));
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

using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;
using SprinklersMod.BlockEntities;
using Vintagestory.API.Client;
using Vintagestory.API.Util;
using System.Collections.Generic;
using System;

namespace SprinklersMod.Blocks
{
    internal class BlockSprinkler : Block
    {

        /*  This is different from the Water Consumption Variable of the config
        *   This one determines how much Watering Can Time is reduced each time
        *   the sprinkler gets filled */
        const float WATER_CONSUMPTION = 2.0f;

        WorldInteraction[] interactions;

        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);

            if (api.Side != EnumAppSide.Client) return;

            interactions = ObjectCacheUtil.GetOrCreate(api, "sprinklerInteractions", () =>
            {
                List<ItemStack> list = getApplicableContainers();

                return new WorldInteraction[] {
                    new WorldInteraction()
                    {
                        MouseButton = EnumMouseButton.Right,
                        ActionLangCode = "blockhelp-sprinkler-fill",
                        Itemstacks = list.ToArray()
                    },
                    new WorldInteraction()
                    {
                        MouseButton = EnumMouseButton.Right,
                        HotKeyCode = "shift",
                        ActionLangCode = "blockhelp-sprinkler-fill2",
                        Itemstacks = list.ToArray()
                    }
                };
            });
        }

        public override void OnBlockPlaced(IWorldAccessor world, BlockPos blockPos, ItemStack byItemStack = null)
        {
            base.OnBlockPlaced(world, blockPos, byItemStack);
        }

        public override void OnBlockBroken(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1)
        {
            base.OnBlockBroken(world, pos, byPlayer, dropQuantityMultiplier);
        }

        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
        {
            BlockEntitySprinkler blockEntitySprinkler = world.BlockAccessor.GetBlockEntity<BlockEntitySprinkler>(blockSel.Position);
            if (blockEntitySprinkler.getMissingWater() > 0)
            {
                int water = checkSelectedItem(byPlayer);
                if (water > 0 && api.Side == EnumAppSide.Server)
                {
                    playFillSound(byPlayer);
                    blockEntitySprinkler.fillWater(water);
                }
            }
            return true;

        }

        public override WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer)
        {
            return interactions.Append(base.GetPlacedBlockInteractionHelp(world, selection, forPlayer));
        }

        //
        // Summary: check the current itemstack for if it is a container
        //
        // Returns the amount of water left to be used
        //
        public int checkSelectedItem(IPlayer byPlayer)
        {
            if (byPlayer.InventoryManager.ActiveHotbarSlot == null || byPlayer.InventoryManager.ActiveHotbarSlot.Itemstack == null)
            {
                return 0;
            }
            var itemStack = byPlayer.InventoryManager.ActiveHotbarSlot.Itemstack;
            if (EnumItemClass.Block == itemStack.Class)
            {
                Block currentlyHeldBlock = itemStack.Block;
                if (currentlyHeldBlock is BlockWateringCan)
                {
                    BlockWateringCan bw = (BlockWateringCan)currentlyHeldBlock;
                    float content = bw.GetRemainingWateringSeconds(itemStack);
                    bool emptyAll = byPlayer.Entity.Controls.ShiftKey;
                    if (!emptyAll && content - WATER_CONSUMPTION > 0f)
                    {
                        bw.SetRemainingWateringSeconds(itemStack, bw.GetRemainingWateringSeconds(itemStack) - WATER_CONSUMPTION);
                        return 10;
                    } else if (emptyAll)
                    {
                        int initialStackSize = (int) bw.GetRemainingWateringSeconds(itemStack);
                        bw.SetRemainingWateringSeconds(itemStack, 0f);
                        return Math.Min(initialStackSize * 5, 150);
                    }
                }
                else if (currentlyHeldBlock is BlockLiquidContainerBase)
                {
                    //This should cover all containers able to hold water (Buckets, Jugs, Bowls etc.)
                    BlockLiquidContainerBase bl = (BlockLiquidContainerBase)currentlyHeldBlock;
                    if (bl.GetContent(itemStack) != null)
                    {
                        int mult = byPlayer.Entity.Controls.ShiftKey ? 10 : 1;
                        ItemStack contents = bl.GetContent(itemStack);
                        if ("game:waterportion".Equals(contents.Item.Code))
                        {
                            float maxConsumption = 1.0f * mult;
                            int initialStackSize = contents.StackSize;
                            bl.TryTakeLiquid(itemStack, 1.0f * maxConsumption);
                            return initialStackSize >= 100 * mult ? 10 * mult : initialStackSize / 100 * mult;
                        }
                    }
                }
            }
            return 0;
        }

        private List<ItemStack> getApplicableContainers()
        {
            List<ItemStack> list = new List<ItemStack>();

            string[] itemCodes = { "game:woodbucket", "game:jug-brown-fired", "game:bowl-red-fired", "game:wateringcan-red-fired" };
            foreach (string code in itemCodes)
            {
                Block b = api.World.GetBlock(new AssetLocation(code));
                if (b != null)
                {
                    ItemStack i = new ItemStack(b, 1);
                    list.Add(i);
                }

            }
            return list;
        }
        
        private void playFillSound(IPlayer player)
        {
            api.World.PlaySoundFor(AssetLocation.Create("sounds/effect/water-fill.ogg"), player, true, 3.0f, 0.6f);
        }
    }
}
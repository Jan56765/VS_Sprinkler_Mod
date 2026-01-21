using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;
using SprinklersMod.BlockEntities;
using Vintagestory.API.Client;
using Vintagestory.API.Util;
using System.Collections.Generic;
using System;

namespace SprinklersMod.Blocks {
    internal class BlockSprinkler : Block {

        /*  This is different from the Water Consumption Variable of the config
        *   This one determines how much Watering Can Time is reduced each time
        *   the sprinkler gets filled */
        const float WATERING_CAN_CONSUMPTION = 2.0f;

        //This is a list of all water types able to be used on a sprinkler to fill it
        //From the base game, only game:waterportion can be used. Other fluids will be declared afterwards
        readonly string[] VALID_WATER_TYPES = {
            "game:waterportion",
            "hydrateordiedrate:waterportion-fresh-rain-clean",
            "hydrateordiedrate:waterportion-fresh-well-clean"
        };

        WorldInteraction[] interactions;

        public override void OnLoaded(ICoreAPI api) {
            base.OnLoaded(api);

            if (api.Side != EnumAppSide.Client) return;

            string dir = Code.Path.Contains("down") ? "down" : "up";

            interactions = ObjectCacheUtil.GetOrCreate(api, "sprinklerInteractions" + dir, () => {
                List<ItemStack> list = getApplicableContainers();

                WorldInteraction[] interactions = {
                    new WorldInteraction() {
                        MouseButton = EnumMouseButton.Right,
                        ActionLangCode = "blockhelp-sprinkler-fill2",
                        Itemstacks = list.ToArray()
                    },
                    new WorldInteraction() {
                        MouseButton = EnumMouseButton.Right,
                        HotKeyCode = "ctrl",
                        ActionLangCode = "blockhelp-sprinkler-fill",
                        Itemstacks = list.ToArray()
                    }
                };

                if ("down".Equals(dir)) {
                    interactions = interactions.Append(
                        new WorldInteraction() {
                            MouseButton = EnumMouseButton.Right,
                            ActionLangCode = "blockhelp-sprinkler-set-range",
                            RequireFreeHand = true
                        }
                    );
                }

                return interactions;
            });
        }

        public override void OnBlockPlaced(IWorldAccessor world, BlockPos blockPos, ItemStack byItemStack = null) {
            base.OnBlockPlaced(world, blockPos, byItemStack);
        }

        public override void OnBlockBroken(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1) {
            base.OnBlockBroken(world, pos, byPlayer, dropQuantityMultiplier);
        }

        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel) {
            BlockEntitySprinkler blockEntitySprinkler = world.BlockAccessor.GetBlockEntity<BlockEntitySprinkler>(blockSel.Position);
            //Check if a downwards facing sprinkler is being interacted with by hand
            if (blockEntitySprinkler.isDownFacing && byPlayer.InventoryManager.ActiveHotbarSlot.Itemstack == null) {
                blockEntitySprinkler.adjustVertRange();
                return true;
            }
            //If not caught by the above block, resume normal behavior
            int missingWater = blockEntitySprinkler.getMissingWater();
            if (missingWater > 0) {
                int water = checkSelectedItem(byPlayer, missingWater);

                if (water > 0 && api.Side == EnumAppSide.Server) {
                    playFillSound(byPlayer);
                    blockEntitySprinkler.fillWater(water);
                }
            }
            return true;

        }

        public override WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer) {
            return interactions.Append(base.GetPlacedBlockInteractionHelp(world, selection, forPlayer));
        }

        //
        // Summary: check the current itemstack for if it is a container
        // 
        // Param byPlayer: required to access the players inventory to check for
        // Param missingWater: required to determine if the action to perform would even be required
        //
        // Returns the amount of water left to be used
        //
        public int checkSelectedItem(IPlayer byPlayer, int missingWater) {
            if (byPlayer.InventoryManager.ActiveHotbarSlot == null || byPlayer.InventoryManager.ActiveHotbarSlot.Itemstack == null) {
                return 0;
            }
            
            var itemStack = byPlayer.InventoryManager.ActiveHotbarSlot.Itemstack;
            if (EnumItemClass.Block == itemStack.Class) {
                Block currentlyHeldBlock = itemStack.Block;
                if (currentlyHeldBlock is BlockWateringCan) {
                    BlockWateringCan bw = (BlockWateringCan) currentlyHeldBlock;
                    float content = bw.GetRemainingWateringSeconds(itemStack);
                    bool emptyAll = byPlayer.Entity.Controls.ShiftKey;
                    if (!emptyAll && content - WATERING_CAN_CONSUMPTION > 0f) {
                        bw.SetRemainingWateringSeconds(itemStack, bw.GetRemainingWateringSeconds(itemStack) - WATERING_CAN_CONSUMPTION);
                        return 10;
                    } else if (emptyAll) {
                        int initialStackSize = (int) bw.GetRemainingWateringSeconds(itemStack);
                        bw.SetRemainingWateringSeconds(itemStack, 0f);
                        return Math.Min(initialStackSize * 5, 150);
                    }
                }
                else if (currentlyHeldBlock is BlockLiquidContainerBase) {
                    //This should cover all containers able to hold water (Buckets, Jugs, Bowls etc.)
                    BlockLiquidContainerBase bl = (BlockLiquidContainerBase) currentlyHeldBlock;
                    if (bl.GetContent(itemStack) != null) {
                        bool consumption = byPlayer.Entity.Controls.CtrlKey;
                        ItemStack contents = bl.GetContent(itemStack); //Stack Size in Buckets equals 1000 Units = 1 Liter so needs to be converted
                        if (VALID_WATER_TYPES.Contains(contents.Item.Code.ToString()) && contents.StackSize > 0) {
                            int liquidToTake = Math.Min(contents.StackSize / 10, Math.Min(consumption ? 10 : 100, missingWater)); //init as int, because otherwise we get floating point imprecision
                            if (liquidToTake > 0) {
                                bl.SplitStackAndPerformAction(byPlayer.Entity, byPlayer.InventoryManager.ActiveHotbarSlot, delegate (ItemStack stack) {
                                    bl.TryTakeLiquid(stack, ((float) liquidToTake) / 10f); //This method calculates with 1 Units being 1 Liter, so we need to convert to float
                                    return 1;
                                });
                            }
                            return liquidToTake;
                        }
                    }
                }
            }
            return 0;
        }

        private List<ItemStack> getApplicableContainers() {
            List<ItemStack> list = new List<ItemStack>();

            string[] itemCodes = { "game:woodbucket", "game:jug-brown-fired", "game:bowl-red-fired", "game:wateringcan-red-fired" };
            foreach (string code in itemCodes) {
                Block b = api.World.GetBlock(new AssetLocation(code));
                if (b != null) {
                    ItemStack i = new ItemStack(b, 1);
                    list.Add(i);
                }

            }
            return list;
        }
        
        private void playFillSound(IPlayer player) {
            api.World.PlaySoundFor(AssetLocation.Create("sounds/effect/water-fill.ogg"), player, true, 3.0f, 0.6f);
        }
    }
}
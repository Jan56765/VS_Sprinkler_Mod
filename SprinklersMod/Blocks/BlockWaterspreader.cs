//See comment in BlockEntityWaterspreader.cs
/* using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;
using SprinklersMod.BlockEntities;

namespace SprinklersMod.Blocks
{
    internal class BlockWaterspreader : Block
    {

        private const float WATER_CONSUMPTION = 1.0f;

        public override void OnBlockPlaced(IWorldAccessor world, BlockPos blockPos, ItemStack byItemStack = null)
        {
            api.Logger.Event("Waterspreader Placed!");
            base.OnBlockPlaced(world, blockPos, byItemStack);
        }

        public override void OnBlockBroken(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1)
        {
            api.Logger.Event("Waterspreader Broken!");
            base.OnBlockBroken(world, pos, byPlayer, dropQuantityMultiplier);
        }

        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
        {
            if (checkSelectedItem(byPlayer)) {
                bool wateredAtLeastOneBlock = false;
                List<BlockPos> blockList = new List<BlockPos>();
                int horizontalRange = 2;
                int verticalRange = 2;
                BlockPos bottomBlock = blockSel.Position.DownCopy(1);
                for (int i = -horizontalRange; i <= horizontalRange; i++) {
                    for (int k = -verticalRange; k <= verticalRange; k++) {
                        if (i == 0 && k == 0) {
                            //Don't care for the block directly beneath
                            continue;
                        }
                        blockList.Add(bottomBlock.NorthCopy(i).EastCopy(k));
                    }
                }
                float blockMoistures = 0.0f;
                float blockCount = 0.0f;
                foreach (BlockPos bPos in blockList) {
                    Block b = world.BlockAccessor.GetBlock(bPos);
                    if (b.GetType() == typeof(BlockFarmland)) {
                        BlockEntityFarmland be = world.BlockAccessor.GetBlockEntity<BlockEntityFarmland>(bPos);
                        blockCount++;
                        if (be.MoistureLevel < 0.8) {
                            be.WaterFarmland(0.1f, false);
                            wateredAtLeastOneBlock = true;
                            consumeWater(byPlayer);
                        }
                        blockMoistures += be.MoistureLevel;
                    }
                }
                if (wateredAtLeastOneBlock) {
                    //Stuff to subtract
                    api.Logger.Event("Least 1");
                }
                BlockEntityWaterspreader bw = world.BlockAccessor.GetBlockEntity<BlockEntityWaterspreader>(blockSel.Position);
                if (blockCount > 0) {
                    bw.avgWaterPercentage = blockMoistures / blockCount;
                } else {
                    bw.avgWaterPercentage = 0.0f;
                }
            }
            return true;
        }

        public bool checkSelectedItem(IPlayer byPlayer) {
            var itemStack = byPlayer.InventoryManager.ActiveHotbarSlot.Itemstack;
            if (EnumItemClass.Block == itemStack.Class) {
                Block currentlyHeldBlock = itemStack.Block;
                api.Logger.Event(currentlyHeldBlock.ToString());
                if (currentlyHeldBlock.GetType() == typeof(BlockWateringCan)) {
                    BlockWateringCan bw = (BlockWateringCan) currentlyHeldBlock;
                    float content = bw.GetRemainingWateringSeconds(itemStack);
                    api.Logger.Event("Before " + content.ToString());
                    if (content - WATER_CONSUMPTION > 0f) {
                        return true;
                    }
                } else if (currentlyHeldBlock.GetType() == typeof(BlockBucket)) {
                    BlockBucket bb = (BlockBucket) currentlyHeldBlock;
                }
            }
            return false;
        }

        public void consumeWater(IPlayer byPlayer) {
            var itemStack = byPlayer.InventoryManager.ActiveHotbarSlot.Itemstack;
            if (EnumItemClass.Block == itemStack.Class) {
                Block currentlyHeldBlock = itemStack.Block;
                api.Logger.Event(currentlyHeldBlock.ToString());
                if (currentlyHeldBlock.GetType() == typeof(BlockWateringCan)) {
                    BlockWateringCan bw = (BlockWateringCan) currentlyHeldBlock;
                    float content = bw.GetRemainingWateringSeconds(itemStack);
                    api.Logger.Event("Before " + content.ToString());
                    if (content - WATER_CONSUMPTION > 0f) {
                        bw.SetRemainingWateringSeconds(itemStack, content - WATER_CONSUMPTION);
                    }
                } else if (currentlyHeldBlock.GetType() == typeof(BlockBucket)) {
                    BlockBucket bb = (BlockBucket) currentlyHeldBlock;
                }
            }
        }
    }
} */
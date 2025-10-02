using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;
using SprinklersMod.BlockEntities;

namespace SprinklersMod.Blocks
{
    internal class BlockSprinkler : Block
    {

        /*  This is different from the Water Consumption Variable of the config
        *   This one determines how much Watering Can Time is reduced each time
        *   the sprinkler gets filled */
        const float WATER_CONSUMPTION = 2.0f;

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
            if (checkSelectedItem(byPlayer))
            {
                BlockEntitySprinkler blockEntitySprinkler = world.BlockAccessor.GetBlockEntity<BlockEntitySprinkler>(blockSel.Position);
                blockEntitySprinkler.fillWater();
            }
            return true;

        }

        public bool checkSelectedItem(IPlayer byPlayer)
        {
            if (byPlayer.InventoryManager.ActiveHotbarSlot == null || byPlayer.InventoryManager.ActiveHotbarSlot.Itemstack == null)
            {
                return false;
            }
            var itemStack = byPlayer.InventoryManager.ActiveHotbarSlot.Itemstack;
            if (EnumItemClass.Block == itemStack.Class)
            {
                Block currentlyHeldBlock = itemStack.Block;
                if (currentlyHeldBlock.GetType() == typeof(BlockWateringCan))
                {
                    BlockWateringCan bw = (BlockWateringCan) currentlyHeldBlock;
                    float content = bw.GetRemainingWateringSeconds(itemStack);
                    if (content - WATER_CONSUMPTION > 0f)
                    {
                        bw.SetRemainingWateringSeconds(itemStack, bw.GetRemainingWateringSeconds(itemStack) - WATER_CONSUMPTION);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
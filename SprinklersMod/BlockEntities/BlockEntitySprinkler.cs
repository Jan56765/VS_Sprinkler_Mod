using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace SprinklersMod.BlockEntities
{
    public class BlockEntitySprinkler : BlockEntity {

        public bool firstTick = true;
        //Water amount measured as int because of floating point nonsense (e.g. 7 = 0.7 Liters)
        public int waterAmount;
        public float avgWaterPercentage;

        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);
            //Create Random Tick Interval so that the sprinklers never execute all at once
            Random rng = new Random();
            int gameTickInterval = rng.Next(SprinklersModSystem.config.minIntervalInMillis, SprinklersModSystem.config.minIntervalInMillis + 5000);
            //Register Listener
            RegisterGameTickListener(OnGameTick, gameTickInterval);
        }

        private void OnGameTick(float dt)
        {
            //Only run after first tick to prevent world loading issues
            if (firstTick)
            {
                firstTick = false;
                return;
            }
            //Only run Script on actually filled sprinklers
            if (waterAmount >= SprinklersModSystem.config.waterConsumption)
            {
                bool wateredAtLeastOneBlock = false;

                //Define Sprinkler Range in Blocks. In its current form it will always be square
                //The "Diameter" will effectively be (2 * range + 1)
                int range = determineRange();

                //Find all blocks eligible for watering
                List<BlockPos> blockList = new List<BlockPos>();
                BlockPos bottomBlock = Pos.DownCopy(1);
                for (int i = -range; i <= range; i++)
                {
                    for (int k = -range; k <= range; k++)
                    {
                        if (i == 0 && k == 0)
                        {
                            //Don't care for the block directly beneath
                            continue;
                        }
                        blockList.Add(bottomBlock.NorthCopy(i).EastCopy(k));
                    }
                }
                float blockMoistures = 0.0f;
                float blockCount = 0.0f;
                //Water (or attempt to) each block that has been selected
                foreach (BlockPos bPos in blockList)
                {
                    Block b = Api.World.BlockAccessor.GetBlock(bPos);
                    if (b.GetType() == typeof(BlockFarmland))
                    {
                        BlockEntityFarmland be = Api.World.BlockAccessor.GetBlockEntity<BlockEntityFarmland>(bPos);
                        blockCount++;
                        if (be.MoistureLevel < 0.8)
                        {
                            be.WaterFarmland(0.1f, false);
                            wateredAtLeastOneBlock = true;
                        }
                        blockMoistures += be.MoistureLevel;
                    }
                }
                if (wateredAtLeastOneBlock)
                {
                    waterAmount -= SprinklersModSystem.config.waterConsumption;
                    avgWaterPercentage = blockMoistures / blockCount;
                }
            }
        }

        public void fillWater() {
            if (waterAmount + 10 > 200)
            {
                waterAmount = 200;
                return;
            }
            waterAmount += 10;
        }

        private int determineRange()
        {
            switch (Block.Code)
                {
                    case "sprinklersmod:tin_sprinkler": return SprinklersModSystem.config.tinSprinklerRange;
                    case "sprinklersmod:iron_sprinkler": return SprinklersModSystem.config.ironSprinklerRange;
                    case "sprinklersmod:steel_sprinkler": return SprinklersModSystem.config.steelSprinklerRange;
                    default: return 1;
                }
        }

        //Provide data to the Block Info Interface
        public override void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
        {
            dsc.Append("Water filled: " + ((float) waterAmount / 10).ToString("0.#") + " / 20 Liters\n");
            dsc.Append("Average Moisture: " + (avgWaterPercentage * 100).ToString("0.#") + "%");
            dsc.ToString();
        }

        //Caching to persist data between world loads
        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            tree.SetBool("firstTick", firstTick);
            tree.SetInt("waterAmount", waterAmount);
            tree.SetFloat("avgWaterPercentage", avgWaterPercentage);
        }

        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldForResolving)
        {
            base.FromTreeAttributes(tree, worldForResolving);
            firstTick = tree.GetBool("firstTick");
            waterAmount = tree.GetInt("waterAmount");
            avgWaterPercentage = tree.GetFloat("avgWaterPercentage");
        }

    }
}
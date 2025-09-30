//Unused for now. Its purpose is to be a cheaper manual click type of sprinkler which would automatically spread
//water in the correct amounts to the adjacent acres
/* using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace SprinklersMod.BlockEntities
{
    public class BlockEntityWaterspreader : BlockEntity {

        public float timer;
        public float avgWaterPercentage;

        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);
            RegisterGameTickListener(OnGameTick, 1000);
        }

        private void OnGameTick(float dt)
        {
            timer++;
            if (timer >= 10.0f) {
                timer = 0.0f;
                List<BlockPos> blockList = new List<BlockPos>();
                int horizontalRange = 2;
                int verticalRange = 2;
                BlockPos bottomBlock = Pos.DownCopy(1);
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
                    Block b = Api.World.BlockAccessor.GetBlock(bPos);
                    if (b.GetType() == typeof(BlockFarmland)) {
                        BlockEntityFarmland be = Api.World.BlockAccessor.GetBlockEntity<BlockEntityFarmland>(bPos);
                        blockCount++;
                        blockMoistures += be.MoistureLevel;
                    }
                }
                avgWaterPercentage = blockMoistures / blockCount;
            }
        }

        public override void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
        {
            dsc.Append("Average Moisture: " + (avgWaterPercentage * 100).ToString("0.#") + "%");
            dsc.ToString();
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            tree.SetFloat("timer", timer);
            tree.SetFloat("avgWaterPercentage", avgWaterPercentage);
        }

        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldForResolving)
        {
            base.FromTreeAttributes(tree, worldForResolving);
            timer = tree.GetFloat("timer");
            avgWaterPercentage = tree.GetFloat("avgWaterPercentage");
        }

    }
} */
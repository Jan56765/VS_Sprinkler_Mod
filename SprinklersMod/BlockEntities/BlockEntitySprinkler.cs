using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace SprinklersMod.BlockEntities {
    public class BlockEntitySprinkler : BlockEntity {

        BlockEntityAnimationUtil animUtil {
            get { return GetBehavior<BEBehaviorAnimatable>().animUtil; }
        }

        //Water amount measured as int because of floating point nonsense (e.g. 7 = 0.7 Liters)
        public int waterAmount;
        public int volume = 1;
        public float avgWaterPercentage;

        public bool isDownFacing = false;
        public int downwardsRange = 3;

        public bool overfillLatch = false; //This is used to create a latching behavior where sprinklers water until a threshold and only begin watering again until a lower threshold is met again. This way, a lot of water will be saved
        private const float LATCH_LOWER_LIMIT = 0.85f;
        private const float LATCH_UPPER_LIMIT = 0.95f;


        public override void Initialize(ICoreAPI api) {
            base.Initialize(api);

            //Safety net if server and client get out of sync with this block Entity
            //In the past this causes invisible sprinklers registered as an air block
            //to cause the animUtil to throw a NullPointer
            if (Block.IsMissing || !Block.Code.Path.StartsWith("t_")) {
                Api.World.BlockAccessor.RemoveBlockEntity(Pos);
                return; //Also DON'T register!!!
            }

            volume = determineStats(false);
            isDownFacing = Block.Code.Path.Contains("down");

            //Register Listener
            RegisterGameTickListener(OnGameTick, getRandomInterval());
        }

        private void OnGameTick(float dt) {
            bool dirty = false;
            //Only run Script on actually filled sprinklers
            if (waterAmount >= Math.Max(1, SprinklersModSystem.config.waterConsumption)) {
                bool wateredAtLeastOneBlock = false;

                //Define Sprinkler Range in Blocks. In its current form it will always be square
                //The "Diameter" will effectively be (2 * range + 1)
                int range = Math.Max(1, determineStats(true));

                //Find all blocks eligible for watering
                List<BlockPos> blockList = new List<BlockPos>();
                int scanningRangeVert = isDownFacing ? downwardsRange : 1;
                BlockPos bottomBlock = Pos.DownCopy(scanningRangeVert);
                for (int i = -range; i <= range; i++) {
                    for (int k = -range; k <= range; k++) {
                        if (!isDownFacing && i == 0 && k == 0) {
                            //Don't care for the block directly beneath
                            continue;
                        }
                        blockList.Add(bottomBlock.NorthCopy(i).EastCopy(k));
                    }
                }
                float blockMoistures = 0.0f;
                float blockCount = 0.0f;
                //Water (or attempt to) each block that has been selected
                foreach (BlockPos bPos in blockList) {
                    Block b = Api.World.BlockAccessor.GetBlock(bPos);
                    if (b.GetType() == typeof(BlockFarmland)) {
                        BlockEntityFarmland be = Api.World.BlockAccessor.GetBlockEntity<BlockEntityFarmland>(bPos);
                        blockCount++;
                        if (be.MoistureLevel < LATCH_UPPER_LIMIT && !overfillLatch) {
                            be.WaterFarmland(0.1f, false);
                            wateredAtLeastOneBlock = true;
                            if (Api.World.Side == EnumAppSide.Client) {
                                spawnParticles(bPos);
                            }
                            be.MarkDirty(true);
                        }
                        blockMoistures += be.MoistureLevel;
                    }
                }
                float newWaterPercentage = blockMoistures / blockCount;
                if (newWaterPercentage != avgWaterPercentage) {
                    avgWaterPercentage = newWaterPercentage;
                    
                    dirty = true;
                }
                if (wateredAtLeastOneBlock) {
                    if (Api.World.Side == EnumAppSide.Client) {
                        playSound();
                        runAnimation("sprinkler-turn");
                    }
                    waterAmount -= Math.Max(1, SprinklersModSystem.config.waterConsumption);
                    if (avgWaterPercentage >= LATCH_UPPER_LIMIT) {
                        overfillLatch = true;
                    }
                    dirty = true;
                }
                else if (overfillLatch && avgWaterPercentage < LATCH_LOWER_LIMIT) {
                    overfillLatch = false;
                    dirty = true;
                }
                if (dirty) {
                    MarkDirty();
                }
            }
        }

        //Provide data to the Block Info Interface
        public override void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc) {
            dsc.Append(
                string.Format("{0}: {1} / {2} {3}\n",
                T("waterfilled"), //Translation for Water Filled
                ((float) waterAmount / 10).ToString("0.#"), //Current Water Amount
                (volume / 10).ToString(), //Max Water Amount
                T("liters")) //Translation for Liters
            );
            dsc.Append(T("averagemoisture") + ": " + (avgWaterPercentage * 100).ToString("0.#") + "%\n");
            if (isDownFacing && downwardsRange > 0) {
                dsc.Append(T("range") + ": " + downwardsRange.ToString());
            }
            dsc.ToString();
        }

        //Caching to persist data between world loads
        public override void ToTreeAttributes(ITreeAttribute tree) {
            base.ToTreeAttributes(tree);
            tree.SetInt("waterAmount", waterAmount);
            tree.SetFloat("avgWaterPercentage", avgWaterPercentage);
            tree.SetInt("downwardsRange", downwardsRange);
            tree.SetBool("overfillLatch", overfillLatch);
        }

        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldForResolving) {
            base.FromTreeAttributes(tree, worldForResolving);
            waterAmount = tree.GetInt("waterAmount", 0);
            avgWaterPercentage = tree.GetFloat("avgWaterPercentage", 0.0f);
            downwardsRange = tree.GetInt("downwardsRange", 3);
            overfillLatch = tree.GetBool("overfillLatch", avgWaterPercentage >= 0.95);
        }

        public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tessThreadTesselator) {
        
            //I don't understand exactly why it's here, I just assume it's because this guarantees that the animator
            //will only be initialized on the client side (Example taken from the Riftward)
            //Also only works here and NOT in the Init function
            if (animUtil?.animator == null) {
                animUtil?.InitializeAnimator("sprinkler");
            }
            return base.OnTesselation(mesher, tessThreadTesselator);
        }

        public void fillWater(int byAmount) {
            if (waterAmount + byAmount > volume) {
                waterAmount = volume;
            } else {
                waterAmount += byAmount;
            }
            MarkDirty();
        }

        public int getMissingWater() {
            return volume - waterAmount;
        }

        public void adjustVertRange() {
            //Adjust range downwards only for down facing sprinklers
            if (downwardsRange >= 6) {
                downwardsRange = 3;
            } else {
                downwardsRange++;
            }
            MarkDirty();
        }
        
        //Translations
        private static string T(string code) {
            return Lang.Get("sprinklersmod:" + code);
        }

        private void spawnParticles(BlockPos p) {
            SimpleParticleProperties WaterParticles;
            if (!isDownFacing) {
                //This particle Spawner will calculate the distance from the sprinkler to the acre
                //and then yeet a sploosh of water in that direction
                WaterParticles = new SimpleParticleProperties(
                    /* minQuantity */ 10,
                    /* maxQuantity */ 15,
                    /* color */ ColorUtil.WhiteArgb,
                    /* minPos */ new Vec3d(Pos.X + 0.4f, Pos.Y + 0.3f, Pos.Z + 0.4f),
                    /* maxPos */ new Vec3d(Pos.X + 0.6f, Pos.Y + 0.5f, Pos.Z + 0.6f),
                    /* minVelocity */ new Vec3f(p.X -0.5f, p.Y + 5.8f, p.Z -0.5f).Sub(new Vec3f(Pos.X, Pos.Y, Pos.Z)),
                    /* maxVelocity */ new Vec3f(p.X + 0.5f, p.Y + 6.2f, p.Z + 0.5f).Sub(new Vec3f(Pos.X, Pos.Y, Pos.Z)),
                    /* lifeLength */ 1.5f,
                    /* gravity */ 1f,
                    /* minSize */ 0.8f,
                    /* maxSize */ 1.4f,
                    /* model */ EnumParticleModel.Cube
                );
            } else {
                //For the life of me, I couldn't figure the right trajectory out for downwards facing
                //As I'm not a huge math nerd this will have to do
                //Spawns litte water splooshs directly emitting from the acre
                WaterParticles = new SimpleParticleProperties(
                    /* minQuantity */ 15,
                    /* maxQuantity */ 20,
                    /* color */ ColorUtil.WhiteArgb,
                    /* minPos */ new Vec3d(p.X + 0.5, p.Y + 1.1f, p.Z + 0.5),
                    /* maxPos */ new Vec3d(p.X + 0.5, p.Y + 1.5f, p.Z + 0.5),
                    /* minVelocity */ new Vec3f(-1.2f, 2f, -1.2f),
                    /* maxVelocity */ new Vec3f(1.2f, 3.5f, 1.2f),
                    /* lifeLength */ 1f,
                    /* gravity */ 1f,
                    /* minSize */ 0.8f,
                    /* maxSize */ 1.4f,
                    /* model */ EnumParticleModel.Cube
                );
            }

            WaterParticles.SizeEvolve = new EvolvingNatFloat(EnumTransformFunction.LINEAR, -0.7f);
            WaterParticles.ClimateColorMap = "climateWaterTint";

            Api.World.SpawnParticles(WaterParticles, null);
        }

        private void runAnimation(string animation) {
            if (animUtil != null) {
                animUtil?.StartAnimation(new AnimationMetaData() { Animation = animation, Code = animation });
            }
        }

        private void playSound() {
            Api.World.PlaySoundAt(AssetLocation.Create("sounds/effect/watering"), Pos, 0, null, false, 12, 0.75f);
        }


        private int getRandomInterval() {
            //Might not be the randomest seed ever but good enough
            Random r = new Random(Pos.GetHashCode());
            int configInterval = Math.Max(1, SprinklersModSystem.config.minIntervalInMillis);
            return r.Next(configInterval, configInterval + 5000);
        }

        private int determineStats(bool range) {
            string prefix = Block.Code.Path.Substring(0, 5); //This feels a bit illegal, but hey, it's a unique identifier
            switch (prefix) {
                case "t_one": return range ? SprinklersModSystem.config.tOneSprinklerRange : SprinklersModSystem.config.tOneSprinklerVolume;
                case "t_two": return range ? SprinklersModSystem.config.tTwoSprinklerRange : SprinklersModSystem.config.tTwoSprinklerVolume;
                case "t_thr": return range ? SprinklersModSystem.config.tThreeSprinklerRange : SprinklersModSystem.config.tThreeSprinklerVolume;
                case "t_fou": return range ? SprinklersModSystem.config.tFourSprinklerRange : SprinklersModSystem.config.tFourSprinklerVolume;
                default: return 1;
            }
        }

    }
}
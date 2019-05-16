using System;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace legacyInfusion
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.seedlessbuns.legacyInfusion", "Legacy Infusion", "1.1.0")]
    public class legacyInfusion : BaseUnityPlugin
    {

        private static ConfigWrapper<int> capConfig { get; set; }    

        public static int cap
        {
            get => capConfig.Value;
            protected set => capConfig.Value = value;
        }



        public void Awake()
        {

            capConfig = Config.Wrap("Settings", "Health limit", "Sets the health limit per infusion (0 = no limit, minimum is 100)", 0);
            cap = capConfig.Value;
            //Chat.AddMessage("Cap: " + cap);

            On.RoR2.Inventory.AddInfusionBonus += (orig, self, value) =>
            {

                value *= (uint)self.GetItemCount(ItemIndex.Infusion);
                //Chat.AddMessage("InfusionBOnus: " + self.infusionBonus);

                orig(self, value);

            };

            On.RoR2.GlobalEventManager.OnCharacterDeath += (orig, self, damageReport) =>
            {
                DamageInfo damageInfo = damageReport.damageInfo;
                GameObject gameObject = damageReport.victim.gameObject;
                

                if (damageInfo.attacker)
                {
                    CharacterBody player = damageInfo.attacker.GetComponent<CharacterBody>();
                    if (player)
                    {
                        CharacterMaster master = player.master;
                        if(master)
                        {
                            Inventory inventory = master.inventory;
                            int infusionCount = inventory.GetItemCount(ItemIndex.Infusion);
                            if(infusionCount > 0)
                            {
                                int maxHp = infusionCount * 100;
                                if (cap != 0)
                                {
                                    if ((ulong)inventory.infusionBonus >= (ulong)((long)maxHp) && inventory.infusionBonus < infusionCount * cap)
                                    {
                                        RoR2.Orbs.InfusionOrb infusionOrb = new RoR2.Orbs.InfusionOrb();
                                        infusionOrb.origin = gameObject.transform.position;
                                        infusionOrb.target = Util.FindBodyMainHurtBox(player);
                                        infusionOrb.maxHpValue = 1;
                                        RoR2.Orbs.OrbManager.instance.AddOrb(infusionOrb);
                                        //Chat.AddMessage("not at cap " + cap + " yet. Have " + inventory.infusionBonus + " hp");

                                    } //if (inventory.infusionBonus >= infusionCount * cap) { Chat.AddMessage("at cap"); }
                                }
                                else
                                {

                                    if ((ulong)inventory.infusionBonus >= (ulong)((long)maxHp))
                                    {
                                        RoR2.Orbs.InfusionOrb infusionOrb = new RoR2.Orbs.InfusionOrb();
                                        infusionOrb.origin = gameObject.transform.position;
                                        infusionOrb.target = Util.FindBodyMainHurtBox(player);
                                        infusionOrb.maxHpValue = 1;
                                        RoR2.Orbs.OrbManager.instance.AddOrb(infusionOrb);
                                        //Chat.AddMessage("no cap: ");

                                    }
                                }
                            }
                        }
                    }
                }
                orig(self, damageReport);
            };

        }


        /*public void Update()
        {
            //This if statement checks if the player has currently pressed F2, and then proceeds into the statement:
            if (Input.GetKeyDown(KeyCode.F2))
            {
                //We grab a list of all available Tier 3 drops:
                var dropList = Run.instance.availableTier2DropList;

                //Randomly get the next item:
                var nextItem = Run.instance.treasureRng.RangeInt(0, dropList.Count);

                //Get the player body to use a position:
                var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

                //And then finally drop it infront of the player.
                PickupDropletController.CreatePickupDroplet(new RoR2.PickupIndex(ItemIndex.Infusion), transform.position, transform.forward * 20f);
            }
        }*/


    }
}

using System;
using BepInEx;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace legacyInfusion
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.seedlessbuns.legacyInfusion", "Legacy Infusion", "1.0.0")]
    public class MyModName : BaseUnityPlugin
    {
        public void Awake()
        {


            On.RoR2.Inventory.AddInfusionBonus += (orig, self, value) =>
            {

                value *= (uint)self.GetItemCount(ItemIndex.Infusion);


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
                                if((ulong)inventory.infusionBonus >= (ulong)((long)maxHp))
                                {
                                    RoR2.Orbs.InfusionOrb infusionOrb = new RoR2.Orbs.InfusionOrb();
                                    infusionOrb.origin = gameObject.transform.position;
                                    infusionOrb.target = Util.FindBodyMainHurtBox(player);
                                    infusionOrb.maxHpValue = 1;
                                    RoR2.Orbs.OrbManager.instance.AddOrb(infusionOrb);

                                }
                            }
                        }
                    }
                }
                orig(self, damageReport);
            };

        }

    }
}

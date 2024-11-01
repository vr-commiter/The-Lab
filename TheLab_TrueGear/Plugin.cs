using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;
using System;
using System.Threading;
using UnityEngine;
using Valve.VR.InteractionSystem;
using MyTrueGear;

namespace TheLab_TrueGear
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        private static bool canPull = true;
        private static bool canFire = true;
        private static bool lastState = false;
        private static TrueGearMod _TrueGear = null;

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
            HarmonyLib.Harmony.CreateAndPatchAll(typeof(Plugin));
            _TrueGear = new TrueGearMod();
            Logger.LogInfo($"New success ~~~~~~~~~~~!!!!!!!!!!");
            _TrueGear.Play("HeartBeat");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(VRHand), "AttachObject", new Type[] { typeof(GameObject), typeof(GrabTypes), typeof(VRHand.AttachmentFlags), typeof(Transform), typeof(string) })]
        private static void VRHand_AttachObject_Postfix(VRHand __instance)
        {
            if (__instance.handType == Valve.VR.SteamVR_Input_Sources.LeftHand)
            {
                Logger.LogInfo("-------------------------------------");
                Logger.LogInfo("LeftHandPickupItem");
                _TrueGear.Play("LeftHandPickupItem");
            }
            else
            {
                Logger.LogInfo("-------------------------------------");
                Logger.LogInfo("RightHandPickupItem");
                _TrueGear.Play("RightHandPickupItem");
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Longbow), "ArrowReleased")]
        private static void Longbow_ArrowReleased_Postfix(Longbow __instance)
        {
            if (__instance.hand.handType == Valve.VR.SteamVR_Input_Sources.LeftHand)
            {
                Logger.LogInfo("-------------------------------------");
                Logger.LogInfo("LeftHandLongbowArrowReleased");
                _TrueGear.Play("LeftHandLongbowArrowReleased");
            }
            else
            {
                Logger.LogInfo("-------------------------------------");
                Logger.LogInfo("RightHandLongbowArrowReleased");
                _TrueGear.Play("RightHandLongbowArrowReleased");
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Longbow), "SetAnimPullPosition")]
        private static void Longbow_SetAnimPullPosition_Postfix(Longbow __instance, float t)
        {
            if (t <= 0f || !canPull)
            {
                return;
            }
            canPull = false;
            Timer pullTimer = new Timer(PullTimerCallBack,null,100,Timeout.Infinite);

            int num = (int)Math.Floor(t * 5 + 1);
            if (num >= 5) num = 5;

            if (__instance.hand.handType == Valve.VR.SteamVR_Input_Sources.LeftHand)
            {
                Logger.LogInfo("-------------------------------------");
                Logger.LogInfo("LeftHandLongbowPull" + num);
                _TrueGear.Play("LeftHandLongbowPull" + num);
            }
            else
            {
                Logger.LogInfo("-------------------------------------");
                Logger.LogInfo("RightHandLongbowPull" + num);
                _TrueGear.Play("RightHandLongbowPull" + num);
            }
            Logger.LogInfo(t);
        }

        private static void PullTimerCallBack(object o)
        {
            canPull = true;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(Slingshot), "Update")]
        private static void Slingshot_Update_Prefix(Slingshot __instance)
        {
            
            if (__instance.slingshotPocket.holdingHand)
            {
                if (__instance.releaseVelocity.magnitude <= 0f || !canPull)
                {
                    return;
                }
                canPull = false;
                Timer pullTimer = new Timer(PullTimerCallBack, null, 100, Timeout.Infinite);
                int num = (int)Math.Floor(__instance.releaseVelocity.magnitude / 10 + 1);
                if (num >= 5) num = 5;
                if (__instance.slingshotPocket.holdingHand.handType == Valve.VR.SteamVR_Input_Sources.LeftHand)
                {
                    Logger.LogInfo("-------------------------------------");
                    Logger.LogInfo("LeftHandSlingshotPull" + num);
                    _TrueGear.Play("LeftHandSlingshotPull" + num);

                }
                else
                {
                    Logger.LogInfo("-------------------------------------");
                    Logger.LogInfo("RightHandSlingshotPull" + num);
                    _TrueGear.Play("RightHandSlingshotPull" + num);
                }
                Logger.LogInfo(__instance.releaseVelocity.magnitude);
                Logger.LogInfo(__instance.slingshotPocket.holdingHand.handType);
            }
            else
            {
                if (__instance.startedPull)
                {
                    Logger.LogInfo("-------------------------------------");
                    Logger.LogInfo("SlingshotReleased");
                    _TrueGear.Play("SlingshotReleased");
                }
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(VRHandCollider), "OnCollisionEnter")]
        private static void VRHandCollider_OnCollisionEnter_Prefix(VRHandCollider __instance, Collision collision)
        {
            float magnitude = collision.relativeVelocity.magnitude;
            if (magnitude > 0.1f && Time.time - __instance.lastCollisionHapticsTime > 0.2f)
            {
                if (collision.gameObject.name.Contains("Cube") || collision.gameObject.name.Contains("Collider") || collision.gameObject.name.Contains("floor"))
                {
                    return;
                }
                if (__instance.hand.hand.handType == Valve.VR.SteamVR_Input_Sources.LeftHand)
                {
                    Logger.LogInfo("-------------------------------------");
                    Logger.LogInfo("LeftHandOnCollisionEnter");
                    _TrueGear.Play("LeftHandPickupItem");
                }
                else
                {
                    Logger.LogInfo("-------------------------------------");
                    Logger.LogInfo("RightHandOnCollisionEnter");
                    _TrueGear.Play("RightHandPickupItem");
                }
                Logger.LogInfo(__instance.hand.hand.handType);
                Logger.LogInfo(collision.gameObject.name);
            }            
        }

        [HarmonyPrefix, HarmonyPatch(typeof(PlayerVRC), "AttemptToFireGun")]
        private static void PlayerVRC_AttemptToFireGun_Prefix(PlayerVRC __instance, VRHand hand)
        {
            if (!__instance.IsAttachedToHand)
            {
                return;
            }
            float num = __instance.overdriveEnabled ? (__instance.LaserFireInterval / (float)__instance.OverdriveDamageModifier) : __instance.LaserFireInterval;
            if (Time.time < __instance.lastGunFireTime + num)
            {
                return;
            }
            Transform transform = __instance.GunFirePoints[__instance.nextGunFirePointIndex];
            EnemyVRC enemyVRC = null;
            float num2 = float.PositiveInfinity;
            foreach (EnemyVRC enemyVRC2 in __instance.autoaimEnemies.Keys)
            {
                Vector3 vector = enemyVRC2.transform.position - transform.position;
                if (enemyVRC2.Targetable && vector.magnitude < num2)
                {
                    enemyVRC = enemyVRC2;
                    num2 = vector.magnitude;
                }
            }
            if (enemyVRC != null && enemyVRC.DisableLaserShooting)
            {
                enemyVRC = null;
            }
            if (!canFire) 
            {
                return;                
            }
            canFire = false;
            Timer fireTimer = new Timer(FireTimerCallBack,null,100,Timeout.Infinite);
            if (__instance.overdriveEnabled && !lastState)
            {
                lastState = true;
                if (hand.handType == Valve.VR.SteamVR_Input_Sources.LeftHand)
                {
                    Logger.LogInfo("-------------------------------------");
                    Logger.LogInfo("StartLeftHandLaszer");
                    _TrueGear.StartLeftHandLaszer();
                }
                else
                {
                    Logger.LogInfo("-------------------------------------");
                    Logger.LogInfo("StartRightHandLaszer");
                    _TrueGear.StartRightHandLaszer();
                }
            }
            else if(!__instance.overdriveEnabled && lastState)
            {
                lastState = false;
                if (hand.handType == Valve.VR.SteamVR_Input_Sources.LeftHand)
                {
                    Logger.LogInfo("-------------------------------------");
                    Logger.LogInfo("StopLeftHandLaszer");
                    _TrueGear.StopLeftHandLaszer();
                }
                else
                {
                    Logger.LogInfo("-------------------------------------");
                    Logger.LogInfo("StopRightHandLaszer");
                    _TrueGear.StopRightHandLaszer();
                }
            }
            
            if (enemyVRC != null && !lastState)
            {
                if (hand.handType == Valve.VR.SteamVR_Input_Sources.LeftHand)
                {
                    Logger.LogInfo("-------------------------------------");
                    Logger.LogInfo("LeftHandRifleShoot");
                    _TrueGear.Play("LeftHandRifleShoot");
                }
                else
                {
                    Logger.LogInfo("-------------------------------------");
                    Logger.LogInfo("RightHandRifleShoot");
                    _TrueGear.Play("RightHandRifleShoot");
                }
            }
        }

        private static void FireTimerCallBack(object o)
        {
            canFire = true;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(PlayerVRC), "SelfDestruct")]
        private static void PlayerVRC_SelfDestruct_Prefix(PlayerVRC __instance)
        {
            Logger.LogInfo("-------------------------------------");
            Logger.LogInfo("PlayerDeath");
            _TrueGear.Play("PlayerDeath");
            _TrueGear.StopLeftHandLaszer();
            _TrueGear.StopRightHandLaszer();
            _TrueGear.StopLeftHandNewBalloon();
            _TrueGear.StopRightHandNewBalloon();
        }

        //[HarmonyPrefix, HarmonyPatch(typeof(PlayerVRC), "OnEnemyDestruction")]
        //private static void PlayerVRC_OnEnemyDestruction_Prefix(PlayerVRC __instance, EnemyVRC enemy)
        //{
        //    Logger.LogInfo("-------------------------------------");
        //    Logger.LogInfo("OnEnemyDestruction");
        //    Logger.LogInfo(enemy.gameObject.name);
        //}

        [HarmonyPrefix, HarmonyPatch(typeof(BalloonTool), "StartNewBalloon")]
        private static void BalloonTool_StartNewBalloon_Prefix(BalloonTool __instance)
        {
            if (__instance.hand.handType == Valve.VR.SteamVR_Input_Sources.LeftHand)
            {
                Logger.LogInfo("-------------------------------------");
                Logger.LogInfo("LeftHandStartNewBalloon");
                _TrueGear.StartLeftHandNewBalloon();
            }
            else
            {
                Logger.LogInfo("-------------------------------------");
                Logger.LogInfo("RightHandStartNewBalloon");
                _TrueGear.StartRightHandNewBalloon();
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(BalloonTool), "ReleaseCurrentBalloon")]
        private static void BalloonTool_ReleaseCurrentBalloon_Prefix(BalloonTool __instance)
        {
            if (__instance.currentBalloon == null)
            {
                return;
            }
            Logger.LogInfo("-------------------------------------");
            if (__instance.hubAnnouncer != null)
            {
                if (__instance.totalAirFlow >= 1f)
                {
                    if (__instance.hand.handType == Valve.VR.SteamVR_Input_Sources.LeftHand)
                    {
                        Logger.LogInfo("-------------------------------------");
                        Logger.LogInfo("LeftHandBalloonBOOM");
                        _TrueGear.Play("LeftHandBalloonBOOM");

                    }
                    else
                    {
                        Logger.LogInfo("-------------------------------------");
                        Logger.LogInfo("RightHandBalloonBOOM");
                        _TrueGear.Play("RightHandBalloonBOOM");
                    }
                }
            }
            if (__instance.hand.handType == Valve.VR.SteamVR_Input_Sources.LeftHand)
            {
                Logger.LogInfo("-------------------------------------");
                Logger.LogInfo("LeftHandStopNewBalloon");
                _TrueGear.StopLeftHandNewBalloon();
            }
            else
            {
                Logger.LogInfo("-------------------------------------");
                Logger.LogInfo("RightHandStopNewBalloon");
                _TrueGear.StopRightHandNewBalloon();
            }
        }



    }
}

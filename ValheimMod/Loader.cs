using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

namespace ValheimMod
{
    public class Loader
    {
        

        public static void Init()
        {
            _Load = new GameObject();
            _Load.AddComponent<Main>();
            GameObject.DontDestroyOnLoad(_Load);


            //Harmony.DEBUG = true;
            //var h = new Harmony("vmp");

            
            //h.PatchAll();
            
            
        }
        public static void Unload()
        {
            _Unload();
        }
        private static void _Unload()
        {
            GameObject.Destroy(_Load);
        }
        public static GameObject _Load;


    }

    [HarmonyPatch(typeof(WaterVolume), "CreateWave")]
    public static class WaterVolume_CreateWave_Patch
    {
        public static bool Prefix(
    Vector3 worldPos,
    float time,
    ref float waveSpeed,
    ref float waveLength,
    ref float waveHeight,
    Vector2 dir2d,
    ref float sharpness)
        {
            try
            {
                waveSpeed += 50f;
                waveHeight += 50f;
                waveLength += 50f;
                sharpness += 50f;
            }
            catch (Exception e)
            {
                //mod.Logger.Error(e.ToString());

            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Player), "FixedUpdate")]
    public static class Pl_FU_Patch
    {
        public static int count = 0;

        public static bool Prefix()
        {
            count++;

            try
            {
                Main._player.Message(MessageHud.MessageType.TopLeft, $"You ran patch Pl_fixedupdate {count} times!", 0, (Sprite)null);
            }
            catch (Exception e)
            {
                //mod.Logger.Error(e.ToString());

            }

            return true;
        }
    }

}
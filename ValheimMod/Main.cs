using BepInEx;
using HarmonyLib;
using IniParser;
using IniParser.Parser;
using IniParser.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using System.Diagnostics;

namespace ValheimMod
{
    [BepInPlugin("org.bepinex.plugins.vmu", "vmu", "1.0.0.0")]
    class Main : BaseUnityPlugin
    {
        public static FileIniDataParser parser = new FileIniDataParser();

        public static Player _player;
        public string pln;
        public float otime;

        //Harmony h;

        public Main M;

        string path;
        string filename;
        string errorfile;

        public static Dictionary<string, int> oms = new Dictionary<string, int>();

        public static bool meaw = false;

        public static bool shouldInit = true;

        public static List<Skill> skills = new List<Skill>();

        public string[] snames = { "Weight", "Agility", "Sailing", "Crafting", "Building" };

        /*[DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();*/

        public void Awake()
        {
            UnityEngine.Debug.LogWarning("UVO Loading!");

            var h = new Harmony("unidarkshin_vm");

            //Type[] types1 = { };

            /*h.Patch(
original: AccessTools.Method(typeof(Player), "FixedUpdate"),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.PFU), types1)
);*/
            h.Patch(
original: AccessTools.Method(typeof(WaterVolume), "GetWaterLevel"),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.GWL))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.CW2))
);

            h.Patch(
original: AccessTools.Method(typeof(Destructible), "RPC_Damage"),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.DDM))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.CW2))
);

            h.Patch(
original: AccessTools.Method(typeof(InventoryGui), "DoCrafting"),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.DC))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.CW2))
);

            Type[] aitypes = { typeof(ItemDrop.ItemData) };

            h.Patch(
original: AccessTools.Method(typeof(Inventory), "AddItem", aitypes),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.AI))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.CW2))
);

            h.Patch(
original: AccessTools.Method(typeof(Inventory), "Save"),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.ISV))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.CW2))
);

            /*h.Patch(
original: AccessTools.Method(typeof(Inventory), "Load"),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.ILD))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.CW2))
);*/

            h.Patch(
original: AccessTools.Method(typeof(Inventory), "Load"),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.ILD)),
postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);

                        h.Patch(
            original: AccessTools.Method(typeof(ItemDrop), "DropItem"),
            postfix: new HarmonyMethod(typeof(Main), nameof(Main.IDI))
            //postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
            );

            h.Patch(
original: AccessTools.Method(typeof(ItemDrop), "LoadFromZDO"),
postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILZDO))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);
            h.Patch(
original: AccessTools.Method(typeof(ItemDrop), "SaveToZDO"),
postfix: new HarmonyMethod(typeof(Main), nameof(Main.ISZDO))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);

            h.Patch(
original: AccessTools.Method(typeof(CharacterDrop), "GenerateDropList"),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.CGDL))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);

            h.Patch(
original: AccessTools.Method(typeof(Player), "UpdatePlacementGhost"),
postfix: new HarmonyMethod(typeof(Main), nameof(Main.PUPG))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);

            h.Patch(
original: AccessTools.Method(typeof(Player), "HaveRequirements", new Type[] { typeof(Piece), typeof(Player.RequirementMode)}),
postfix: new HarmonyMethod(typeof(Main), nameof(Main.PHR))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);

            h.Patch(
original: AccessTools.Method(typeof(Inventory), "GetAllItems", new Type[] { typeof(string), typeof(List<ItemDrop.ItemData>) }),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.IGAI))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);

            h.Patch(
original: AccessTools.Method(typeof(WearNTear), "OnPlaced", new Type[] {}),
postfix: new HarmonyMethod(typeof(Main), nameof(Main.WOP))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);

            h.Patch(
original: AccessTools.Method(typeof(WearNTear), "GetMaterialProperties"),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.WGMP))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);

            h.Patch(
original: AccessTools.Method(typeof(Player), "CheckCanRemovePiece", new Type[] { typeof(Piece) }),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.PCCRP))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);

            h.Patch(
original: AccessTools.Method(typeof(Inventory), "RemoveItem", new Type[] { typeof(ItemDrop.ItemData) }),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.IRI))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);

            //ZNet.instance.m_serverPlayerLimit = 99;
        }

        public static ItemDrop.ItemData cupgitem = null;

        public static void IRI(ItemDrop.ItemData item)
        {
            if (iscrafting && item.m_shared.m_name.Contains(" (UVO"))
                cupgitem = item;
        }

            public static bool IGAI(ref Inventory __instance, string name, ref List<ItemDrop.ItemData> items)
        {
            try
            {
                foreach (ItemDrop.ItemData itemData in __instance.GetAllItems())
                {
                    if (!itemData.m_shared.m_name.Contains(" (UVO"))
                    {
                        if (itemData.m_shared.m_name == name)
                            items.Add(itemData);
                    }
                    else
                    {
                        int i = itemData.m_shared.m_name.IndexOf(" (UVO");
                        string str = itemData.m_shared.m_name.Substring(0, i);

                        if (str == name)
                            items.Add(itemData);
                            
                    }
                }

                return false;
            }
            catch (Exception ex)
            {

                return true;
            }
        }

        public static bool ICI(ref Inventory __instance, ref int __result, string name)
        {
            try
            {


                int num = 0;
                foreach (ItemDrop.ItemData itemData in __instance.GetAllItems())
                {
                    if (!itemData.m_shared.m_name.Contains(" (UVO"))
                    {
                        if (itemData.m_shared.m_name == name)
                            num += itemData.m_stack;
                    }
                    else
                    {
                        int i = itemData.m_shared.m_name.IndexOf(" (UVO");
                        string str = itemData.m_shared.m_name.Substring(0, i);
                        if (str == name)
                            num += itemData.m_stack;

                        UnityEngine.Debug.LogWarning($"ICI output: {str}, {name}");
                    }
                }

                __result = num;

                return false;

            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning("ICI failed: " + ex.ToString());
                return true;
            }
        }

        public static bool PCCRP(ref bool __result)
        {
            try
            {
                __result = true;

                return false;
            }
            catch (Exception ex)
            {

                UnityEngine.Debug.LogWarning("PCCRP failed: " + ex.ToString());

                return true;
            }
        }

            public static bool WGMP(ref WearNTear __instance,
  out float maxSupport,
  out float minSupport,
  out float horizontalLoss,
  out float verticalLoss)
        {

            maxSupport = 0.0f;
            minSupport = 0.0f;
            verticalLoss = 0.0f;
            horizontalLoss = 0.0f;

            try
            {
                Skill b = skills.Where(sk => sk.name.ToLower() == "building").FirstOrDefault();

                if (b == null)
                    return true;

                switch (__instance.m_materialType)
                {
                    case WearNTear.MaterialType.Wood:
                        maxSupport = 100f * (1.0f + (b.level * 0.01f));
                        minSupport = 10f * (1.0f + (b.level * 0.01f));
                        verticalLoss = 0.125f / (1.0f + (b.level * 0.01f));
                        horizontalLoss = 0.2f / (1.0f + (b.level * 0.01f));
                        break;
                    case WearNTear.MaterialType.Stone:
                        maxSupport = 1000f * (1.0f + (b.level * 0.01f));
                        minSupport = 100f * (1.0f + (b.level * 0.01f));
                        verticalLoss = 0.125f / (1.0f + (b.level * 0.012f));
                        horizontalLoss = 1f / (1.0f + (b.level * 0.01f));
                        break;
                    case WearNTear.MaterialType.Iron:
                        maxSupport = 1500f * (1.0f + (b.level * 0.01f));
                        minSupport = 20f * (1.0f + (b.level * 0.01f));
                        verticalLoss = 0.07692308f / (1.0f + (b.level * 0.01f));
                        horizontalLoss = 0.07692308f / (1.0f + (b.level * 0.01f));
                        break;
                    case WearNTear.MaterialType.HardWood:
                        maxSupport = 140f * (1.0f + (b.level * 0.01f));
                        minSupport = 10f * (1.0f + (b.level * 0.01f));
                        verticalLoss = 0.1f / (1.0f + (b.level * 0.01f));
                        horizontalLoss = 0.1666667f / (1.0f + (b.level * 0.01f));
                        break;
                    default:
                        maxSupport = 0.0f;
                        minSupport = 0.0f;
                        verticalLoss = 0.0f;
                        horizontalLoss = 0.0f;
                        break;
                }
                return false;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning("WGMP failed: " + ex.ToString());


                return true;
            }
        }

        public static void WOP(ref WearNTear __instance)
        {
            try
            {

                Skill b = skills.Where(sk => sk.name.ToLower() == "building").FirstOrDefault();
                Piece p = __instance.GetComponent<Piece>();

                int reqttl = 1;

                foreach (Piece.Requirement pr in p.m_resources)
                {
                    reqttl += pr.m_amount;
                }

                int xp = 1 + ((1 * ((int)__instance.m_materialType) + 1) * (1 + (int)(reqttl / 5)) * (1 + (int)(b.level / 20)));

                b.xp = b.xp + xp;

            }
            catch (Exception ex)
            {

                UnityEngine.Debug.LogWarning("WOP failed: " + ex.ToString());
            }
        }

        public static void PHR(ref Player __instance, ref bool __result, ref Piece piece, ref Player.RequirementMode mode)
        {
            try
            {
                bool cb = true;
                HashSet<string> mkm = typeof(Player).GetField("m_knownMaterial", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) as HashSet<string>;
                foreach (Piece.Requirement resource in piece.m_resources)
                {
                    if ((bool)(UnityEngine.Object)resource.m_resItem && resource.m_amount > 0)
                    {
                        switch (mode)
                        {
                            case Player.RequirementMode.CanBuild:
                                if (__instance.GetInventory().CountItems(resource.m_resItem.m_itemData.m_shared.m_name) < resource.m_amount)
                                    cb = false;
                                continue;
                            case Player.RequirementMode.IsKnown:
                                if (!mkm.Contains(resource.m_resItem.m_itemData.m_shared.m_name))
                                    cb = false;
                                continue;
                            case Player.RequirementMode.CanAlmostBuild:
                                if (!__instance.GetInventory().HaveItem(resource.m_resItem.m_itemData.m_shared.m_name))
                                    cb = false;
                                continue;
                            default:
                                continue;
                        }
                    }

                    if (!cb)
                        break;
                }

                if (cb)
                    __result = true;
            }
            catch (Exception ex)
            {

                UnityEngine.Debug.LogWarning("PHR failed: " + ex.ToString());
            }
        }

            public static void PUPG(ref Player __instance)
        {
            try
            {

                //var etype = ;
                AccessTools.Field(typeof(Player), "m_placementStatus").SetValue(__instance, Enum.GetValues(AccessTools.Field(typeof(Player), "m_placementStatus").GetUnderlyingType()).GetValue(0));

                //UnityEngine.Debug.LogWarning(Enum.GetValues(AccessTools.Field(typeof(Player), "m_placementStatus").GetUnderlyingType()).GetValue(0).ToString());
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning("PUPG failed: " + ex.ToString());

            }
        }

        public static Type GetEnumType(string enumName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(enumName);
                if (type == null)
                    continue;
                if (type.IsEnum)
                    return type;
            }
            return null;
        }


        public static bool CGDL(ref CharacterDrop __instance, ref List<KeyValuePair<GameObject, int>> __result)
        {

            try
            {

                Character c = __instance.GetComponent<Character>();

                ZNetView znv = typeof(Character).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(c) as ZNetView;
                bool vml = int.TryParse(znv.GetZDO().GetString("VMMML", ""), out int cl);
                int level;

                if (!vml)
                    return true;
                else
                    level = cl;

                List<KeyValuePair<GameObject, int>> keyValuePairList = new List<KeyValuePair<GameObject, int>>();

                foreach (CharacterDrop.Drop drop in __instance.m_drops)
                {
                    if (!((UnityEngine.Object)drop.m_prefab == (UnityEngine.Object)null))
                    {
                        float chance = drop.m_chance;
                        if (drop.m_levelMultiplier)
                            chance *= (level * 2);
                        if ((double)UnityEngine.Random.value <= (double)chance)
                        {
                            int num2 = UnityEngine.Random.Range(drop.m_amountMin, drop.m_amountMax);
                            if (drop.m_levelMultiplier)
                                num2 *= level;
                            if (UnityEngine.Random.value <= (0.001f * level * level))
                                num2 = Mathf.RoundToInt(num2 * 1.5f);
                            if (drop.m_onePerPlayer)
                                num2 = ZNet.instance.GetNrOfPlayers();
                            if (num2 > 0)
                                keyValuePairList.Add(new KeyValuePair<GameObject, int>(drop.m_prefab, num2));
                        }
                    }
                }

                if (UnityEngine.Random.value <= (0.001f * level * level))
                {
                    GameObject it = ObjectDB.instance.m_items[UnityEngine.Random.Range(0, ObjectDB.instance.m_items.Count)];
                    ItemDrop itd = it.GetComponent<ItemDrop>();

                    int amt;

                    if (itd.m_itemData.m_shared.m_maxStackSize == 1)
                        amt = 1;
                    else
                        amt = UnityEngine.Random.Range(1, level + 1);

                    keyValuePairList.Add(new KeyValuePair<GameObject, int>(it, amt));
                }

                __result = keyValuePairList;

                return false;
            }
            catch (Exception ex)
            {

                UnityEngine.Debug.LogWarning("CGDL failed: " + ex.ToString());
                return true;
            }

        }

            public static void ILZDO(ref ItemDrop.ItemData itemData, ref ZDO zdo)
        {
            try
            {
                if (!zdo.GetString("crafterName", "").Contains(" (UVO"))
                    return;

                List<float> battr = getAttr(itemData.m_shared);
                List<float> attr = new List<float>();

                for (int i = 0; i < battr.Count; i++)
                {
                    attr.Add(zdo.GetFloat($"attr{i}", battr[i]));
                }

                bool repairable = zdo.GetBool($"attr22", itemData.m_shared.m_canBeReparied);

                setAttr(ref itemData, attr, repairable);

            }
            catch (Exception ex)
            {

                UnityEngine.Debug.LogWarning("ILZDO failed: " + ex.ToString());
            }
        }

            public static void ISZDO(ref ItemDrop.ItemData itemData, ref ZDO zdo)
        {
            try
            {
                if (!zdo.GetString("crafterName", "").Contains(" (UVO"))
                    return;

                List<float> attr = getAttr(itemData.m_shared);

                for (int i = 0; i < attr.Count; i++)
                {
                    zdo.Set($"attr{i}", attr[i]);
                }

                zdo.Set($"attr22", itemData.m_shared.m_canBeReparied);

                
            }
            catch (Exception ex)
            {

                UnityEngine.Debug.LogWarning("ISZDO failed: " + ex.ToString());
            }
        }

            public static void IDI(ref ItemDrop __result, ref ItemDrop.ItemData item)
        {
            try
            {

                __result.m_itemData = item;
                AccessTools.Method(typeof(ItemDrop), "Save").Invoke(__result, new object[] { });

                UnityEngine.Debug.LogWarning("Updated and saved " + __result.m_itemData.m_shared.m_name);
            }
            catch (Exception ex)
            {

                UnityEngine.Debug.LogWarning("IDI Error: " + ex.ToString());

            }
        }

            public static bool ISV(ref Inventory __instance, ref ZPackage pkg)
        {
            int opos = pkg.GetPos();

            try
            {
                int currentVersion = (int)(typeof(Inventory).GetField("currentVersion", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance));
                List<ItemDrop.ItemData> m_inventory = (typeof(Inventory).GetField("m_inventory", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance)) as List<ItemDrop.ItemData>;

                pkg.Write(currentVersion);
                pkg.Write(m_inventory.Count);
                foreach (ItemDrop.ItemData itemData in m_inventory)
                {
                    string str = "";

                    if (itemData.m_shared.m_name.Contains(" (UVO"))
                    {
                        str = itemData.m_shared.m_name.Substring(itemData.m_shared.m_name.IndexOf(" (UVO"));
                    }

                    if ((UnityEngine.Object)itemData.m_dropPrefab == (UnityEngine.Object)null)
                    {
                        ZLog.Log((object)("Item missing prefab " + itemData.m_shared.m_name));
                        pkg.Write("");
                    }
                    else
                        pkg.Write(itemData.m_dropPrefab.name);
                    pkg.Write(itemData.m_stack);
                    pkg.Write(itemData.m_durability);
                    pkg.Write(itemData.m_gridPos);
                    pkg.Write(itemData.m_equiped);
                    pkg.Write(itemData.m_quality);
                    pkg.Write(itemData.m_variant);
                    pkg.Write(itemData.m_crafterID);

                    if (!str.Contains(" (UVO") || itemData.m_crafterName.Contains(" (UVO"))
                    {
                        pkg.Write(itemData.m_crafterName);
                    }
                    else
                    {
                        itemData.m_crafterName += str;
                        pkg.Write(itemData.m_crafterName);
                    }

                    

                    if (itemData.m_crafterName.Contains(" (UVO"))
                    {
                        foreach (float atr in getAttr(itemData.m_shared))
                        {
                            pkg.Write(atr);
                        }

                        pkg.Write(itemData.m_shared.m_canBeReparied);
                    }
                }

                return false;
            }
            catch (Exception ex)
            {

                UnityEngine.Debug.LogWarning($"ISV ERROR: {ex.ToString()}, {LineNumber(ex)}, {ex.Message}, {ex.StackTrace}, {ex.InnerException}, {ex.Source}");

                pkg.SetPos(opos);

                return true;
            }
        }

        public static bool ILD(ref Inventory __instance, ref ZPackage pkg)
        {
            int cle = 177;

            int opos = pkg.GetPos();

            try
            {
                //return true;
                //if (_player == null)
                //    return true;
                
                List<ItemDrop.ItemData> m_inventory = (typeof(Inventory).GetField("m_inventory", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance)) as List<ItemDrop.ItemData>;
                Action m_onChanged = __instance.m_onChanged; //(typeof(Inventory).GetField("m_onChanged", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_player.GetInventory())) as Action;

                //UnityEngine.Debug.LogWarning("ILD load start:");
                cle = 191;
                int num1 = pkg.ReadInt();
                int num2 = pkg.ReadInt();
                m_inventory.Clear();
                for (int index = 0; index < num2; ++index)
                {
                    string name = pkg.ReadString();
                    int stack = pkg.ReadInt();
                    float durability = pkg.ReadSingle();
                    Vector2i pos = pkg.ReadVector2i();
                    bool equiped = pkg.ReadBool();
                    int quality = 1;
                    if (num1 >= 101)
                        quality = pkg.ReadInt();
                    int variant = 0;
                    if (num1 >= 102)
                        variant = pkg.ReadInt();
                    long crafterID = 0;
                    string crafterName = "";
                    if (num1 >= 103)
                    {
                        crafterID = pkg.ReadLong();
                        crafterName = pkg.ReadString();
                    }
                    cle = 215;
                    if (name != "")
                    {


                        GameObject itemPrefab = ObjectDB.instance.GetItemPrefab(name);
                        if ((UnityEngine.Object)itemPrefab == (UnityEngine.Object)null)
                        {
                            ZLog.Log((object)("Failed to find item prefab " + name));
                            return false;
                        }
                        ZNetView.m_forceDisableInit = true;
                        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(itemPrefab);
                        ZNetView.m_forceDisableInit = false;
                        ItemDrop component = gameObject.GetComponent<ItemDrop>();
                        if ((UnityEngine.Object)component == (UnityEngine.Object)null)
                        {
                            ZLog.Log((object)("Missing itemdrop in " + name));
                            UnityEngine.Object.Destroy((UnityEngine.Object)gameObject);
                            return false;
                        }
                        cle = 236;
                        component.m_itemData.m_stack = Mathf.Min(stack, component.m_itemData.m_shared.m_maxStackSize);
                        component.m_itemData.m_durability = durability;
                        component.m_itemData.m_equiped = equiped;
                        component.m_itemData.m_quality = quality;
                        component.m_itemData.m_variant = variant;
                        component.m_itemData.m_crafterID = crafterID;
                        component.m_itemData.m_crafterName = crafterName;
                        cle = 244;
                        if (!crafterName.Contains(" (UVO") || pkg.GetPos() >= pkg.Size())
                        {
                            //_player.GetInventory().AddItem(component.m_itemData, component.m_itemData.m_stack, pos.x, pos.y);
                            //typeof(Inventory).GetMethod("AddItem", BindingFlags.NonPublic | BindingFlags.Instance,).Invoke(_player.GetInventory(), new object[] { component.m_itemData, component.m_itemData.m_stack, pos.x, pos.y });
                            AccessTools.Method(typeof(Inventory), "AddItem", new Type[] { typeof(ItemDrop.ItemData), typeof(int), typeof(int), typeof(int) }).Invoke(__instance, new object[] { component.m_itemData, component.m_itemData.m_stack, pos.x, pos.y });
                            UnityEngine.Object.Destroy((UnityEngine.Object)gameObject);
                            cle = 251; 
                        }
                        else
                        {
                            //UnityEngine.Debug.LogWarning($"{component.m_itemData.m_shared.m_name}, {component.m_itemData.m_crafterName}");

                            List<float> attr = new List<float>();
                            bool repairable;

                            for (int i = 0; i < mfatts; i++)
                            {
                                attr.Add(pkg.ReadSingle());
                            }

                            repairable = pkg.ReadBool();
                            //pkg.ReadInt();

                            component.m_itemData.m_shared.m_armor = attr[0];
                            component.m_itemData.m_shared.m_attackForce = attr[1];
                            component.m_itemData.m_shared.m_backstabBonus = attr[2];
                            component.m_itemData.m_shared.m_blockPower = attr[3];
                            component.m_itemData.m_shared.m_damages.m_blunt = attr[4];
                            component.m_itemData.m_shared.m_damages.m_chop = attr[5];
                            component.m_itemData.m_shared.m_damages.m_damage = attr[6];
                            component.m_itemData.m_shared.m_damages.m_fire = attr[7];
                            component.m_itemData.m_shared.m_damages.m_frost = attr[8];
                            component.m_itemData.m_shared.m_damages.m_lightning = attr[9];
                            component.m_itemData.m_shared.m_damages.m_pickaxe = attr[10];
                            component.m_itemData.m_shared.m_damages.m_pierce = attr[11];
                            component.m_itemData.m_shared.m_damages.m_poison = attr[12];
                            component.m_itemData.m_shared.m_damages.m_slash = attr[13];
                            component.m_itemData.m_shared.m_damages.m_spirit = attr[14];
                            component.m_itemData.m_shared.m_deflectionForce = attr[15];
                            component.m_itemData.m_shared.m_durabilityDrain = attr[16];
                            component.m_itemData.m_shared.m_maxDurability = attr[17];
                            component.m_itemData.m_shared.m_movementModifier = attr[18];
                            component.m_itemData.m_shared.m_timedBlockBonus = attr[19];
                            component.m_itemData.m_shared.m_useDurabilityDrain = attr[20];
                            component.m_itemData.m_shared.m_weight = attr[21];

                            component.m_itemData.m_shared.m_canBeReparied = repairable;

                            component.m_itemData.m_shared.m_name += crafterName.Substring(crafterName.IndexOf(" (UVO"));

                            //_player.GetInventory().AddItem(component.m_itemData);
                            AccessTools.Method(typeof(Inventory), "AddItem", new Type[] { typeof(ItemDrop.ItemData), typeof(int), typeof(int), typeof(int) }).Invoke(__instance, new object[] { component.m_itemData, component.m_itemData.m_stack, pos.x, pos.y });

                            string str2 = "ADDINGITEM ATTR:";

                            foreach (float fl in attr)
                            {
                                str2 += $" {fl},";
                            }

                            //pkg.SetPos(pkg.GetPos() - 1);

                            //str2 += $" {pkg.ReadBool()},";

                            //UnityEngine.Debug.LogWarning(str2);

                            UnityEngine.Object.Destroy((UnityEngine.Object)gameObject);

                            //pkg.SetPos(pkg.GetPos()-((4*22) + 1));
                        }
                    }
                    //_player.GetInvento.AddItem(name, stack, durability, pos, equiped, quality, variant, crafterID, crafterName);
                }
                cle = 303;
                typeof(Inventory).GetMethod("Changed", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[] { });
                
                return false;
            }
            catch (Exception ex)
            {
                //int ln = 0;
                /*System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                foreach (StackFrame fr in trace.GetFrames())
                {
                    if (fr.GetFileLineNumber() != 0)
                    {
                        ln = fr.GetFileLineNumber();
                        break;
                    }

                }*/
                

                UnityEngine.Debug.LogWarning($"ILD ERROR: {cle}, {ex.ToString()}, {ex.Message}, {ex.StackTrace}, {ex.InnerException}, {ex.Source}");
                //UnityEngine.Debug.LogException(ex);

                ile = true;

                pkg.SetPos(opos);

                return true;
            }
        }

        public static void ILD2(ref Inventory __instance, ref ZPackage pkg)
        {
            try
            {
                if (!ile)
                    return;

                foreach (ItemDrop.ItemData item in __instance.GetAllItems())
                {
                    if(item.m_crafterName.Contains(" (UVO"))
                    {
                        string str = item.m_crafterName;

                        item.m_crafterName = item.m_crafterName.Substring(0, item.m_crafterName.IndexOf(" (UVO"));

                        UnityEngine.Debug.LogWarning($"changing craftername for {item.m_shared.m_name}, {str} to {item.m_crafterName}");
                    }
                    
                }
            }
            catch (Exception ex)
            {

                UnityEngine.Debug.LogWarning($"ILD2 ERROR: {ex.ToString()}, {LineNumber(ex)}, {ex.Message}, {ex.StackTrace}, {ex.InnerException}, {ex.Source}");
            }

            ile = false;
        }

        public static bool ile = false;

        public static int LineNumber(Exception e)
        {

            int linenum = 0;
            try
            {
                //linenum = Convert.ToInt32(e.StackTrace.Substring(e.StackTrace.LastIndexOf(":line") + 5));

                //For Localized Visual Studio ... In other languages stack trace  doesn't end with ":Line 12"
                linenum = Convert.ToInt32(e.StackTrace.Substring(e.StackTrace.LastIndexOf(' ')));

            }


            catch
            {
                //Stack trace is not available!
            }
            return linenum;
        }

        public static int mfatts = 22;

        public static List<float> getAttr(ItemDrop.ItemData.SharedData item)
        {
            List<float> temp = new List<float>();

            temp.Add(rndf2(item.m_armor));
            temp.Add(rndf2(item.m_attackForce));
            temp.Add(rndf2(item.m_backstabBonus));
            temp.Add(rndf2(item.m_blockPower));
            temp.Add(rndf2(item.m_damages.m_blunt));
            temp.Add(rndf2(item.m_damages.m_chop));
            temp.Add(rndf2(item.m_damages.m_damage));
            temp.Add(rndf2(item.m_damages.m_fire));
            temp.Add(rndf2(item.m_damages.m_frost));
            temp.Add(rndf2(item.m_damages.m_lightning));
            temp.Add(rndf2(item.m_damages.m_pickaxe));
            temp.Add(rndf2(item.m_damages.m_pierce));
            temp.Add(rndf2(item.m_damages.m_poison));
            temp.Add(rndf2(item.m_damages.m_slash));
            temp.Add(rndf2(item.m_damages.m_spirit));
            temp.Add(rndf2(item.m_deflectionForce));
            temp.Add(rndf2(item.m_durabilityDrain));
            temp.Add(rndf2(item.m_maxDurability));
            temp.Add(rndf2(item.m_movementModifier));
            temp.Add(rndf2(item.m_timedBlockBonus));
            temp.Add(rndf2(item.m_useDurabilityDrain));
            temp.Add(rndf2(item.m_weight));


            return temp;
        }

        public static void GMCW(ref float __result)
        {

            //__result += ((float)wsl * 5f);
        }
        public static void RP(ref Player __result, Vector3 spawnPoint)
        {
            _player = __result;
            cs = false;

            //__result += ((float)wsl * 5f);
        }
        public static void DDM(Destructible __instance, long sender, ref HitData hit)
        {
            try
            {
                List<Skills.Skill> gskills = _player.GetSkills().GetSkillList();

                if (__instance.m_destructibleType == DestructibleType.Tree)
                {
                    Skills.Skill skl = gskills.Where(sk => sk.m_info.m_skill == Skills.SkillType.WoodCutting).FirstOrDefault();

                    if (skl == null)
                        return;

                    if (UnityEngine.Random.Range(0.0f, 1.0f) < (skl.m_level * 0.003f))
                    {
                        hit.m_damage.m_damage += hit.m_damage.GetTotalDamage();
                    }

                }
                else if (__instance.m_destructibleType == DestructibleType.Default)
                {
                    Skills.Skill skl = gskills.Where(sk => sk.m_info.m_skill == Skills.SkillType.Pickaxes).FirstOrDefault();

                    if (skl == null)
                        return;

                    if (UnityEngine.Random.Range(0.0f, 1.0f) < (skl.m_level * 0.003f))
                    {
                        hit.m_damage.m_damage += hit.m_damage.GetTotalDamage();
                    }

                }
                else if (__instance.m_destructibleType == DestructibleType.Character)
                {
                    ItemDrop.ItemData item = _player.GetCurrentWeapon();

                    Skills.Skill skl = gskills.Where(sk => sk.m_info.m_skill == item.m_shared.m_skillType).FirstOrDefault();

                    if (skl == null || (skl.m_info.m_skill == Skills.SkillType.Axes && (item.m_shared.m_name.ToLower().Contains("stone") || item.m_shared.m_name.ToLower().Contains("flint"))))
                        return;

                    float rnd = UnityEngine.Random.Range(0.0f, 1.0f);

                    if (rnd < (skl.m_level * 0.003f))
                    {
                        if (skl.m_info.m_skill == Skills.SkillType.Knives)
                        {
                            hit.m_damage.m_damage += (hit.m_damage.GetTotalDamage() * 3);
                        }
                        else if (skl.m_info.m_skill == Skills.SkillType.Bows)
                        {
                            if (_player.IsSneaking())
                                hit.m_damage.m_damage += (hit.m_damage.GetTotalDamage() * 3);
                            else
                                hit.m_damage.m_damage += hit.m_damage.GetTotalDamage();
                        }
                        else
                        {
                            hit.m_damage.m_damage += hit.m_damage.GetTotalDamage();
                        }

                    }

                }

                //_player.Message(MessageHud.MessageType.TopLeft, $"VM DDM: {hit.m_damage.m_damage}", 0, (Sprite)null);
                //__result += ((float)wsl * 5f);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning(ex.Message);
            }
        }

        Type[] cwTypes = { typeof(Vector3), typeof(float), typeof(float), typeof(float), typeof(float), typeof(Vector2), typeof(float) };

        public static void CW(
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
                EnvSetup env = EnvMan.instance.GetCurrentEnvironment();


                //waveSpeed *= UnityEngine.Random.Range(0.75f, env.m_windMax);
                //waveLength *= UnityEngine.Random.Range(0.75f, env.m_windMax);
                //sharpness *= UnityEngine.Random.Range(0.75f, 0.75f + (env.m_windMax / 2.0f));
                waveHeight *= env.m_windMax;

                //__result += ((float)wsl * 5f);
                //_player.Message(MessageHud.MessageType.TopLeft, $"Wave upd.", 0, (Sprite)null);

                //return true;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning($"Error in waves: {ex.Message}");
            }
        }

        Type[] calcwtypes = { typeof(Vector3), typeof(float), typeof(Vector4), typeof(float), typeof(float) };

        public static void CW2(
          Vector3 worldPos,
          ref float depth,
          Vector4 wind,
          float _WaterTime,
          float waveFactor)
        {
            try
            {
                EnvSetup env = EnvMan.instance.GetCurrentEnvironment();

                //depth *= env.m_windMax;
            }
            catch (Exception)
            {


            }
        }

        public static void GWL(Vector3 p, ref float waveFactor)
        {
            try
            {
                EnvSetup env = EnvMan.instance.GetCurrentEnvironment();

                waveFactor = 1f + (env.m_windMax * 0.06f);
            }
            catch
            {
                UnityEngine.Debug.LogWarning("Fail in GWL patch.");
            }
        }

        public static bool iscrafting = false;
        public static ItemDrop.ItemData lastitem = null;

        public static void DC(Player player)
        {
            try
            {
                iscrafting = true;
            }
            catch
            {
                UnityEngine.Debug.LogWarning("Fail in DC patch.");
            }
        }

        public static void AI(ref ItemDrop.ItemData item)
        {
            try
            {
                if (!iscrafting)
                {
                    cupgitem = null;
                    return;
                }
                else if (item == null)
                {
                    cupgitem = null;
                    iscrafting = false;
                    return;
                }



                Skill c = skills.Where(sk => sk.name.ToLower() == "crafting").FirstOrDefault();

                int xp = 1 + (int)(c.level / 10.0);

                ItemDrop.ItemData.ItemType it = item.m_shared.m_itemType;
                int type = 0;

                if (it == ItemDrop.ItemData.ItemType.Bow || it == ItemDrop.ItemData.ItemType.OneHandedWeapon || it == ItemDrop.ItemData.ItemType.TwoHandedWeapon || it == ItemDrop.ItemData.ItemType.Tool)
                    type = 1;
                else if (it == ItemDrop.ItemData.ItemType.Chest || it == ItemDrop.ItemData.ItemType.Hands || it == ItemDrop.ItemData.ItemType.Helmet || it == ItemDrop.ItemData.ItemType.Legs || it == ItemDrop.ItemData.ItemType.Shoulder || it == ItemDrop.ItemData.ItemType.Shield)
                    type = 2;


                if (type == 0)
                {
                    cupgitem = null;
                    iscrafting = false;
                    return;
                }

                int r = GenerateItemRarity();
                //int r = 8;

                int oir = 0;
                if (cupgitem != null)
                {
                    string sub = cupgitem.m_shared.m_name.Remove(cupgitem.m_shared.m_name.Length - 1);

                    if (int.TryParse(sub.Substring(sub.IndexOf("UVO: ") + 5), out int orar))
                        oir = orar;

                    float chance = keepOldR(oir, c.level, cupgitem.m_quality);

                    if (UnityEngine.Random.value > chance)
                    {
                        oir = 0;
                        r = 1;
                    }
                    else if (oir != 0)
                    {

                        List<float> attr = getAttr(cupgitem.m_shared);

                        setAttr(ref item, attr, cupgitem.m_shared.m_canBeReparied);

                        r = r + oir;
                    }
                }


                if (r > 1)
                {
                    if (type == 1)
                    {

                        if (UnityEngine.Random.value < Mathf.Min(r * r * 0.0015f, 0.25f))
                            item.m_shared.m_armor += UnityEngine.Random.Range(0, r + 1);

                        item.m_shared.m_attackForce = rndf2(item.m_shared.m_attackForce * (UnityEngine.Random.Range(1.0f, 1.0f + (r * r * .01f))));
                        item.m_shared.m_backstabBonus = rndf2(item.m_shared.m_backstabBonus * UnityEngine.Random.Range(1.0f, 1.0f + (r * r * .01f)));
                        item.m_shared.m_blockPower = rndf2(item.m_shared.m_blockPower * UnityEngine.Random.Range(1.0f, 1.0f + (r * r * .01f)));


                        int cbr = UnityEngine.Random.Range(0, 2);

                        if (cbr == 0)
                            item.m_shared.m_canBeReparied = false;
                        else
                            item.m_shared.m_canBeReparied = true;

                        if (UnityEngine.Random.value < Mathf.Min(r * r * 0.0015f, 0.25f))
                            item.m_shared.m_damages.m_blunt += UnityEngine.Random.Range(0, r + 1);
                        if (UnityEngine.Random.value < Mathf.Min(r * r * 0.0015f, 0.25f))
                            item.m_shared.m_damages.m_damage += UnityEngine.Random.Range(0 , r + 1);
                        if (UnityEngine.Random.value < Mathf.Min(r * r * 0.0015f, 0.25f))
                            item.m_shared.m_damages.m_fire += UnityEngine.Random.Range(0, r + 1);
                        if (UnityEngine.Random.value < Mathf.Min(r * r * 0.0015f, 0.25f))
                            item.m_shared.m_damages.m_frost += UnityEngine.Random.Range(0, r + 1);
                        if (UnityEngine.Random.value < Mathf.Min(r * r * 0.0015f, 0.25f))
                            item.m_shared.m_damages.m_lightning += UnityEngine.Random.Range(0, r + 1);
                        if (UnityEngine.Random.value < Mathf.Min(r * r * 0.0015f, 0.25f))
                            item.m_shared.m_damages.m_pierce += UnityEngine.Random.Range(0, r + 1);
                        if (UnityEngine.Random.value < Mathf.Min(r * r * 0.0015f, 0.25f))
                            item.m_shared.m_damages.m_poison += UnityEngine.Random.Range(0, r + 1);
                        if (UnityEngine.Random.value < Mathf.Min(r * r * 0.0015f, 0.25f))
                            item.m_shared.m_damages.m_slash += UnityEngine.Random.Range(0, r + 1);
                        if (UnityEngine.Random.value < Mathf.Min(r * r * 0.0015f, 0.25f))
                            item.m_shared.m_damages.m_spirit += UnityEngine.Random.Range(0, r + 1);


                        item.m_shared.m_damages.Modify(rndf2(UnityEngine.Random.Range(1.0f, 1.0f + (r * r * 0.0078f))));
                        item.m_shared.m_deflectionForce = rndf2(item.m_shared.m_deflectionForce * UnityEngine.Random.Range(1.0f, 1.0f + (r * r * .01f)));
                        item.m_shared.m_useDurabilityDrain = rndf2(item.m_shared.m_useDurabilityDrain / UnityEngine.Random.Range(1.0f, 1.0f + (r * r * 0.0078f)));
                        item.m_shared.m_maxDurability = rndf2(item.m_shared.m_maxDurability * UnityEngine.Random.Range(1.0f, 1.0f + (r * r * .01f)));
                        item.m_shared.m_movementModifier = rndf2(item.m_shared.m_movementModifier / UnityEngine.Random.Range(1.0f, 1.0f + (r * r * 0.0078f)));
                        item.m_shared.m_timedBlockBonus = rndf2(item.m_shared.m_timedBlockBonus * UnityEngine.Random.Range(1.0f, 1.0f + (r * r * .01f)));

                        item.m_shared.m_weight = rndf2(item.m_shared.m_weight * (1 + UnityEngine.Random.Range(0, r + 1)));
                        item.m_durability = item.m_shared.m_maxDurability;


                    }
                    else if (type == 2)
                    {

                        item.m_shared.m_armor = rndf2(item.m_shared.m_armor * UnityEngine.Random.Range(1.0f, 1.0f + (r * r * .01f)));
                        item.m_shared.m_attackForce = rndf2(item.m_shared.m_attackForce * (UnityEngine.Random.Range(1.0f, 1.0f + (r * r * .01f))));
                        item.m_shared.m_backstabBonus = rndf2(item.m_shared.m_backstabBonus * UnityEngine.Random.Range(1.0f, 1.0f + (r * r * .01f)));
                        item.m_shared.m_blockPower = rndf2(item.m_shared.m_blockPower * UnityEngine.Random.Range(1.0f, 1.0f + (r * r * .01f)));


                        int cbr = UnityEngine.Random.Range(0, 2);

                        if (cbr == 0)
                            item.m_shared.m_canBeReparied = false;
                        else
                            item.m_shared.m_canBeReparied = true;

                        if (UnityEngine.Random.value < Mathf.Min(r * r * 0.0015f, 0.25f))
                            item.m_shared.m_damages.m_blunt += UnityEngine.Random.Range(0, r + 1);
                        if (UnityEngine.Random.value < Mathf.Min(r * r * 0.0015f, 0.25f))
                            item.m_shared.m_damages.m_damage += UnityEngine.Random.Range(0, r + 1);
                        if (UnityEngine.Random.value < Mathf.Min(r * r * 0.0015f, 0.25f))
                            item.m_shared.m_damages.m_fire += UnityEngine.Random.Range(0, r + 1);
                        if (UnityEngine.Random.value < Mathf.Min(r * r * 0.0015f, 0.25f))
                            item.m_shared.m_damages.m_frost += UnityEngine.Random.Range(0, r + 1);
                        if (UnityEngine.Random.value < Mathf.Min(r * r * 0.0015f, 0.25f))
                            item.m_shared.m_damages.m_lightning += UnityEngine.Random.Range(0, r + 1);
                        if (UnityEngine.Random.value < Mathf.Min(r * r * 0.0015f, 0.25f))
                            item.m_shared.m_damages.m_pierce += UnityEngine.Random.Range(0, r + 1);
                        if (UnityEngine.Random.value < Mathf.Min(r * r * 0.0015f, 0.25f))
                            item.m_shared.m_damages.m_poison += UnityEngine.Random.Range(0, r + 1);
                        if (UnityEngine.Random.value < Mathf.Min(r * r * 0.0015f, 0.25f))
                            item.m_shared.m_damages.m_slash += UnityEngine.Random.Range(0, r + 1);
                        if (UnityEngine.Random.value < Mathf.Min(r * r * 0.0015f, 0.25f))
                            item.m_shared.m_damages.m_spirit += UnityEngine.Random.Range(0, r + 1);


                        item.m_shared.m_damages.Modify(rndf2(UnityEngine.Random.Range(1.0f, 1.0f + (r * r * 0.0078f))));
                        item.m_shared.m_deflectionForce = rndf2(item.m_shared.m_deflectionForce * UnityEngine.Random.Range(1.0f, 1.0f + (r * r * .01f)));
                        item.m_shared.m_useDurabilityDrain = rndf2(item.m_shared.m_useDurabilityDrain / UnityEngine.Random.Range(1.0f, 1.0f + (r * r * 0.0078f)));
                        item.m_shared.m_maxDurability = rndf2(item.m_shared.m_maxDurability * UnityEngine.Random.Range(1.0f, 1.0f + (r * r * .01f)));
                        item.m_shared.m_movementModifier = rndf2(item.m_shared.m_movementModifier / UnityEngine.Random.Range(1.0f, 1.0f + (r * r * 0.0078f)));
                        item.m_shared.m_timedBlockBonus = rndf2(item.m_shared.m_timedBlockBonus * UnityEngine.Random.Range(1.0f, 1.0f + (r * r * .01f)));

                        item.m_shared.m_weight = rndf2(item.m_shared.m_weight * (1 + UnityEngine.Random.Range(0, r + 1)));
                        item.m_durability = item.m_shared.m_maxDurability;

                    }
                }

                if (r != 1)
                {
                    //globalid += 1;

                    if (!item.m_shared.m_name.Contains(" (UVO"))
                    {
                        string str = $" (UVO: R{r})";
                        item.m_shared.m_name += str;
                        //item.m_crafterName += str;
                        //item.m_dropPrefab.name += $" (UVO: R{r}, {globalid})";

                        //ObjectDB.instance.m_items.Add(item.m_dropPrefab);

                        //ObjectDB.instance.m_items
                    }
                    else
                    {
                        item.m_shared.m_name = item.m_shared.m_name.Substring(0, item.m_shared.m_name.IndexOf(" (UVO"));

                        string str = $" (UVO: R{r})";
                        item.m_shared.m_name += str;
                    }

                    if (oir == 0)
                        xp = xp + (int)(r * r);
                    else
                        xp = xp + (int)(r * r * (0.5 * oir));

                    c.xp = c.xp + xp;
                }
                else if (r == 1 && oir == 0)
                {
                    if (item.m_shared.m_name.Contains(" (UVO"))
                    {
                        item.m_shared.m_name = item.m_shared.m_name.Substring(0, item.m_shared.m_name.IndexOf(" (UVO"));

                        string str = $" (UVO: R{1})";
                        item.m_shared.m_name += str;
                    }
                }

                cupgitem = null;
                iscrafting = false;
            }
            catch
            {
                UnityEngine.Debug.LogWarning("Fail in AI patch.");
            }
        }

        public static float keepOldR(int or, int cl, int q)
        {
            float chance = 0f;

            if (or < 13)
            {
                chance = (0.95f - (or * 0.05f)) - (0.1f * q) + (cl * 0.0010f);
            }
            else
            {
                chance = 0.1f + (cl * 0.0010f);
            }

            return chance;
        }

        public static void setAttr(ref ItemDrop.ItemData item, List<float> attr, bool repairable)
        {
            item.m_shared.m_armor = attr[0];
            item.m_shared.m_attackForce = attr[1];
            item.m_shared.m_backstabBonus = attr[2];
            item.m_shared.m_blockPower = attr[3];
            item.m_shared.m_damages.m_blunt = attr[4];
            item.m_shared.m_damages.m_chop = attr[5];
            item.m_shared.m_damages.m_damage = attr[6];
            item.m_shared.m_damages.m_fire = attr[7];
            item.m_shared.m_damages.m_frost = attr[8];
            item.m_shared.m_damages.m_lightning = attr[9];
            item.m_shared.m_damages.m_pickaxe = attr[10];
            item.m_shared.m_damages.m_pierce = attr[11];
            item.m_shared.m_damages.m_poison = attr[12];
            item.m_shared.m_damages.m_slash = attr[13];
            item.m_shared.m_damages.m_spirit = attr[14];
            item.m_shared.m_deflectionForce = attr[15];
            item.m_shared.m_durabilityDrain = attr[16];
            item.m_shared.m_maxDurability = attr[17];
            item.m_shared.m_movementModifier = attr[18];
            item.m_shared.m_timedBlockBonus = attr[19];
            item.m_shared.m_useDurabilityDrain = attr[20];
            item.m_shared.m_weight = attr[21];

            item.m_shared.m_canBeReparied = repairable;
        }

        public static float rndf2(float val)
        {

            return (Mathf.Round(val * 100f) / 100f);
        }

        public static int globalid = 0;

        public void Uninitialize()
        {
            _player = null;
            pln = "";
            otime = 0f;
            path = "";
            filename = "";
            errorfile = "";

            //oms = new Dictionary<string, int>();

            meaw = false;

            shouldInit = true;

            skills = new List<Skill>();
        }

        public static int GenerateItemRarity()
        {
            float rnd = UnityEngine.Random.value;

            int r = 1;

            Skill c = skills.Where(sk => sk.name.ToLower() == "crafting").FirstOrDefault();

            for (int i = 2; i < 101; i++)
            {
                if ((rnd < ((1.0f / (i * i * i)) * (1.0 + (0.01 * c.level)))))
                {
                    r = i;
                }
                else
                {
                    break;
                }

            }

            //UnityEngine.Debug.LogWarning($"GIR: rnd={rnd}, r={r}");
            return r;
        }

        public void Initialize()
        {
            //M = this;
            //ZNet.instance.m_serverPlayerLimit = 99;

            try
            {

                _player = Player.m_localPlayer;

                pln = _player.GetPlayerName();

                int count = 0;


                foreach (Player pl in Player.GetAllPlayers())
                {
                    ZNetView tznv = typeof(Player).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(pl) as ZNetView;
                    if (tznv.GetZDO().GetString("VMMHM") == "True")
                    {
                        count++;
                        break;
                    }
                }

                if (count == 0)
                {
                    meaw = true;

                }
                else
                {
                    meaw = false;
                }

                ZNetView znv = typeof(Player).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_player) as ZNetView;
                znv.GetZDO().Set("VMMHM", $"{meaw}");

                _player.Message(MessageHud.MessageType.TopLeft, $"Host Modifier: {meaw}, ZNV: {znv.GetZDO().GetString("VMMHM")}.", 0, (Sprite)null);

                path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/VMU_Data";

                filename = path + $"/{_player.GetPlayerName()}_VM_Data.ini";
                errorfile = path + $"/VM_Error.ini";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                IniData data;

                if (File.Exists(filename))
                {

                    data = parser.ReadFile(filename);

                    for (int i = 1; i < snames.Length + 1; i++)
                    {
                        if (!data.Sections.ContainsSection($"SkillData{i}"))
                            break;

                        string name = data[$"SkillData{i}"]["SkillName"];

                        if (int.TryParse(data[$"SkillData{i}"]["SkillLevel"], out int level) && int.TryParse(data[$"SkillData{i}"]["SkillXP"], out int xp))
                        {

                            skills.Add(new Skill(name, level, xp));

                        }

                    }
                }
                else
                {
                    FileStream configStream = File.Create(filename);
                    configStream.Close();
                }

                if (skills.Count < snames.Length)
                {

                    data = parser.ReadFile(filename);

                    for (int i = skills.Count + 1; i < snames.Length + 1; i++)
                    {
                        string secName = $"SkillData{i}";
                        data.Sections.AddSection(secName);
                        string name = snames[i - 1];
                        data[secName].AddKey("SkillName", name);
                        data[secName].AddKey("SkillLevel", "1");
                        data[secName].AddKey("SkillXP", "1");

                        skills.Add(new Skill(snames[i - 1], 1, 1));

                    }

                    parser.WriteFile(filename, data);

                }


                otime = Time.time;



                //Type[] types1 = { };

                //h.Patch(typeof(WaterVolume).GetMethod("CreateWaver"), prefix: new HarmonyMethod(typeof(Main), nameof(this.CW), cwTypes));
                //h.PatchAll(Assembly.GetExecutingAssembly());
                //h.Patch(typeof(Game).GetMethod("SpawnPlayer"), postfix: new HarmonyMethod(typeof(Main), nameof(this.RP)));

                //h.Patch(typeof(TreeLog).GetMethod("RPC_Damage"), prefix: new HarmonyMethod(typeof(Main), nameof(this.DDM)));

                //InvokeRepeating("VMU", 30.0f, 30.0f);
                try
                {

                    foreach (GameObject go in ObjectDB.instance.m_items)
                    {


                        ItemDrop item = go.GetComponent<ItemDrop>();

                        if (!oms.ContainsKey(item.m_itemData.m_shared.m_name))
                            oms.Add(item.m_itemData.m_shared.m_name, item.m_itemData.m_shared.m_maxStackSize);

                    }

                }
                catch (Exception ex)
                {
                    _player.Message(MessageHud.MessageType.TopLeft, $"VM Error in start: {ex.Message}", 0, (Sprite)null);


                }

                //wsl = twsl;
                //wsxp = twsxp;


                foreach (Skill sk in skills)
                {
                    sk.updateEffects();
                }


            }
            catch (Exception ex)
            {
                _player.Message(MessageHud.MessageType.TopLeft, $"VM Error in patching: {ex.Message}", 0, (Sprite)null);

                /*FileStream fs2 = new FileStream(errorfile, FileMode.OpenOrCreate, FileAccess.ReadWrite);


                string data = $"\n Error in patching: {ex.Source}, {ex.Message}, {ex.StackTrace}, {ex.InnerException} ";
                byte[] bytes = Encoding.UTF8.GetBytes(data);

                fs2.Write(bytes, 0, bytes.Length);

                fs2.Close();*/
            }
        }

        public static void PFU()
        {
            _player.Message(MessageHud.MessageType.TopLeft, $"PL FixedUpdate Patch", 0, (Sprite)null);

            //return true;
        }

        float elapsed = 0f;
        float elapsed2 = 0f;
        float elapsed3 = 0f;
        float elapsed4 = 0f;
        float elapsed5 = 0f;
        float elapsed6 = 0f;
        float elapsed7 = 0f;
        float savetime = 0f;
        public static bool cs = false;
        public bool us = true;
        public Vector3 ovel = new Vector3(0, 0, 0);

        float stc = 0f;
        float ostam = -99999f;

        bool showSkills = false;
        //Player _playert;
        public void Update()
        {
            try
            {
                if (Player.m_localPlayer != null)
                {


                    if (shouldInit)
                    {
                        shouldInit = false;
                        Initialize();
                    }

                    if (iscrafting)
                        iscrafting = false;

                    if (ostam == -99999f)
                    {
                        ostam = _player.GetStamina();
                        stc = 0f;
                    }
                    else if (ostam != _player.GetStamina())
                    {
                        if (ostam > _player.GetStamina())
                            stc += (ostam - _player.GetStamina());
                        else
                            ostam = _player.GetStamina();
                    }

                    elapsed6 += Time.deltaTime;

                    if (elapsed6 >= 30f)
                    {
                        elapsed6 = 0f;
                        ostam = _player.GetStamina();

                        Skill a = skills.Where(sk => sk.name.ToLower() == "agility").FirstOrDefault();
                        if (a != null)
                        {
                            int extra = a.level / 2;

                            if (extra < 1)
                                extra = 1;

                            a.xp = a.xp + 1 + (int)Mathf.Round(stc * 0.000025f * (extra));
                            a.updateEffects();
                        }

                        stc = 0f;
                    }

                    if (_player.GetSelectedPiece() != null)
                    {
                        //typeof(Player).GetField("m_placementStatus", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(_player, 0);
                        CraftingStation cr = _player.GetCurrentCraftingStation();
                        //_player.GetHoveringPiece()
                        //if (cr != null && cr.m_rangeBuild != 100f)
                        //    cr.m_rangeBuild = 100f;
                    }



                    if (_player.GetControlledShip() != null)
                    {
                        elapsed7 += Time.deltaTime;

                        Skill s = skills.Where(sk => sk.name.ToLower() == "sailing").FirstOrDefault();

                        if (elapsed7 >= 30f && _player.GetVelocity().magnitude > 1f)
                        {
                            elapsed7 = 0;

                            EnvSetup env = EnvMan.instance.GetCurrentEnvironment();

                            s.xp = s.xp + 1 + (int)(env.m_windMax / 5.0) + (int)(s.level * 0.8);
                        }
                        else if (_player.GetVelocity().magnitude <= 1f)
                        {
                            elapsed7 = 0;
                        }

                        if (_player.GetControlledShip().m_backwardForce != (0.5f + (s.level * 0.005f)))
                        {
                            Ship sh = _player.GetControlledShip();
                            sh.m_backwardForce = (0.5f + (s.level * 0.005f));
                            sh.m_sailForceFactor = (0.05f + (s.level * 0.0005f));
                            sh.m_stearForce = (0.5f + (s.level * 0.005f));
                            sh.m_force = 0.60f;
                            sh.m_waterImpactDamage = Mathf.Max(10.0f - (s.level * 0.07f), 3.0f);
                            sh.m_minWaterImpactForce = (3.5f + (s.level * 0.07f));
                            sh.m_minWaterImpactInterval = (10.0f + (s.level * 0.1f));

                            _player.Message(MessageHud.MessageType.TopLeft, $"Modified boat forces.", 0, (Sprite)null);
                        }
                    }

                    elapsed4 += Time.deltaTime;

                    if (elapsed4 >= 300f)
                    {

                        if (meaw)
                        {
                            EnvSetup env = EnvMan.instance.GetCurrentEnvironment();

                            float rnd;

                            if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.05f)
                                rnd = UnityEngine.Random.Range(50f, 150f);
                            else
                                rnd = UnityEngine.Random.Range(1.0f, 20.0f);

                            ZNetView znv = typeof(Player).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_player) as ZNetView;

                            znv.GetZDO().Set("VMMWS", rnd);
                            //env.m_windMax = rnd;

                            //typeof(EnvMan).GetField("m_currentEnv", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(EnvMan.instance, env);
                            //_player.Message(MessageHud.MessageType.TopLeft, $"The max wind speed has changed: {rnd}", 0, (Sprite)null);
                        }

                        elapsed4 = 0f;


                    }

                    /*if (cs)
                    {
                        elapsed3 += Time.deltaTime;
                    }
                    else
                    {
                        elapsed5 += Time.deltaTime;
                    }

                    if (cs && !us)
                        us = true;

                    if (_player.IsDead() && !cs)
                    {
                        //List<ItemDrop.ItemData> items = new List<ItemDrop.ItemData>();

                        //(Inventory).GetField("m_inventory", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(_player.GetInventory(), items);

                        //(typeof(Player).GetField("m_playerProfile", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_player) as PlayerProfile).SavePlayerData(_player);

                        _player = UnityEngine.Object.Instantiate<GameObject>(Game.instance.m_playerPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity).GetComponent<Player>();


                        //_player.m_name = "temp";
                        //_player.gameObject.
                        //_player = new Player();
                        //_player.m_name = "RS";
                        cs = true;
                    }
                    else if (cs && (bool)(UnityEngine.Object)Player.m_localPlayer && !Player.m_localPlayer.IsDead()) //elapsed3 > 15f)
                    {
                        //ZNetScene.instance.Destroy(_player.gameObject);
                        _player = Player.m_localPlayer;
                        _player.UnequipAllItems();
                        _player.GetInventory().RemoveAll();
                        //if (_player.m_name.Contains("VMM"))
                        //_player.m_name += "VMM";

                        cs = false;
                        elapsed3 = 0f;
                    }*/


                    if (!cs)
                    {
                        if (ovel.magnitude == 0)
                        {
                            ovel = _player.GetVelocity();
                        }

                        if (us)
                        {
                            foreach (Skill sk in skills)
                            {
                                sk.updateEffects();
                            }

                            us = false;
                        }

                        elapsed += Time.deltaTime;
                        elapsed2 += Time.deltaTime;


                        if (elapsed >= 30.0f && ovel.magnitude == 0f)
                        {
                            elapsed = 0f;
                        }
                        else if (elapsed >= 30.0f && ovel.magnitude > 0f)
                        {
                            elapsed = 0f;
                            ovel = new Vector3(0, 0, 0);

                            //_player = Player.m_localPlayer;

                            float ratio = (_player.GetInventory().GetTotalWeight() / _player.GetMaxCarryWeight());

                            if (ratio > 1.0f)
                            {
                                ratio = 1.0f;
                            }

                            Skill w = skills.Where(sk => sk.name.ToLower() == "weight").FirstOrDefault();

                            w.xp = w.xp + 1 + (int)(ratio * (float)w.level);


                            //File.WriteAllText(filename, $"{wsl},{wsxp}");

                            //File.Create(filename);


                            /*using (System.IO.StreamWriter file =
                        new System.IO.StreamWriter(filename, false))
                            {
                                file.WriteLine($"{wsl},{wsxp}");
                            */

                            //ZPackage z = new ZPackage();

                            //FileStream fs = File.OpenWrite(filename);

                            /*FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite);


                            string data = $"{wsl},{wsxp}";
                            byte[] bytes = Encoding.UTF8.GetBytes(data);

                            fs.Write(bytes, 0, bytes.Length);

                            fs.Close();
                            */

                            otime = Time.time;
                            //frame = Time.frameCount;
                            //Console.print($"Level {wsl}, XP {wsxp}");


                        }

                        if (elapsed2 >= 30.0f * Player.GetAllPlayers().Count)
                        {
                            elapsed2 = 0f;


                            try
                            {
                                //if (meaw)
                                //{
                                Character[] chars = GameObject.FindObjectsOfType(typeof(Character)) as Character[];
                                List<Character> chars2 = new List<Character>();

                                foreach (Character ch in chars)
                                {

                                    ZNetView znv = typeof(Character).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(ch) as ZNetView;
                                    //znv.GetZDO().Set("VMMML", $"{}");

                                    if (ch.IsMonsterFaction() && znv.GetZDO().GetString("VMMML") == "")
                                    {
                                        chars2.Add(ch);
                                    }


                                }

                                if (chars2.Count > 0)
                                {
                                    Character c = chars2[UnityEngine.Random.Range(0, chars2.Count - 1)];

                                    int lvl = UnityEngine.Random.Range(1, 8);

                                    if (lvl > c.GetLevel())
                                    {
                                        c.SetLevel(lvl);
                                        c.m_speed = c.m_speed * (1.0f + (lvl / 10.0f));
                                        

                                        /*CharacterDrop component2 = (CharacterDrop)((Component)c).GetComponent<CharacterDrop>();

                                        foreach (CharacterDrop.Drop item in component2.m_drops)
                                        {
                                            item.m_chance = lvl * 0.1f;
                                            item.m_levelMultiplier = false;
                                            item.m_amountMax += (lvl * 2);

                                        }*/

                                        ZNetView znv = typeof(Character).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(c) as ZNetView;
                                        znv.GetZDO().Set("VMMML", $"{lvl}");

                                    }

                                }




                                //}
                            }
                            catch (Exception ex)
                            {

                                /*List<string> cbt = typeof(Chat).GetField("m_chatBuffer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Chat.instance) as List<string>;
                                cbt.Add($"VM Error (Enemy Modifiers): {ex.Message}");
                                typeof(Chat).GetField("m_chatBuffer", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Chat.instance, cbt);
                                typeof(Chat).GetMethod("UpdateChat", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Chat.instance, new object[] { });*/

                                _player.Message(MessageHud.MessageType.TopLeft, $"VM Error(Enemy Modifiers): {ex.Message}", 0, (Sprite)null);
                            }



                        }

                        if (Input.GetKeyDown(KeyCode.K))
                        {
                            //_player.SetHealth(_player.GetHealth() + 1);
                            //_player.m_maxCarryWeight = 
                            //wsl += 1;
                        }

                        if (Input.GetKeyDown(KeyCode.L))
                        {
                            //_player.SetHealth(_player.GetHealth() - 1);
                            //_player.m_maxCarryWeight = 
                            //wsxp += 1;

                            /*object tm_wind = typeof(EnvMan).GetField("m_wind", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(EnvMan.instance);

                            Vector4 tmw = new Vector4();

                            if (tm_wind is Vector4 tm_w)
                                tmw = tm_w;

                            tmw.w = UnityEngine.Random.Range(5.0f, 25.0f);

                            typeof(EnvMan).GetField("m_wind", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(EnvMan.instance, tmw);
                            */
                            ZNetView znv = typeof(Player).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_player) as ZNetView;

                            znv.GetZDO().Set("VMMWS", 200.0f);


                        }

                        if (Input.GetKeyDown(KeyCode.O))
                        {
                            _player.SetHealth(_player.GetHealth() - 1);
                            //Console.print("You subtracted health.");
                        }

                        if (Input.GetKeyDown(KeyCode.I))
                        {
                            int count = 0;


                            foreach (Player pl in Player.GetAllPlayers())
                            {
                                ZNetView tznv = typeof(Player).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(pl) as ZNetView;
                                if (tznv.GetZDO().GetString("VMMHM") == "True")
                                {
                                    count++;
                                    break;
                                }
                            }



                            if (count == 0)
                            {
                                meaw = true;

                            }
                            else
                            {
                                meaw = false;
                            }

                            ZNetView znv = typeof(Player).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_player) as ZNetView;
                            znv.GetZDO().Set("VMMHM", $"{meaw}");

                            _player.Message(MessageHud.MessageType.TopLeft, $"Host modifications set to {meaw}. Check revealed {count} active host modifiers.", 0, (Sprite)null);
                            //Console.print("You subtracted health.");
                        }

                        if (Input.GetKeyDown(KeyCode.P))
                        {
                            //_player.SetHealth(_player.GetHealth() + 1);

                            try
                            {
                                //typeof(Chat).GetMethod("AddString", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Chat.instance, new object[] { $"Weight level: {wsl}, Weight XP: {wsxp}" });
                                /*List<string> cbt = typeof(Chat).GetField("m_chatBuffer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Chat.instance) as List<string>;
                                cbt.Add($"Weight level: {wsl}, Weight XP: {wsxp}, Required XP: {requiredXP()}");
                                typeof(Chat).GetField("m_chatBuffer", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Chat.instance, cbt);
                                typeof(Chat).GetMethod("UpdateChat", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Chat.instance, new object[] { });
                                Chat.instance.m_chatWindow.gameObject.SetActive(true);
                                //Reflection.GetMethod(Game1.currentLocation, "isMonsterDamageApplicable").Invoke<bool>(who, character, true)*/

                                //Skill w = skills.Where(sk => sk.name.ToLower() == "weight").FirstOrDefault();
                                //_player.Message(MessageHud.MessageType.TopLeft, $"Weight level: {w.level}, Weight XP: {w.xp}, Required XP: {w.requiredXP()}", 0, (Sprite)null);

                                if (showSkills)
                                    showSkills = false;
                                else
                                    showSkills = true;
                            }
                            catch
                            {

                            }
                        }
                        if (Input.GetKeyDown(KeyCode.U))
                        {
                            GameObject itemPrefab = ObjectDB.instance.GetItemPrefab("Wood");
                            
                            ZNetView.m_forceDisableInit = true;
                            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(itemPrefab);
                            ZNetView.m_forceDisableInit = false;
                            ItemDrop component = gameObject.GetComponent<ItemDrop>();
                            component.m_itemData.m_stack = 999;

                            _player.GetInventory().AddItem(component.m_itemData);
                        }

                        if (Input.GetKeyDown(KeyCode.Delete)) // Will just unload our DLL
                        {
                            Loader.Unload();
                        }

                        if (elapsed5 >= 20.0f)
                        {
                            elapsed5 = 0f;

                            if (!meaw)
                            {
                                ZNetView znv = typeof(Player).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_player) as ZNetView;

                                znv.GetZDO().Set("VMMWS", 0);
                            }

                            foreach (Player pl in Player.GetAllPlayers())
                            {
                                ZNetView znv = typeof(Player).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(pl) as ZNetView;

                                float rnd = znv.GetZDO().GetFloat("VMMWS");

                                if (rnd != 0)
                                {
                                    EnvSetup env = EnvMan.instance.GetCurrentEnvironment();
                                    env.m_windMax = rnd;

                                    typeof(EnvMan).GetField("m_currentEnv", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(EnvMan.instance, env);
                                    _player.Message(MessageHud.MessageType.TopLeft, $"The max wind speed has changed: {rnd}", 0, (Sprite)null);
                                }
                            }

                            Character[] chars3 = GameObject.FindObjectsOfType(typeof(Character)) as Character[];

                            foreach (Character ch in chars3)
                            {

                                ZNetView znv = typeof(Character).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(ch) as ZNetView;
                                //znv.GetZDO().Set("VMMML", $"{}");

                                if (ch.IsMonsterFaction() && znv.GetZDO().GetString("VMMML") != "" && !ch.m_name.Contains("VMM") && int.TryParse(znv.GetZDO().GetString("VMMML"), out int level))
                                {
                                    ch.m_name += $" (VMM: {level})";
                                }


                            }
                        }
                        else
                        {
                            elapsed5 += Time.deltaTime;
                        }
                    }

                    savetime += Time.deltaTime;

                    if (savetime >= 60f)
                    {
                        savetime = 0f;
                        SaveSkillData();
                    }


                }
                else if (!shouldInit)
                {
                    //shouldInit = true;
                    Uninitialize();
                }
            }
            catch (Exception ex)
            {
                //_player = Player.m_localPlayer;

                /*Player[] pls = GameObject.FindObjectsOfType<Player>();

                foreach (Player pl in pls)
                {
                    if (pl.GetPlayerName() == pln)
                    {
                        _player = pl;
                        break;
                    }
                }*/

                UnityEngine.Debug.LogWarning($"Error in Update: {ex.Message}");
            }


        }

        public void SaveSkillData()
        {
            int i = 1;

            IniData data = parser.ReadFile(filename);

            foreach (Skill sk in skills)
            {
                data[$"SkillData{i}"]["SkillLevel"] = sk.level.ToString();
                data[$"SkillData{i}"]["SkillXP"] = sk.xp.ToString();

                i++;
            }

            parser.WriteFile(filename, data);
        }


        public void OnGUI()
        {
            if (showSkills)
            {
                float ox = 40f;
                float oy = 140f;
                float sep = 70f;
                GUI.Label(new Rect(ox, oy, 100f, 50f), "Unidarkshin's Valheim Overhaul:");

                int i = 1;

                foreach (Skill skill in skills)
                {
                    GUI.Label(new Rect(ox, 40 + oy + (i * sep), 100f, 60f), $"{skill.name} -> \nLevel: {skill.level} \nXP: {skill.xp}/{skill.requiredXP()}");

                    i++;
                }
                //GUI.DrawTexture(new Rect(Screen.width / 2, Screen.height / 2, 150f, 50f), "GAME INJECTED"); // Should work and when injected you will see this text in the middle of the screen
            }
        }

    }

    /*[HarmonyPatch(typeof(WaterVolume), "CreateWave")]
    static class Application_loadedLevelName_Patch
    {
        static void Prefix(
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
                waveSpeed += 50000f;
                waveHeight += 50000f;
                waveLength += 50000f;
                sharpness += 50000f;
            }
            catch (Exception e)
            {
                //mod.Logger.Error(e.ToString());
                 
            }
        }
    }*/

    public class Skill
    {
        public string name;

        private int Level;
        public int level
        {
            get { return Level; }
            set
            {
                Level = value;

                updateEffects();
            }
        }

        private int XP;
        public int xp
        {
            get { return XP; }
            set
            {
                XP = value;

                checkForLevelUp();
            }
        }

        public Skill(string name, int level, int xp)
        {
            this.name = name;
            this.xp = xp;
            this.level = level;
        }

        private void checkForLevelUp()
        {
            try
            {
                int rxp = requiredXP();
                if (xp >= rxp)
                {
                    level += 1;
                    xp = xp - rxp;

                    Main._player.Message(MessageHud.MessageType.TopLeft, $"You leveled up your {name} skill to {level}!", 0, (Sprite)null);
                }
            }
            catch
            {

            }
        }

        public int requiredXP()
        {

            int extra = (int)(0.25 * level);

            if (extra < 1)
            {
                extra = 1;
            }

            return ((10 * level) + (level * level * extra));

        }

        public void updateEffects()
        {
            if (name.ToLower() == "weight")
            {
                updateStacks();
                Main._player.m_maxCarryWeight = 300f + (5f * (float)level);
            }
            if (name.ToLower() == "agility")
            {
                Main._player.m_staminaRegen = 5f + (level * 0.1f);
            }
        }

        public void updateStacks()
        {
            try
            {

                foreach (GameObject go in ObjectDB.instance.m_items)
                {


                    ItemDrop item = go.GetComponent<ItemDrop>();

                    if (item.m_itemData.m_shared.m_maxStackSize == 1)
                        continue;

                    if (Main.oms.TryGetValue(item.m_itemData.m_shared.m_name, out int ms))
                        item.m_itemData.m_shared.m_maxStackSize = ms * (1 + (level / 10));


                }

                List<ItemDrop.ItemData> items = Main._player.GetInventory().GetAllItems();

                foreach (ItemDrop.ItemData item in items)
                {
                    if (item.m_shared.m_maxStackSize == 1)
                        continue;

                    if (Main.oms.TryGetValue(item.m_shared.m_name, out int ms))
                        item.m_shared.m_maxStackSize = ms * (1 + (level / 10));

                }
                typeof(Inventory).GetField("m_inventory", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Main._player.GetInventory(), items);
            }
            catch
            {

            }
        }
    }
}


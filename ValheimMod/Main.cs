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
using UnityEngine.UI;
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
        public static string filename;
        string configname;
        string errorfile;

        public static Dictionary<string, int> oms = new Dictionary<string, int>();

        public static bool meaw = false;

        public static bool shouldInit = true;

        public static List<Skill> skills = new List<Skill>();

        public static string[] snames = { "Weight", "Agility", "Sailing", "Crafting", "Building" };

        /*[DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();*/

        public static configData cdata;

        //public static int WeightSkillId = 2123;
        //public static Skills.SkillType WeightSkill = (Skills.SkillType)Main.WeightSkillId;
        //public static Skills.SkillDef WeightSkillDef;

        public static int startSkillID = 2123;
        public static List<string> sDescs = new List<string> { "Moving with encumbrance over time will increase your max weight and stack size.", "Using large amounts of stamina allows you regenerate stamina faster.", "Continuous sailing makes your boat faster.", "Crafting items reduces your resource cost to make them.", "Building over time will allow you to build structures with less support." };
        public static List<skillDef> sDefs = new List<skillDef>();

        public void Awake()
        {
            UnityEngine.Debug.LogWarning("UVO Loading!");

            configname = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/VMU_Data" + $"/VM_Config.json";

            try
            {
                cdata = JsonUtility.FromJson<configData>(File.ReadAllText(configname));
                saveConfig();
                UnityEngine.Debug.LogWarning($"Loaded UVO configuration successfully!");
            }
            catch
            {
                cdata = new configData();
                saveConfig();
                UnityEngine.Debug.LogWarning($"UVO Config not found, new configuration created.");
            }

            invrowadd = cdata.extraInvRowsPlayer;

            for (int i = 0; i < snames.Length; i++)
            {
                sDefs.Add(new skillDef(startSkillID + i));
            }


            /*Texture2D texture = Texture2D.blackTexture;

            Main.WeightSkillDef = new Skills.SkillDef()
            {
                m_skill = (Skills.SkillType)Main.WeightSkillId,
                m_icon = Sprite.Create(Texture2D.blackTexture, new Rect(0.0f, 0.0f, (float)texture.width, (float)texture.height), new Vector2(0.5f, 0.5f)),
                m_description = "Moving with encumbrance over time will increase your max weight and stack size.",
                m_increseStep = 1f
            };*/

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

            /*h.Patch(
original: AccessTools.Method(typeof(InventoryGui), "DoCrafting"),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.DC))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.CW2))
);*/

            Type[] aitypes = { typeof(ItemDrop.ItemData) };

            /*h.Patch(
original: AccessTools.Method(typeof(Inventory), "AddItem", aitypes),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.AI))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.CW2))
);*/

            /*h.Patch(
original: AccessTools.Method(typeof(Inventory), "Save"),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.ISV))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.CW2))
);*/

            /*h.Patch(
original: AccessTools.Method(typeof(Inventory), "Load"),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.ILD))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.CW2))
);*/

            /*h.Patch(
original: AccessTools.Method(typeof(Inventory), "Load"),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.ILD)),
postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);*/

            /*h.Patch(
original: AccessTools.Method(typeof(ItemDrop), "DropItem"),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.IDI))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);*/

            /*h.Patch(
original: AccessTools.Method(typeof(ItemDrop), "LoadFromZDO"),
postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILZDO))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);
            h.Patch(
original: AccessTools.Method(typeof(ItemDrop), "SaveToZDO"),
postfix: new HarmonyMethod(typeof(Main), nameof(Main.ISZDO))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);*/

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
original: AccessTools.Method(typeof(Player), "HaveRequirements", new Type[] { typeof(Piece), typeof(Player.RequirementMode) }),
postfix: new HarmonyMethod(typeof(Main), nameof(Main.PHR))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);

            /*h.Patch(
original: AccessTools.Method(typeof(Inventory), "GetAllItems", new Type[] { typeof(string), typeof(List<ItemDrop.ItemData>) }),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.IGAI))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);*/

            h.Patch(
original: AccessTools.Method(typeof(WearNTear), "OnPlaced", new Type[] { }),
postfix: new HarmonyMethod(typeof(Main), nameof(Main.WOP))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);

            /*h.Patch(
original: AccessTools.Method(typeof(WearNTear), "GetMaterialProperties"),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.WGMP))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);*/

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

            h.Patch(
original: AccessTools.Method(typeof(Character), "Jump", new Type[] { }),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.CJ))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);

            /*h.Patch(
original: AccessTools.Method(typeof(ObjectDB), "GetRecipe", new Type[] { typeof(ItemDrop.ItemData) }),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.OGR))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);*/

            h.Patch(
original: AccessTools.Method(typeof(Player), "UpdateCrouch", new Type[] { typeof(float) }),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.PUC))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);

            h.Patch(
original: AccessTools.Method(typeof(Inventory), "UpdateTotalWeight", new Type[] { }),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.IUTW))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);

            h.Patch(
original: AccessTools.Method(typeof(Chat), "InputText", new Type[] { }),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.CIT))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);

            h.Patch(
original: AccessTools.Method(typeof(CraftingStation), "Start", new Type[] { }),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.CSS))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);

            /*h.Patch(
original: AccessTools.Method(typeof(ItemDrop.ItemData), "GetTooltip", new Type[] { typeof(ItemDrop.ItemData), typeof(int), typeof(bool) }),
postfix: new HarmonyMethod(typeof(Main), nameof(Main.IDGTT))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);*/

            h.Patch(
original: AccessTools.Method(typeof(Character), "Awake", new Type[] { }),
postfix: new HarmonyMethod(typeof(Main), nameof(Main.CAW))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);

            h.Patch(
original: AccessTools.Method(typeof(Humanoid), "Awake", new Type[] { }),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.HAW))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);

            h.Patch(
original: AccessTools.Method(typeof(Container), "Awake", new Type[] { }),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.CTAW))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);

            /*            h.Patch(
            original: typeof(Inventory).GetConstructor(new Type[] { typeof(string), typeof(Sprite), typeof(int), typeof(int)}),
            postfix: new HarmonyMethod(typeof(Main), nameof(Main.II))
            //postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
            );*/

            /*h.Patch(
original: AccessTools.Method(typeof(InventoryGui), "Show", new Type[] { typeof(Container) }),
postfix: new HarmonyMethod(typeof(Main), nameof(Main.IGS))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);*/

            /*            h.Patch(
            original: typeof(InventoryGrid).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, new Type[] { }, new ParameterModifier[] { }),
            postfix: new HarmonyMethod(typeof(Main), nameof(Main.IGA))
            //postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
            );*/

            //UnityEngine.UI.Scrollbar scr = new UnityEngine.UI.Scrollbar();
            //ZNet.instance.m_serverPlayerLimit = 99;

            h.Patch(
original: AccessTools.Method(typeof(Skills), "GetSkillDef", new Type[] { typeof(Skills.SkillType) }),
postfix: new HarmonyMethod(typeof(Main), nameof(Main.SGSD))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);

            h.Patch(
original: AccessTools.Method(typeof(PlayerProfile), "LoadPlayerData", new Type[] { typeof(Player) }),
postfix: new HarmonyMethod(typeof(Main), nameof(Main.PPLPD))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);

            h.Patch(
original: AccessTools.Method(typeof(Localization), "SetupLanguage", new Type[] { typeof(string) }),
postfix: new HarmonyMethod(typeof(Main), nameof(Main.LSL))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);

            h.Patch(
original: AccessTools.Method(typeof(PlayerProfile), "SavePlayerData", new Type[] { typeof(Player) }),
postfix: new HarmonyMethod(typeof(Main), nameof(Main.PPSPD))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);

            h.Patch(
original: AccessTools.Method(typeof(Skills), "IsSkillValid", new Type[] { typeof(Skills.SkillType) }),
prefix: new HarmonyMethod(typeof(Main), nameof(Main.SISV))
//postfix: new HarmonyMethod(typeof(Main), nameof(Main.ILD2))
);

        }

        public static bool SISV(Skills __instance, Skills.SkillType type, ref bool __result)
        {

            foreach (skillDef sd in sDefs)
            {
                if (type == sd.sType)
                {
                    __result = true;
                    return false;
                }
                    
            }

            return true;
        }


        public static void PPSPD(PlayerProfile __instance, Player player)
        {

            saveSkillData();

        }


        public static void LSL(Localization __instance, string language)
        {
            if (!(language == "English"))
                return;

            foreach (skillDef sd in sDefs)
            {

                AccessTools.Method(typeof(Localization), "AddWord", (System.Type[])null, (System.Type[])null).Invoke((object)__instance, new object[2]
                {
        (object) $"skill_{sd.ID}",
        (object) sd.name
                });
            }
        }


        public static void SGSD(
      Skills __instance,
      Skills.SkillType type,
      ref Skills.SkillDef __result)
        {
            foreach (skillDef sd in sDefs)
            {
                if (type == sd.sType)
                {
                    __result = sd.sDef;
                    break;
                }
            }
        }

        public static void PPLPD(PlayerProfile __instance, Player player)
        {
            SkillData sk = loadSkillData(WeightSkillId);
            Skills.Skill skill = (Skills.Skill)AccessTools.Method(typeof(Skills), "GetSkill", (System.Type[])null, (System.Type[])null).Invoke((object)player.GetSkills(), new object[1]
            {
        (object) WeightSkill
            });
            skill.m_level = (float)sk.Level;
            skill.m_accumulator = sk.Progress;

        }

        public static bool CTAW(ref Container __instance, ref int ___m_width, ref int ___m_height)
        {
            try
            {
                if (__instance == null || !cdata.overrideChestInventorySize || cdata.overrideInvWidthChest == 0 || cdata.overrideInvHeightChest == 0)
                    return true;


                ___m_width = cdata.overrideInvWidthChest;
                ___m_height = cdata.overrideInvHeightChest;

                return true;

            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning("CTAW failed: " + ex.ToString());
                return true;
            }
        }

        public static bool HAW(ref Humanoid __instance)
        {
            try
            {
                if (!__instance.IsPlayer())
                    return true;


                typeof(Humanoid).GetField("m_inventory", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, new Inventory("Inventory", (Sprite)null, 8, 4 + cdata.extraInvRowsPlayer));


                return true;

            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning("HAW failed: " + ex.ToString());
                return true;
            }
        }

        public static void IGS(Container container)
        {


        }

        public static void IGA(ref Scrollbar ___m_scrollbar)
        {
            //___m_width = 16;
            //___m_height = 4;

            //___m_scrollbar.gameObject.SetActive(true);
            //___m_scrollbar.enabled = true;

        }

        public static int invrowadd = 4;

        public static void II(ref int ___m_width, ref int ___m_height, ref Sprite ___m_bkg, string name, Sprite bkg, int w, int h)
        {
            try
            {
                ___m_width = 8;
                ___m_height = 4 + invrowadd;
            }
            catch (Exception ex)
            {

                UnityEngine.Debug.LogWarning("II failed: " + ex.ToString());
            }
            //___m_bkg = GameObject.FindObjectsOfType<Container>()[0].GetInventory().GetBkg();


        }

        public static void CAW(ref Character __instance, ref ZNetView ___m_nview)
        {
            try
            {
                if (!cdata.strongerMonsters || __instance == null || !__instance.IsMonsterFaction() || __instance.m_name.Contains(" (UVO") || ___m_nview.GetZDO().GetBool("cmod", false))
                    return;

                ___m_nview.GetZDO().Set("cmod", true);

                string on = __instance.m_name;
                int ol = __instance.GetLevel();
                int lev;

                if (__instance.m_name.ToLower().Contains("blob"))
                    lev = getMonsterUpgrade(ol, 4, 0.5f);
                else
                    lev = getMonsterUpgrade(ol, 8);

                __instance.SetLevel(lev);
                __instance.m_speed = __instance.m_speed * (1.0f + (lev / 10.0f));

                //ZNetView znv = typeof(Character).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) as ZNetView;

                //__instance.SetLevel(lev);

                //UnityEngine.Debug.LogWarning($"Enemy: {__instance.m_name} upgraded to level {lev}.");

                UnityEngine.Debug.LogWarning($"Updating creature: {on}, {ol} --> {__instance.m_name}, {__instance.GetLevel()}");
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning("CAW failed: " + ex.ToString());
            }
        }

        public static string[] ams = { "Armor", "Attack", "Backstab", "BlockPower", "Blunt", "Chop", "Damage", "Fire", "Frost", "Lightning",
            "Pickaxe", "Pierce", "Poison", "Slash" , "Spirit", "Deflection", "Durability Drain", "Durability" , "Movement", "ParryBonus", "Use Durability Drain", "Weight"};

        public static void IDGTT(ref string __result, ref ItemDrop.ItemData item, int qualityLevel, bool crafting)
        {
            try
            {
                //if (!cdata.showExtraItemAttrs)
                return;

                ItemDrop.ItemData.ItemType it = item.m_shared.m_itemType;
                int type;

                if (it == ItemDrop.ItemData.ItemType.Bow || it == ItemDrop.ItemData.ItemType.OneHandedWeapon || it == ItemDrop.ItemData.ItemType.TwoHandedWeapon || it == ItemDrop.ItemData.ItemType.Tool)
                    type = 1;
                else if (it == ItemDrop.ItemData.ItemType.Chest || it == ItemDrop.ItemData.ItemType.Hands || it == ItemDrop.ItemData.ItemType.Helmet || it == ItemDrop.ItemData.ItemType.Legs || it == ItemDrop.ItemData.ItemType.Shoulder || it == ItemDrop.ItemData.ItemType.Shield)
                    type = 2;
                else
                    return;

                List<float> attr = getAttr(item.m_shared);

                for (int i = 0; i < ams.Length; i++)
                {
                    if (attr[i] != 0 && i != 1 && !__result.ToLower().Contains(ams[i].ToLower()))
                        __result += $"\n{ams[i]}: <color=blue>{attr[i]}</color>";
                }

            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning("IDGTT failed: " + ex.ToString());
            }
        }

        public static bool CFU(ref Character __instance, ref ZNetView ___m_nview)
        {
            try
            {

                //UnityEngine.Debug.Log($"CSL attempting monster incr.");

                /*if (!__instance.IsMonsterFaction())
                    return true;

                int vml = ___m_nview.GetZDO().GetInt("VMMML", -1);

                if (vml >= 0)
                    return true;

                int olev = __instance.GetLevel();
                int lev = olev;

                if (olev < 4)
                {
                    lev = getMonsterUpgrade(olev);
                    __instance.SetLevel(lev);
                }
                    ___m_nview.GetZDO().Set("VMMML", lev);*/

                //UnityEngine.Debug.Log($"CSL monster incr. {__instance.GetHoverName()}, {lev}");

                return true;

            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning("CSL failed: " + ex.ToString());
                return true;
            }
        }

        public static int getMonsterUpgrade(int level, int maxLevel, float mod = 1.0f)
        {
            int newlevel = level;

            for (int i = newlevel; i < maxLevel; i++)
            {
                if (UnityEngine.Random.value <= (cdata.initialMonsterUpgradeChance + Mathf.Min(Player.GetAllPlayers().Count * cdata.bonusMonsterUpgradeChancePerPlayer, cdata.maxPlayerBonusMonsterUpgradeChance)) * mod)
                {
                    newlevel = i + 1;
                }
                else
                {
                    break;
                }
            }

            return newlevel;
        }


        public static void CSS(ref CraftingStation __instance)
        {
            try
            {
                if (!cdata.lesserCraftingRestrictions)
                    return;

                __instance.m_craftRequireFire = false;
                __instance.m_craftRequireRoof = false;
                __instance.m_rangeBuild = 100f;


            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning("CSS failed: " + ex.ToString());
            }

        }

        public static bool CIT(ref Chat __instance)
        {
            try
            {
                string text = __instance.m_input.text;
                string[] args;
                string[] oargs;

                if (text[0] == '/')
                {
                    args = text.Substring(1).ToLower().Split(',');
                    oargs = text.Substring(1).Split(',');

                    if (args.Length < 1)
                        return true;
                }
                else
                {
                    return true;
                }

                if (args[0] == "deleteaiog")
                {
                    foreach (ItemDrop item in GameObject.FindObjectsOfType<ItemDrop>())
                    {
                        ZNetView tznv = typeof(ItemDrop).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(item) as ZNetView;
                        tznv.Destroy();
                    }

                    return false;
                }
                else if (args[0] == "tpto" && args.Length > 1 && _player != null)
                {
                    foreach (ZNet.PlayerInfo pl in ZNet.instance.GetPlayerList())
                    {
                        if (args[1] == pl.m_name.ToLower())
                        {
                            _player.TeleportTo(pl.m_position + new Vector3(0f, 1.5f, 0f), _player.transform.rotation, true);

                            break;
                        }
                    }

                    return false;
                }
                else if (args[0] == "tptoc" && args.Length == 3 && int.TryParse(args[1], out int x) && int.TryParse(args[2], out int z))
                {
                    Vector3 pos = new Vector3(x, _player.transform.position.y, z);

                    if (Physics.Raycast(new Ray(_player.transform.position - new Vector3(0f, 1.0f, 0f), -1 * _player.transform.up), out RaycastHit hit))
                    {
                        pos.y = hit.point.y + 1.0f;
                        _player.TeleportTo(pos, _player.transform.rotation, true);
                    }

                    return false;
                }
                else if (args[0] == "giveitemtp" && args.Length > 2 && _player != null)
                {
                    Player p = null;

                    foreach (Player pl in Player.GetAllPlayers())
                    {
                        if (args[1] == pl.GetPlayerName().ToLower())
                        {
                            p = pl;
                            break;
                        }
                    }

                    if (p == null)
                        return false;

                    GameObject itemPrefab = ObjectDB.instance.GetItemPrefab(oargs[2]);

                    if (itemPrefab == null)
                        return false;

                    int stack = 1;

                    if (args.Length > 3 && int.TryParse(args[3], out int st))
                        stack = st;
                    else
                        return false;

                    ZNetView.m_forceDisableInit = true;
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(itemPrefab);
                    ZNetView.m_forceDisableInit = false;

                    if (gameObject == null)
                        return false;

                    ItemDrop component = gameObject.GetComponent<ItemDrop>();

                    if (component == null)
                        return false;

                    component.m_itemData.m_stack = st;

                    p.GetInventory().AddItem(component.m_itemData);


                    return false;
                }
                else if (args[0] == "giveitem" && args.Length > 1 && _player != null)
                {

                    GameObject itemPrefab = ObjectDB.instance.GetItemPrefab(oargs[1]);

                    if (itemPrefab == null)
                        return false;

                    int stack = 1;

                    if (args.Length > 2 && int.TryParse(args[2], out int st))
                        stack = st;
                    else
                        return false;

                    ZNetView.m_forceDisableInit = true;
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(itemPrefab);
                    ZNetView.m_forceDisableInit = false;

                    if (gameObject == null)
                        return false;

                    ItemDrop component = gameObject.GetComponent<ItemDrop>();

                    if (component == null)
                        return false;

                    component.m_itemData.m_stack = st;

                    _player.GetInventory().AddItem(component.m_itemData);

                    return false;
                }
                else if (args[0] == "deleteac")
                {
                    foreach (Character c in GameObject.FindObjectsOfType<Character>())
                    {
                        if (c.IsMonsterFaction())
                        {
                            ZNetScene.instance.Destroy(c.gameObject);
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning("CIT failed: " + ex.ToString());
                return true;
            }
        }

        public static bool IUTW(ref Inventory __instance, ref float ___m_totalWeight)
        {
            try
            {
                Skill a = skills.Where(sk => sk.name.ToLower() == "agility").FirstOrDefault();

                if (a == null)
                    return true;

                ___m_totalWeight = 0.0f;
                foreach (ItemDrop.ItemData itemData in __instance.GetAllItems())
                {
                    if (!itemData.m_equiped)
                        ___m_totalWeight += itemData.GetWeight();
                    else
                        ___m_totalWeight += (itemData.GetWeight() / (1.0f + (a.level * 0.04f)));
                }

                return false;

            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning("IUTW failed: " + ex.ToString());
                return true;
            }
        }

        public static bool PUC(ref Player __instance, ref bool ___m_crouchToggled, ref bool ___m_run, ref int ___crouching, ref ZSyncAnimation ___m_zanim, ref float dt)
        {
            try
            {
                if (__instance == null)
                    return true;

                if (___m_crouchToggled)
                {
                    if (!__instance.HaveStamina(0.0f) || __instance.IsSwiming() || (__instance.InBed() || __instance.InPlaceMode()) || (___m_run || __instance.IsBlocking() || __instance.IsFlying()))
                        ___m_crouchToggled = false;
                    //bool flag = __instance.InAttack() || __instance.IsHoldingAttack();
                    ___m_zanim.SetBool(___crouching, ___m_crouchToggled); //&& !flag);
                }
                else
                    ___m_zanim.SetBool(___crouching, false);

                return false;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning("PUC failed: " + ex.ToString());
                return true;
            }
        }

        public static bool OGR(ref ObjectDB __instance, ref Recipe __result, ItemDrop.ItemData item)
        {
            try
            {
                if (!item.m_shared.m_name.Contains(" (UVO"))
                    return true;

                __result = (Recipe)null;

                string str = item.m_shared.m_name.Substring(0, item.m_shared.m_name.IndexOf(" (UVO"));
                foreach (Recipe recipe in __instance.m_recipes)
                {
                    if (!((UnityEngine.Object)recipe.m_item == (UnityEngine.Object)null) && recipe.m_item.m_itemData.m_shared.m_name == str)
                    {
                        __result = recipe;
                        return false;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning("OGR failed: " + ex.ToString());
                return true;
            }
        }


        public static int jc = 0;

        public static void CJ(ref Character __instance, ref float ___m_lastGroundTouch, ref float ___m_maxAirAltitude, ref float ___m_jumpForce)
        {
            try
            {
                if (!__instance.IsPlayer())
                    return;

                /*if (jc > 0 && __instance.IsOnGround())
                    jc = 0;

                if (jc <= getMaxJumps())
                {
                    ___m_maxAirAltitude = __instance.transform.position.y;
                    ___m_lastGroundTouch = 0.1f;

                    if (jc == 0)
                        ___m_jumpForce = 10f;
                    else
                        ___m_jumpForce = 5f;

                    jc++;
                }*/
                Skill a = skills.Where(sk => sk.name.ToLower() == "agility").FirstOrDefault();

                if (a == null)
                    return;

                //___m_jumpForce = 10f + (a.level / 2);
            }
            catch (Exception ex)
            {

                UnityEngine.Debug.LogWarning("CJ failed: " + ex.ToString());
            }
        }

        public static int getMaxJumps()
        {
            Skill a = skills.Where(sk => sk.name.ToLower() == "agility").FirstOrDefault();

            return (a.level / 2);
        }

        public static ItemDrop.ItemData cupgitem = null;

        public static void IRI(ItemDrop.ItemData item)
        {
            try
            {

                if (iscrafting && item.m_shared.m_name.Contains(" (UVO"))
                    cupgitem = item;
                else
                    cupgitem = null;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning("IRI failed: " + ex.ToString());
            }
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
                UnityEngine.Debug.LogWarning("IGAI failed: " + ex.ToString());
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
                        minSupport = 10f / (1.0f + (b.level * 0.01f));
                        verticalLoss = 0.125f / (1.0f + (b.level * 0.01f));
                        horizontalLoss = 0.2f / (1.0f + (b.level * 0.01f));
                        //UnityEngine.Debug.LogWarning($"WGMP W: maxs = {maxSupport}, mins = {minSupport}");
                        break;
                    case WearNTear.MaterialType.Stone:
                        maxSupport = 1000f * (1.0f + (b.level * 0.01f));
                        minSupport = 50f / (1.0f + (b.level * 0.01f));
                        verticalLoss = 0.125f / (1.0f + (b.level * 0.012f));
                        horizontalLoss = 1f / (1.0f + (b.level * 0.01f));
                        break;
                    case WearNTear.MaterialType.Iron:
                        maxSupport = 1500f * (1.0f + (b.level * 0.01f));
                        minSupport = 20f / (1.0f + (b.level * 0.01f));
                        verticalLoss = 0.07692308f / (1.0f + (b.level * 0.01f));
                        horizontalLoss = 0.07692308f / (1.0f + (b.level * 0.01f));
                        break;
                    case WearNTear.MaterialType.HardWood:
                        maxSupport = 140f * (1.0f + (b.level * 0.01f));
                        minSupport = 10f / (1.0f + (b.level * 0.01f));
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

                b.xp = b.xp + (int)(xp * cdata.buildingXPModifier);

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
                if (!cdata.removeBuildRestrictions)
                    return;

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
                if (!cdata.removeBuildRestrictions)
                    return;
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
            //return true;

            try
            {
                if (!cdata.adjustMonsterDrops)
                    return true;

                Character c = __instance.GetComponent<Character>();

                ZNetView znv = typeof(Character).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(c) as ZNetView;
                int vml = znv.GetZDO().GetInt("level", 0);
                int level;

                if (vml < 4)
                    return true;
                else
                    level = vml;

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
                            {
                                num2 = (int)(num2 * cdata.itemDropAmountModifier);

                                keyValuePairList.Add(new KeyValuePair<GameObject, int>(drop.m_prefab, num2));
                            }
                        }
                    }
                }

                if (UnityEngine.Random.value <= (0.001f * level * level) * cdata.uniqueItemChanceModifier)
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

                //bool repairable = zdo.GetBool($"attr22", itemData.m_shared.m_canBeReparied);

                setAttr(ref itemData, attr);
                itemData.m_shared.m_name = zdo.GetString("attriname", itemData.m_shared.m_name);

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

                zdo.Set("attriname", itemData.m_shared.m_name);

                //zdo.Set($"attr22", itemData.m_shared.m_canBeReparied);


            }
            catch (Exception ex)
            {

                UnityEngine.Debug.LogWarning("ISZDO failed: " + ex.ToString());
            }
        }

        public static bool IDI(ref ItemDrop __result, ref ItemDrop.ItemData item, int amount, Vector3 position, Quaternion rotation)
        {
            try
            {

                if (!item.m_crafterName.Contains(" (UVO"))
                    return true;

                ItemDrop component = UnityEngine.Object.Instantiate<GameObject>(item.m_dropPrefab, position, rotation).GetComponent<ItemDrop>();
                //setAttr(ref component.m_itemData, getAttr(item.m_shared));
                component.m_itemData = item;
                //if (amount > 0)
                //    component.m_itemData.m_stack = amount;

                ZNetView znv = typeof(ItemDrop).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(component) as ZNetView;

                //AccessTools.Method(typeof(ItemDrop), "SaveToZDO").Invoke(component, new object[] { component.m_itemData,  znv.GetZDO() });
                //typeof(ItemDrop).GetMethod("SaveToZDO", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(component, new object[] { component.m_itemData, znv.GetZDO() });
                if (znv == null)
                    return true;

                ItemDrop.SaveToZDO(component.m_itemData, znv.GetZDO());

                __result = component;



                //UnityEngine.Debug.LogWarning($"item drop: {znv.GetZDO().GetString("attriname", "..")}, {znv.GetZDO().GetFloat("attr3", -56.2f)}");

                return false;

                //__result.m_itemData = item;
                //AccessTools.Method(typeof(ItemDrop), "Save").Invoke(__result, new object[] { });

                //UnityEngine.Debug.LogWarning("Updated and saved " + __result.m_itemData.m_shared.m_name);
            }
            catch (Exception ex)
            {

                UnityEngine.Debug.LogWarning("IDI Error: " + ex.ToString());
                return true;
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
                    if (item.m_crafterName.Contains(" (UVO"))
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

        public static void addArmorModsToHit(ref HitData hit, ItemDrop.ItemData.SharedData m_shared)
        {

            //hit.m_backstabBonus += m_shared.m_backstabBonus;
            hit.m_damage.m_blunt += m_shared.m_damages.m_blunt;
            hit.m_damage.m_chop += m_shared.m_damages.m_chop;
            hit.m_damage.m_damage += m_shared.m_damages.m_damage;
            hit.m_damage.m_fire += m_shared.m_damages.m_fire;
            hit.m_damage.m_frost += m_shared.m_damages.m_frost;
            hit.m_damage.m_lightning += m_shared.m_damages.m_lightning;
            hit.m_damage.m_pickaxe += m_shared.m_damages.m_pickaxe;
            hit.m_damage.m_pierce += m_shared.m_damages.m_pierce;
            hit.m_damage.m_poison += m_shared.m_damages.m_poison;
            hit.m_damage.m_slash += m_shared.m_damages.m_slash;
            hit.m_damage.m_spirit += m_shared.m_damages.m_spirit;
            //hit.m_pushForce += m_shared.m_deflectionForce;

        }

        public static void DDM(Destructible __instance, long sender, ref HitData hit)
        {
            try
            {
                if (!cdata.damageModifiers)
                    return;

                foreach (ItemDrop.ItemData item in _player.GetInventory().GetEquipedtems())
                {
                    if (item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Chest || item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Hands || item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Helmet || item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Legs || item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Shoulder || item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Shield)
                    {
                        addArmorModsToHit(ref hit, item.m_shared);
                    }
                }

                List<Skills.Skill> gskills = _player.GetSkills().GetSkillList();

                if (__instance.m_destructibleType == DestructibleType.Tree)
                {
                    Skills.Skill skl = gskills.Where(sk => sk.m_info.m_skill == Skills.SkillType.WoodCutting).FirstOrDefault();

                    if (skl == null)
                        return;

                    if (UnityEngine.Random.Range(0.0f, 1.0f) < (skl.m_level * 0.003f))
                    {
                        hit.m_damage.m_damage += hit.m_damage.GetTotalDamage();

                        if (__instance.TryGetComponent<TreeLog>(out TreeLog t))
                        {

                            List<GameObject> dropList = t.m_dropWhenDestroyed.GetDropList();

                            if (dropList.Count > 0)
                            {
                                int index = UnityEngine.Random.Range(0, dropList.Count);

                                Vector3 position = t.transform.position + t.transform.up * UnityEngine.Random.Range(-t.m_spawnDistance, t.m_spawnDistance) + Vector3.up * 0.3f * (float)index;
                                Quaternion rotation = Quaternion.Euler(0.0f, (float)UnityEngine.Random.Range(0, 360), 0.0f);
                                UnityEngine.Object.Instantiate<GameObject>(dropList[index], position, rotation);
                            }
                        }
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

                        if (__instance.TryGetComponent<MineRock>(out MineRock t))
                        {

                            List<GameObject> dropList = t.m_dropItems.GetDropList();

                            if (dropList.Count > 0)
                            {
                                int index = UnityEngine.Random.Range(0, dropList.Count);

                                UnityEngine.Object.Instantiate<GameObject>(dropList[index], hit.m_point - hit.m_dir * 0.2f + UnityEngine.Random.insideUnitSphere * 0.3f, Quaternion.identity);
                            }
                        }

                        if (__instance.TryGetComponent<MineRock5>(out MineRock5 t2))
                        {

                            List<GameObject> dropList = t2.m_dropItems.GetDropList();

                            if (dropList.Count > 0)
                            {
                                int index = UnityEngine.Random.Range(0, dropList.Count);

                                UnityEngine.Object.Instantiate<GameObject>(dropList[index], hit.m_point + UnityEngine.Random.insideUnitSphere * 0.3f, Quaternion.identity);
                            }
                        }
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
                            if (_player.IsCrouching())
                                hit.m_damage.m_damage += (hit.m_damage.GetTotalDamage() * 2.5f);
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

                waveFactor = 1f + ((env.m_windMax * 0.06f) * cdata.waveFactorMultiplier);
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
                if (!iscrafting)// || !cdata.itemRarityAndRandomization)
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

                int r;
                //int r = 8;

                //int oir = 0;

                int or = 0;
                if (cupgitem != null)
                {
                    string sub = cupgitem.m_shared.m_name.Remove(cupgitem.m_shared.m_name.Length - 1);

                    if (int.TryParse(sub.Substring(sub.IndexOf("UVO: ") + 6), out int orar))
                        or = orar;

                    //float chance = keepOldR(oir, c.level, cupgitem.m_quality);

                    //UnityEngine.Debug.LogWarning($"OIR: sub: {sub}, {oir}");

                    r = GenerateItemRarity(or, c.level, cupgitem.m_quality);

                    if (r > 1)
                    {
                        //oir = 0;
                        r = r + or; // + UnityEngine.Random.Range(0, 2);
                    }


                    List<float> attr = getAttr(cupgitem.m_shared);

                    setAttr(ref item, attr);

                    item.m_shared.m_name = cupgitem.m_shared.m_name;

                }
                else
                {
                    r = GenerateItemRarity(or, c.level, 1);
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

                        item.m_shared.m_weight = rndf2(item.m_shared.m_weight + (1 + UnityEngine.Random.Range(0, r + 1)));
                        //item.m_durability = item.m_shared.m_maxDurability;

                        if (UnityEngine.Random.value < Mathf.Min(r * r * 0.0015f, 0.25f))
                            item.m_shared.m_toolTier += 1;
                    }
                    else if (type == 2)
                    {

                        item.m_shared.m_armor = rndf2(item.m_shared.m_armor * UnityEngine.Random.Range(1.0f, 1.0f + (r * r * .0078f)));
                        item.m_shared.m_attackForce = rndf2(item.m_shared.m_attackForce * (UnityEngine.Random.Range(1.0f, 1.0f + (r * r * .01f))));
                        item.m_shared.m_backstabBonus = rndf2(item.m_shared.m_backstabBonus * UnityEngine.Random.Range(1.0f, 1.0f + (r * r * .01f)));
                        item.m_shared.m_blockPower = rndf2(item.m_shared.m_blockPower * UnityEngine.Random.Range(1.0f, 1.0f + (r * r * .01f)));


                        //int cbr = UnityEngine.Random.Range(0, 2);

                        //if (cbr == 0)
                        //    item.m_shared.m_canBeReparied = false;
                        //else
                        //    item.m_shared.m_canBeReparied = true;

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

                        item.m_shared.m_weight = rndf2(item.m_shared.m_weight + (1 + UnityEngine.Random.Range(0, r + 1)));


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


                }
                else
                {
                    if (item.m_shared.m_name.Contains(" (UVO"))
                    {
                        item.m_shared.m_name = item.m_shared.m_name.Substring(0, item.m_shared.m_name.IndexOf(" (UVO"));
                        int newr = or;

                        //if (cdata.allowUpgradeFailRarityDegradation)
                        newr = Mathf.Max(or - UnityEngine.Random.Range(1, 3), 1);

                        string str = $" (UVO: R{newr})";
                        item.m_shared.m_name += str;

                        /*if (type == 1)
                            item.m_shared.m_damages.Modify(cdata.upgradeFailDamageReduction);
                        else if (type == 2)
                            item.m_shared.m_armor = Mathf.Round(item.m_shared.m_armor * cdata.upgradeFailArmorReduction);*/
                    }
                }

                if (or == 0)
                    xp = xp + (int)(r * r * 0.5);
                else
                    xp = xp + (int)(r * r * (0.5 * or));

                c.xp = c.xp + (int)(xp * cdata.craftingXPModifier);

                cupgitem = null;
                iscrafting = false;

                item.m_durability = item.m_shared.m_maxDurability;
                item.m_shared.m_canBeReparied = true;

                if (item.m_shared.m_name.Contains(" (UVO"))
                {
                    if (item.m_crafterName.Contains(" (UVO"))
                        item.m_crafterName = item.m_crafterName.Substring(0, item.m_crafterName.IndexOf(" (UVO"));

                    item.m_crafterName += item.m_shared.m_name.Substring(item.m_shared.m_name.IndexOf(" (UVO"));
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning($"Fail in AI patch: {ex.ToString()}");
            }
        }

        public static float keepOldR(int or, int cl, int q)
        {
            //float chance = 0f;

            /*if (or < 13)
            {
                chance = (0.70f - (or * 0.05f)) - (0.1f * q) + (cl * 0.0010f);
            }
            else
            {
                chance = 0.1f + (cl * 0.0010f);
            }*/

            float chance = (1.0f / (Mathf.Max(or * or * 0.4f, 1.5f) * (0.5f * (q + 1.0f)))) * Mathf.Min(1.0f + (cl * 0.004f), 1.4f);

            return chance;
        }

        public static void setAttr(ref ItemDrop.ItemData item, List<float> attr)
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
            configname = "";
            errorfile = "";

            //oms = new Dictionary<string, int>();

            meaw = false;

            shouldInit = true;

            skills = new List<Skill>();

            //cdata = new configData();
        }

        public static int GenerateItemRarity(int or, int level, int quality)
        {
            float rnd = UnityEngine.Random.value;

            int r = 1;

            /*for (int i = 2; i < cdata.maxItemRarityValue; i++)
            {
   

                if ((rnd <= ((1.0f / ((i * i * (1.0f + (i * 0.1f))) * cdata.itemRarityChanceBaseModifier)) * (Mathf.Min(1.0f + (0.005f * level), 1.5f) * cdata.itemRarityChanceCraftingLevelModifier) * (Mathf.Min(1.0f + ((quality - 1) * 0.1f), 1.5f) * cdata.itemRarityChanceItemQualityModifier))))
                {
                    r = i;
                }
                else
                {
                    break;
                }

            }*/

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


                foreach (ZNet.PlayerInfo pl in ZNet.instance.GetPlayerList())
                {
                    ZDO zd2 = ZDOMan.instance.GetZDO(pl.m_characterID);

                    if (zd2 == null)
                        continue;
                    //ZNetView tznv = typeof(Player).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(pl) as ZNetView;
                    if (zd2.GetString("VMMHM") == "True")
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


                //ZDO zd = ZDOMan.instance.GetZDO(_player.GetZDOID());
                ZNetView znv = typeof(Player).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_player) as ZNetView;
                znv.GetZDO().Set("VMMHM", $"{meaw}");

                _player.Message(MessageHud.MessageType.TopLeft, $"Host Modifier: {meaw}, ZNV: {znv.GetZDO().GetString("VMMHM", "")}.", 0, (Sprite)null);

                path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/VMU_Data";

                filename = path + $"/{_player.GetPlayerName()}_VM_Data.ini";
                configname = path + $"/VM_Config.json";
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
                    //_player.Message(MessageHud.MessageType.TopLeft, $"VM Error in start: {ex.Message}", 0, (Sprite)null);
                    UnityEngine.Debug.LogWarning($"VM Error in start: {ex.Message}");

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
                //_player.Message(MessageHud.MessageType.TopLeft, $"VM Error in patching: {ex.Message}", 0, (Sprite)null);
                UnityEngine.Debug.LogWarning($"VM Error in END start: {ex.Message}");
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
        float elapsed8 = 0f;
        float savetime = 0f;
        float p2psynct = 0f;
        float ragtime = 0f;
        public static bool cs = false;
        public bool us = true;
        public Vector3 ovel = new Vector3(0, 0, 0);

        bool active = false;
        bool activechanges = false;

        float stc = 0f;
        float ostam = -99999f;

        bool showSkills = false;

        bool invScroll = false;
        //Player _playert;
        public void Update()
        {
            try
            {

                if (Input.GetKeyDown(KeyCode.K))
                {


                }

                if (Player.m_localPlayer != null)
                {



                    if (shouldInit)
                    {
                        shouldInit = false;
                        Initialize();
                    }

                    if (iscrafting)
                        iscrafting = false;

                    if (ragtime >= 2.0f)
                    {

                        foreach (Ragdoll rag in GameObject.FindObjectsOfType<Ragdoll>())
                        {
                            ZNetView rznv = typeof(Ragdoll).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(rag) as ZNetView;

                            if (rznv == null)
                                continue;

                            for (int i = 0; i < rznv.GetZDO().GetInt("drops", 0); i++)
                            {
                                int amt = rznv.GetZDO().GetInt("drop_amount" + (object)i, 0);
                                if (amt > 30)
                                    rznv.GetZDO().Set("drop_amount" + (object)i, UnityEngine.Random.Range(20, 1 + (int)(amt * cdata.largeItemDropReductionModifier)));
                            }

                        }

                        ragtime = 0f;
                    }
                    else
                    {
                        ragtime += Time.deltaTime;
                    }

                    if (!invScroll && InventoryGui.IsVisible())
                    {
                        setupInvScroll();
                    }

                    if (!active && _player.GetVelocity().magnitude > 1f)
                        active = true;

                    if (p2psynct >= 10.0f)
                    {
                        p2psynct = 0f;

                        foreach (ZNet.PlayerInfo pli in ZNet.instance.GetPlayerList())
                        {
                            ZDO zd = ZDOMan.instance.GetZDO(pli.m_characterID);

                            if (zd == null)
                                continue;

                            if (zd.GetString("p2pdata", "").Contains(_player.GetZDOID().ToString()))
                                processP2PData(zd, zd.GetString("p2pdata"));
                        }

                    }
                    else
                    {
                        p2psynct += Time.deltaTime;
                    }



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


                        Skill a = skills.Where(sk => sk.name.ToLower() == "agility").FirstOrDefault();
                        if (active && a != null)
                        {
                            int extra = a.level / 2;

                            if (extra < 1)
                                extra = 1;

                            a.xp = a.xp + (int)((1 + (int)Mathf.Round(stc * 0.000025f * (extra))) * cdata.agilityXPModifier);
                            //a.updateEffects();
                        }

                        elapsed6 = 0f;
                        ostam = _player.GetStamina();
                        stc = 0f;

                        activechanges = true;
                    }

                    /*CraftingStation cr = _player.GetCurrentCraftingStation();

                    if (cr != null && cr.m_rangeBuild != 100f)
                    {

                        cr.m_rangeBuild = 100f;
                        cr.m_craftRequireRoof = false;
                        
                    }*/



                    if (_player.GetControlledShip() != null)
                    {
                        elapsed7 += Time.deltaTime;

                        Skill s = skills.Where(sk => sk.name.ToLower() == "sailing").FirstOrDefault();

                        if (elapsed7 >= 30f && active)
                        {
                            elapsed7 = 0;

                            EnvSetup env = EnvMan.instance.GetCurrentEnvironment();

                            s.xp = s.xp + (int)(1 + (int)(env.m_windMax / 5.0) + (int)(s.level * 0.8) * cdata.sailingXPModifier);

                            activechanges = true;
                        }
                        else if (elapsed7 >= 30f)
                        {
                            elapsed7 = 0;

                            activechanges = true;
                        }

                        if (_player.GetControlledShip().m_backwardForce != (0.5f + (s.level * 0.004f)) && cdata.allowShipForceAdjustments)
                        {
                            Ship sh = _player.GetControlledShip();
                            sh.m_backwardForce = (0.5f + (s.level * 0.004f)) * cdata.shipForwardsBackwardsForceModifier;
                            sh.m_sailForceFactor = (0.05f + (s.level * 0.0004f)) * cdata.shipSailForceModifier;
                            sh.m_stearForce = (0.5f + (s.level * 0.004f)) * cdata.shipSteerForceModifier;
                            sh.m_force = 0.55f * cdata.shipVerticalForceModifier;
                            sh.m_waterImpactDamage = Mathf.Max(10.0f - (s.level * 0.07f), 3.0f) * cdata.shipWaterDamageModifier;
                            sh.m_minWaterImpactForce = (3.5f + (s.level * 0.07f)) * cdata.shipMinWaterForceToTakeDamageModifier;
                            sh.m_minWaterImpactInterval = (10.0f + (s.level * 0.1f)) * cdata.shipMinIntervalToTakeDamageModifier;

                            _player.Message(MessageHud.MessageType.TopLeft, $"Modified boat forces.", 0, (Sprite)null);
                        }
                    }

                    elapsed4 += Time.deltaTime;

                    if (elapsed4 >= 300f)
                    {

                        if (meaw && cdata.allowWindModification)
                        {
                            EnvSetup env = EnvMan.instance.GetCurrentEnvironment();

                            float rnd;

                            if (cdata.allowSuperWinds && UnityEngine.Random.Range(0.0f, 1.0f) < 0.03f)
                                rnd = UnityEngine.Random.Range(cdata.superWindMinSpeed, cdata.superWindMaxSpeed);
                            else
                                rnd = UnityEngine.Random.Range(cdata.normalWindMinSpeed, cdata.normalWindMaxSpeed);

                            ZNetView znv = typeof(Player).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_player) as ZNetView;

                            znv.GetZDO().Set("VMMWS", rndf2(rnd));
                            //env.m_windMax = rnd;

                            //typeof(EnvMan).GetField("m_currentEnv", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(EnvMan.instance, env);
                            //_player.Message(MessageHud.MessageType.TopLeft, $"The max wind speed has changed: {rnd}", 0, (Sprite)null);
                        }

                        elapsed4 = 0f;


                    }


                    if (!cs)
                    {
                        elapsed8 += Time.deltaTime;

                        if (ovel.magnitude == 0)
                        {
                            ovel = _player.GetVelocity();
                        }

                        if (elapsed8 >= 30f || us)
                        {
                            foreach (Skill sk in skills)
                            {
                                sk.updateEffects();
                            }

                            us = false;
                            elapsed8 = 0f;
                        }

                        elapsed += Time.deltaTime;
                        elapsed2 += Time.deltaTime;



                        if (elapsed >= 30.0f)
                        {
                            elapsed = 0f;
                            ovel = new Vector3(0, 0, 0);

                            //_player = Player.m_localPlayer;

                            if (active)
                            {

                                float ratio = (_player.GetInventory().GetTotalWeight() / _player.GetMaxCarryWeight());

                                if (ratio > 1.0f)
                                {
                                    ratio = 1.0f;
                                }

                                Skill w = skills.Where(sk => sk.name.ToLower() == "weight").FirstOrDefault();

                                w.xp = w.xp + (int)((1 + (int)(ratio * (float)w.level)) * cdata.weightXPModifier);

                            }

                            //otime = Time.time;

                            activechanges = true;
                        }

                        if (elapsed2 >= 10.0f)
                        {
                            elapsed2 = 0f;
                            /*

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
                                        */

                            /*CharacterDrop component2 = (CharacterDrop)((Component)c).GetComponent<CharacterDrop>();

                            foreach (CharacterDrop.Drop item in component2.m_drops)
                            {
                                item.m_chance = lvl * 0.1f;
                                item.m_levelMultiplier = false;
                                item.m_amountMax += (lvl * 2);

                            }*/
                            /*
                            ZNetView znv = typeof(Character).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(c) as ZNetView;
                            znv.GetZDO().Set("VMMML", $"{lvl}");

                        }

                    }




                    //}
                }
                catch (Exception ex)
                {
                */
                            /*List<string> cbt = typeof(Chat).GetField("m_chatBuffer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Chat.instance) as List<string>;
                            cbt.Add($"VM Error (Enemy Modifiers): {ex.Message}");
                            typeof(Chat).GetField("m_chatBuffer", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Chat.instance, cbt);
                            typeof(Chat).GetMethod("UpdateChat", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Chat.instance, new object[] { });*/

                            //    _player.Message(MessageHud.MessageType.TopLeft, $"VM Error(Enemy Modifiers): {ex.Message}", 0, (Sprite)null);
                            //}
                            if (cdata.allowUpgradedMonsterNameChanges)
                            {
                                foreach (Character c in Character.GetAllCharacters())
                                {
                                    if (c.IsMonsterFaction())
                                    {
                                        ZNetView znv = typeof(Character).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(c) as ZNetView;

                                        int lev = znv.GetZDO().GetInt("level", 0);

                                        if (lev > 0 && !c.m_name.Contains(" (UVO"))
                                        {
                                            if (lev > 3)
                                            {
                                                //int lev = znv.GetZDO().GetInt("VMMML");
                                                c.m_name += $" (UVO: {lev})";

                                                UnityEngine.Debug.LogWarning($"Enemy: {c.m_name} -> Name updated.");
                                            }
                                            //continue;
                                        }

                                    }
                                }
                            }




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
                            //ZNetView znv = typeof(Player).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_player) as ZNetView;

                            //znv.GetZDO().Set("VMMWS", 200.0f);


                        }

                        if (Input.GetKeyDown(KeyCode.O))
                        {
                            //_player.SetHealth(_player.GetHealth() - 1);
                            //Console.print("You subtracted health.");


                        }

                        if (Input.GetKeyDown(KeyCode.I))
                        {
                            int count = 0;


                            foreach (ZNet.PlayerInfo pl in ZNet.instance.GetPlayerList())
                            {
                                ZDO zd2 = ZDOMan.instance.GetZDO(pl.m_characterID);

                                if (zd2 == null)
                                    continue;
                                //ZNetView tznv = typeof(Player).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(pl) as ZNetView;
                                if (zd2.GetString("VMMHM") == "True")
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
                            ZDO zd = ZDOMan.instance.GetZDO(_player.GetZDOID());

                            //ZNetView znv = typeof(Player).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_player) as ZNetView;

                            if (zd != null)
                            {
                                zd.Set("VMMHM", $"{meaw}");

                                _player.Message(MessageHud.MessageType.TopLeft, $"Host modifications set to {meaw}. Check revealed {count} active host modifiers.", 0, (Sprite)null);
                                //Console.print("You subtracted health.");
                            }
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
                        if (false)
                        {
                            GameObject itemPrefab = ObjectDB.instance.GetItemPrefab("Wood");

                            ZNetView.m_forceDisableInit = true;
                            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(itemPrefab);
                            ZNetView.m_forceDisableInit = false;
                            ItemDrop component = gameObject.GetComponent<ItemDrop>();
                            component.m_itemData.m_stack = 999;

                            _player.GetInventory().AddItem(component.m_itemData);

                        }
                        if (false)
                        {
                            GameObject itemPrefab = ObjectDB.instance.GetItemPrefab("Stone");

                            ZNetView.m_forceDisableInit = true;
                            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(itemPrefab);
                            ZNetView.m_forceDisableInit = false;
                            ItemDrop component = gameObject.GetComponent<ItemDrop>();
                            component.m_itemData.m_stack = 999;

                            _player.GetInventory().AddItem(component.m_itemData);
                        }
                        if (Input.GetKeyDown(KeyCode.Delete) && cdata.allowDeleteItemKey)
                        {
                            InventoryGrid g = InventoryGui.instance.m_player.GetComponentInChildren<InventoryGrid>();
                            ItemDrop.ItemData it = g.GetItem(new Vector2i((int)Input.mousePosition.x, (int)Input.mousePosition.y));


                            if (g != null && it != null)
                            {
                                _player.GetInventory().RemoveItem(it);
                            }

                            Container c = typeof(InventoryGui).GetField("m_currentContainer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(InventoryGui.instance) as Container;

                            if (c != null)
                            {
                                InventoryGrid g2 = InventoryGui.instance.m_container.GetComponentInChildren<InventoryGrid>();
                                ItemDrop.ItemData it2 = g2.GetItem(new Vector2i((int)Input.mousePosition.x, (int)Input.mousePosition.y));

                                if (g2 != null && it2 != null)
                                {
                                    c.GetInventory().RemoveItem(it2);
                                }
                            }

                        }

                        if (Input.GetKeyDown(KeyCode.PageDown) && cdata.allowUninjectDLLKey) // Will just unload our DLL
                        {
                            Loader.Unload();
                        }

                        if (Input.GetKeyDown(KeyCode.N))
                        {

                            //spawn object
                            //objToSpawn = new GameObject("Cool GameObject made from Code");
                            //Add Components
                            //objToSpawn.AddComponent<Rigidbody>();

                            //gr1 = gr2;

                            //gr1.transform.SetParent(nip.transform);

                            //string prt = getHier(gr1.gameObject);

                            /*string prt = "";
                            int count = 0;

                            foreach (Component co in gr1.GetComponentsInChildren<Component>())
                            {
                                prt += $"\n{count} -> {co.name}";
                                count++;
                            }*/

                            //UnityEngine.Debug.LogWarning(prt);
                            /*
                            foreach (MonoBehaviour go in scr.GetComponents<MonoBehaviour>())
                            {
                                UnityEngine.Debug.LogWarning($"Base: {go.name}, {go.GetType()}");
                            }

                            foreach (MonoBehaviour go in scr.GetComponentsInChildren<MonoBehaviour>())
                            {
                                UnityEngine.Debug.LogWarning($"Child: {go.name}, {go.GetType()}");
                                
                            }

                            foreach (MonoBehaviour go in scr.GetComponentsInParent<MonoBehaviour>())
                            {
                                UnityEngine.Debug.LogWarning($"Parent: {go.name}, {go.GetType()}");
                            }
                            */


                            /*foreach (Component go in scr.GetComponentsInChildren<Component>())
                            {
                                UnityEngine.Debug.LogWarning($"{go.name}");
                            }
                            foreach (Component go in scr.GetComponentsInParent<Component>())
                            {
                                UnityEngine.Debug.LogWarning($"{go.name}");
                            }*/


                            //typeof(Inventory).GetField("m_bkg", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(_player.GetInventory(), co.GetInventory().GetBkg());

                            //g = InventoryGui.instance.m_container.GetComponentInChildren<InventoryGrid>();

                            //g.ResetView();

                        }

                        if (elapsed5 >= 10.0f)
                        {
                            elapsed5 = 0f;

                            if (!meaw)
                            {
                                ZNetView znv = typeof(Player).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_player) as ZNetView;

                                znv.GetZDO().Set("VMMWS", 0f);
                            }

                            foreach (Player pl in Player.GetAllPlayers())
                            {
                                ZNetView znv = typeof(Player).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(pl) as ZNetView;

                                float rnd = znv.GetZDO().GetFloat("VMMWS", 0f);

                                if (rnd != 0 && EnvMan.instance.GetCurrentEnvironment().m_windMax != rnd)
                                {
                                    EnvSetup env = EnvMan.instance.GetCurrentEnvironment();
                                    env.m_windMax = rnd;

                                    typeof(EnvMan).GetField("m_currentEnv", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(EnvMan.instance, env);
                                    _player.Message(MessageHud.MessageType.TopLeft, $"The max wind speed has changed: {rnd}", 0, (Sprite)null);
                                }
                            }


                        }
                        else
                        {
                            elapsed5 += Time.deltaTime;
                        }
                    }

                    savetime += Time.deltaTime;

                    if (savetime >= cdata.saveSkillDataInterval)
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

                if (activechanges)
                    active = false;
            }
            catch (Exception ex)
            {


                UnityEngine.Debug.LogWarning($"Error in Update: {ex.ToString()}");
            }


        }

        public void saveConfig()
        {
            try
            {
                string json = JsonUtility.ToJson(cdata, true);
                //write string to file
                System.IO.File.WriteAllText(configname, json);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning($"Write json failed: {ex.ToString()}");
            }
        }

        public void setupInvScroll()
        {
            try
            {
                if (cdata.allowInventoryScrolling)
                {
                    //UnityEngine.Debug.LogWarning("2746");
                    //InventoryGrid g = InventoryGui.instance.m_player.GetComponentInChildren<InventoryGrid>();
                    //InventoryGui.instance.m_player = InventoryGui.instance.m_container;
                    //Container c = typeof(InventoryGui).GetField("m_currentContainer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(InventoryGui.instance) as Container;

                    GameObject scr = GameObject.FindObjectsOfType<GameObject>().Where(g => g.name.ToLower().Contains("inventory_screen")).SingleOrDefault();


                    InventoryGrid gr1 = scr.GetComponentsInChildren<InventoryGrid>().Where(g => g.name.ToLower() == "playergrid").SingleOrDefault();
                    GameObject rt = scr.GetComponentsInChildren<GameObject>().Where(g => g.name.ToLower() == "player").SingleOrDefault();

                    //RectTransform bkgrt = gr1.GetComponentsInChildren<RectTransform>().Where(g => g.name.ToLower().Contains("bkg")).SingleOrDefault();
                    //Scrollbar gr2 = scr.GetComponentsInChildren<Scrollbar>().Where(g => g.name.ToLower().Contains("scroll")).SingleOrDefault();
                    //UnityEngine.Debug.LogWarning("2755");
                    GameObject nip = new GameObject("UVOGO1", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                    GameObject nip2 = new GameObject("UVOGO2", typeof(RectTransform));
                    //UnityEngine.Debug.LogWarning("2757");
                    //nip.transform.SetParent(gr1.transform);

                    UnityEngine.Debug.LogWarning(gr1.transform.parent.gameObject.name);
                    nip.transform.SetParent(gr1.transform);
                    nip2.transform.SetParent(nip.transform);
                    //nip.transform.SetParent(gr1.transform.parent.gameObject.transform);
                    //gr1.transform.SetParent(nip.transform);
                    //UnityEngine.Debug.LogWarning("2759");
                    RectTransform niprt = nip.GetComponent<RectTransform>();
                    //niprt = GetCopyOf<RectTransform>()
                    //UnityEngine.Debug.LogWarning("2761");
                    RectTransform rtpg = gr1.GetComponent<RectTransform>();
                    niprt.position = rtpg.position;
                    //UnityEngine.Debug.LogWarning(rtpg.sizeDelta);
                    niprt.sizeDelta = rtpg.sizeDelta;// * new Vector2(1f, 2f);// / 2f;
                    niprt.rotation = rtpg.rotation;
                    niprt.localPosition = rtpg.localPosition;
                    niprt.anchoredPosition = rtpg.anchoredPosition;
                    niprt.anchorMax = rtpg.anchorMax;
                    niprt.anchorMin = rtpg.anchorMin;
                    niprt.eulerAngles = rtpg.eulerAngles;
                    niprt.localEulerAngles = rtpg.localEulerAngles;
                    niprt.localRotation = rtpg.localRotation;
                    niprt.localScale = rtpg.localScale;
                    niprt.offsetMax = rtpg.offsetMax;
                    niprt.offsetMin = rtpg.offsetMin;
                    niprt.pivot = rtpg.pivot;

                    //rtpg.sizeDelta = rtpg.sizeDelta * new Vector2(2f, 2f);
                    RectTransform niprt2 = nip2.GetComponent<RectTransform>();



                    niprt2.anchorMax = new Vector2(1f, 1.025f);
                    niprt2.anchorMin *= new Vector2(0f, 0f - (invrowadd / 2.0f));
                    niprt2.pivot = new Vector2(0.5f, 1.025f);

                    niprt2.sizeDelta = rtpg.sizeDelta;// * new Vector2(1f, 2f);// / 2f;
                    niprt2.rotation = rtpg.rotation;
                    niprt2.localPosition = rtpg.localPosition;
                    niprt2.anchoredPosition = rtpg.anchoredPosition;

                    niprt2.eulerAngles = rtpg.eulerAngles;
                    niprt2.localEulerAngles = rtpg.localEulerAngles;
                    niprt2.localRotation = rtpg.localRotation;
                    niprt2.localScale = rtpg.localScale;
                    niprt2.offsetMax = rtpg.offsetMax;
                    niprt2.offsetMin = rtpg.offsetMin;

                    //UnityEngine.Debug.LogWarning($"s1 -> {niprt2.sizeDelta}");

                    niprt2.ForceUpdateRectTransforms();


                    niprt.GetComponent<Image>().gameObject.SetActive(true);

                    //UnityEngine.Debug.LogWarning("2763");
                    //niprt.sizeDelta = new Vector2(50f, 50f);
                    //UnityEngine.Debug.LogWarning("2765");

                    Mask m = nip.AddComponent<Mask>();
                    m.showMaskGraphic = false;
                    m.gameObject.SetActive(true);




                    ScrollRect scrb = nip.AddComponent(typeof(ScrollRect)) as ScrollRect;
                    //VerticalLayoutGroup vlg = nip2.gameObject.AddComponent<VerticalLayoutGroup>();

                    //scrb = gr2.gameObject.clo;
                    //vlg.gameObject.SetActive(true);
                    //scrb.transform.position = gr2.transform.position + (0f, );
                    scrb.horizontal = false;
                    scrb.enabled = true;
                    scrb.scrollSensitivity = 25f;
                    //scrb.verticalScrollbar = new Scrollbar()
                    scrb.content = nip2.GetComponent<RectTransform>();
                    scrb.viewport = nip.GetComponent<RectTransform>();
                    //scrb.CalculateLayoutInputVertical();
                    scrb.gameObject.SetActive(true);


                    Image image = nip.GetComponent<Image>();

                    var tempColor = image.color;
                    tempColor.a = 1f;
                    image.color = tempColor;

                    foreach (Component co in gr1.GetComponentsInChildren<Component>())
                    {
                        if (nip2.gameObject == co.gameObject)
                            continue;

                        co.transform.SetParent(nip2.transform, true);
                    }
                }

                invScroll = true;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning("InvScrollSetup failed: " + ex.ToString());
            }
        }


        public GameObject oog = null;

        public string getHier(GameObject scr, string prt = "", int level = 0)
        {
            if (prt.Length == 0)
            {
                prt = $"OG Object: {scr.name}";
                oog = scr;
                level = 0;
            }
            else
            {
                prt += $"\n{addSpaces(level)}Level {level}: {scr.name}";
            }

            if (scr.GetComponentsInChildren<Component>().Length > 0)
                level++;
            else
                return prt;

            foreach (MonoBehaviour co in scr.GetComponentsInChildren<MonoBehaviour>())
            {
                prt += getHier(co.gameObject, prt, level).Substring(prt.Length);
            }

            return prt;
        }

        public string addSpaces(int num)
        {
            string str = "";

            for (int i = 0; i < num; i++)
            {
                str += " ";
            }

            return str;
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

        public static void processP2PData(ZDO zd, string data)
        {

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

        public static SkillData[] loadSkillData()
        {
            SkillData[] sks = new SkillData[sDefs.Count];

            try
            {
                sks = JsonHelper.FromJson<SkillData>(File.ReadAllText(filename));
            }
            catch
            {
                int i = 0;

                foreach (skillDef sd in sDefs)
                {
                    sks[i] = new SkillData();
                    sks[i].ID = sd.ID;

                    i++;
                }
            }

            return sks;
        }

        public static void saveSkillData(Player player)
        {
            SkillData[] sks = new SkillData[sDefs.Count];

            try
            {
                sks = new SkillData[sDefs.Count];

                int i = 0;

                foreach (skillDef sd in sDefs)
                {
                    sks[i] = new SkillData();

                    Skills.Skill skill = (Skills.Skill)AccessTools.Method(typeof(Skills), "GetSkill", (System.Type[])null, (System.Type[])null).Invoke((object)player.GetSkills(), new object[1]
                    {
                        (object) sd.sType
                    });

                    sks[i].ID = sd.ID;
                    sks[i].Level = (int)skill.m_level;
                    sks[i].Progress = skill.m_accumulator;
                        
                    i++;
                }


            }
            catch
            {
                sks = new SkillData[sDefs.Count];

                int i = 0;

                foreach (skillDef sd in sDefs)
                {
                    sks[i] = new SkillData();

                    sks[i].ID = sd.ID;

                    i++;
                }
            }

            File.WriteAllText(filename, JsonHelper.ToJson<SkillData>(sks, true));
        }

    }

    public class skillDef
    {
        public int ID;
        public Skills.SkillType sType;
        public Skills.SkillDef sDef;
        public string name;

        public skillDef(int id)
        {
            ID = id;
            sType = (Skills.SkillType)ID;

            Texture2D texture = Texture2D.blackTexture;

            sDef = new Skills.SkillDef()
            {
                m_skill = (Skills.SkillType)ID,
                m_icon = Sprite.Create(texture, new Rect(0.0f, 0.0f, (float)texture.width, (float)texture.height), new Vector2(0.5f, 0.5f)),
                m_description = Main.sDescs[ID - Main.startSkillID],
                m_increseStep = 1f
            };

            name = Main.snames[ID - Main.startSkillID];
        }
    }

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
            if (name.ToLower() == "weight" && Main.cdata.allowWeightSkillEffects)
            {
                if (Main.cdata.allowWeightSkillStackIncrease)
                    updateStacks();

                Main._player.m_maxCarryWeight = 300f + ((5f * (float)level) * Main.cdata.extraWeightPerWeightLevelModifier);
            }
            if (name.ToLower() == "agility")
            {
                Main._player.m_staminaRegen = 5f + ((level * 0.1f) * Main.cdata.extraStaminaRegenPerAgilityLevelModifier);
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
                        item.m_itemData.m_shared.m_maxStackSize = ms * (int)((1 + (level / 10)) * Main.cdata.weightLevelStackSizeModifier);


                }

                List<ItemDrop.ItemData> items = Main._player.GetInventory().GetAllItems();

                foreach (ItemDrop.ItemData item in items)
                {
                    if (item.m_shared.m_maxStackSize == 1)
                        continue;

                    if (Main.oms.TryGetValue(item.m_shared.m_name, out int ms))
                        item.m_shared.m_maxStackSize = ms * (int)((1 + (level / 10)) * Main.cdata.weightLevelStackSizeModifier);

                }
                typeof(Inventory).GetField("m_inventory", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Main._player.GetInventory(), items);
            }
            catch
            {
               
            }
        }
    }

    public class SkillData
    {
        public int ID = 0;
        public int Level = 1;
        public float Progress = 0.0f;
    }

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }

    [System.Serializable]
    public class configData
    {
        public int extraInvRowsPlayer = 4;
        public bool strongerMonsters = true;
        public float bonusMonsterUpgradeChancePerPlayer = 0.025f;
        public float initialMonsterUpgradeChance = 0.55f;
        public float maxPlayerBonusMonsterUpgradeChance = 0.25f;
        public bool lesserCraftingRestrictions = true;
        public bool removeBuildRestrictions = true;
        public float itemDropAmountModifier = 1.0f;
        public float uniqueItemChanceModifier = 1.0f;
        public bool adjustMonsterDrops = true;
        public bool damageModifiers = true;
        public float waveFactorMultiplier = 1.0f;
        public float craftingXPModifier = 1.0f;
        public float largeItemDropReductionModifier = 1.0f;
        public bool allowInventoryScrolling = true;
        public float agilityXPModifier = 1.0f;
        public float sailingXPModifier = 1.0f;
        public bool allowShipForceAdjustments = true;
        public float shipForwardsBackwardsForceModifier = 1.0f;
        public float shipSailForceModifier = 1.0f;
        public float shipSteerForceModifier = 1.0f;
        public float shipVerticalForceModifier = 1.0f;
        public float shipWaterDamageModifier = 1.0f;
        public float shipMinWaterForceToTakeDamageModifier = 1.0f;
        public float shipMinIntervalToTakeDamageModifier = 1.0f;
        public bool allowSuperWinds = true;
        public float superWindMinSpeed = 100f;
        public float superWindMaxSpeed = 200f;
        public bool allowWindModification = true;
        public float normalWindMinSpeed = 1f;
        public float normalWindMaxSpeed = 20f;
        public float weightXPModifier = 1.0f;
        public bool allowUpgradedMonsterNameChanges = true;
        public bool allowDeleteItemKey = true;
        public bool allowUninjectDLLKey = false;
        public float saveSkillDataInterval = 20f;
        public bool allowWeightSkillEffects = true;
        public float extraWeightPerWeightLevelModifier = 1.0f;
        public float extraStaminaRegenPerAgilityLevelModifier = 1.0f;
        public bool allowWeightSkillStackIncrease = true;
        public float weightLevelStackSizeModifier = 1.0f;
        public float buildingXPModifier = 1.0f;
        public int overrideInvWidthChest = 8;
        public int overrideInvHeightChest = 4;
        public bool overrideChestInventorySize = true;
    }
}


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

        public List<Skill> skills = new List<Skill>();

        public string[] snames = { "Weight", "Agility", "Sailing" };

        /*[DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();*/

        public void Awake()
        {
            UnityEngine.Debug.LogWarning("Hello, world!");
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

        Type[] calcwtypes = { typeof(Vector3), typeof(float), typeof(Vector4), typeof(float), typeof(float)};

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

        public void Initialize()
        {
            //M = this;


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

                if (File.Exists(filename))
                {

                    IniData data = parser.ReadFile(filename);

                    for (int i = 1; i < snames.Length + 1; i++)
                    {

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

                    IniData data = parser.ReadFile(filename);


                    for (int i = 1; i < snames.Length + 1; i++)
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

                    if(ostam == -99999f)
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
                        CraftingStation cr = CraftingStation.GetCraftingStation(_player.transform.position);
                        //_player.GetHoveringPiece()
                        if (cr != null && cr.m_rangeBuild != 100f)
                            cr.m_rangeBuild = 100f;
                    }



                    if (_player.GetControlledShip() != null)
                    {
                        elapsed7 += Time.deltaTime;

                        Skill s = skills.Where(sk => sk.name.ToLower() == "sailing").FirstOrDefault();

                        if (elapsed7 >= 30f)
                        {
                            elapsed7 = 0;
                            s.xp = s.xp + 1 + (int)Mathf.Round((elapsed7 / 6.0f) * s.level);
                        }

                        if (_player.GetControlledShip().m_backwardForce != (0.5f + (s.level * 0.005f)))
                        {
                            Ship sh = _player.GetControlledShip();
                            sh.m_backwardForce = (0.5f + (s.level * 0.005f));
                            sh.m_sailForceFactor = (0.05f + (s.level * 0.0005f));
                            sh.m_stearForce = (0.5f + (s.level * 0.005f));
                            sh.m_force = 0.60f;
                            sh.m_waterImpactDamage = 0;
                            
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

                                        CharacterDrop component2 = (CharacterDrop)((Component)c).GetComponent<CharacterDrop>();

                                        foreach (CharacterDrop.Drop item in component2.m_drops)
                                        {
                                            item.m_chance = lvl * 0.1f;
                                            item.m_levelMultiplier = false;
                                            item.m_amountMax += (lvl * 2);

                                        }

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


                    if (Main.oms.TryGetValue(item.m_itemData.m_shared.m_name, out int ms))
                        item.m_itemData.m_shared.m_maxStackSize = ms * (1 + (level / 10));


                }

                List<ItemDrop.ItemData> items = Main._player.GetInventory().GetAllItems();

                foreach (ItemDrop.ItemData item in items)
                {
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


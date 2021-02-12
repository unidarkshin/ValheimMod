using HarmonyLib;
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
    class Main : MonoBehaviour
    {
        public string pln;
        public float otime;

        private int WSL;
        public int wsl
        {
            get { return WSL; }
            set
            {
                WSL = value;
                updateStacks();
            }
        }
        public int wsxp;

        //Harmony h;

        public Main M;

        string path;
        string filename;
        string errorfile;

        Dictionary<string, int> oms = new Dictionary<string, int>();

        public static bool meaw = false;

        /*[DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();*/


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
        public static void DDM(long sender, ref HitData hit)
        {
            hit.m_damage.m_damage += 100f;
            _player.Message(MessageHud.MessageType.TopLeft, $"VM DDM: {hit.m_damage.m_damage}", 0, (Sprite)null);
            //__result += ((float)wsl * 5f);
        }

        Type[] cwTypes = { typeof(Vector3), typeof(float), typeof(float), typeof(float), typeof(float), typeof(Vector2), typeof(float) };

        public static void CW(
    Vector3 worldPos,
    float time,
    ref float waveSpeed,
    float waveLength,
    float waveHeight,
    Vector2 dir2d,
    float sharpness)
        {
            if (meaw)
            waveSpeed *= UnityEngine.Random.Range(0.75f, 50.0f);
            //__result += ((float)wsl * 5f);
            _player.Message(MessageHud.MessageType.TopLeft, $"Wave upd.", 0, (Sprite)null);

            //return true;
        }

        public void Start()
        {
            //M = this;

            //AllocConsole();

            int twsl = 1;
            int twsxp = 1;

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


                //_player = Player.GetAllPlayers();

                path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                //path = Application.persistentDataPath;

                filename = path + $"/{_player.GetPlayerName()}_VM_Data.txt";
                errorfile = path + "/VM_Error.txt";
                //File.WriteAllText(filename, $"0,0");

                //File.Create(filename);


                //_player = FindObjectsOfType<Player>()[0];




                FileStream fs;
                try
                {
                    fs = File.OpenRead(filename);

                    //StreamReader sr = new StreamReader(fs);

                    //string data = sr.ReadLine();

                    byte[] buf = new byte[1024];
                    int c;

                    while ((c = fs.Read(buf, 0, buf.Length)) > 0)
                    {
                        string[] data;
                        string s = Encoding.UTF8.GetString(buf, 0, c);
                        //_player.Message(MessageHud.MessageType.TopLeft, $"VM ReadFile: {s}", 0, (Sprite)null);
                        if (s.Contains(","))
                        {
                            data = s.Split(',');

                            if (data.Length >= 2 && int.TryParse(data[0], out int level) && int.TryParse(data[1], out int xp))
                            {
                                wsl = level;
                                wsxp = xp;
                            }

                        }
                        else
                        {
                            //wsl = 1;
                            //wsxp = 1;
                        }

                        break;
                    }

                    fs.Close();

                }
                catch (Exception ex)
                {
                    //ZLog.Log((object)("  failed to load " + path));
                    //return (ZPackage)null;

                    //wsl = 1;
                    //wsxp = 1;

                    _player.Message(MessageHud.MessageType.TopLeft, $"VM Error reading file data: {ex.Message}", 0, (Sprite)null);
                }


                /*if (false) {

                    string line1 = File.ReadLines(filename).First();

                    string[] data = line1.Split(',');

                    if (data.Length >= 2 && int.TryParse(data[0], out int level) && int.TryParse(data[1], out int xp))
                    {
                        wsl = level;
                        wsxp = xp;
                    }
                    else
                    {
                        wsl = 0;
                        wsxp = 0;
                    }


                }
                else
                {
                    wsl = 0;
                    wsxp = 0;
                }*/

                //wsl = 0;
                //wsxp = 0;

                otime = Time.time;

                //var h = new Harmony("unidarkshin_vm");

                Type[] types1 = { };

                /*h.Patch(
   original: AccessTools.Method(typeof(Player), "FixedUpdate"),
   prefix: new HarmonyMethod(typeof(Main), nameof(Main.PFU), types1)
);
                h.Patch(
   original: AccessTools.Method(typeof(WaterVolume), "CreateWave", cwTypes),
   prefix: new HarmonyMethod(typeof(Main), nameof(Main.CW))
);*/

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

                _player.m_maxCarryWeight = 300f + (5f * (float)wsl);


            }
            catch (Exception ex)
            {
                _player.Message(MessageHud.MessageType.TopLeft, $"VM Error in patching: {ex.Message}", 0, (Sprite)null);

                FileStream fs2 = new FileStream(errorfile, FileMode.OpenOrCreate, FileAccess.ReadWrite);


                string data = $"\n Error in patching: {ex.Source}, {ex.Message}, {ex.StackTrace}, {ex.InnerException} ";
                byte[] bytes = Encoding.UTF8.GetBytes(data);

                fs2.Write(bytes, 0, bytes.Length);

                fs2.Close();
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
        public static bool cs = false;
        public bool us = true;
        public Vector3 ovel = new Vector3(0,0,0);
        //Player _playert;
        public void Update()
        {
            try
            {


                if (_player.GetHoveringPiece() != null)
                {
                    typeof(Player).GetField("m_placementStatus", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(_player, 0);
                }

                if (_player.GetControlledShip() != null && _player.GetControlledShip().m_backwardForce != 0.75f)
                {
                    Ship s = _player.GetControlledShip();
                    s.m_backwardForce = 0.75f;
                    s.m_sailForceFactor = 0.10f;
                    s.m_stearForce = 0.75f;
                    s.m_force = 0.65f;

                    _player.Message(MessageHud.MessageType.TopLeft, $"Modified boat forces.", 0, (Sprite)null);
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

                if (cs)
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
                    _player.GetInventory().RemoveAll();
                    (typeof(Player).GetField("m_playerProfile", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_player) as PlayerProfile).SavePlayerData(_player);

                    _player = UnityEngine.Object.Instantiate<GameObject>(Game.instance.m_playerPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity).GetComponent<Player>();


                    //_player.m_name = "temp";
                    //_player.gameObject.
                    //_player = new Player();
                    //_player.m_name = "RS";
                    cs = true;
                }
                else if (cs && elapsed3 > 20f)
                {
                    //ZNetScene.instance.Destroy(_player.gameObject);
                    _player = Player.m_localPlayer;
                    //if (_player.m_name.Contains("VMM"))
                        //_player.m_name += "VMM";

                    cs = false;
                    elapsed3 = 0f;
                }


                if (!cs)
                {
                    if (ovel.magnitude == 0)
                    {
                        ovel = _player.GetVelocity();
                    }

                    if (us)
                    {
                        updateStacks();
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

                        wsxp = wsxp + 1 + (int)(ratio * (float)wsl);


                        checkForLevelUp();


                        _player.m_maxCarryWeight = 300f + (5f * (float)wsl);

                        //File.WriteAllText(filename, $"{wsl},{wsxp}");

                        //File.Create(filename);


                        /*using (System.IO.StreamWriter file =
                    new System.IO.StreamWriter(filename, false))
                        {
                            file.WriteLine($"{wsl},{wsxp}");
                        */

                        //ZPackage z = new ZPackage();

                        //FileStream fs = File.OpenWrite(filename);

                        FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);


                        string data = $"{wsl},{wsxp}";
                        byte[] bytes = Encoding.UTF8.GetBytes(data);

                        fs.Write(bytes, 0, bytes.Length);

                        fs.Close();

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

                        znv.GetZDO().Set("VMMWS", 150.0f);


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

                            _player.Message(MessageHud.MessageType.TopLeft, $"Weight level: {wsl}, Weight XP: {wsxp}, Required XP: {requiredXP()}", 0, (Sprite)null);
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
                }


            }
            catch
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


            }

        }

        private void checkForLevelUp()
        {
            try
            {
                int rxp = requiredXP();
                if (wsxp >= rxp)
                {
                    wsl += 1;
                    wsxp = wsxp - rxp;

                    _player.Message(MessageHud.MessageType.TopLeft, $"You leveled up your weight skill to {wsl}!", 0, (Sprite)null);
                }
            }
            catch
            {

            }
        }

        private void updateStacks()
        {
            try
            {

                foreach (GameObject go in ObjectDB.instance.m_items)
                {


                    ItemDrop item = go.GetComponent<ItemDrop>();


                    if (oms.TryGetValue(item.m_itemData.m_shared.m_name, out int ms))
                        item.m_itemData.m_shared.m_maxStackSize = ms * (1 + (wsl / 10));


                }

                List<ItemDrop.ItemData> items = _player.GetInventory().GetAllItems();

                foreach (ItemDrop.ItemData item in items)
                {
                    if (oms.TryGetValue(item.m_shared.m_name, out int ms))
                        item.m_shared.m_maxStackSize = ms * (1 + (wsl / 10));

                }
                typeof(Inventory).GetField("m_inventory", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(_player.GetInventory(), items);
            }
            catch
            {

            }
        }

        public int requiredXP()
        {

            int extra = (int)(0.25 * wsl);

            if (extra < 1)
            {
                extra = 1;
            }

            return ((10 * wsl) + (wsl * wsl * extra));

        }

        public void OnGUI()
        {

            //GUI.DrawTexture(new Rect(Screen.width / 2, Screen.height / 2, 150f, 50f), "GAME INJECTED"); // Should work and when injected you will see this text in the middle of the screen
        }
        public static Player _player;
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

}


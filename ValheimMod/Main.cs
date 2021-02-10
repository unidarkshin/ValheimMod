﻿using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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

        Harmony h;

        public Main M;

        string path;
        string filename;

        Dictionary<string, int> oms = new Dictionary<string, int>();

        public bool meaw = false;

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
                    if (pl.m_name.Contains("VMM"))
                    {
                        count++;
                        break;
                    }
                }

                _player.m_name += "VMM";

                if (count == 0)
                {
                    meaw = true;
                    
                }

                _player.Message(MessageHud.MessageType.TopLeft, $"Host Modifier: {meaw}", 0, (Sprite)null);
                

                //_player = Player.GetAllPlayers();

                path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                //path = Application.persistentDataPath;

                filename = path + $"/{_player.GetPlayerName()}_VM_Data.txt";

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

                //h = new Harmony("vmp");

                Type[] types1 = { };

                //h.Patch(typeof(Player).GetMethod("GetMaxCarryWeight"), postfix: new HarmonyMethod(typeof(Main), nameof(this.GMCW)));

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
            catch
            {

            }
        }



        float elapsed = 0f;
        float elapsed2 = 0f;
        float elapsed3 = 0f;
        float elapsed4 = 0f;
        public static bool cs = false;
        public bool us = false;
        //Player _playert;
        public void Update()
        {
            try
            {
                if (_player.GetControlledShip() != null && _player.GetControlledShip().m_backwardForce != 100f)
                {
                    Ship s = _player.GetControlledShip();
                    s.m_backwardForce = 55f;
                    s.m_sailForceFactor = 0.15f;
                    s.m_stearForce = 0.75f;
                    s.m_force = 2.5f;

                    _player.Message(MessageHud.MessageType.TopLeft, $"Modified boat forces.", 0, (Sprite)null);
                }

                elapsed4 += Time.deltaTime;

                if (elapsed4 >= 300f)
                {

                    if (meaw)
                    {
                        EnvSetup env = EnvMan.instance.GetCurrentEnvironment();
                        float rnd = UnityEngine.Random.Range(1.0f, 20.0f);
                        env.m_windMax = rnd;

                        typeof(EnvMan).GetField("m_currentEnv", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(EnvMan.instance, env);
                        _player.Message(MessageHud.MessageType.TopLeft, $"The max wind speed has changed: {rnd}", 0, (Sprite)null);
                    }

                    elapsed4 = 0f;

                    
                }

                if (cs)
                    elapsed3 += Time.deltaTime;

                if (cs && !us)
                    us = true;

                if (_player.IsDead() && !cs)
                {
                    List<ItemDrop.ItemData> items = new List<ItemDrop.ItemData>();

                    typeof(Inventory).GetField("m_inventory", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(_player.GetInventory(), items);

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
                    if (_player.m_name.Contains("VMM"))
                        _player.m_name += "VMM";

                    cs = false;
                    elapsed3 = 0f;
                }


                if (!cs)
                {
                    if (us)
                    {
                        updateStacks();
                        us = false;
                    }

                    elapsed += Time.deltaTime;
                    elapsed2 += Time.deltaTime;



                    if (elapsed >= 30.0f)
                    {
                        elapsed = 0f;


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

                        FileStream fs = new FileStream(filename, FileMode.Truncate, FileAccess.Write);


                        string data = $"{wsl},{wsxp}";
                        byte[] bytes = Encoding.UTF8.GetBytes(data);

                        fs.Write(bytes, 0, bytes.Length);

                        fs.Close();

                        otime = Time.time;
                        //frame = Time.frameCount;
                        //Console.print($"Level {wsl}, XP {wsxp}");


                    }

                    if (elapsed2 >= 30.0f)
                    {
                        elapsed2 = 0f;


                        try
                        {
                            if (meaw)
                            {
                                Character[] chars = GameObject.FindObjectsOfType(typeof(Character)) as Character[];
                                List<Character> chars2 = new List<Character>();

                                foreach (Character ch in chars)
                                {
                                    if (ch.IsMonsterFaction() && !ch.m_name.Contains("VMM"))
                                        chars2.Add(ch);

                                }

                                if (chars2.Count > 0)
                                {
                                    Character c = chars2[UnityEngine.Random.Range(0, chars2.Count - 1)];

                                    int lvl = UnityEngine.Random.Range(1, 6);

                                    if (lvl > c.GetLevel())
                                    {
                                        c.SetLevel(lvl);
                                        c.m_name += $" (VMM: {lvl})";
                                    }

                                }
                            }
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



                    }

                    if (Input.GetKeyDown(KeyCode.O))
                    {
                        _player.SetHealth(_player.GetHealth() - 1);
                        //Console.print("You subtracted health.");
                    }

                    if (Input.GetKeyDown(KeyCode.I))
                    {
                        if (meaw == false)
                            meaw = true;
                        else
                            meaw = false;

                        _player.Message(MessageHud.MessageType.TopLeft, $"You modified host modifications to {meaw}.", 0, (Sprite)null);
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
        private static Player _player;
    }

}


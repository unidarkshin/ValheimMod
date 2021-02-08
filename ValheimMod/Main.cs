using HarmonyLib;
using System;
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
        public float otime;
        public static int wsl;
        public static int wsxp;

        Harmony h;

        public Main M;

        string path;
        string filename;

        /*[DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();*/
        

        public static void GMCW(ref float __result)
        {

            __result += ((float)wsl * 5f);
        }

        public void Start()
        {
            //M = this;

            //AllocConsole();

            //_player = Player.m_localPlayer;

            foreach (Player pl in Player.GetAllPlayers())
            {
                if (pl.GetPlayerName().ToLower() == "scumpty tumpty")
                    _player = pl;
            }

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
                        wsl = 1;
                        wsxp = 1;
                    }

                    break;
                }

            }
            catch
            {
                //ZLog.Log((object)("  failed to load " + path));
                //return (ZPackage)null;

                wsl = 1;
                wsxp = 1;
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
        }
        public void Update()
        {
            if ((Time.time - otime) >= 30)
            {
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

                FileStream fs = File.OpenWrite(filename);

                string data = $"{wsl},{wsxp}";
                byte[] bytes = Encoding.UTF8.GetBytes(data);

                fs.Write(bytes, 0, bytes.Length);

                otime = Time.time;

                //Console.print($"Level {wsl}, XP {wsxp}");
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                //_player.SetHealth(_player.GetHealth() - 1);
                //_player.m_maxCarryWeight = 
                //wsl += 1;
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                //_player.SetHealth(_player.GetHealth() - 1);
                //_player.m_maxCarryWeight = 
                //wsxp += 1;
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                _player.SetHealth(_player.GetHealth() - 1);
                //Console.print("You subtracted health.");
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                //_player.SetHealth(_player.GetHealth() + 1);

                try
                {
                    //typeof(Chat).GetMethod("AddString", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Chat.instance, new object[] { $"Weight level: {wsl}, Weight XP: {wsxp}" });
                    List<string> cbt = typeof(Chat).GetField("m_chatBuffer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Chat.instance) as List<string>;
                    cbt.Add($"Weight level: {wsl}, Weight XP: {wsxp}, Required XP: {requiredXP()}");
                    typeof(Chat).GetField("m_chatBuffer", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Chat.instance, cbt);
                    typeof(Chat).GetMethod("UpdateChat", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Chat.instance, new object[] {});
                    //Reflection.GetMethod(Game1.currentLocation, "isMonsterDamageApplicable").Invoke<bool>(who, character, true)
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

        private void checkForLevelUp()
        { 
            int rxp = requiredXP();
            if (wsxp >= rxp)
            {
                wsl += 1;
                wsxp = wsxp - rxp;
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
        private Player _player;
    }

}


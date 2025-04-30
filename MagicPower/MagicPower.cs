using HarmonyLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

using JumpKing;
using JumpKing.Mods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using JumpKing.PauseMenu;
using JumpKing.PauseMenu.BT;

using MagicPower.Menu;

namespace MagicPower;
[JumpKingMod(IDENTIFIER)]
public static class MagicPower
{
    const string IDENTIFIER = "JeFi.MagicPower";
    const string HARMONY_IDENTIFIER = "JeFi.MagicPower.Harmony";
    const string SETTINGS_FILE = "JeFi.MagicPower.Preferences.xml";

    public static string AssemblyPath { get; set; }
    public static Preferences Prefs { get; private set; }

    public static int OffsetX {get; private set; }
    public static int OffsetY {get; private set; }

    [BeforeLevelLoad]
    public static void BeforeLevelLoad()
    {
        AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#if DEBUG
            Debugger.Launch();
            Harmony.DEBUG = true;
            Environment.SetEnvironmentVariable("HARMONY_LOG_FILE", $@"{AssemblyPath}\harmony.log.txt");
#endif
        try
        {
            Prefs = XmlSerializerHelper.Deserialize<Preferences>($@"{AssemblyPath}\{SETTINGS_FILE}");
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[ERROR] [{IDENTIFIER}] {e.Message}");
            Prefs = new Preferences();
        }
        Prefs.PropertyChanged += SaveSettingsOnFile;

        Harmony harmony = new Harmony(HARMONY_IDENTIFIER);

        try {
            new Patching.NBPDecisionState(harmony);
        }
        catch (Exception e) {
            Debug.WriteLine(e.ToString());
        }

#if DEBUG
        Environment.SetEnvironmentVariable("HARMONY_LOG_FILE", null);
#endif
    }

#region UI
    [MainMenuItemSetting]
    [PauseMenuItemSetting]
    public static ToggleMagicPower ToggleMagicPower(object factory, GuiFormat format)
    {
        return new ToggleMagicPower();
    }
#endregion

    private static void SaveSettingsOnFile(object sender, System.ComponentModel.PropertyChangedEventArgs args)
    {
        try
        {
            XmlSerializerHelper.Serialize($@"{AssemblyPath}\{SETTINGS_FILE}", Prefs);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[ERROR] [{IDENTIFIER}] {e.Message}");
        }
    }
}

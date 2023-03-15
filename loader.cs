using ColossalFramework.UI;
using ICities;
using UnityEngine;
using System.IO;
using ColossalFramework;
using System.Reflection;
using System.Collections.Generic;
using ColossalFramework.Plugins;
using InfoViews;
using InfoViews.Util;
using InfoViews.UI;

namespace InfoViews
{
    public class Loader : LoadingExtensionBase
    {
        public static bool HarmonyDetourInited = false;
        public static bool HarmonyDetourFailed = true;
        public static bool GuiAdded = false;
        public static LoadMode CurrentLoadMode;

        public static string m_atlasName = "InfoView";
        public static bool m_atlasLoaded;

        private static TransportationButton transportationButton;
        public override void OnCreated(ILoading loading)
        {
            transportationButton = null;
            base.OnCreated(loading);
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            CurrentLoadMode = mode;
            if (InfoViews.IsEnabled)
            {
                InfoViews.mode = InfoViews.ExtendedMode.None;
                if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame)
                {
                    HarmonyInitDetour();
                    SetupGui();
                }
            }
            else
            {

            }
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            if (CurrentLoadMode == LoadMode.LoadGame || CurrentLoadMode == LoadMode.NewGame)
            {
                if (InfoViews.IsEnabled)
                {
                    RemoveGui();
                    HarmonyRevertDetour();
                }
            }
        }

        public override void OnReleased()
        {
            base.OnReleased();
        }

        private static void LoadSprites()
        {
            if (SpriteUtilities.GetAtlas(m_atlasName) != null) return;
            var modPath = PluginManager.instance.FindPluginInfo(Assembly.GetExecutingAssembly()).modPath;
            m_atlasLoaded = SpriteUtilities.InitialiseAtlas(Path.Combine(modPath, "Icon/InfoViews.png"), m_atlasName);
            if (m_atlasLoaded)
            {
                bool spriteSuccess = SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(382, 0), new Vector2(191, 191)), "TransportButton", m_atlasName)
                             && SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(0, 0), new Vector2(191, 191)), "Blank", m_atlasName);
                if (!spriteSuccess)
                    DebugLog.LogToFileOnly("Some sprites haven't been loaded. This is abnormal; you should probably report this to the mod creator.");
            }
            else
                DebugLog.LogToFileOnly("The texture atlas (provides custom icons) has not loaded. All icons have reverted to text prompts.");
        }

        public static void SetupGui()
        {
            LoadSprites();
            if (m_atlasLoaded)
            {   
                UIPanel parentGuiView = UIView.Find<UIPanel>("(Library) DistrictWorldInfoPanel");

                if (transportationButton == null)
                    transportationButton = (parentGuiView.AddUIComponent(typeof(TransportationButton)) as TransportationButton);
                
                transportationButton.Show();
                GuiAdded = true;
            }
        }

        public static void RemoveGui()
        {
            GuiAdded = false;
            if (transportationButton != null)
            {
                Object.Destroy(transportationButton);
                transportationButton = null;
            }
        }

        public static void HarmonyInitDetour()
        {
            if (!HarmonyDetourInited)
            {
                HarmonyDetours.Apply();
                HarmonyDetourInited = true;
            }
        }

        public static void HarmonyRevertDetour()
        {
            if (HarmonyDetourInited)
            {
                HarmonyDetours.DeApply();
                HarmonyDetourFailed = true;
                HarmonyDetourInited = false;
            }
        }
    }
}

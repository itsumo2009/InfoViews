using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

using InfoViews.Util;

namespace InfoViews.UI
{
    public class TransportationButton : UIButton
    {
        public void Init()
        {
            UIComponent panel = ToolsModifierControl.infoViewsPanel.m_ChildContainer;
            var buttons = panel.GetComponentsInChildren<UIButton>();
            normalBgSprite =  buttons[8].normalBgSprite;
            normalFgSprite =  buttons[8].normalFgSprite;
            hoveredBgSprite = buttons[8].hoveredBgSprite;
            hoveredFgSprite = buttons[8].hoveredFgSprite;
            hoveredFgSprite  =buttons[8].hoveredFgSprite  ;
            disabledFgSprite =buttons[8].disabledFgSprite ;
            disabledBgSprite =buttons[8].disabledBgSprite ;
            normalBgSprite  = buttons[8].normalBgSprite   ;
            hoveredBgSprite = buttons[8].hoveredBgSprite  ;
            focusedBgSprite = buttons[8].focusedBgSprite  ;
            focusedFgSprite = buttons[8].focusedFgSprite;

            foreach (var b in buttons)
            {
                DebugLog.LogToFileOnly($"{b.position.x} {b.position.y} {b.absolutePosition.x} {b.absolutePosition.y} {b.size.x} {b.size.y} {b.relativePosition.x} {b.relativePosition.y}");
            }
    }

    public void BuildingUIToggle()
        {
            InfoManager infoManager = Singleton<InfoManager>.instance;
            infoManager.SetCurrentMode(InfoManager.InfoMode.Traffic, InfoManager.SubInfoMode.None);
        }

        public override void Start()
        {
            district = 0;
            playAudioEvents = true;
            name = "TransportationButton";

            size = new Vector2(32, 32);
            relativePosition = new Vector3(260, 6);

            Init();
            eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                BuildingUIToggle();
            };
        }

        public override void Update()
        {
            InfoManager infoManager = Singleton<InfoManager>.instance;
            if (infoManager.CurrentMode == InfoManager.InfoMode.None)
            {
                Unfocus();
                InfoViews.mode = InfoViews.ExtendedMode.None;
            }
            else if (infoManager.CurrentMode == InfoManager.InfoMode.Traffic)
            {
                Focus();
            }

            bool ch;
            InstanceID instanceId = WorldInfoPanel.GetCurrentInstanceID();
            if (instanceId.Type == InstanceType.District)
            {
                ch = district != instanceId.District;
                district = instanceId.District;
            }
            else
            {
                ch = district != 0;
                district = 0;
            }

            if (ch)
            {
                infoManager.SetCurrentMode(InfoManager.InfoMode.Density, InfoManager.SubInfoMode.None);
                infoManager.SetCurrentMode(InfoManager.InfoMode.Traffic, InfoManager.SubInfoMode.None);
            }

            base.Update();
        }

        byte district;
    }
}

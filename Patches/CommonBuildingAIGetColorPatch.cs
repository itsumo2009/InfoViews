﻿using ColossalFramework;
using Harmony;
using System;
using System.Reflection;
using UnityEngine;
using InfoViews;
using InfoViews.CustomAI;

namespace InfoViews.Patch
{
    [HarmonyPatch]
    public class CommonBuildingAIGetColorPatch
    {
        public static MethodBase TargetMethod()
        {
            return typeof(CommonBuildingAI).GetMethod(
                    "GetColor",
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    new[] { typeof(ushort), typeof(Building).MakeByRefType(), typeof(InfoManager.InfoMode), typeof(InfoManager.SubInfoMode) },
                    new ParameterModifier[0]);
        }

        public static void Postfix(ushort buildingID, ref Building data, InfoManager.InfoMode infoMode, InfoManager.SubInfoMode subInfoMode, ref Color __result)
        {
            InfoViewsPrivateBuildingAI.PrivateBuildingAIGetColorPostFix(buildingID, ref data, infoMode, subInfoMode, ref __result);
        }
    }
}

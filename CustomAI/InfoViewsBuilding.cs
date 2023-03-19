using ColossalFramework;
using ColossalFramework.Math;
using InfoViews.Util;
using InfoViews;
using System;

namespace InfoViews.CustomAI
{
    public class InfoViewsPrivateBuildingAI
    {
        static Randomizer r = new Randomizer();
        static readonly UnityEngine.Color threatColor = new UnityEngine.Color(255 / 255.0f, 69 / 255.0f, 0);
        static readonly UnityEngine.Color halfThreatColor = new UnityEngine.Color(220 / 255.0f, 235 / 255.0f, 20 / 255.0f);
        static readonly UnityEngine.Color normalColor = new UnityEngine.Color(56 / 255.0f, 136 / 255.0f, 1.0f);
        static readonly UnityEngine.Color capacityThreatColor = new UnityEngine.Color(125 / 255.0f, 36 / 255.0f, 163 / 255.0f);

        static int CalculateFreeVacancies(int dr0, int dr1, int dr2, int dr3, Citizen.Education edulevel)
        {
            if (dr3 < 0)
            {
                dr2 += dr3;
                dr3 = 0;
            }
            if (dr2 < 0)
            {
                dr1 += dr2;
                dr2 = 0;
            }
            if (dr1 < 0)
            {
                dr0 += dr1;
                dr1 = 0;
            }
            if (dr0 < 0)
                dr0 = 0;
            
            switch (edulevel)
            {
                case Citizen.Education.ThreeSchools:
                    return dr3;
                case Citizen.Education.TwoSchools:
                    return dr2;
                case Citizen.Education.OneSchool:
                    return dr1;
                case Citizen.Education.Uneducated:
                    return dr0;
                default:
                    return 0;
            }
        }

        static void GetWorkerCount(ref Building data, out int r0, out int r1, out int r2, out int r3)
        {
            r0 = 0;
            r1 = 0;
            r2 = 0;
            r3 = 0;

            CitizenManager instance = Singleton<CitizenManager>.instance;
            uint num = data.m_citizenUnits;
            while (num != 0u)
            {
                var citizenIstance = instance.m_units.m_buffer[num];

                if ((citizenIstance.m_flags & CitizenUnit.Flags.Work) != 0)
                {
                    for (int i = 0; i <= 4; ++i)
                    {
                        var citizenID = citizenIstance.GetCitizen(i);
                        if (citizenID != 0u)
                        {
                            switch (instance.m_citizens.m_buffer[citizenID].EducationLevel)
                            {
                                case Citizen.Education.ThreeSchools:
                                    ++r3;
                                    break;
                                case Citizen.Education.TwoSchools:
                                    ++r2;
                                    break;
                                case Citizen.Education.OneSchool:
                                    ++r1;
                                    break;
                                case Citizen.Education.Uneducated:
                                    ++r0;
                                    break;
                            }
                        }
                    }
                }
                num = instance.m_units.m_buffer[(int)((UIntPtr)num)].m_nextUnit;
                if (num > 524288)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                    break;
                }
            }
        }

        static float getTransportationThreat(ref Building data, byte districtID)
        {
            CitizenManager instance = Singleton<CitizenManager>.instance;
            DistrictManager districtManager = Singleton<DistrictManager>.instance;
            BuildingManager buildingManager = Singleton<BuildingManager>.instance;
            bool inDistrict = districtManager.GetDistrict(data.m_position) == districtID;

            int total = 0;
            int factor = 0;
            uint num = data.m_citizenUnits;
            while (num != 0u)
            {
                var citizenIstance = instance.m_units.m_buffer[num];

                if ((citizenIstance.m_flags & (CitizenUnit.Flags.Work | CitizenUnit.Flags.Visit | CitizenUnit.Flags.Student | CitizenUnit.Flags.Home)) != 0)
                {
                    for (int i = 0; i <= 4; ++i)
                    {
                        var citizenID = citizenIstance.GetCitizen(i);
                        if (citizenID != 0u)
                        {
                            ushort home = instance.m_citizens.m_buffer[citizenID].m_homeBuilding;
                            if (home != 0)
                            {
                                
                                bool targetInDistrict = districtManager.GetDistrict(buildingManager.m_buildings.m_buffer[home].m_position) == districtID;
                                if (targetInDistrict != inDistrict)
                                {
                                    factor += 3;
                                    total += 3;
                                }
                                else
                                    total++;
                            }
                            ushort work = instance.m_citizens.m_buffer[citizenID].m_workBuilding;
                            if (work != 0)
                            {
                                bool targetInDistrict = districtManager.GetDistrict(buildingManager.m_buildings.m_buffer[work].m_position) == districtID;
                                if (targetInDistrict != inDistrict)
                                {
                                    factor+= 3;
                                    total += 3;
                                }
                                else
                                    total++;
                            }
                        }
                    }
                }

                num = instance.m_units.m_buffer[(int)((UIntPtr)num)].m_nextUnit;
                if (num > 524288)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                    break;
                }
            }

            return total == 0 ? 0.0f : 1.0f * factor / total;
        }

        static float getEducationThreat(ref Building data, bool lookingForJob = false)
        {
            CitizenManager instance = Singleton<CitizenManager>.instance;
            int total = 0;
            int factor = 0;

            uint num = data.m_citizenUnits;
            while (num != 0)
            {
                if ((ushort)(instance.m_units.m_buffer[(int)((UIntPtr)num)].m_flags & CitizenUnit.Flags.Home) != 0)
                {
                    var citizenInstance = instance.m_units.m_buffer[(int)((UIntPtr)num)];
                    for (int i=0; i<=4; ++i)
                    {
                        var citizenID = citizenInstance.GetCitizen(i);
                        if (citizenID != 0)
                        {
                            var citizen = instance.m_citizens.m_buffer[citizenID];
                            var ageGroup = Citizen.GetAgeGroup(citizen.Age);
                            
                            if (lookingForJob)
                            {
                                if (ageGroup == Citizen.AgeGroup.Adult)
                                {
                                    ++total;

                                    if (citizen.m_workBuilding == 0)
                                        ++factor;
                                    break;
                                }
                            }
                            else
                            {
                                switch (ageGroup)
                                {
                                    case Citizen.AgeGroup.Child:
                                        ++total;

                                        if (!citizen.Education1 && citizen.m_workBuilding == 0)
                                            ++factor;
                                        break;
                                    case Citizen.AgeGroup.Teen:
                                        ++total;

                                        if (!citizen.Education2 && citizen.m_workBuilding == 0)
                                            ++factor;
                                        break;
                                    case Citizen.AgeGroup.Young:
                                        ++total;

                                        if (!citizen.Education3 && citizen.m_workBuilding == 0)
                                            ++factor;
                                        break;
                                }
                            }
                            
                        }
                    }
                }
                num = instance.m_units.m_buffer[(int)((UIntPtr)num)].m_nextUnit;
                if (num > 524288)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                    break;
                }
            }

            if (total > 0)
                return factor * 1.0f / total;

            return 0.0f;
        }

        private static UnityEngine.Color getColorByThreat(float threat)
        {
            if (threat > 0.5f)
                return UnityEngine.Color.Lerp(halfThreatColor, threatColor, (threat - 0.5f) * 2.0f);
            else
                return UnityEngine.Color.Lerp(normalColor, halfThreatColor, threat * 2.0f);
        }

        public static void PrivateBuildingAIGetColorPostFix(ushort buildingID, ref Building data, InfoManager.InfoMode infoMode, InfoManager.SubInfoMode subMode, ref UnityEngine.Color __result)
        {
            ItemClass @class = data.Info.m_class;
            ItemClass.Service service = @class.m_service;
            InfoManager instance = Singleton<InfoManager>.instance;

            if (infoMode == InfoManager.InfoMode.Education)
            {
                int r0;
                int r1;
                int r2;
                int r3;

                GetWorkerCount(ref data, out r0, out r1, out r2, out r3);
                int l0;
                int l1;
                int l2;
                int l3;

                switch (service)
                {
                    case ItemClass.Service.Office:
                    case ItemClass.Service.Industrial:
                    case ItemClass.Service.Commercial:

                        PrivateBuildingAI buildingAI = data.Info.m_buildingAI as PrivateBuildingAI;

                        if (buildingAI)
                            buildingAI.CalculateWorkplaceCount((ItemClass.Level)data.m_level, r, data.Width, data.Length, out l0, out l1, out l2, out l3);
                        else
                            return;
                        break;
                    default:
                        PlayerBuildingAI playerbuildingAI = data.Info.m_buildingAI as PlayerBuildingAI;
                        if (playerbuildingAI)
                            playerbuildingAI.CountWorkPlaces(out l0, out l1, out l2, out l3);
                        else
                            return;
                        break;
                }

                int vac = 0;
                int total = 0;

                switch (subMode)
                {
                    case InfoManager.SubInfoMode.ElementarySchool:
                        vac = CalculateFreeVacancies(l0 - r0, l1 - r1, l2 - r2, l3 - r3, Citizen.Education.Uneducated);
                        total = l0;
                        break;
                    case InfoManager.SubInfoMode.HighSchool:
                        vac = CalculateFreeVacancies(l0 - r0, l1 - r1, l2 - r2, l3 - r3, Citizen.Education.OneSchool);
                        total = l1;
                        break;
                    case InfoManager.SubInfoMode.University:
                        vac = CalculateFreeVacancies(l0 - r0, l1 - r1, l2 - r2, l3 - r3, Citizen.Education.TwoSchools);
                        total = l2;
                        break;
                    case InfoManager.SubInfoMode.LibraryEducation:
                        vac = CalculateFreeVacancies(l0 - r0, l1 - r1, l2 - r2, l3 - r3, Citizen.Education.ThreeSchools);
                        total = l3;
                        break;
                }

                if (total > 0)
                {
                    float f = 1.0f * vac / total;
                    __result = UnityEngine.Color.Lerp(__result, capacityThreatColor, f);
                }
            }
            else if (infoMode == InfoManager.InfoMode.Traffic)
            {
                WorldInfoPanel worldInfoPanel = Singleton<WorldInfoPanel>.instance;
                DistrictManager districtManager = Singleton<DistrictManager>.instance;

                InstanceID instanceId = WorldInfoPanel.GetCurrentInstanceID();
                if (instanceId.Type == InstanceType.District && instanceId.District != 0)
                {
                    if (districtManager.GetDistrict(data.m_position) == instanceId.District)
                        __result = UnityEngine.Color.Lerp(__result, normalColor, getTransportationThreat(ref data, instanceId.District));
                    else
                        __result = UnityEngine.Color.Lerp(__result, capacityThreatColor, getTransportationThreat(ref data, instanceId.District));
                }
                else
                    __result = getColorByThreat(getEducationThreat(ref data));
            }
            else if (infoMode == InfoManager.InfoMode.LandValue)
            {
                __result = getColorByThreat(getEducationThreat(ref data, true));
            }
        }
    }
}

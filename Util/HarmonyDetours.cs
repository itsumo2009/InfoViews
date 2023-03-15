namespace InfoViews.Util
{
    public class HarmonyDetours
    {
        public const string Id = "Itsumo2009.InfoViews";
        public static void Apply()
        {
            var harmony = Harmony.HarmonyInstance.Create(Id);
            harmony.PatchAll(typeof(HarmonyDetours).Assembly);
            Loader.HarmonyDetourFailed = false;
        }

        public static void DeApply()
        {
            var harmony = Harmony.HarmonyInstance.Create(Id);
            harmony.UnpatchAll(Id);
        }
    }
}

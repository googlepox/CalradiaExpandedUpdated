using HarmonyLib;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Library;

namespace CalradiaExpanded
{
    [HarmonyPatch(typeof(DefaultMapWeatherModel), "GetWeatherEventInPosition")]
    internal class WeatherPatch
    {
        public static bool Prefix(DefaultMapWeatherModel __instance, MapWeatherModel.WeatherEvent[] ____weatherDataCache, ref MapWeatherModel.WeatherEvent __result, Vec2 pos)
        {
            object[] parameters = new object[] { pos, null, null };
            int num, num2, num3;

            AccessTools.Method(typeof(DefaultMapWeatherModel), "GetNodePositionForWeather").Invoke(__instance, parameters);
            num = (int)parameters[1];
            num2 = (int)parameters[2];
            num3 = (num2 * 32) + num;

            __result = num3 >= 0 && num3 < 1024 ? ____weatherDataCache[num3] : MapWeatherModel.WeatherEvent.Clear;

            return false;
        }
    }
}

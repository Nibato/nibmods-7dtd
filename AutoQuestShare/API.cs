using System.Reflection;

namespace AutoQuestShare
{
    public class AutoQuestShare
    {
        public class API : IModApi
        {
            public void InitMod(Mod _modInstance)
            {
                Log.Out(" Loading Patch: " + GetType());

                var harmony = new HarmonyLib.Harmony(GetType().ToString());
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
        }
    }
}

using System.Reflection;

namespace FocalLengthAiming
{
    public class API : IModApi
    {
        public void InitMod(Mod _modInstance)
        {
            Log.Out(" Loading Patch: " + this.GetType());

            var harmony = new HarmonyLib.Harmony(this.GetType().ToString());

            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}

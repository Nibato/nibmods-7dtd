using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace UnlockedByDefault
{
    [HarmonyPatch(typeof(TileEntitySecureLootContainer), "SetOwner")]
    public static class LootContainerPatches
    {
        public static void Postfix(TileEntitySecureLootContainer __instance)
        {
            __instance.SetLocked(false);
        }
    }


    [HarmonyPatch(typeof(TileEntitySecure), "SetOwner")]
    public static class DoorPatches
    {
        public static void Postfix(TileEntitySecure __instance)
        {
            __instance.SetLocked(false);
        }
    }


    [HarmonyPatch(typeof(TileEntitySign), "SetOwner")]
    public static class SignPatches
    {
        public static void Postfix(TileEntitySign __instance)
        {
            __instance.SetLocked(false);
        }
    }

    /*
    [HarmonyPatch(typeof(EntityDrone), "SetOwner")]
    public static class DronePatches
    {
        public static void Postfix(EntityDrone __instance)
        {
            __instance.SetLocked(false);
        }
    }

    [HarmonyPatch(typeof(EntityVehicle), "SetOwner")]
    public static class VehiclePatches
    {
        public static void Postfix(EntityVehicle __instance)
        {
            __instance.SetLocked(false);
        }
    }
    */
}



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace FocalLengthAiming
{


    [HarmonyPatch(typeof(PlayerMoveController))]
    [HarmonyPatch("Update")]
    class PlayerMoveControllerAimPatch
    {

        private static float Radians(float degrees)
        {
            return ((float)Math.PI / 180) * degrees;
        }

        public static bool Prefix(PlayerMoveController __instance, 
            EntityPlayerLocal ___entityPlayerLocal, 
            float ___defaultSensitivity, 
            ref float ___aimingSensitivity)


        {
            if (!___entityPlayerLocal.AimingGun)
            {
                ___aimingSensitivity = ___defaultSensitivity;
                return true;
            }

            float defaultFOV = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxFOV);
            var aimFOV = ___entityPlayerLocal.cameraTransform.GetComponent<Camera>().fieldOfView;

            var sensMult = (float)(Math.Tan(Radians(aimFOV / 2)) / Math.Tan(Radians(defaultFOV / 2)));
            ___aimingSensitivity = ___defaultSensitivity * sensMult;

            return true;
        }
    }
}

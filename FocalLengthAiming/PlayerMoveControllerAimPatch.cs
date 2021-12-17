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

            var sensMult = (Mathf.Tan((aimFOV / 2) * Mathf.Deg2Rad) / Mathf.Tan((defaultFOV / 2) * Mathf.Deg2Rad));
            ___aimingSensitivity = ___defaultSensitivity * sensMult;

            return true;
        }
    }
}

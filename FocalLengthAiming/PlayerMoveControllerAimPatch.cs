using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace FocalLengthAiming
{
    [HarmonyPatch(typeof(PlayerMoveController))]
    [HarmonyPatch("Update")]
    internal class PlayerMoveControllerAimPatch
    {
        public static bool Prefix(PlayerMoveController __instance,
            EntityPlayerLocal ___entityPlayerLocal,
            float ___lookSensitivity,
            ref float ___zoomSensitivity)

        {
            if (!___entityPlayerLocal.AimingGun)
            {
                ___zoomSensitivity = ___lookSensitivity;
                return true;
            }

            float defaultFOV = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxFOV);
            var aimFOV = ___entityPlayerLocal.cameraTransform.GetComponent<Camera>().fieldOfView;

            var sensMult = Mathf.Tan(aimFOV / 2 * Mathf.Deg2Rad) / Mathf.Tan(defaultFOV / 2 * Mathf.Deg2Rad);
            ___zoomSensitivity = ___lookSensitivity * sensMult;

            return true;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var lookSensitivityRef = AccessTools.Field(typeof(PlayerMoveController), "lookSensitivity");
            var zoomSensitivityRef = AccessTools.Field(typeof(PlayerMoveController), "zoomSensitivity");

            var c = new CodeMatcher(instructions)
                .MatchForward(false,
                              new CodeMatch((i) => i.opcode == OpCodes.Ldloc_S),
                              new CodeMatch(OpCodes.Ldarg_0),
                              new CodeMatch(OpCodes.Ldfld, lookSensitivityRef),
                              new CodeMatch((i) => i.opcode == OpCodes.Ble_Un));
            c.RemoveInstructions(4);
            c.Advance(1);

            c.Operand = zoomSensitivityRef;
            return c.InstructionEnumeration();
        }
    }
}
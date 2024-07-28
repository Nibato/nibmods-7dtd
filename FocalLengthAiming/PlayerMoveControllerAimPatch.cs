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
            Vector2 ___mouseLookSensitivity,
            ref float ___mouseZoomSensitivity)

        {
            if (!___entityPlayerLocal.AimingGun)
            {
                ___mouseZoomSensitivity = 1.0f;
                return true;
            }

            float defaultFOV = Constants.cDefaultCameraFieldOfView; //GamePrefs.GetInt(EnumGamePrefs.OptionsGfxFOV);
            var aimFOV = ___entityPlayerLocal.cameraTransform.GetComponent<Camera>().fieldOfView;

            var sensMult = Mathf.Tan(aimFOV / 2 * Mathf.Deg2Rad) / Mathf.Tan(defaultFOV / 2 * Mathf.Deg2Rad);
            ___mouseZoomSensitivity = sensMult;

            return true;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var entityPlayerLocalRef = AccessTools.Field(typeof(PlayerMoveController), "entityPlayerLocal");
            var lookSensitivityRef = AccessTools.Field(typeof(PlayerMoveController), "mouseLookSensitivity");
            var zoomSensitivityRef = AccessTools.Field(typeof(PlayerMoveController), "mouseZoomSensitivity");
            var previousMouseInputRef = AccessTools.Field(typeof(PlayerMoveController), "previousMouseInput");
            var isAimingGunRef = AccessTools.PropertyGetter(typeof(EntityAlive), "AimingGun");
            var vector2MultFloatRef = AccessTools.Method(typeof(Vector2), "op_Multiply", new[] { typeof(Vector2), typeof(float) });
            var vector2AddVector2Ref = AccessTools.Method(typeof(Vector2), "op_Addition", new[] { typeof(Vector2), typeof(Vector2) });
            var vector2DivFloatRef = AccessTools.Method(typeof(Vector2), "op_Division", new[] { typeof(Vector2), typeof(float) });

            var c = new CodeMatcher(instructions);

            // Multiply our mouse sensitivity by the zoom sensitivity calculated in the prefix
            c.MatchForward(true,
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldfld, lookSensitivityRef)
            )
            .ThrowIfInvalid("Unable to find mouseLookSensitivity access")
            .Advance(1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, zoomSensitivityRef))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Call, vector2MultFloatRef));

            // Delete mouse smoothing code
            c.MatchForward(false,
                new CodeMatch(OpCodes.Ldloc_S),
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldfld, previousMouseInputRef),
                new CodeMatch(OpCodes.Call, vector2AddVector2Ref),
                new CodeMatch(OpCodes.Ldc_R4, 2f),
                new CodeMatch(OpCodes.Call, vector2DivFloatRef),
                new CodeMatch(OpCodes.Stloc_S))
                .ThrowIfInvalid("Unable to find mouse smoothing");

            c.Advance(1);
            c.RemoveInstructions(5);

            // Disable the vanilla arbritrary ADS scaling
            c.MatchForward(false,
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Ldfld, entityPlayerLocalRef),
                    new CodeMatch(OpCodes.Callvirt, isAimingGunRef))
                .ThrowIfInvalid("Unable to find entityPlayerLocal.AimingGun branch");

            c.RemoveInstructions(3);
            c.InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_0));

            return c.InstructionEnumeration();
        }
    }
}
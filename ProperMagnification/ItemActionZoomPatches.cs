﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace ProperMagnification
{ 

    [HarmonyPatch(typeof(ItemActionZoom), "ConsumeScrollWheel")]
    public static class ItemActionZoom_ConsumeScrollWheel
    {
        // Value per "tick" of scroll wheel
        private const float WHEEL_INCREMENT = 0.05f;

        // The original method multiplies the wheel value by this to get the FOV increment
        private const float WHEEL_SCALE = 25f;

        // Number of wheel ticks to go from minimum zoom to maximum zoom
        private const float ZOOM_STEPS = 8f;


        public static bool Prefix(ItemActionData _actionData, ref float _scrollWheelInput)
        {
            if (_scrollWheelInput == 0)
                return true;

            if (!_actionData.invData.holdingEntity.AimingGun)
                return true;

            var zoomData = new ItemActionZoomDataAccessor(_actionData);

            var wheelIncrements = Mathf.Floor(_scrollWheelInput / WHEEL_INCREMENT);
            var zoomStep = (zoomData.MaxZoomOut - zoomData.MaxZoomIn) / ZOOM_STEPS;
           
            _scrollWheelInput = zoomStep * wheelIncrements / WHEEL_SCALE;

            
            return true;
        }
    }

    [HarmonyPatch(typeof(ItemActionZoom), "OnModificationsChanged")]
    public static class ItemActionZoom_OnModificationsChanged
    {       
        public static void Postfix(ItemActionData _data)
        {
            var zoomData = new ItemActionZoomDataAccessor(_data);

            // The default/intended FOV
            var defaultFOV = (int)GamePrefs.GetDefault(EnumGamePrefs.OptionsGfxFOV);

            // Player's current FOV
            var lookFOV = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxFOV);

            // Calculate current magnifications based on default fov
            var zoomInMag = FOVTools.GetMagnification(defaultFOV, zoomData.MaxZoomIn);
            var zoomOutMag = FOVTools.GetMagnification(defaultFOV, zoomData.MaxZoomOut);

            // Apply magnifications based on player's currently selected FOV
            zoomData.MaxZoomIn = Mathf.RoundToInt(FOVTools.Magnify(zoomInMag, lookFOV));
            zoomData.MaxZoomOut = Mathf.RoundToInt(FOVTools.Magnify(zoomOutMag, lookFOV));
            zoomData.CurrentZoom = zoomData.MaxZoomOut;
        }

    }
}

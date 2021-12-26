using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;


namespace ProperMagnification
{
    public class ItemActionZoomDataAccessor
    {
        private ItemActionData data;
        private static AccessTools.FieldRef<object, int> maxZoomInAccessor = null;
        private static AccessTools.FieldRef<object, int> maxZoomOutAccessor = null;
        private static AccessTools.FieldRef<object, float> currentZoomAccessor = null;
        private static AccessTools.FieldRef<object, bool> zoomInProgressAccessor = null;

        public ItemActionZoomDataAccessor(ItemActionData data)
        {
            this.data = data;

            var t = data.GetType();
            if (maxZoomInAccessor == null)
            {
                maxZoomInAccessor = AccessTools.FieldRefAccess<int>(t, "MaxZoomIn");
            }

            if (maxZoomOutAccessor == null)
            { 
                maxZoomOutAccessor = AccessTools.FieldRefAccess<int>(t, "MaxZoomOut");               
            }

            if (currentZoomAccessor == null)
            {
                currentZoomAccessor = AccessTools.FieldRefAccess<float>(t, "CurrentZoom");
            }

            if (zoomInProgressAccessor == null)
            {
                zoomInProgressAccessor = AccessTools.FieldRefAccess<bool>(t, "bZoomInProgress");
            }
        }


        public int MaxZoomIn
        {
            get
            {
                return maxZoomInAccessor(data);
            }
            set
            {
                maxZoomInAccessor(data) = value;
            }
        }

        public int MaxZoomOut
        {
            get
            {
                return maxZoomOutAccessor(data);
            }
            set
            {
                maxZoomOutAccessor(data) = value;
            }
        }

        public float CurrentZoom
        {
            get
            {
                return currentZoomAccessor(data);
            }
            set
            {
                currentZoomAccessor(data) = value;
            }
        }

        public bool ZoomInProgress
        {
            get
            {
                return zoomInProgressAccessor(data);
            }
            set
            {
                zoomInProgressAccessor(data) = value;
            }
        }
    }
}

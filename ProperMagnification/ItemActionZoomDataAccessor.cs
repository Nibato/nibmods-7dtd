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
        private AccessTools.FieldRef<object, int> maxZoomInAccessor;
        private AccessTools.FieldRef<object, int> maxZoomOutAccessor;
        private AccessTools.FieldRef<object, float> currentZoomAccessor;
        private AccessTools.FieldRef<object, bool> zoomInProgressAccessor;

        public ItemActionZoomDataAccessor(ItemActionData data)
        {
            this.data = data;

            var t = data.GetType();
            maxZoomInAccessor = AccessTools.FieldRefAccess<int>(t, "MaxZoomIn");
            maxZoomOutAccessor = AccessTools.FieldRefAccess<int>(t, "MaxZoomOut");
            currentZoomAccessor = AccessTools.FieldRefAccess<float>(t, "CurrentZoom");
            zoomInProgressAccessor = AccessTools.FieldRefAccess<bool>(t, "bZoomInProgress");
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

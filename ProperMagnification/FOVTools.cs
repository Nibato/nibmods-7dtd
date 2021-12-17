using UnityEngine;

namespace ProperMagnification
{
    public static class FOVTools
    {
        public static float Magnify(float magnification, float fov)
        {
            return (Mathf.Atan(Mathf.Tan((fov/2) * Mathf.Deg2Rad) / magnification) * Mathf.Rad2Deg)*2;
        }

        public static float GetMagnification(float originalFOV, float zoomedFOV)
        {

                return Mathf.Tan((originalFOV/2)* Mathf.Deg2Rad) / Mathf.Tan((zoomedFOV/2)* Mathf.Deg2Rad);
        }
    }
}

using UnityEngine;
using GoogleARCore;

namespace DataFlows.Commons
{
    public static class Raycastings
    {
        /// <summary>
        /// Perform a classic Unity Raycast to find `GameObject`s tapped from the user.
        /// </summary>
        /// <param name="touchX">The x-coordinate of the touch event based on the camera canvas</param>
        /// <param name="touchY">The y-cooridnate of the touch event based on the camera canvas</param>
        /// <param name="camera">The camera to raycast from</param>
        /// <returns>A gameobject found by raycasting, or null otherwise</returns>
        public static GameObject RaycastOnDevice(float touchX, float touchY, Camera camera)
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(new Vector2(touchX, touchY));

            /// Check if succeded and if the parent object (that is, the object that contains the mesh) has the asked tag
            if (Physics.Raycast(ray, out hit) && hit.transform.parent.gameObject.tag == "Selectable")
            {
                return hit.transform.parent.gameObject;
            }
            return null;
        }

        /// <summary>
        /// Perform a ARCore raycast to find planes.
        /// </summary>
        /// <param name="touchX">The x-coordinate of the touch event based on the camera canvas</param>
        /// <param name="touchY">The y-cooridnate of the touch event based on the camera canvas</param>
        /// <param name="camera">The camera to raycast from</param>
        /// <returns>A hit plane if there is one, otherwise null</returns>

        public static TrackableHit? RaycastOnPlane(float touchX, float touchY, Camera camera)
        {
            // Raycast against the location the player tapped to search for planes
            TrackableHit hit;
            TrackableHitFlags raycastFlags = TrackableHitFlags.PlaneWithinPolygon | TrackableHitFlags.FeaturePointWithSurfaceNormal;

            if (Frame.Raycast(touchX, touchY, raycastFlags, out hit))
            {
                if (hit.Trackable is DetectedPlane && Vector3.Dot(camera.transform.position - hit.Pose.position,
                                hit.Pose.rotation * Vector3.up) >= 0)
                {

                    DetectedPlane plane = hit.Trackable as DetectedPlane;
                    if (plane.PlaneType == DetectedPlaneType.HorizontalUpwardFacing)
                    {
                        return hit;
                    }
                }
            }
            return null;
        }
    }
}
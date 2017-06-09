using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CommonCode.Drawing
{
    public struct DrawListItem
    {
        /// <summary>
        /// The distance from the camera to the item squared.
        /// </summary>
        public float DistFromCamera;
        /// <summary>
        /// The item to draw.
        /// </summary>
        public IDrawable3D ListItem;

        public DrawListItem(Camera camera, IDrawable3D item)
        {
            DistFromCamera = Vector3.DistanceSquared(camera.CameraPosition, item.WorldPosition);
            ListItem = item;
        }

        public DrawListItem(float distFromCameraSquared, IDrawable3D item)
        {
            DistFromCamera = distFromCameraSquared;
            ListItem = item;
        }

        /// <summary>
        /// Compares two items based on their distances from the camera.
        /// </summary>
        public static int DistanceCompare(DrawListItem a, DrawListItem b)
        {
            if (a.DistFromCamera > b.DistFromCamera)
                return -1;
            else if (a.DistFromCamera < b.DistFromCamera)
                return 1;
            return 0;

        }

        /// <summary>
        /// Compares two items based on their depth biases.
        /// </summary>
        public static int DepthCompare(DrawListItem a, DrawListItem b)
        {
            //Typically things are going to have the same depth bias (0), so the equality case comes first here
            if (a.ListItem.DepthBias == b.ListItem.DepthBias)
                return 0;
            else if (a.ListItem.DepthBias > b.ListItem.DepthBias)
                return 1;
            return -1;
        }
    }
}

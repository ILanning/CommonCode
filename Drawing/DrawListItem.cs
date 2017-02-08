using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CommonCode.Drawing
{
    public struct DrawListItem : IComparer<DrawListItem>
    {
        public float DistFromCamera;
        public IModifiable3D ListItem;

        public DrawListItem(Camera camera, IModifiable3D item)
        {
            DistFromCamera = Vector3.DistanceSquared(camera.CameraPosition, item.WorldPosition);
            ListItem = item;
        }

        public DrawListItem(float distFromCamera, IModifiable3D item)
        {
            DistFromCamera = distFromCamera;
            ListItem = item;
        }

        public int Compare(DrawListItem x, DrawListItem y)
        {
            if (x.DistFromCamera > y.DistFromCamera)
                return -1;
            else if (x.DistFromCamera < y.DistFromCamera)
                return 1;
            return 0;
        }
    }
}

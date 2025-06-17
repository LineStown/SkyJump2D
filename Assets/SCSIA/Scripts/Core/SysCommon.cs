using UnityEngine;

namespace SCSIA
{
    public enum PlayerConfigId
    {
        MaskDude,
        NinjaFrog,
        PinkMan,
        VirtualGuy
    }

    public enum PlatformSpawnerConfigId
    {
        Level1
    }

    public struct PlatformPlace
    {
        public float minX;
        public float maxX;
        public float width;

        public PlatformPlace(float minX, float maxX, float width)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.width = width;
        }

        public void Set(float minX, float maxX)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.width = maxX - minX;
        }
    }

    public struct PlatformPlacePoint
    {
        public float x;
        public float width;

        public PlatformPlacePoint(float x, float width)
        {
            this.x = x;
            this.width = width;
        }

        public void Set(float x, float width)
        {
            this.x = x;
            this.width = width;
        }
    }
}

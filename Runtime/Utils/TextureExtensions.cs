using System;
using UnityEngine;

namespace TerrainGeneration.Utils
{
    public static class TextureExtensions
    {
        public static Texture2D Clone(this Texture2D texture2D)
        {
            var copy = new Texture2D(texture2D.width, texture2D.height);
            copy.SetPixels(texture2D.GetPixels());
            copy.Apply();
            return copy;
        }

        public static void ForEachPixel(this Texture2D texture2D, Action<int, int, Color> forEachPixelAction)
        {
            var pixels = texture2D.GetPixels();

            for (var x = 0; x < texture2D.width; x++)
            {
                for (var y = 0; y < texture2D.height; y++)
                {
                    var sample = pixels[y * texture2D.height + x];
                    forEachPixelAction.Invoke(x, y, sample);
                }
            }
        }

        public static void ForEachPixelRelative(this Texture2D texture2D,
            Action<float, float, Color> forEachPixelAction)
        {
            ForEachPixel(texture2D, (x, y, color) =>
                forEachPixelAction.Invoke(x / (float) texture2D.width, y / (float) texture2D.height, color));
        }
    }
}
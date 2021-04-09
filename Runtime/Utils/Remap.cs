namespace TerrainGeneration.Utils
{
    public static class Utils
    {
        public static float Remap(float value, float lowEndOfRange1, float highEndOfRange1, float lowEndOfRange2,
            float highEndOfRange2)
        {
            return (value - lowEndOfRange1) / (highEndOfRange1 - lowEndOfRange1) * (highEndOfRange2 - lowEndOfRange2) +
                   lowEndOfRange2;
        }
    }
}
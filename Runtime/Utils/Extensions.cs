namespace TerrainGeneration.Utils
{
    public static class Extensions
    {
        public static float NextFloat(this System.Random random)
        {
            return (float) random.NextDouble();
        }
    }
}
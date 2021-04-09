namespace TerrainGeneration.Utils
{
    internal interface IFloatValueProvider
    {
        void Setup(ushort seed);
        float GetValue();
    }
}
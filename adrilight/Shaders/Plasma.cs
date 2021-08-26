

using ComputeSharp;

namespace adrilight.Shaders
{
    /// <summary>
    /// A shader creating an abstract and colorful animation.
    /// Ported from <see href="https://www.shadertoy.com/view/WtjyzR"/>.
    /// <para>Created by Benoit Marini.</para>
    /// <para>License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.</para>
    /// </summary>
    [AutoConstructor]
    internal readonly partial struct Plasma : IPixelShader<Float4>
    {
        /// <summary>
        /// The current time since the start of the application.
        /// </summary>
        public readonly float time;

        /// <summary>
        /// The total number of layers for the final animation.
        /// </summary>
        private const float Pi = 3.14159265f;

        /// <summary>
        /// The number of iterations to calculate a texel value.
        /// </summary>


        /// <summary>
        /// Makes some magic happen.
        /// </summary>


        /// <inheritdoc/>
        public Float4 Execute()
        {
            float t = time * 0.2f;
            float color1, color2, color;
            color1 = (Hlsl.Sin(Hlsl.Dot(ThreadIds.XY, new Float2(Hlsl.Sin(t * 3.0f), Hlsl.Cos(t * 3.0f))) * 0.02f + t * 3.0f) + 1.0f) / 2.0f;
            Float2 center = new Float2(640.0f / 2.0f, 360.0f / 2.0f) + new Float2(640.0f / 2.0f * Hlsl.Sin(-t * 3.0f), 360.0f / 2.0f * Hlsl.Cos(-t * 3.0f));
            color2 = (Hlsl.Cos(Hlsl.Length(ThreadIds.XY - center) * 0.03f) + 1.0f) / 2.0f;
            color = (color1 + color2) / 2.0f;
            float red = (Hlsl.Cos(Pi * color / 0.5f + t * 3.0f) + 1.0f) / 2.0f;
            float green = (Hlsl.Cos(Pi * color / 0.5f + t * 3.0f) + 1.0f) / 2.0f;
            float blue = (Hlsl.Cos(+t * 3.0f) + 1.0f) / 2.0f;
            return new Float4(red, green, blue, 1.0f);
        }
    }
}

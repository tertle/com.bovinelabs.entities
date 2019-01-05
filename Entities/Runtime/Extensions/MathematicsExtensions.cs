// <copyright file="MathematicsExtensions.cs" company="Timothy Raines">
//     Copyright (c) Timothy Raines. All rights reserved.
// </copyright>

namespace BovineLabs.Entities.Extensions
{
    using Unity.Mathematics;

    /// <summary>
    /// Extensions for types within the <see cref="Unity.Mathematics"/> namespace.
    /// </summary>
    public static class MathematicsExtensions
    {
        /// <summary>
        /// Floor a <see cref="float"/> to an integer.
        /// </summary>
        /// <param name="v">The <see cref="float"/> to floor.</param>
        /// <returns>A <see cref="int"/>.</returns>
        public static int FloorToInt(this float v) => (int)math.floor(v);

        /// <summary>
        /// Floor a <see cref="float2"/> to integers.
        /// </summary>
        /// <param name="f2">The <see cref="float2"/> to floor.</param>
        /// <returns>A <see cref="int2"/>.</returns>
        public static int2 FloorToInt(this float2 f2) => (int2)math.floor(f2);

        /// <summary>
        /// Floor a <see cref="float3"/> to integers.
        /// </summary>
        /// <param name="f3">The <see cref="float3"/> to floor.</param>
        /// <returns>A <see cref="int3"/>.</returns>
        public static int3 FloorToInt(this float3 f3) => (int3)math.floor(f3);

        /// <summary>
        /// Returns the euler angles of the right axis from a <see cref="quaternion"/> rotation.
        /// </summary>
        /// <param name="q">The <see cref="quaternion"/> rotation.</param>
        /// <returns>The right axis in euler angles.</returns>
        public static float3 Right(this quaternion q) => new float3x3(q).c0;

        /// <summary>
        /// Returns the euler angles of the up axis from a <see cref="quaternion"/> rotation.
        /// </summary>
        /// <param name="q">The <see cref="quaternion"/> rotation.</param>
        /// <returns>The up axis in euler angles.</returns>
        public static float3 Up(this quaternion q) => new float3x3(q).c1;

        /// <summary>
        /// Returns the euler angles of the forward axis from a <see cref="quaternion"/> rotation.
        /// </summary>
        /// <param name="q">The <see cref="quaternion"/> rotation.</param>
        /// <returns>The forward axis in euler angles.</returns>
        public static float3 Forward(this quaternion q) => new float3x3(q).c2;

        /*/// <summary>
        /// Gets the position from a transform matrix.
        /// </summary>
        /// <param name="matrix">The transform matrix.</param>
        /// <returns>The position.</returns>
        public static float3 GetPosition(this float4x4 matrix) => matrix.c3.xyz;

        /// <summary>
        /// Gets the rotation from a transform matrix.
        /// </summary>
        /// <param name="matrix">The transform matrix.</param>
        /// <returns>The rotation.</returns>
        public static quaternion GetRotation(this float4x4 matrix) => new quaternion(matrix); // this only works when scale = 1

        /// <summary>
        /// Gets the scale from a transform matrix.
        /// </summary>
        /// <param name="matrix">The transform matrix.</param>
        /// <returns>The lossy scale.</returns>
        public static float3 GetLossyScale(this float4x4 matrix) => new float3(
                math.length(matrix.c0.xyz),
                math.length(matrix.c1.xyz),
                math.length(matrix.c2.xyz));*/

        /// <summary>
        /// Scales a transform matrix.
        /// </summary>
        /// <param name="matrix">The transform matrix.</param>
        /// <param name="scale">The scale.</param>
        /// <returns>The scaled matrix.</returns>
        public static float4x4 Scale(this float4x4 matrix, float3 scale)
        {
            return math.mul(matrix, float4x4.Scale(scale));
        }
    }
}
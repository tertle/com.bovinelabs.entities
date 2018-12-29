// <copyright file="NoAllocStringExtension.cs" company="Timothy Raines">
//     Copyright (c) Timothy Raines. All rights reserved.
// </copyright>

namespace BovineLabs.Common.Extensions
{
    using Unity.Mathematics;

    /// <summary>
    /// No allocation extension for converting <see cref="int"/> and <see cref="float"/> to <see cref="string"/>.
    /// </summary>
    /// <remarks>
    /// Idea taken from Graphy. <a href="https://assetstore.unity.com/packages/tools/gui/graphy-ultimate-fps-counter-stats-monitor-debugger-105778"/>
    /// </remarks>
    public static class NoAllocStringExtension
    {
        private const int MinIntValue = -1001;
        private const int MaxIntValue = 16386;
        private const float MinFloatValue = -1001f;
        private const float MaxFloatValue = 16386f;
        private const int DecimalPlaces = 1;

        private static readonly float DecimalMultiplayer;
        private static readonly string[] IntPositiveBuffer;
        private static readonly string[] IntNegativeBuffer;
        private static readonly string[] FloatPositiveBuffer;
        private static readonly string[] FloatNegativeBuffer;

        static NoAllocStringExtension()
        {
            // Int
            if (MaxIntValue >= 0)
            {
                IntPositiveBuffer = new string[MaxIntValue];
                for (int i = 0; i < MaxIntValue; i++)
                {
                    IntPositiveBuffer[i] = i.ToString();
                }
            }

            if (MinIntValue <= 0)
            {
                int lenght = math.abs(MinIntValue);
                IntNegativeBuffer = new string[lenght];
                for (int i = 0; i < lenght; i++)
                {
                    IntNegativeBuffer[i] = (-i).ToString();
                }
            }

            // Float
            DecimalMultiplayer = math.pow(10, math.clamp(DecimalPlaces, 1, 5));

            int negativeLenght = GetIndex(MinFloatValue);
            int positiveLenght = GetIndex(MaxFloatValue);

            if (positiveLenght >= 0)
            {
                FloatPositiveBuffer = new string[positiveLenght];
                for (int i = 0; i < positiveLenght; i++)
                {
                    FloatPositiveBuffer[i] = FromIndex(i).ToString("0.0");
                }
            }

            if (negativeLenght >= 0)
            {
                FloatNegativeBuffer = new string[negativeLenght];
                for (int i = 0; i < negativeLenght; i++)
                {
                    FloatNegativeBuffer[i] = FromIndex(-i).ToString("0.0");
                }
            }
        }

        /// <summary>
        /// Convert an <see cref="int"/> to a <see cref="string"/> without allocations.
        /// </summary>
        /// <param name="value">The <see cref="int"/> value to convert to a  <see cref="string"/>.</param>
        /// <returns>A <see cref="string"/> representing the <see cref="int"/> value.</returns>
        public static string ToStringNonAlloc(this int value)
        {
            if (value >= 0 && value < IntPositiveBuffer.Length)
            {
                return IntPositiveBuffer[value];
            }

            if (value < 0 && -value < IntNegativeBuffer.Length)
            {
                return IntNegativeBuffer[-value];
            }

            return value.ToString();
        }

        /// <summary>
        /// Convert an <see cref="float"/> to a <see cref="float"/> without allocations.
        /// </summary>
        /// <param name="value">The <see cref="float"/> value to convert to a  <see cref="string"/>.</param>
        /// <param name="format">Optional formatting for the float. Only used when the float can't be looked up and allocation is required.</param>
        /// <returns>A <see cref="string"/> representing the <see cref="float"/> value.</returns>
        public static string ToStringNonAlloc(this float value, string format = null)
        {
            var valIndex = GetIndex(value);
            if (value >= 0 && valIndex < FloatPositiveBuffer.Length)
            {
                return FloatPositiveBuffer[valIndex];
            }

            if (value < 0 && valIndex < FloatNegativeBuffer.Length)
            {
                return FloatNegativeBuffer[valIndex];
            }

            return value.ToString(format);
        }

        private static int GetIndex(float f)
        {
            return (int)math.abs(f * DecimalMultiplayer);
        }

        private static float FromIndex(int i)
        {
            return i / DecimalMultiplayer;
        }
    }
}
//******************************************************************************************
// Copyright © 2016 - 2022 Wolfgang Foerster (wolfoerster@gmx.de)
//
// This file is part of the WFTools3D project which can be found on github.com.
//
// WFTools3D is free software: you can redistribute it and/or modify it under the terms 
// of the GNU General Public License as published by the Free Software Foundation, 
// either version 3 of the License, or (at your option) any later version.
// 
// WFTools3D is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
// See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//******************************************************************************************

namespace WFTools3D
{
    using System;
    using System.Globalization;
    using System.Windows;

    public static class MathUtils
    {
        static public readonly double PI = 3.1415926535897932384626433;
        static public readonly double PIx2 = PI * 2.0;
        static public readonly double PIo2 = PI * 0.5;

        /// <summary>
        /// Normalizes an angle to be inbetween -PI and PI.
        /// </summary>
        public static double NormalizeAngle(this double angle)
        {
            if (angle < -PI || angle > PI)
            {
                angle = Math.IEEERemainder(angle, PIx2);

                if (angle < -PI)
                    angle += PIx2;

                else if (angle > PI)
                    angle -= PIx2;
            }

            return angle;
        }

        static public double ToRadians(double angleInDegrees)
        {
            return angleInDegrees * PI / 180.0;
        }

        static public double ToDegrees(double angleInRadians)
        {
            return angleInRadians * 180.0 / PI;
        }

        static public double ToSeconds(int d, int h, int m, double s)
        {
            return ((d * 24 + h) * 60 + m) * 60 + s;
        }

        static public bool IsValidIndex(int index, int count)
        {
            if (index < 0 || index >= count)
                return false;

            return true;
        }

        static public bool IsValidNumber(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                return false;

            return true;
        }

        public static double Clamp(double value, double minValue, double maxValue)
        {
            return Math.Min(Math.Max(value, minValue), maxValue);
        }

        public static int Clamp(int value, int minValue, int maxValue)
        {
            return Math.Min(Math.Max(value, minValue), maxValue);
        }

        public static bool IsValid(this Point pt)
        {
            if (!MathUtils.IsValidNumber(pt.X) || !MathUtils.IsValidNumber(pt.Y))
                return false;

            return true;
        }

        /// <summary>
        /// Calling ToStringExt() with invariant culture.
        /// </summary>
        public static string ToStringInv(this double value, string format = null)
        {
            return ToStringExt(value, CultureInfo.InvariantCulture, format);
        }

        /// <summary>
        /// Calling ToStringExt() with current culture.
        /// </summary>
        public static string ToStringCur(this double value, string format = null)
        {
            return ToStringExt(value, CultureInfo.CurrentCulture, format);
        }

        /// <summary>
        /// Extended ToString() method. Converts the numeric value of this double to a string, using the specified format.<para/>
        /// There are two additional features on top of the basic double.ToString() implementation:<para/>
        /// 1. New format specifier 's' or 'S' to specify the number of significant figures.<para/>
        /// 2. Optimized scientific notation (remove dispensable characters, e.g. 1.2300e+004 will become 1.23e4)
        /// </summary>
        /// <param name="value">The double value.</param>
        /// <param name="cultureInfo">An object that supplies culture-specific formatting information.</param>
        /// <param name="format">The format specifier. If this is null or empty, "g4" is used.</param>
        public static string ToStringExt(this double value, CultureInfo cultureInfo, string format = null)
        {
            var formatInfo = cultureInfo.NumberFormat;

            //--- special value?
            if (double.IsNaN(value))
                return formatInfo.NaNSymbol;

            if (double.IsPositiveInfinity(value))
                return formatInfo.PositiveInfinitySymbol;

            if (double.IsNegativeInfinity(value))
                return formatInfo.NegativeInfinitySymbol;

            if (string.IsNullOrWhiteSpace(format))
                format = "g4";

            //--- significant figures?
            if (format[0] == 's' || format[0] == 'S')
            {
                int.TryParse(format.Remove(0, 1), out int sigFigures);
                double roundedValue = RoundSignificantDigits(value, sigFigures, out int roundingPosition);

                //--- 0 shall be formatted as 1 or any other integer < 10:
                if (roundedValue == 0.0d)
                {
                    sigFigures = Clamp(sigFigures, 1, 14);
                    return string.Format(formatInfo, "{0:F" + (sigFigures - 1) + "}", value);
                }

                // Check if the rounding position is positive and is not past 100 decimal places.
                // If the rounding position is greater than 100, string.format won't represent the number correctly.
                // ToDo:  What happens when the rounding position is greater than 100?
                if (roundingPosition > 0 && roundingPosition < 100)
                    return string.Format(formatInfo, "{0:F" + roundingPosition + "}", roundedValue);

                return roundedValue.ToString("F0", formatInfo);
            }

            //--- Convert to string using format
            string text = value.ToString(format, formatInfo);

            //--- If text is not in scientific notation, just return it as is:
            int e = text.IndexOfAny(new char[] { 'e', 'E' });
            if (e < 0)
                return text;

            //--- Remove trailing zeros and possibly decimal separator from the mantissa
            char sep = formatInfo.NumberDecimalSeparator[0];
            string mantissa = text.Substring(0, e);

            mantissa = mantissa.TrimEnd(new char[] { '0', sep });
            if (mantissa.Length == 0)
                return "0";

            //--- Remove leading zeros and possibly plus sign from the exponent
            char negativeSign = formatInfo.NegativeSign[0];
            char positiveSign = formatInfo.PositiveSign[0];

            string exponent = text.Substring(e + 1);
            bool isNegative = exponent[0] == negativeSign;

            exponent = exponent.Trim(new char[] { '0', positiveSign, negativeSign });
            if (exponent.Length == 0)
                return mantissa;

            //--- Build up the result
            if (isNegative)
                return mantissa + text[e] + negativeSign + exponent;

            return mantissa + text[e] + exponent;
        }

        public static double RoundSignificantDigits(this double value, int sigFigures)
        {
            return RoundSignificantDigits(value, sigFigures, out _);
        }

        private static double RoundSignificantDigits(double value, int sigFigures, out int roundingPosition)
        {
            // the sigFigures parameter must be between 0 and 15, exclusive.
            roundingPosition = 0;

            if (double.IsNaN(value))
                return double.NaN;

            if (double.IsPositiveInfinity(value))
                return double.PositiveInfinity;

            if (double.IsNegativeInfinity(value))
                return double.NegativeInfinity;

            //--- have to set a limit somewhere
            if (Math.Abs(value) <= 1e-98)
                return 0;

            //--- don't throw exceptions if sigFigures is out of range
            sigFigures = Clamp(sigFigures, 1, 14);

            // The resulting rounding position will be negative for rounding at whole numbers, and positive for decimal places.
            roundingPosition = sigFigures - 1 - (int)(Math.Floor(Math.Log10(Math.Abs(value))));

            // Try to use a rounding position directly, if no scale is needed.
            // This is because the scale mutliplication after the rounding can introduce error, although 
            // this only happens when you're dealing with really tiny numbers, i.e 9.9e-14.
            if (roundingPosition > 0 && roundingPosition < 15)
                return Math.Round(value, roundingPosition, MidpointRounding.AwayFromZero);

            // Shouldn't get here unless we need to scale it.
            // Set the scaling value, for rounding whole numbers or decimals past 15 places
            double scale = Math.Pow(10, Math.Ceiling(Math.Log10(Math.Abs(value))));
            return Math.Round(value / scale, sigFigures, MidpointRounding.AwayFromZero) * scale;
        }
    }
}

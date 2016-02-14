//******************************************************************************************
// Copyright © 2016 Wolfgang Foerster (wolfoerster@gmx.de)
//
// This file is part of the WFTools3D project which can be found on github.com
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
using System;
using System.Windows;
using System.Windows.Input;

namespace WFTools3D
{
	public class TextureTransform
	{
		public TextureTransform(double from1, double from2, double tx1, double tx2, double ty1, double ty2)
		{
			tx.Init(from1, from2, tx1, tx2);
			ty.Init(from1, from2, ty1, ty2);
		}
		LinearTransform tx = new LinearTransform();
		LinearTransform ty = new LinearTransform();

		public Point Transform(double x, double y)
		{
			return new Point(MathUtils.Clamp(tx.Transform(x), 0, 1), MathUtils.Clamp(ty.Transform(y), 0, 1));
		}
	}

#if !WFToolsAvailable
	public static class Helpers
	{
		public static bool IsShiftDown()
		{
			return Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
		}

		public static bool IsCtrlDown()
		{
			return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
		}

		public static bool IsAltDown()
		{
			return Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt) || Keyboard.IsKeyDown(Key.System);
		}
	}

	public static class MathUtils
	{
		static public readonly double PI = 3.1415926535897932384626433;
		static public readonly double PIx2 = PI * 2.0;
		static public readonly double PIo2 = PI * 0.5;

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

		public static bool IsValid(this Point pt)
		{
			if (!MathUtils.IsValidNumber(pt.X) || !MathUtils.IsValidNumber(pt.Y))
				return false;

			return true;
		}
	}

	public class LinearTransform
	{
		public LinearTransform()
		{
			Init(0, 1, 0, 1);
		}

		public LinearTransform(double from1, double from2, double to1, double to2)
		{
			Init(from1, from2, to1, to2);
		}

		public void Init(double from1, double from2, double to1, double to2)
		{
			slope = (to2 - to1) / (from2 - from1);
			offset = to1 - slope * from1;
		}
		double slope, offset;

		public double Transform(double value)
		{
			return slope * value + offset;
		}

		public double BackTransform(double value)
		{
			return (value - offset) / slope;
		}
	}
#endif
}

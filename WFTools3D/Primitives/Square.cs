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
using System.Windows.Media.Media3D;

namespace WFTools3D
{
	/// <summary>
	/// A rectangle in the xy plane centered at the origin with size 2 (from -1 to +1 in both x and y).
	/// </summary>
	public class Square : Primitive3D
	{
		public Square()
			: base(1)
		{
		}

		public Square(int divisions)
			: base(divisions)
		{
		}

#if todo
		/// <summary>
		/// Gets or sets the extent in x.
		/// </summary>
		public Range RangeX
		{
			get { return new Range(Position.X - ScaleX, Position.X + ScaleX); }
			set
			{
				LockUpdates(true);
				ScaleX = value.Width * 0.5;
				Point3D position = Position;
				position.X = value.Center;
				Position = position;
				LockUpdates(false);
			}
		}

		/// <summary>
		/// Gets or sets the extent in y.
		/// </summary>
		public Range RangeY
		{
			get { return new Range(Position.Y - ScaleY, Position.Y + ScaleY); }
			set
			{
				LockUpdates(true);
				ScaleY = value.Width * 0.5;
				Point3D position = Position;
				position.Y = value.Center;
				Position = position;
				LockUpdates(false);
			}
		}

		/// <summary>
		/// Gets or sets the extent in x and y.
		/// </summary>
		public Area Area
		{
			get { return new Area(RangeX, RangeY); }
			set
			{
				LockUpdates(true);
				RangeX = value.RangeX;
				RangeY = value.RangeY;
				LockUpdates(false);
			}
		}
#endif

		protected override MeshGeometry3D CreateMesh()
		{
			return MeshUtils.CreateSquare(divisions);
		}
	}
}

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
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WFTools3D
{
	public class CubeModel : Object3D
	{
		public CubeModel(int divisions = 1)
		{
			Right = InitSquare(Math3D.UnitX, divisions);
			Left = InitSquare(-Math3D.UnitX, divisions);
			Rear = InitSquare(Math3D.UnitY, divisions);
			Front = InitSquare(-Math3D.UnitY, divisions);
			Top = InitSquare(Math3D.UnitZ, divisions);
			Bottom = InitSquare(-Math3D.UnitZ, divisions);
		}

		public Square Left, Right, Front, Rear, Top, Bottom;

		private Square InitSquare(Vector3D v, int divisions)
		{
			Square square = new Square(divisions);
			square.LockUpdates(true);
			square.Position = (Point3D)v;
			if (v.Z == 0)
			{
				square.Rotation1 = Math3D.RotationX(90);
				square.Rotation2 = Math3D.RotationZ(90 * (v.X != 0 ? v.X : (v.Y + 1)));
			}
			else if (v.Z < 0)
			{
				square.Rotation1 = Math3D.RotationX(180);
			}
			square.LockUpdates(false);
			square.Transform.Freeze();
			Children.Add(square);
			return square;
		}
	}
}

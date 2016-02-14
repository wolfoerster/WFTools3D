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
	/// A cylinder or cylinder segment based in the xy plane with the base circle centered at the origin and a height of 1.<para/>
	/// Using properties From and To might be more convenient to use than the Rotation properties.<para/>
	/// NOTE: The Position property corresponds with centre of the base circle and not with the center of the body (as it does for Cube or Sphere).
	/// </summary>
	public class Cylinder : Primitive3D
	{
		public Cylinder()
			: base(32)
		{
		}

		public Cylinder(int divisions)
			: base(divisions)
		{
		}

		protected override MeshGeometry3D CreateMesh()
		{
			return MeshUtils.CreateCylinderSegment(divisions, isClosed, upperRadius, startDegrees, stopDegrees);
		}

		public bool IsClosed
		{
			get { return isClosed; }
			set { isClosed = value; InitMesh(); }
		}
		private bool isClosed = false;

		public double Radius
		{
			get { return ScaleX; }
			set { ScaleX = ScaleY = value; }
		}

		/// <summary>
		/// Gets or sets the ratio of upper radius at z = 1 to lower radius at z = 0
		/// </summary>
		public double UpperRadius
		{
			get { return upperRadius; }
			set { upperRadius = value; InitMesh(); }
		}
		private double upperRadius = 1.0;

		/// <summary>
		/// Gets or sets the start angle.
		/// </summary>
		public double StartDegrees
		{
			get { return startDegrees; }
			set { startDegrees = value; InitMesh(); }
		}
		private double startDegrees = 0;

		/// <summary>
		/// Gets or sets the stop angle.
		/// </summary>
		public double StopDegrees
		{
			get { return stopDegrees; }
			set { stopDegrees = value; InitMesh(); }
		}
		private double stopDegrees = 360;

		/// <summary>
		/// Gets or sets the center point of the base circle.
		/// </summary>
		public Point3D From
		{
			get { return from; }
			set { from = value; OnFromToChanged(); }
		}
		private Point3D from = new Point3D(0, 0, 0);

		/// <summary>
		/// Gets or sets the center point of the top circle.
		/// </summary>
		public Point3D To
		{
			get { return to; }
			set { to = value; OnFromToChanged(); }
		}
		private Point3D to = new Point3D(0, 0, 1);

		/// <summary>
		/// Gets the main axis of this cylinder.
		/// </summary>
		public Vector3D MainAxis
		{
			get { return mainAxis = to - from; }
		}
		private Vector3D mainAxis;

		/// <summary>
		/// Called when From or To are changed.
		/// </summary>
		private void OnFromToChanged()
		{
			ScaleZ = MainAxis.Length;
			if (ScaleZ == 0)
				return;

			Position = From;
			double phi = Math3D.UnitZ.AngleTo(mainAxis);

			if (phi < 0.01 || phi > 179.99)
			{
				Rotation1 = Math3D.RotationX(phi);
				return;
			}

			Vector3D rotAxis = Math3D.UnitZ.Cross(mainAxis);
			Rotation1 = new Quaternion(rotAxis, phi);
		}
	}
}

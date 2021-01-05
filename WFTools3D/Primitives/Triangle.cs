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
    /// A triangle in the xy plane with points P1=(0,0,0), P2=(1,0,0) and P3=(0,1,0).
    /// </summary>
    public class Triangle : Primitive3D
    {
        public Triangle()
            : base(1)
        {
        }

        public Triangle(int divisions)
            : base(divisions)
        {
        }

        public Point3D P1
        {
            get { return p1; }
            set { p1 = value; InitMesh(); }
        }
        private Point3D p1 = new Point3D(0, 0, 0);

        public Point3D P2
        {
            get { return p2; }
            set { p2 = value; InitMesh(); }
        }
        private Point3D p2 = new Point3D(1, 0, 0);

        public Point3D P3
        {
            get { return p3; }
            set { p3 = value; InitMesh(); }
        }
        private Point3D p3 = new Point3D(0, 1, 0);

        protected override MeshGeometry3D CreateMesh()
        {
            return MeshUtils.CreateTriangle(p1, p2, p3, divisions);
        }
    }
}

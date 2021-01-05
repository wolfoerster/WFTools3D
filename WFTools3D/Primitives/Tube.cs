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
using System.Collections.Generic;
using System.Windows.Media.Media3D;

#if WFToolsAvailable
using WFTools;
#endif

namespace WFTools3D
{
    /// <summary>
    /// A tube with a given radius along a given path.
    /// </summary>
    public class Tube : Primitive3D
    {
        public Tube()
            : base(16)
        {
        }

        public Tube(int divisions)
            : base(divisions)
        {
        }

        public double Radius
        {
            get { return radius; }
            set { radius = value; InitMesh(); }
        }
        private double radius = 0.2;

        public IList<Point3D> Path
        {
            get { return path; }
            set { path = value; InitMesh(); }
        }
        IList<Point3D> path;

        public bool IsPathClosed
        {
            get { return isPathClosed; }
            set { isPathClosed = value; InitMesh(); }
        }
        private bool isPathClosed;

        protected override MeshGeometry3D CreateMesh()
        {
            if (divisions < 3 || divisions > 999 || radius <= 0 || path == null || path.Count < 2)
                return null;

            List<Point> section = new List<Point>(divisions + 1);
            for (int id = 0; id <= divisions; id++)
            {
                double phi = id * MathUtils.PIx2 / divisions;
                section.Add(new Point(radius * Math.Cos(phi), radius * Math.Sin(phi)));
            }

            return MeshUtils.CreateTube(path, section, isPathClosed);
        }
    }
}

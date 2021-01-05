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
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WFTools3D
{
    public class AxisModel : Object3D
    {
        public AxisModel()
            : this(1)
        {
        }

        public AxisModel(double length)
            : this(length, length, length)
        {
        }

        public AxisModel(double length, double radius)
            : this(length, length, length, radius)
        {
        }

        public AxisModel(double length, double radius, int divisions)
            : this(length, length, length, radius, divisions)
        {
        }

        public AxisModel(double lengthX, double lengthY, double lengthZ)
            : this(lengthX, lengthY, lengthZ, 0.01, 4)
        {
        }

        public AxisModel(double lengthX, double lengthY, double lengthZ, double radius)
            : this(lengthX, lengthY, lengthZ, radius, 4)
        {
        }

        public AxisModel(double lengthX, double lengthY, double lengthZ, double radius, int divisions)
        {
            AddLine(Math3D.Origin, new Point3D(lengthX, 0, 0), radius, Brushes.DarkRed, divisions);
            AddLine(Math3D.Origin, new Point3D(0, lengthY, 0), radius, Brushes.DarkGreen, divisions);
            AddLine(Math3D.Origin, new Point3D(0, 0, lengthZ), radius, Brushes.Blue, divisions);
        }

        private void AddLine(Point3D p0, Point3D p1, double radius, Brush brush, int divisions)
        {
            Cylinder line = new Cylinder();
            line.Divisions = divisions;
            line.IsClosed = true;
            line.DiffuseMaterial.Brush = brush;
            line.Radius = radius;
            line.From = p0;
            line.To = p1;
            Children.Add(line);
        }
    }
}

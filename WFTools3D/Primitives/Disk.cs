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
    /// A disk or disk segment in the xy plane centered at the origin with radius 1.
    /// </summary>
    public class Disk : Primitive3D
    {
        public Disk()
            : base(32)
        {
        }

        public Disk(int divisions)
            : base(divisions)
        {
        }

        public double Radius
        {
            get { return ScaleX; }
            set { ScaleX = ScaleY = value; }
        }

        public double InnerRadius
        {
            get { return innerRadius; }
            set { innerRadius = value; InitMesh(); }
        }
        private double innerRadius = 0;

        public double StartDegrees
        {
            get { return startDegrees; }
            set { startDegrees = value; InitMesh(); }
        }
        private double startDegrees = 0;

        public double StopDegrees
        {
            get { return stopDegrees; }
            set { stopDegrees = value; InitMesh(); }
        }
        private double stopDegrees = 360;

        protected override MeshGeometry3D CreateMesh()
        {
            return MeshUtils.CreateDiskSegment(divisions, innerRadius / Radius, startDegrees, stopDegrees);
        }
    }
}

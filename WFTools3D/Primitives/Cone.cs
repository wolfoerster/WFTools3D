﻿//******************************************************************************************
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
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A cone based in the xy plane with the base circle centered at the origin and a height of 1.<para/>
    /// NOTE: The Position property corresponds with centre of the base circle and not with the center of the body (as it does for Cube or Sphere).
    /// </summary>
    public class Cone : Primitive3D
    {
        public Cone()
            : base(32)
        {
        }

        public Cone(int divisions)
            : base(divisions)
        {
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

        protected override MeshGeometry3D CreateMesh()
        {
            return MeshUtils.CreateCone(divisions, isClosed);
        }
    }
}

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
    /// <summary>
    /// A cube centered at the origin with extent of 1 in all directions.<para/>
    /// The Position property corresponds with the overall centre of the cube (same as it does for a Sphere).
    /// </summary>
    public class Cube : Primitive3D
    {
        public Cube()
            : base(1)
        {
        }

        public Cube(int divisions)
            : base(divisions)
        {
        }

        public bool IsClosed
        {
            get { return isClosed; }
            set { isClosed = value; InitMesh(); }
        }
        private bool isClosed = true;

        protected override MeshGeometry3D CreateMesh()
        {
            return MeshUtils.CreateCube(divisions, isClosed);
        }
    }
}

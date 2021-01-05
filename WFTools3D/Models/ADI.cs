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
    /// Something like an attitude director indicator for flight simulators.
    /// </summary>
    public class ADI : Object3D
    {
        /// <summary>
        /// ctor
        /// </summary>
        public ADI()
        {
            Children.Add(horizon);
            Children.Add(targetIndicator);
        }
        Bone horizon = new Bone(0.0002, 2, Brushes.Azure, Brushes.DarkBlue, Brushes.DarkBlue);
        Bone targetIndicator = new Bone(0.0002, 2.5, Brushes.Azure, Brushes.Green, Brushes.Red);

        public Point3D TargetPoint = Math3D.Origin;

        /// <summary>
        /// Visualize roll angle and direction to origin (or height above ground).
        /// </summary>
        public void Update(CameraBox Camera)
        {
            Vector3D leftDirection = Camera.LeftDirection.Rotate(Camera.LookDirection, -Camera.RollAngle);

            double length = 0.007;
            Point3D position = Camera.Position + (Camera.NearPlaneDistance + 1.1 * length) * Camera.LookDirection;
            Point3D pt = position + length * leftDirection;
            horizon.SetLocation(pt, pt + 2 * (position - pt));

            Vector3D toTarget = TargetPoint - position;
            toTarget.Normalize();
            targetIndicator.SetLocation(position, position + length * toTarget);
        }
    }
}

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
    /// A stick with a ball at each end.
    /// </summary>
    public class Bone : Object3D
    {
        /// <summary>
        /// 
        /// </summary>
        public Bone(double radius, double factor, Brush brush1, Brush brush2, Brush brush3)
        {
            stick.Divisions = 6;
            stick.Radius = radius;

            ball1.Divisions = ball2.Divisions = 4;
            ball1.Radius = ball2.Radius = factor * radius;

            stick.DiffuseMaterial.Brush = brush1;
            ball1.DiffuseMaterial.Brush = brush2;
            ball2.DiffuseMaterial.Brush = brush3;

            Children.Add(ball1);
            Children.Add(ball2);
            Children.Add(stick);
        }
        Sphere ball1 = new Sphere();
        Sphere ball2 = new Sphere();
        Cylinder stick = new Cylinder();

        /// <summary>
        /// 
        /// </summary>
        public void SetLocation(Point3D from, Point3D to)
        {
            ball1.Position = stick.From = from;
            ball2.Position = stick.To = to;
        }
    }
}

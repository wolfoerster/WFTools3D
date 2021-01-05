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
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Reflection;

namespace WFTools3D
{
    /// <summary>
    /// A small airplane.
    /// </summary>
    public class Airplane : Object3D
    {
        static Airplane()
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string[] names = assembly.GetManifestResourceNames();
                string name = names.FirstOrDefault(x => x.Contains("WFTools3D.jpg"));
                if (name != null)
                {
                    Stream stream = assembly.GetManifestResourceStream(name);
                    if (stream != null)
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                        ImageBrush imbrush = new ImageBrush(bitmap);
                        imbrush.TileMode = TileMode.Tile;
                        imbrush.Viewport = new Rect(0, 0, 0.5, 1);
                        imbrush.Freeze();
                        brush = imbrush;
                    }
                }
            }
            catch
            {
                brush = Brushes.Silver;
            }
        }
        static Brush brush;

        public Airplane()
        {
            Cylinder body = new Cylinder();
            body.DiffuseMaterial.Brush = brush;
            body.To = new Point3D(0, -1, 0);
            body.From = new Point3D(0, 0.4, 0);
            body.IsClosed = true;
            body.Radius = 0.1;
            Children.Add(body);

            Cone head = new Cone();
            head.DiffuseMaterial.Brush = Brushes.Red;
            head.Rotation1 = Math3D.RotationX(-90);
            head.Radius = 0.1;
            head.ScaleZ = 0.2;
            head.Position = new Point3D(0, 0.4, 0);
            Children.Add(head);

            Cylinder wings = new Cylinder();
            wings.DiffuseMaterial.Brush = Brushes.Yellow;
            wings.From = new Point3D(-1, 0, -0.01);
            wings.To = new Point3D(1, 0, -0.01);
            wings.ScaleX = 0.015;
            wings.ScaleY = 0.1;
            wings.IsClosed = true;
            Children.Add(wings);

            Cylinder tail = new Cylinder();
            tail.DiffuseMaterial.Brush = Brushes.Yellow;
            tail.From = new Point3D(-0.35, -0.9, 0.03);
            tail.To = new Point3D(0.35, -0.9, 0.03);
            tail.ScaleX = 0.015;
            tail.ScaleY = 0.1;
            tail.IsClosed = true;
            Children.Add(tail);

            Cylinder fin = new Cylinder();
            fin.DiffuseMaterial.Brush = Brushes.Green;
            fin.UpperRadius = 0.5;
            fin.IsClosed = true;
            fin.ScaleX = 0.015;
            fin.ScaleY = 0.1;
            fin.ScaleZ = 0.3;
            fin.Position = new Point3D(0, -0.9, 0);
            fin.Rotation2 = Math3D.RotationX(10);
            Children.Add(fin);
        }
    }
}

//******************************************************************************************
// Copyright © 2021 Wolfgang Foerster (wolfoerster@gmx.de)
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Collections.Generic;
using System.Windows.Controls;
using WFTools3D;

namespace WFTools3DDemo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;

            ImageBrush brush = GetBrush("Ground", false);
            brush.TileMode = TileMode.Tile;
            brush.Viewport = new Rect(0, 0, 0.02, 0.02);
            brush.Freeze();

            InitializeScene(brush);
            InitializeCameras();
            InitializeInfo();
        }

        ImageBrush GetBrush(string name, bool doFreeze = true)
        {
            ImageBrush brush = Resources[name] as ImageBrush;
            if (doFreeze) brush.Freeze();
            return brush;
        }

        void InitializeScene(Brush brush)
        {
            Sphere skyBox = new Sphere(32) { Radius = 90 };
            skyBox.Rotation1 = Math3D.RotationZ(90);
            skyBox.Transform.Freeze();

            ImageBrush skyBrush = GetBrush("Sky", false);
            skyBrush.Viewport = new Rect(0, 0, 1, 0.5);
            skyBrush.Freeze();

            skyBox.Material = null;
            skyBox.BackMaterial = new MaterialGroup();
            skyBox.BackMaterial.Children.Add(new DiffuseMaterial(skyBrush));
            scene.Models.Add(skyBox);

            Square square = new Square(100);
            square.ScaleX = 60;
            square.ScaleY = 40;
            square.Transform.Freeze();
            square.DiffuseMaterial.Brush = brush;
            square.BackMaterial = square.Material;
            scene.Models.Add(square);

            Brush brush1 = GetBrush("Facade");
            Brush brush2 = GetBrush("Poster");

            for (int i = 0; i < 170; i++)
            {
                double x = 58 * (2 * randy.NextDouble() - 1);
                double y = 38 * (2 * randy.NextDouble() - 1);
                double z = 3 * randy.NextDouble() + 1;
                Object3D obj = RandomPrimitive(x, y, z, brush1, brush2);
                scene.Models.Add(obj);
            }

            Tube tube = new Tube(12) { Radius = 0.1, IsPathClosed = true };
            int n = 180;
            List<Point3D> path = new List<Point3D>(n);
            for (int i = 0; i < n; i++)
            {
                double t = MathUtils.PIx2 * i / n;
                path.Add(new Point3D(Math.Cos(t), Math.Sin(t), Math.Cos(6 * t) / 3));
            }
            tube.Path = path;
            tube.DiffuseMaterial.Brush = Brushes.Green;
            tube.Position = new Point3D(-2, 2, 1);
            scene.Models.Add(tube);

            RunningMan man = new RunningMan(60, 40);
            scene.Models.Add(man);
            scene.TimerTicked += man.TimerTick;
            scene.TimerTicked += TimerTick;

            scene.ToggleHelperModels();
            FocusManager.SetFocusedElement(this, scene);
        }
        Random randy = new Random(0);

        Object3D RandomPrimitive(double x, double y, double z, Brush brush1, Brush brush2)
        {
            Primitive3D obj = null;
            double angle1 = 180 * randy.NextDouble();
            double angle2 = 180 * (1 + randy.NextDouble());
            int index = count > 150 ? 2 : ++count % 10;
            switch (index)
            {
                case 0: //--- more cubes, please :-)
                case 1: obj = new Cube(); break;
                case 2: obj = new Balloon(z); break;
                case 3: obj = new Cone { IsClosed = true }; break;
                case 4: obj = new Cylinder { IsClosed = true }; break;
                case 5: obj = new Cylinder { StartDegrees = angle1, StopDegrees = angle2, IsClosed = true }; break;
                case 6: obj = new Disk(); break;
                case 7: obj = new Disk { StartDegrees = angle1, StopDegrees = angle2 }; break;
                case 8: obj = new Square(); break;
                case 9: obj = new Triangle(); break;
            }
            obj.ScaleZ = z;
            obj.Position = new Point3D(x, y, z);
            obj.DiffuseMaterial.Brush = GetRandomBrush();

            if (index > 5)
            {
                //--- flat objects need a BackMaterial and are rotated
                obj.BackMaterial = obj.Material;
                obj.Rotation1 = Math3D.RotationX(angle1);
                obj.Rotation2 = Math3D.RotationY(angle2);
            }
            else if (index < 2)//--- cubes
            {
                obj.DiffuseMaterial.Brush = brush1;
                obj.Position = new Point3D(x, y, z + 0.01);//--- avoid z fighting with the ground
            }
            else if (index == 2)//--- balloon
            {
                obj.ScaleZ = obj.ScaleX * 1.2;
            }
            else if (index == 4 || index == 5)//--- cylinder
            {
                obj.DiffuseMaterial.Brush = brush2;
                obj.Rotation1 = new Quaternion(Math3D.UnitZ, angle1);
            }
            return obj;
        }
        int count;

        class Balloon : Sphere
        {
            public Balloon(double z)
            {
                DeltaZ = z * 0.01;
            }
            public double DeltaZ;
        }

        void TimerTick(object sender, EventArgs e)
        {
            foreach (var item in scene.Models)
            {
                Balloon balloon = item as Balloon;
                if (balloon != null)
                {
                    Point3D pos = balloon.Position;
                    if ((pos.Z > 10 && balloon.DeltaZ > 0)
                        || (pos.Z < balloon.ScaleZ && balloon.DeltaZ < 0))
                        balloon.DeltaZ *= -1;
                    pos.Z += balloon.DeltaZ;
                    balloon.Position = pos;
                }
                else
                {
                    if (item is Cylinder || item is Tube)
                    {
                        Object3D obj = item as Object3D;
                        Quaternion q = obj.Rotation1;
                        obj.Rotation1 = new Quaternion(Math3D.UnitZ, q.Angle + 0.3);
                    }
                }
            }

            string msg = checker.GetResult();
            DateTime t1 = DateTime.Now;
            if ((t1 - t0).TotalSeconds > 1)
            {
                t0 = t1;
                Title = string.Format("WFTools3D Demo ({0})", msg);
            }
        }
        DateTime t0;
        PerformanceChecker checker = new PerformanceChecker();

        Brush GetRandomBrush()
        {
            byte[] b = new byte[4];
            randy.NextBytes(b);
            Color c1 = Color.FromRgb(b[0], b[1], b[2]);
            randy.NextBytes(b);
            Color c2 = Color.FromRgb(b[0], b[1], b[2]);
            Brush brush = new LinearGradientBrush(c1, c2, 90);
            brush.Freeze();
            return brush;
        }

        void InitializeCameras()
        {
            scene.ActivateCamera(2);
            scene.Camera.Position = new Point3D(80, 0, 15);
            scene.Camera.LookAtOrigin();

            scene.ActivateCamera(1);
            scene.Camera.Position = new Point3D(-8, 12, 2);
            scene.Camera.LookDirection = -Math3D.UnitY;
            scene.Camera.UpDirection = Math3D.UnitZ;
            scene.Camera.ChangeRoll(15);
            scene.Camera.Speed = 15;

            scene.ActivateCamera(0);
            scene.Camera.Position = new Point3D(-67, 19, 1);
            scene.Camera.LookDirection = new Vector3D(1, -0.24, 0);
            scene.Camera.UpDirection = Math3D.UnitZ;
            scene.Camera.FieldOfView = 60;
            scene.Camera.Speed = 15;
        }

        void InitializeInfo()
        {
            AddInfo("1, 2, 3:", "Activate camera 1, 2 or 3");
            AddInfo("W, S:", "Increase/decrease speed");
            AddInfo("X:", "Set speed to 0");
            AddInfo("T:", "Turn backwards");
            AddInfo("Space:", "Turn to origin");
            AddInfo("Wheel:", "Increase/decrease field of view");
            AddInfo("H:", "Toggle airplanes and ADI");
            AddInfo();
            AddInfo("SPEED <> 0", "――――――――――――――");
            AddInfo("LMB,");
            AddInfo("Arrows:", "Change pitch and roll angles");
            AddInfo("Ctrl+LMB", "Change look direction");
            AddInfo("A, D:", "Fly standard turn left/right");
            AddInfo("F:", "Fly parallel to the ground");
            AddInfo();
            AddInfo("SPEED == 0", "――――――――――――――");
            AddInfo("LMB:", "Rotate scene about origin");
            AddInfo("Ctrl+LMB:", "Rotate scene about touchpoint");
            AddInfo("Arrows:", "Change look direction");
            AddInfo("PgUp, PgDn:", "Change roll angle");
            AddInfo("Ctrl+Arrows:", "Move camera left/right");
            AddInfo("", "and forward/backward");
            AddInfo("Ctrl+PgUp:", "Move camera up");
            AddInfo("Ctrl+PgDn:", "Move camera down");
            AddInfo("Shift:", "Increase all above motion steps");
        }

        void AddInfo(string s1 = null, string s2 = null)
        {
            int row = info.RowDefinitions.Count;
            info.RowDefinitions.Add(new RowDefinition());
            AddInfo(row, 0, s1);
            if (s2 != null)
                AddInfo(row, 1, s2);
        }

        void AddInfo(int row, int col, string text)
        {
            TextBlock tb = new TextBlock { Text = text, Padding = new Thickness(4, 0, 4, 0) };
            Grid.SetRow(tb, row);
            Grid.SetColumn(tb, col);
            info.Children.Add(tb);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape)
                Close();
        }
    }
}

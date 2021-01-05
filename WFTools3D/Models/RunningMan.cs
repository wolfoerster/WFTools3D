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

#if WFToolsAvailable
using WFTools;
#endif

namespace WFTools3D
{
    public class RunningMan : Object3D
    {
        public RunningMan(double maxx, double maxy)
        {
            maxX = maxx;
            maxY = maxy;
            InitializeBody();
            RunningDir = new Vector3D(1, 0.2, 0);
        }
        double maxX, maxY;

        /// <summary>
        /// 
        /// </summary>
        Vector3D RunningDir
        {
            set
            {
                runningDir = value;
                runningDir.Normalize();
                double angle = Math.Atan2(runningDir.Y, runningDir.X);
                body.Rotation2 = Math3D.RotationZ(angle, false);
            }
        }
        Vector3D runningDir;

        /// <summary>
        /// 
        /// </summary>
        void InitializeBody()
        {
            body.Scale = 0.1;
            body.Position = new Point3D(0, 0, 0.21);
            body.Rotation1 = Math3D.RotationY(12);
            Children.Add(body);

            double tlength = 1.2;
            Brush brush = Brushes.DarkGreen;

            torso.Radius = 0.2;
            torso.DiffuseMaterial.Brush = brush;
            torso.From = new Point3D(0, 0, -0.1);
            torso.To = new Point3D(0, 0, tlength + 0.1);
            body.Children.Add(torso);

            head.Radius = 0.25;
            head.ScaleZ = 0.35;
            head.DiffuseMaterial.Brush = Brushes.RosyBrown;
            head.Position = new Point3D(0, 0, tlength + 0.4);
            body.Children.Add(head);

            leftArm = new DoubleBone(0.06, brush);
            leftArm.Position = new Point3D(0, 0.35, tlength);
            leftArm.Angle = -60;
            body.Children.Add(leftArm);

            rightArm = new DoubleBone(0.06, brush);
            rightArm.Position = new Point3D(0, -0.35, tlength);
            rightArm.Angle = -60;
            body.Children.Add(rightArm);

            brush = Brushes.Navy;
            leftLeg = new DoubleBone(0.08, brush);
            leftLeg.Position = new Point3D(0, 0.18, 0);
            body.Children.Add(leftLeg);

            rightLeg = new DoubleBone(0.08, brush);
            rightLeg.Position = new Point3D(0, -0.18, 0);
            body.Children.Add(rightLeg);
        }
        Object3D body = new Object3D();
        Sphere head = new Sphere();
        Cylinder torso = new Cylinder { IsClosed = true };
        DoubleBone leftArm, rightArm, leftLeg, rightLeg;

        /// <summary>
        /// 
        /// </summary>
        public void TimerTick(object sender, EventArgs e)
        {
            phi += dphi;
            if (phi < 0) dphi *= -1;
            if (phi > maxphi) dphi *= -1;

            UpdateBones(leftLeg, leftArm, phi);
            UpdateBones(rightLeg, rightArm, maxphi - phi);

            if (Position.X > maxX || Position.X < -maxX)
            {
                runningDir.X *= -1;
                RunningDir = runningDir;
            }

            if (Position.Y > maxY || Position.Y < -maxY)
            {
                runningDir.Y *= -1;
                RunningDir = runningDir;
            }

            Position += runningDir * 0.01;
        }
        double phi, dphi = 4, maxphi = 50, phase = 20;

        private void UpdateBones(DoubleBone leg, DoubleBone arm, double phi)
        {
            leg.Angle = phi;
            double angle = phi - maxphi * 0.5;
            leg.Rotation1 = Math3D.RotationY(angle);
            arm.Rotation1 = Math3D.RotationY(phase - angle);
        }

        /// <summary>
        /// 
        /// </summary>
        class DoubleBone : Object3D
        {
            public DoubleBone(double radius, Brush brush)
            {
                UpperBone = CreateBone(radius, 0, -1, brush);
                LowerBone = CreateBone(radius, -1, -2, brush);
            }
            public Bone UpperBone, LowerBone;

            Bone CreateBone(double radius, double z1, double z2, Brush brush)
            {
                Bone bone = new Bone(radius, 1.5, brush, brush, brush);
                bone.SetLocation(new Point3D(0, 0, z1), new Point3D(0, 0, z2));
                Children.Add(bone);
                return bone;
            }

            public double Angle
            {
                get { return angle; }
                set
                {
                    angle = value;
                    double cos = Math.Cos(MathUtils.ToRadians(angle));
                    double sin = Math.Sin(MathUtils.ToRadians(angle));
                    LowerBone.SetLocation(new Point3D(0, 0, -1), new Point3D(-sin, 0, -1 - cos));
                }
            }
            double angle;
        }
    }
}

//******************************************************************************************
// Copyright © 2017 - 2024 Wolfgang Foerster (wolfoerster@gmx.de)
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

namespace WFTools3DDemo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using WFTools3D;

    public class FlightPath : Primitive3D
    {
        private List<Point3D> points;
        private List<double> textureX;
        private double maxZ;

        public FlightPath()
        {
            var brush = new LinearGradientBrush();
            brush.GradientStops.Add(new GradientStop(Colors.Green, 0.0));
            brush.GradientStops.Add(new GradientStop(Colors.Yellow, 0.5));
            brush.GradientStops.Add(new GradientStop(Colors.Red, 1.0));
            brush.EndPoint = new Point(0, 1);

            DiffuseMaterial.Brush = brush;
            BackMaterial = Material;
        }

        public void Init(List<Point3D> points)
        {
            if (points.Count > 1)
            {
                this.points = points;
                CalcTexture();
                InitMesh();
            }
        }

        private void CalcTexture()
        {
            maxZ = points[0].Z;
            var length = new List<double> { 0 };

            for (int i = 1; i < points.Count; i++)
            {
                maxZ = Math.Max(maxZ, points[i].Z);

                var v = points[i] - points[i - 1];
                v.Z = 0; // project to xy-plane

                length.Add(length[i - 1] + v.Length);
            }

            var totalLength = length[length.Count - 1];
            textureX = length.Select(x => x / totalLength).ToList();
        }

        protected override MeshGeometry3D CreateMesh()
        {
            if (points == null || points.Count < 2)
                return null;

            MeshGeometry3D mesh = new MeshGeometry3D();

            for (int i = 1; i < points.Count; i++)
            {
                var p0 = points[i - 1];
                var p1 = points[i];
                var p2 = ProjectXY(p1);
                var p3 = ProjectXY(p0);

                var u = p0 - p3;
                var v = p2 - p3;
                var n = u.Cross(v);
                n.Normalize();

                Add(mesh, p0, n, i - 1);
                Add(mesh, p1, n, i);
                Add(mesh, p2, n, i);
                Add(mesh, p3, n, i - 1);

                var j = mesh.Positions.Count - 4;
                MeshUtils.AddTriangleIndices(mesh, j, j + 1, j + 2);
                MeshUtils.AddTriangleIndices(mesh, j + 2, j + 3, j);
            }

            return mesh;
        }

        private void Add(MeshGeometry3D mesh, Point3D point, Vector3D normal, int pointIndex)
        {
            mesh.Positions.Add(point);
            mesh.Normals.Add(normal);
            mesh.TextureCoordinates.Add(new Point(textureX[pointIndex], point.Z / maxZ));
        }

        private static Point3D ProjectXY(Point3D pt) => new Point3D(pt.X, pt.Y, 0);
    }
}

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
            DiffuseMaterial.Brush = new LinearGradientBrush(Colors.Green, Colors.Blue, 90);
            BackMaterial = Material;
        }

        public void Init(IEnumerable<Point3D> points)
        {
            this.points = points.ToList();
            CalcTexture();
            InitMesh();
        }

        private void CalcTexture()
        {
            if (points == null || points.Count < 2)
                return;

            maxZ = double.NegativeInfinity;
            textureX = new List<double>();

            for (int i = 0; i < points.Count; i++)
            {
                maxZ = Math.Max(maxZ, points[i].Z);
                if (i == 0)
                {
                    textureX.Add(0);
                }
                else
                {
                    var v = points[i] - points[i - 1];
                    v.Z = 0;
                    textureX.Add(textureX[i - 1] + v.Length);
                }
            }

            for (int i = 0; i < textureX.Count; i++)
                textureX[i] /= textureX[textureX.Count - 1];
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

                mesh.Positions.Add(p0);
                mesh.Normals.Add(n);
                mesh.TextureCoordinates.Add(GetTexCoordinate(p0, i - 1));

                mesh.Positions.Add(p1);
                mesh.Normals.Add(n);
                mesh.TextureCoordinates.Add(GetTexCoordinate(p1, i));

                mesh.Positions.Add(p2);
                mesh.Normals.Add(n);
                mesh.TextureCoordinates.Add(GetTexCoordinate(p2, i));

                var j = mesh.Positions.Count - 3;
                MeshUtils.AddTriangleIndices(mesh, j, j + 1, j + 2);

                mesh.Positions.Add(p3);
                mesh.Normals.Add(n);
                mesh.TextureCoordinates.Add(GetTexCoordinate(p3, i - 1));
 
                MeshUtils.AddTriangleIndices(mesh, j + 2, j + 3, j);
            }

            return mesh;
        }

        private Point GetTexCoordinate(Point3D pt, int index)
        {
            return new Point(textureX[index], pt.Z / maxZ);
        }

        private static Point3D ProjectXY(Point3D pt) => new Point3D(pt.X, pt.Y, 0);
    }
}

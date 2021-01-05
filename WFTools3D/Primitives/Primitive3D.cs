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
using System.Collections.Generic;

#if WFToolsAvailable
using WFTools;
#endif

namespace WFTools3D
{
    /// <summary>
    /// An Object3D with a mesh and material.
    /// </summary>
    public abstract class Primitive3D : Object3D
    {
        /// <summary>
        /// Create the MeshGeometry3D of this Primitive3D.
        /// </summary>
        protected abstract MeshGeometry3D CreateMesh();

        /// <summary>
        /// The number of divisions is used when creating the mesh. A square for example can be created<para/>
        /// by two rectangles only (Divisions = 1) or both sides can be divided into smaller parts<para/>
        /// leading to a larger number of triangles (Divisions > 1).
        /// </summary>
        public int Divisions
        {
            get { return divisions; }
            set
            {
                if (divisions != value && value > 0 && value < 1000)
                {
                    divisions = value;
                    InitMesh();
                }
            }
        }
        protected int divisions = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="Primitive3D"/> class.
        /// </summary>
        public Primitive3D()
        {
            Initialize();
        }

        public Primitive3D(int divisions)
        {
            this.divisions = divisions;
            Initialize();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected virtual void Initialize()
        {
            InitMesh();
            InitMaterial();
            Content = Model;
        }

        /// <summary>
        /// Initializes the mesh.
        /// </summary>
        protected void InitMesh()
        {
            MeshGeometry3D mesh = CreateMesh();
            if (mesh != null)
            {
                mesh.Freeze();
                mesh.Positions.Freeze();
                mesh.TriangleIndices.Freeze();
                mesh.TextureCoordinates.Freeze();
            }
            Mesh = mesh;
        }

        /// <summary>
        /// Initializes the material.
        /// </summary>
        protected void InitMaterial()
        {
            Material = new MaterialGroup();
            Material.Children.Add(diffuseMaterial);
            Material.Children.Add(specularMaterial);
            Material.Children.Add(emissiveMaterial);
        }

        /// <summary>
        /// Gets or sets the 3D model.
        /// </summary>
        public GeometryModel3D Model
        {
            get { return model; }
            set { model = value; }
        }
        private GeometryModel3D model = new GeometryModel3D();

        /// <summary>
        /// Gets or sets the material.
        /// </summary>
        public MaterialGroup Material
        {
            get { return model.Material as MaterialGroup; }
            set { model.Material = value; }
        }

        /// <summary>
        /// Gets or sets the back material.
        /// </summary>
        public MaterialGroup BackMaterial
        {
            get { return model.BackMaterial as MaterialGroup; }
            set { model.BackMaterial = value; }
        }

        /// <summary>
        /// Gets or sets the mesh.
        /// </summary>
        public MeshGeometry3D Mesh
        {
            get { return model.Geometry as MeshGeometry3D; }
            set { model.Geometry = value; }
        }

        /// <summary>
        /// Gets or sets the diffuse material.
        /// </summary>
        public DiffuseMaterial DiffuseMaterial
        {
            get { return diffuseMaterial; }
            set { diffuseMaterial = value; }
        }
        private DiffuseMaterial diffuseMaterial = new DiffuseMaterial(Brushes.SteelBlue);

        /// <summary>
        /// Gets or sets the specular material.
        /// </summary>
        public SpecularMaterial SpecularMaterial
        {
            get { return specularMaterial; }
            set { specularMaterial = value; }
        }
        private SpecularMaterial specularMaterial = new SpecularMaterial(Brushes.Gray, 100);

        /// <summary>
        /// Gets or sets the emissive material.
        /// </summary>
        public EmissiveMaterial EmissiveMaterial
        {
            get { return emissiveMaterial; }
            set { emissiveMaterial = value; }
        }
        private EmissiveMaterial emissiveMaterial = new EmissiveMaterial(Brushes.Transparent);

        /// <summary>
        /// Transforms a point from 3D model space to 2D viewport space.
        /// </summary>
        public Point ModelToPlot(Point3D point)
        {
            bool success;
            Viewport3DVisual vp;

            Matrix3D modelToViewport =
                Math3D.TryTransformTo2DAncestor(this, out vp, out success);

            if (!success)
                return new Point(double.NaN, double.NaN);

            point = modelToViewport.Transform(point);
            return new Point(point.X, point.Y);
        }

        /// <summary>
        /// Transforms a point from 2D viewport space to 3D model space.
        /// </summary>
        /// <param name="point">The point in viewport space.</param>
        /// <param name="zValue">The z coordinate of the point in model space.</param>
        /// <returns>The point in model space.</returns>
        public Point3D PlotToModel(Point point, double zValue)
        {
            Point3D ptNear, ptFar;
            if (!Math3D.GetRay(point, this, out ptNear, out ptFar))
                return new Point3D(double.NaN, double.NaN, double.NaN);

            return GetRayValue(ptNear, ptFar, zValue);
        }

        /// <summary>
        /// Gets the ray value.
        /// </summary>
        public Point3D GetRayValue(Point3D ptNear, Point3D ptFar, double zValue)
        {
            LinearTransform t = new LinearTransform();
            t.Init(ptNear.Z, ptFar.Z, 0, 1);
            double f = t.Transform(zValue);

            t.Init(0, 1, ptNear.X, ptFar.X);
            double x = t.Transform(f);

            t.Init(0, 1, ptNear.Y, ptFar.Y);
            double y = t.Transform(f);

            return new Point3D(x, y, zValue);
        }

        /// <summary>
        /// Gets the nearest 3D point of the model underneath the specified 2D point.
        /// </summary>
        /// <param name="pt">The 2D point in viewport space.</param>
        /// <returns>The 3D point in model space.</returns>
        public Point3D GetHitPoint(Point pt)
        {
            RayMeshGeometry3DHitTestResult htr = Math3D.HitTest(this, pt);

            if (htr == null)
                return new Point3D(double.NaN, 0, 0);

            return htr.PointHit;
        }

        /// <summary>
        /// Returns the 2D bounding box.
        /// </summary>
        public Rect GetBoundingBox()
        {
            if (Mesh == null || Mesh.Positions == null || Mesh.Positions.Count == 0)
                return Rect.Empty;

            bool success;
            Viewport3DVisual vp;
            Matrix3D modelToViewport = Math3D.TryTransformTo2DAncestor(this, out vp, out success);
            if (!success)
                return Rect.Empty;

            Rect bounds = Rect.Empty;
            foreach (var position in Mesh.Positions)
            {
                Point3D pt = modelToViewport.Transform(position);
                Point point = new Point(pt.X, pt.Y);
                bounds.Union(point);
            }
            return bounds;
        }
    }
}

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
    public class Object3D : ModelVisual3D
    {
        /// <summary>
        /// Gets or sets the scaling in X.
        /// </summary>
        public virtual double ScaleX
        {
            get { return (double)GetValue(ScaleXProperty); }
            set { SetValue(ScaleXProperty, value); }
        }

        /// <summary>
        /// Gets or sets the scaling in Y.
        /// </summary>
        public virtual double ScaleY
        {
            get { return (double)GetValue(ScaleYProperty); }
            set { SetValue(ScaleYProperty, value); }
        }

        /// <summary>
        /// Gets or sets the scaling in Z.
        /// </summary>
        public virtual double ScaleZ
        {
            get { return (double)GetValue(ScaleZProperty); }
            set { SetValue(ScaleZProperty, value); }
        }

        /// <summary>
        /// Sets the overall scaling.
        /// </summary>
        public virtual double Scale
        {
            set { ScaleX = ScaleY = ScaleZ = value; }
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public virtual Point3D Position
        {
            get { return (Point3D)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the first rotation (about the object's origin).
        /// </summary>
        public virtual Quaternion Rotation1
        {
            get { return (Quaternion)GetValue(Rotation1Property); }
            set { SetValue(Rotation1Property, value); }
        }

        /// <summary>
        /// Gets or sets the second rotation (about the object's origin).
        /// </summary>
        public virtual Quaternion Rotation2
        {
            get { return (Quaternion)GetValue(Rotation2Property); }
            set { SetValue(Rotation2Property, value); }
        }

        /// <summary>
        /// Gets or sets the third rotation (about the global origin).
        /// </summary>
        public virtual Quaternion Rotation3
        {
            get { return (Quaternion)GetValue(Rotation3Property); }
            set { SetValue(Rotation3Property, value); }
        }

        #region DependencyProperties

        public static DependencyProperty ScaleXProperty =
            DependencyProperty.Register("ScaleX", typeof(double),
            typeof(Object3D), new UIPropertyMetadata(1.0,
            new PropertyChangedCallback(OnNewTransform)));

        public static DependencyProperty ScaleYProperty =
            DependencyProperty.Register("ScaleY", typeof(double),
            typeof(Object3D), new UIPropertyMetadata(1.0,
            new PropertyChangedCallback(OnNewTransform)));

        public static DependencyProperty ScaleZProperty =
            DependencyProperty.Register("ScaleZ", typeof(double),
            typeof(Object3D), new UIPropertyMetadata(1.0,
            new PropertyChangedCallback(OnNewTransform)));

        public static DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(Point3D),
            typeof(Object3D), new UIPropertyMetadata(Math3D.Origin,
            new PropertyChangedCallback(OnNewTransform)));

        public static readonly DependencyProperty Rotation1Property =
            DependencyProperty.Register("Rotation1", typeof(Quaternion),
            typeof(Object3D), new UIPropertyMetadata(Quaternion.Identity,
            new PropertyChangedCallback(OnNewTransform)));

        public static readonly DependencyProperty Rotation2Property =
            DependencyProperty.Register("Rotation2", typeof(Quaternion),
            typeof(Object3D), new UIPropertyMetadata(Quaternion.Identity,
            new PropertyChangedCallback(OnNewTransform)));

        public static readonly DependencyProperty Rotation3Property =
            DependencyProperty.Register("Rotation3", typeof(Quaternion),
            typeof(Object3D), new UIPropertyMetadata(Quaternion.Identity,
            new PropertyChangedCallback(OnNewTransform)));

        internal static void OnNewTransform(Object sender, DependencyPropertyChangedEventArgs e)
        {
            Object3D obj = sender as Object3D;
            obj.NewTransform();
        }

        #endregion DependencyProperties

        protected virtual void NewTransform()
        {
            if (lockCount != 0)
                return;

            Matrix3D m = new Matrix3D();
            m.Scale(new Vector3D(ScaleX, ScaleY, ScaleZ));
            m.Rotate(Rotation1);
            m.Rotate(Rotation2);
            m.Translate(new Vector3D(Position.X, Position.Y, Position.Z));
            m.Rotate(Rotation3);
            Transform = new MatrixTransform3D(m);
        }

        public void LockUpdates(bool mode)
        {
            lockCount += mode ? 1 : -1;
            if (lockCount == 0)
                NewTransform();
        }
        int lockCount;

        /// <summary>
        /// Translates a point relative to this object to coordinates of the global coordinate system 
        /// or to coordinates that are relative to the specified parent object. 
        /// </summary>
        public Point3D TranslatePoint(Point3D pt, DependencyObject relativeTo = null)
        {
            Matrix3D m = Math3D.GetTransformationMatrix(this, relativeTo);
            return m.Transform(pt);
        }

        /// <summary>
        /// 
        /// </summary>
        static public Object3D CreateTriangleObject(Primitive3D primitive)
        {
            return CreateTriangleObject(primitive, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        static public Object3D CreateTriangleObject(Primitive3D primitive, int coloringMode)
        {
            Object3D container = new Object3D();
            Color[,] colors = { { Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow },
                              { Colors.Green, Colors.Cyan, Colors.Magenta, Colors.Orange } };

            Point3DCollection positions = primitive.Mesh.Positions;
            Int32Collection indices = primitive.Mesh.TriangleIndices;
            Random randy = new Random();

            for (int i = 0; i < indices.Count; i += 3)
            {
                Triangle triangle = new Triangle();
                container.Children.Add(triangle);
                triangle.P1 = positions[indices[i]];
                triangle.P2 = positions[indices[i + 1]];
                triangle.P3 = positions[indices[i + 2]];

                Color color = coloringMode == 0 ? GetColor(randy) : colors[coloringMode - 1, i % 4];
                triangle.DiffuseMaterial.Brush = new SolidColorBrush(color);
            }

            container.ScaleX = primitive.ScaleX;
            container.ScaleY = primitive.ScaleY;
            container.ScaleZ = primitive.ScaleZ;
            container.Position = primitive.Position;
            container.Rotation1 = primitive.Rotation1;
            container.Rotation2 = primitive.Rotation2;

            return container;
        }

        static Color GetColor(Random randy)
        {
            byte[] bytes = new byte[3];
            randy.NextBytes(bytes);
            return Color.FromRgb(bytes[0], bytes[1], bytes[2]);
        }
    }
}

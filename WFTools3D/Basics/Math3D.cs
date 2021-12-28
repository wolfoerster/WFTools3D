//******************************************************************************************
// Copyright © 2016 Wolfgang Foerster (wolfoerster@gmx.de)
//
// This file is part of the WFTools3D project which can be found on github.com
// Parts of this file is part of the 3D Tools for WPF project which can be found on codeplex.com
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
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Collections.Generic;

#if WFToolsAvailable
using WFTools;
#endif

namespace WFTools3D
{
    /// <summary>
    /// Static methods for 3D calculations.
    /// </summary>
    public static class Math3D
    {
        public static double Distance(this Point3D pt)
        {
            return Math.Sqrt(pt.X * pt.X + pt.Y * pt.Y + pt.Z * pt.Z);
        }

        public static double DistanceSquared(this Point3D pt)
        {
            return pt.X * pt.X + pt.Y * pt.Y + pt.Z * pt.Z;
        }

        public static Point3D Add(this Point3D pt, Point3D add)
        {
            return new Point3D(pt.X + add.X, pt.Y + add.Y, pt.Z + add.Z);
        }

        public static Point3D Subtract(this Point3D pt, Point3D add)
        {
            return new Point3D(pt.X - add.X, pt.Y - add.Y, pt.Z - add.Z);
        }

        public static Point3D Inverse(this Point3D pt)
        {
            return new Point3D(-pt.X, -pt.Y, -pt.Z);
        }

        public static bool IsValid(this Point3D pt)
        {
            if (!MathUtils.IsValidNumber(pt.X) || !MathUtils.IsValidNumber(pt.Y) || !MathUtils.IsValidNumber(pt.Z))
                return false;

            return true;
        }

        public static bool IsValid(this Vector3D dir)
        {
            if (!MathUtils.IsValidNumber(dir.X) || !MathUtils.IsValidNumber(dir.Y) || !MathUtils.IsValidNumber(dir.Z))
                return false;

            if (dir.LengthSquared < 1e-12)
                return false;

            return true;
        }

        /// <summary>
        /// Rotates a vector using a quaternion.
        /// </summary>
        public static Vector3D Transform(this Quaternion q, Vector3D v)
        {
            double x2 = q.X + q.X;
            double y2 = q.Y + q.Y;
            double z2 = q.Z + q.Z;
            double wx2 = q.W * x2;
            double wy2 = q.W * y2;
            double wz2 = q.W * z2;
            double xx2 = q.X * x2;
            double xy2 = q.X * y2;
            double xz2 = q.X * z2;
            double yy2 = q.Y * y2;
            double yz2 = q.Y * z2;
            double zz2 = q.Z * z2;
            double x = v.X * (1.0 - yy2 - zz2) + v.Y * (xy2 - wz2) + v.Z * (xz2 + wy2);
            double y = v.X * (xy2 + wz2) + v.Y * (1.0 - xx2 - zz2) + v.Z * (yz2 - wx2);
            double z = v.X * (xz2 - wy2) + v.Y * (yz2 + wx2) + v.Z * (1.0 - xx2 - yy2);
            return new Vector3D(x, y, z);
        }

        /// <summary>
        /// Rotates a point using a quaternion.
        /// </summary>
        public static Point3D Transform(this Quaternion q, Point3D p)
        {
            return (Point3D)q.Transform((Vector3D)p);
        }

        /// <summary>
        /// Rotates a vector about an axis.
        /// </summary>
        public static Vector3D Rotate(this Vector3D v, Vector3D rotationAxis, double angleInDegrees)
        {
            Quaternion q = new Quaternion(rotationAxis, angleInDegrees);
            return q.Transform(v);
        }

        /// <summary>
        /// Calculates the cross product of two vectors.
        /// </summary>
        public static Vector3D Cross(this Vector3D v, Vector3D vector)
        {
            return Vector3D.CrossProduct(v, vector);
        }

        /// <summary>
        /// Calculates the dot product of two vectors.
        /// </summary>
        public static double Dot(this Vector3D v, Vector3D vector)
        {
            return Vector3D.DotProduct(v, vector);
        }

        /// <summary>
        /// Calculates the angle between two vectors in degrees.
        /// </summary>
        public static double AngleTo(this Vector3D v, Vector3D vector)
        {
            return Vector3D.AngleBetween(v, vector);
        }

        /// <summary>
        /// Gets the unit direction vector from this point to a target point.
        /// </summary>
        public static Vector3D DirectionTo(this Point3D thisPoint, Point3D targetPoint)
        {
            Vector3D v = targetPoint - thisPoint;
            v.Normalize();
            return v;
        }

        /// <summary>
        /// Gets the aspect ratio.
        /// </summary>
        public static double GetAspectRatio(Size size)
        {
            return size.Width / size.Height;
        }

        private static Matrix3D GetViewMatrix(ProjectionCamera camera)
        {
            Debug.Assert(camera != null,
                "Caller needs to ensure camera is non-null.");

            // This math is identical to what you find documented for
            // D3DXMatrixLookAtRH with the exception that WPF uses a
            // LookDirection vector rather than a LookAt point.

            Vector3D zAxis = -camera.LookDirection;
            zAxis.Normalize();

            Vector3D xAxis = camera.UpDirection.Cross(zAxis);
            xAxis.Normalize();

            Vector3D yAxis = zAxis.Cross(xAxis);

            Vector3D position = (Vector3D)camera.Position;
            double offsetX = -xAxis.Dot(position);
            double offsetY = -yAxis.Dot(position);
            double offsetZ = -zAxis.Dot(position);

            return new Matrix3D(
                xAxis.X, yAxis.X, zAxis.X, 0,
                xAxis.Y, yAxis.Y, zAxis.Y, 0,
                xAxis.Z, yAxis.Z, zAxis.Z, 0,
                offsetX, offsetY, offsetZ, 1);
        }

        /// <summary>
        /// Computes the effective view matrix for the given camera.
        /// </summary>
        public static Matrix3D GetViewMatrix(Camera camera)
        {
            if (camera == null)
            {
                throw new ArgumentNullException("camera");
            }

            ProjectionCamera projectionCamera = camera as ProjectionCamera;

            if (projectionCamera != null)
            {
                return GetViewMatrix(projectionCamera);
            }

            MatrixCamera matrixCamera = camera as MatrixCamera;

            if (matrixCamera != null)
            {
                return matrixCamera.ViewMatrix;
            }

            throw new ArgumentException(String.Format("Unsupported camera type '{0}'.", camera.GetType().FullName), "camera");
        }

        private static Matrix3D GetProjectionMatrix(OrthographicCamera camera, double aspectRatio)
        {
            Debug.Assert(camera != null,
                "Caller needs to ensure camera is non-null.");

            // This math is identical to what you find documented for
            // D3DXMatrixOrthoRH with the exception that in WPF only
            // the camera's width is specified.  Height is calculated
            // from width and the aspect ratio.

            double w = camera.Width;
            double h = w / aspectRatio;
            double zn = camera.NearPlaneDistance;
            double zf = camera.FarPlaneDistance;

            double m33 = 1 / (zn - zf);
            double m43 = zn * m33;

            return new Matrix3D(
                    2 / w, 0, 0, 0,
                    0, 2 / h, 0, 0,
                    0, 0, m33, 0,
                    0, 0, m43, 1);
        }

        private static Matrix3D GetProjectionMatrix(PerspectiveCamera camera, double aspectRatio)
        {
            Debug.Assert(camera != null,
                "Caller needs to ensure camera is non-null.");

            // This math is identical to what you find documented for
            // D3DXMatrixPerspectiveFovRH with the exception that in
            // WPF the camera's horizontal rather the vertical
            // field-of-view is specified.

            double hFoV = MathUtils.ToRadians(camera.FieldOfView);
            double zn = camera.NearPlaneDistance;
            double zf = camera.FarPlaneDistance;

            double xScale = 1 / Math.Tan(hFoV / 2);
            double yScale = aspectRatio * xScale;
            double m33 = (zf == double.PositiveInfinity) ? -1 : (zf / (zn - zf));
            double m43 = zn * m33;

            return new Matrix3D(
                    xScale, 0, 0, 0,
                    0, yScale, 0, 0,
                    0, 0, m33, -1,
                    0, 0, m43, 0);
        }

        /// <summary>
        /// Computes the effective projection matrix for the given camera.
        /// </summary>
        public static Matrix3D GetProjectionMatrix(Camera camera, double aspectRatio)
        {
            if (camera == null)
            {
                throw new ArgumentNullException("camera");
            }

            PerspectiveCamera perspectiveCamera = camera as PerspectiveCamera;

            if (perspectiveCamera != null)
            {
                return GetProjectionMatrix(perspectiveCamera, aspectRatio);
            }

            OrthographicCamera orthographicCamera = camera as OrthographicCamera;

            if (orthographicCamera != null)
            {
                return GetProjectionMatrix(orthographicCamera, aspectRatio);
            }

            MatrixCamera matrixCamera = camera as MatrixCamera;

            if (matrixCamera != null)
            {
                return matrixCamera.ProjectionMatrix;
            }

            throw new ArgumentException(String.Format("Unsupported camera type '{0}'.", camera.GetType().FullName), "camera");
        }

        private static Matrix3D GetHomogeneousToViewportTransform(Rect viewport)
        {
            double scaleX = viewport.Width / 2;
            double scaleY = viewport.Height / 2;
            double offsetX = viewport.X + scaleX;
            double offsetY = viewport.Y + scaleY;

            return new Matrix3D(
                 scaleX, 0, 0, 0,
                      0, -scaleY, 0, 0,
                      0, 0, 1, 0,
                offsetX, offsetY, 0, 1);
        }

        /// <summary>
        ///     Computes the transform from world space to the Viewport3DVisual's
        ///     inner 2D space.
        /// 
        ///     This method can fail if Camera.Transform is non-invertable
        ///     in which case the camera clip planes will be coincident and
        ///     nothing will render.  In this case success will be false.
        /// </summary>
        public static Matrix3D TryWorldToViewportTransform(Viewport3DVisual visual, out bool success)
        {
            success = false;
            if (visual.Viewport.Width == 0 || visual.Viewport.Height == 0)
                return ZeroMatrix;

            Matrix3D result = TryWorldToCameraTransform(visual, out success);

            if (success)
            {
                result.Append(GetProjectionMatrix(visual.Camera, GetAspectRatio(visual.Viewport.Size)));
                result.Append(GetHomogeneousToViewportTransform(visual.Viewport));
            }

            return result;
        }

        /// <summary>
        ///     Computes the transform from world space to camera space
        /// 
        ///     This method can fail if Camera.Transform is non-invertable
        ///     in which case the camera clip planes will be coincident and
        ///     nothing will render.  In this case success will be false.
        /// </summary>
        public static Matrix3D TryWorldToCameraTransform(Viewport3DVisual visual, out bool success)
        {
            success = false;

            if (visual == null)
                return ZeroMatrix;

            Matrix3D result = Matrix3D.Identity;

            Camera camera = visual.Camera;

            if (camera == null)
            {
                return ZeroMatrix;
            }

            Rect viewport = visual.Viewport;

            if (viewport == Rect.Empty)
            {
                return ZeroMatrix;
            }

            Transform3D cameraTransform = camera.Transform;

            if (cameraTransform != null)
            {
                Matrix3D m = cameraTransform.Value;

                if (!m.HasInverse)
                {
                    return ZeroMatrix;
                }

                m.Invert();
                result.Append(m);
            }

            result.Append(GetViewMatrix(camera));

            success = true;
            return result;
        }

        /// <summary>
        /// Gets the object space to world space (or a parent object space) transformation for the given 3D visual.
        /// </summary>
        public static Matrix3D GetTransformationMatrix(DependencyObject visual, DependencyObject relativeTo = null)
        {
            Matrix3D matrix = Matrix3D.Identity;

            while (visual is ModelVisual3D)
            {
                Transform3D transform = (Transform3D)visual.GetValue(ModelVisual3D.TransformProperty);
                if (transform != null)
                    matrix.Append(transform.Value);

                visual = VisualTreeHelper.GetParent(visual);
                if (visual == relativeTo)
                    break;
            }

            return matrix;
        }

        /// <summary>
        /// Gets the object space to world space transformation for the given DependencyObject
        /// </summary>
        /// <param name="visual">The visual whose world space transform should be found</param>
        /// <param name="viewport">The Viewport3DVisual the Visual is contained within</param>
        /// <returns>The world space transformation</returns>
        public static Matrix3D GetWorldTransformationMatrix(DependencyObject visual, out Viewport3DVisual viewport)
        {
            Matrix3D worldTransform = Matrix3D.Identity;
            viewport = null;

            if (!(visual is Visual3D))
            {
                throw new ArgumentException("Must be of type Visual3D.", "visual");
            }

            while (visual != null)
            {
                if (!(visual is ModelVisual3D))
                {
                    break;
                }

                Transform3D transform = (Transform3D)visual.GetValue(ModelVisual3D.TransformProperty);

                if (transform != null)
                {
                    worldTransform.Append(transform.Value);
                }

                visual = VisualTreeHelper.GetParent(visual);
            }

            viewport = visual as Viewport3DVisual;

            if (viewport == null)
            {
                if (visual != null)
                {
                    // In WPF 3D v1 the only possible configuration is a chain of
                    // ModelVisual3Ds leading up to a Viewport3DVisual.

                    throw new ApplicationException(
                        String.Format("Unsupported type: '{0}'.  Expected tree of ModelVisual3Ds leading up to a Viewport3DVisual.",
                        visual.GetType().FullName));
                }

                return ZeroMatrix;
            }

            return worldTransform;
        }

        /// <summary>
        /// Computes the transform from the inner space of the given
        /// Visual3D to the 2D space of the Viewport3DVisual which
        /// contains it.
        /// The result will contain the transform of the given visual.
        /// This method can fail if Camera.Transform is non-invertable
        /// in which case the camera clip planes will be coincident and
        /// nothing will render.  In this case success will be false.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <param name="viewport">The viewport.</param>
        public static Matrix3D TryTransformTo2DAncestor(DependencyObject visual, out Viewport3DVisual viewport, out bool success)
        {
            Matrix3D to2D = GetWorldTransformationMatrix(visual, out viewport);
            to2D.Append(TryWorldToViewportTransform(viewport, out success));

            if (!success)
            {
                return ZeroMatrix;
            }

            return to2D;
        }

        /// <summary>
        /// Computes the transform from the inner space of the given
        /// Visual3D to the camera coordinate space
        /// The result will contain the transform of the given visual.
        /// This method can fail if Camera.Transform is non-invertable
        /// in which case the camera clip planes will be coincident and
        /// nothing will render.  In this case success will be false.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <param name="viewport">The viewport.</param>
        public static Matrix3D TryTransformToCameraSpace(DependencyObject visual, out Viewport3DVisual viewport, out bool success)
        {
            Matrix3D toViewSpace = GetWorldTransformationMatrix(visual, out viewport);
            toViewSpace.Append(TryWorldToCameraTransform(viewport, out success));

            if (!success)
            {
                return ZeroMatrix;
            }

            return toViewSpace;
        }

        /// <summary>
        /// Given a ModelVisual3D and a 2D point on the screen 
        /// this function calculates the corresponding 3D ray.
        /// </summary>
        /// <param name="ptPlot">The 2D point on the screen.</param>
        /// <param name="mv3D">The ModelVisual3D.</param>
        /// <param name="ptNear">The 3D point which belongs to the near clipping plane.</param>
        /// <param name="ptFar">The 3D point which belongs to the far clipping plane.</param>
        static public bool GetRay(Point ptPlot, ModelVisual3D mv3D, out Point3D ptNear, out Point3D ptFar)
        {
            bool success;
            Viewport3DVisual vp;
            Matrix3D modelToViewport = TryTransformTo2DAncestor(mv3D, out vp, out success);

            if (!success || !modelToViewport.HasInverse)
            {
                ptNear = ptFar = new Point3D();
                return false;
            }

            Matrix3D viewportToModel = modelToViewport;
            viewportToModel.Invert();

            Point3D ptMouse = new Point3D(ptPlot.X, ptPlot.Y, 0);
            ptNear = viewportToModel.Transform(ptMouse);

            ptMouse.Z = 1;
            ptFar = viewportToModel.Transform(ptMouse);

            return true;
        }

        /// <summary>
        /// Performs a 3D hit test. Point pt is a 2D point in viewport space. 
        /// The object needs to be a Viewport3D or a ModelVisual3D.
        /// </summary>
        static public RayMeshGeometry3DHitTestResult HitTest(object obj, Point pt)
        {
            hitTestResult = null;

            Viewport3D viewport = obj as Viewport3D;
            if (viewport != null)
            {
                VisualTreeHelper.HitTest(viewport, null, HitTestResultCallback, new PointHitTestParameters(pt));
            }
            else
            {
                ModelVisual3D model = obj as ModelVisual3D;
                if (model != null)
                {
                    Point3D ptNear, ptFar;
                    if (GetRay(pt, model, out ptNear, out ptFar))
                    {
                        RayHitTestParameters paras = new RayHitTestParameters(ptNear, ptFar - ptNear);
                        VisualTreeHelper.HitTest(model, null, HitTestResultCallback, paras);
                    }
                }
            }

            return hitTestResult;
        }
        static RayMeshGeometry3DHitTestResult hitTestResult;

        static HitTestResultBehavior HitTestResultCallback(HitTestResult result)
        {
            RayMeshGeometry3DHitTestResult htr = result as RayMeshGeometry3DHitTestResult;
            if (htr != null)
            {
                if (hitTestResult == null)
                    hitTestResult = htr;

                else if (htr.DistanceToRayOrigin < hitTestResult.DistanceToRayOrigin)
                    hitTestResult = htr;
            }
            return HitTestResultBehavior.Continue;
        }

        /// <summary>
        /// Calculates the intersections between a ray and a mesh.
        /// <para>
        /// See also http://jgt.akpeters.com/papers/MollerTrumbore97/ and 
        /// http://www.cs.virginia.edu/~gfx/Courses/2003/ImageSynthesis/papers/Acceleration/Fast%20MinimumStorage%20RayTriangle%20Intersection.pdf
        /// </para>
        /// </summary>
        /// <param name="orig">The origin of the ray.</param>
        /// <param name="dir">The direction of the ray.</param>
        /// <param name="mesh">The mesh.</param>
        /// <param name="frontFacesOnly">if set to <c>true</c> calculates front faces intersections only.</param>
        /// <returns>A list of doubles [t1,...,tN] which can be used to calculate 
        /// the intersection points by equation pI = orig + tI * dir, 1 &lt;= I &lt;= N.</returns>
        [Obsolete]
        static public List<double> RayMeshIntersections(Point3D orig, Vector3D dir, MeshGeometry3D mesh, bool frontFacesOnly)
        {
            List<double> result = new List<double>();

            if (mesh == null || !dir.IsValid())
                return result;

            double epsilon = 1E-9;
            Int32Collection indices = mesh.TriangleIndices;
            Point3DCollection positions = mesh.Positions;

            for (int i = 0; i < indices.Count; i += 3)
            {
                Point3D vert0 = positions[indices[i]];
                Point3D vert1 = positions[indices[i + 1]];
                Point3D vert2 = positions[indices[i + 2]];

                //--- find vectors for two edges sharing vert0
                Vector3D edge1 = vert1 - vert0;
                Vector3D edge2 = vert2 - vert0;

                //--- begin calculating determinant  also used to calculate U parameter
                Vector3D pvec = dir.Cross(edge2);

                //--- if determinant is near zero ray lies in plane of triangle
                double det = edge1.Dot(pvec);
                if (det < epsilon)
                {
                    if (frontFacesOnly)
                        continue;

                    else if (det > -epsilon)
                        continue;
                }
                double inv_det = 1.0 / det;

                //--- calculate distance from vert0 to ray origin
                Vector3D tvec = orig - vert0;

                //--- calculate U parameter and test bounds
                double u = tvec.Dot(pvec) * inv_det;
                if (u < 0.0 || u > 1.0)
                    continue;

                //--- prepare to test V parameter
                Vector3D qvec = tvec.Cross(edge1);

                //--- calculate V parameter and test bounds
                double v = dir.Dot(qvec) * inv_det;
                if (v < 0.0 || u + v > 1.0)
                    continue;

                //--- calculate t scale parameters, ray intersects triangle
                double t = edge2.Dot(qvec) * inv_det;

                result.Add(t);
            }

            return result;
        }

        /// <summary>
        /// Transforms the axis-aligned bounding box 'bounds' by 'transform'.
        /// </summary>
        /// <param name="bounds">The AABB to transform.</param>
        /// <param name="transform">The transform.</param>
        /// <returns>Transformed AABB</returns>
        public static Rect3D TransformBounds(Rect3D bounds, Matrix3D transform)
        {
            double x1 = bounds.X;
            double y1 = bounds.Y;
            double z1 = bounds.Z;
            double x2 = bounds.X + bounds.SizeX;
            double y2 = bounds.Y + bounds.SizeY;
            double z2 = bounds.Z + bounds.SizeZ;

            Point3D[] points = new Point3D[] {
                new Point3D(x1, y1, z1),
                new Point3D(x1, y1, z2),
                new Point3D(x1, y2, z1),
                new Point3D(x1, y2, z2),
                new Point3D(x2, y1, z1),
                new Point3D(x2, y1, z2),
                new Point3D(x2, y2, z1),
                new Point3D(x2, y2, z2),
            };

            transform.Transform(points);

            // reuse the 1 and 2 variables to stand for smallest and largest
            Point3D p = points[0];
            x1 = x2 = p.X;
            y1 = y2 = p.Y;
            z1 = z2 = p.Z;

            for (int i = 1; i < points.Length; i++)
            {
                p = points[i];

                x1 = Math.Min(x1, p.X); y1 = Math.Min(y1, p.Y); z1 = Math.Min(z1, p.Z);
                x2 = Math.Max(x2, p.X); y2 = Math.Max(y2, p.Y); z2 = Math.Max(z2, p.Z);
            }

            return new Rect3D(x1, y1, z1, x2 - x1, y2 - y1, z2 - z1);
        }

        /// <summary>
        ///     Computes the center of 'box'
        /// </summary>
        /// <param name="box">The Rect3D we want the center of</param>
        /// <returns>The center point</returns>
        public static Point3D GetCenter(Rect3D box)
        {
            return new Point3D(box.X + box.SizeX / 2, box.Y + box.SizeY / 2, box.Z + box.SizeZ / 2);
        }

        /// <summary>
        /// A matrix filled with zeros.
        /// </summary>
        public static readonly Matrix3D ZeroMatrix = new Matrix3D(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        /// <summary>
        /// The origin of the coordinate system, i.e. the point (0,0,0).
        /// </summary>
        public static readonly Point3D Origin = new Point3D(0, 0, 0);

        /// <summary>
        /// The x unit vector, i.e. the vector (1,0,0).
        /// </summary>
        public static readonly Vector3D UnitX = new Vector3D(1, 0, 0);

        /// <summary>
        /// The y unit vector, i.e. the vector (0,1,0).
        /// </summary>
        public static readonly Vector3D UnitY = new Vector3D(0, 1, 0);

        /// <summary>
        /// The z unit vector, i.e. the vector (0,0,1).
        /// </summary>
        public static readonly Vector3D UnitZ = new Vector3D(0, 0, 1);

        /// <summary>
        /// Linear interpolation of 3D vectors.
        /// </summary>
        public static Vector3D Lerp(Vector3D from, Vector3D to, double t)
        {
            Vector3D v = new Vector3D();
            v.X = from.X * (1 - t) + to.X * t;
            v.Y = from.Y * (1 - t) + to.Y * t;
            v.Z = from.Z * (1 - t) + to.Z * t;
            return v;
        }

        /// <summary>
        /// Linear interpolation of quaternions.
        /// </summary>
        public static Quaternion Lerp(Quaternion from, Quaternion to, double t)
        {
            double angle = from.Angle * (1 - t) + to.Angle * t;
            Vector3D axis = Lerp(from.Axis, to.Axis, t);
            return new Quaternion(axis, angle);
        }

        /// <summary>
        /// Rotates the specified point about the specified axis by the specified angle.
        /// </summary>
        static public Point3D Rotate(Point3D pt, Point3D ptAxis1, Point3D ptAxis2, double angle, bool isAngleInDegrees = true)
        {
            Vector3D axis = ptAxis2 - ptAxis1;
            Quaternion q = Rotation(axis, angle, isAngleInDegrees);
            return (Point3D)q.Transform(pt - ptAxis1) + (Vector3D)ptAxis1;
        }

        /// <summary>
        /// Calculates a rotation quaternion. Rotation axis does not have to be a unit vector.
        /// </summary>
        public static Quaternion Rotation(Vector3D rotationAxis, double angle, bool isAngleInDegrees = true)
        {
            if (!isAngleInDegrees)
                angle = MathUtils.ToDegrees(angle);

            //--- Angle should be within 0 and 360. Otherwise the following happens: if angle is -1 and axis is (0,0,1) 
            //--- the returned Quaternion will have an axis of (0,0,-1) and an angle of +1, which for sure leads to the 
            //--- same rotation, but is troublesome when reusing the quaternion in animation calculations.
            angle %= 360;
            if (angle < 0) angle += 360;
            if (angle > 360) angle -= 360;

            return new Quaternion(rotationAxis, angle);
        }

        /// <summary>
        /// Gets a quaternion for the rotation about the global x axis.
        /// </summary>
        public static Quaternion RotationX(double angle, bool isAngleInDegrees = true)
        {
            return Rotation(UnitX, angle, isAngleInDegrees);
        }

        /// <summary>
        /// Gets a quaternion for the rotation about the global y axis.
        /// </summary>
        public static Quaternion RotationY(double angle, bool isAngleInDegrees = true)
        {
            return Rotation(UnitY, angle, isAngleInDegrees);
        }

        /// <summary>
        /// Gets a quaternion for the rotation about the global z axis.
        /// </summary>
        public static Quaternion RotationZ(double angle, bool isAngleInDegrees = true)
        {
            return Rotation(UnitZ, angle, isAngleInDegrees);
        }

        /// <summary>
        /// Calculates the look direction and up direction for an observer looking at a target point.
        /// The look direction is simply the normalized difference between target point and observer position.
        /// From the infinite number of possible up directions the one with maximum z coordinate is returned.
        /// If all possible up directions have a z coordinate of 0, i.e. the observer position is right above 
        /// or below the target point, the up direction is set to UnitY if looking down or -UnitY if looking up.
        /// In this case the return value is false, in the first case it is true.
        /// </summary>
        public static bool LookAt(Point3D targetPoint, Point3D observerPosition, out Vector3D lookDirection, out Vector3D upDirection)
        {
            lookDirection = targetPoint - observerPosition;
            lookDirection.Normalize();

            double a = lookDirection.X;
            double b = lookDirection.Y;
            double c = lookDirection.Z;

            //--- Find the one and only up vector (x, y, z) which has a positive z value (1), 
            //--- which is perpendicular to the look vector (2) and and which ensures that 
            //--- the resulting roll angle is 0, i.e. the resulting left vector (= up cross look)
            //--- lies within the xy-plane (or has a z value of 0) (3). In other words: 
            //--- 1. z > 0 (e.g. 1)
            //--- 2. ax + by + cz = 0
            //--- 3. ay - bx = 0
            //--- If the observer position is right above or below the target point, i.e. a = b = 0 and c != 0, 
            //--- we set the up vector to (1, 0, 0) for c > 0 and to (-1, 0, 0) for c < 0.

            double length = (a * a + b * b);
            if (length > 1e-12)
            {
                upDirection = new Vector3D(-c * a / length, -c * b / length, 1);
                upDirection.Normalize();
                return true;
            }
            else
            {
                upDirection = c < 0 ? UnitY : -UnitY;
                return false;
            }
        }
    }

    /// <summary>
    /// A transformation for 3D points.
    /// </summary>
    public class Point3DTransform
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Point3DTransform"/> class.
        /// </summary>
        public Point3DTransform()
        {
            TX = new LinearTransform();
            TY = new LinearTransform();
            TZ = new LinearTransform();
        }

        /// <summary>
        /// Initializes this instance of the <see cref="Point3DTransform"/> class.
        /// </summary>
        public void Init(Point3D p0, Point3D p1, double t0 = 0, double t1 = 1)
        {
            TX.Init(t0, t1, p0.X, p1.X);
            TY.Init(t0, t1, p0.Y, p1.Y);
            TZ.Init(t0, t1, p0.Z, p1.Z);
        }

        public Point3D GetPoint(double t)
        {
            return new Point3D(TX.Transform(t), TY.Transform(t), TZ.Transform(t));
        }

        /// <summary>
        /// Gets the transformed point.
        /// </summary>
        public Point3D Transform(Point3D pt)
        {
            return new Point3D(TX.Transform(pt.X),
                TY.Transform(pt.Y), TZ.Transform(pt.Z));
        }

        /// <summary>
        /// Gets the inverse transformed point.
        /// </summary>
        public Point3D BackTransform(Point3D pt)
        {
            return new Point3D(TX.BackTransform(pt.X),
                TY.BackTransform(pt.Y), TZ.BackTransform(pt.Z));
        }

        /// <summary>
        /// Gets or sets the transformation for x values.
        /// </summary>
        public LinearTransform TX { get; set; }

        /// <summary>
        /// Gets or sets the transformation for y values.
        /// </summary>
        public LinearTransform TY { get; set; }

        /// <summary>
        /// Gets or sets the transformation for z values.
        /// </summary>
        public LinearTransform TZ { get; set; }
    }

    public class Vector3DTransform
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3DTransform"/> class.
        /// </summary>
        public Vector3DTransform()
        {
            TX = new LinearTransform();
            TY = new LinearTransform();
            TZ = new LinearTransform();
        }

        /// <summary>
        /// Initializes this instance of the <see cref="Vector3DTransform"/> class.
        /// </summary>
        public void Init(Vector3D v0, Vector3D v1, double t0 = 0, double t1 = 1)
        {
            TX.Init(t0, t1, v0.X, v1.X);
            TY.Init(t0, t1, v0.Y, v1.Y);
            TZ.Init(t0, t1, v0.Z, v1.Z);
        }

        public Vector3D GetVector(double t)
        {
            return new Vector3D(TX.Transform(t), TY.Transform(t), TZ.Transform(t));
        }

        /// <summary>
        /// Gets the transformed vector.
        /// </summary>
        public Vector3D Transform(Vector3D v)
        {
            return new Vector3D(TX.Transform(v.X),
                TY.Transform(v.Y), TZ.Transform(v.Z));
        }

        /// <summary>
        /// Gets the inverse transformed vector.
        /// </summary>
        public Vector3D BackTransform(Vector3D v)
        {
            return new Vector3D(TX.BackTransform(v.X),
                TY.BackTransform(v.Y), TZ.BackTransform(v.Z));
        }

        /// <summary>
        /// Gets or sets the transformation for x values.
        /// </summary>
        public LinearTransform TX { get; set; }

        /// <summary>
        /// Gets or sets the transformation for y values.
        /// </summary>
        public LinearTransform TY { get; set; }

        /// <summary>
        /// Gets or sets the transformation for z values.
        /// </summary>
        public LinearTransform TZ { get; set; }
    }

    /// <summary>
    /// A 3D point in spherical coordinates. Angles are in radians.
    /// </summary>
    public struct Point3DS
    {
        public Point3DS(double length, double theta, double phi)
        {
            Length = length;
            Theta = theta;
            Phi = phi;
        }

        public Point3DS(Point3D pt)
        {
            Length = Math.Sqrt(pt.X * pt.X + pt.Y * pt.Y + pt.Z * pt.Z);
            Theta = Math.Acos(pt.Z / Length);
            Phi = Math.Atan2(pt.Y, pt.X);
        }

        /// <summary>
        /// The length of the position vector.
        /// </summary>
        public double Length;

        /// <summary>
        /// Angle to the z axis in radians.
        /// </summary>
        public double Theta;

        /// <summary>
        /// Angle to the x axis in radians.
        /// </summary>
        public double Phi;

        /// <summary>
        /// Convert this point to cartesian coordinates.
        /// </summary>
        public Point3D ToCartesian()
        {
            double sinTheta = Math.Sin(Theta);

            Point3D pt = new Point3D(
                Length * Math.Cos(Phi) * sinTheta,
                Length * Math.Sin(Phi) * sinTheta,
                Length * Math.Cos(Theta));

            return pt;
        }
    }
}

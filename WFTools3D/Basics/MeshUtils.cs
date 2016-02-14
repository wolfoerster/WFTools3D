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
using System.Windows.Media.Media3D;

#if WFToolsAvailable
using WFTools;
#endif

namespace WFTools3D
{
	static public class MeshUtils
	{
		/// <summary>
		/// Add triangle indices i, j and k to the mesh.
		/// </summary>
		public static void AddTriangleIndices(MeshGeometry3D mesh, int i, int j, int k)
		{
			mesh.TriangleIndices.Add(i);
			mesh.TriangleIndices.Add(j);
			mesh.TriangleIndices.Add(k);
		}

		/// <summary>
		/// Invert all texture coordinate y values to 1 - y.
		/// </summary>
		public static void FlipTexture(MeshGeometry3D mesh)
		{
			for (int i = 0; i < mesh.TextureCoordinates.Count; ++i)
			{
				Point pt = mesh.TextureCoordinates[i];
				pt.Y = 1 - pt.Y;
				mesh.TextureCoordinates[i] = pt;
			}
		}

		/// <summary>
		/// Mesh for a triangle.
		/// </summary>
		static public MeshGeometry3D CreateTriangle(Point3D p0, Point3D p1, Point3D p2, int divisions = 1)
		{
			Vector3D u = p1 - p0;
			Vector3D v = p2 - p0;
			if (divisions < 1 || divisions > 999 || u.Length < 1e-12 || v.Length < 1e-12)
				return null;

			MeshGeometry3D mesh = new MeshGeometry3D();
			AddTriangles(mesh, divisions, p0, u, v, 0, 1, false);
			return mesh;
		}

		/// <summary>
		/// Mesh for a square from (-1, -1, 0) to (1, 1, 0).
		/// </summary>
		static public MeshGeometry3D CreateSquare(int divisions = 1)
		{
			if (divisions < 1 || divisions > 999)
				return null;

			MeshGeometry3D mesh = new MeshGeometry3D();
			AddTriangles(mesh, divisions, new Point3D(-1, -1, 0), 2 * Math3D.UnitX, 2 * Math3D.UnitY, 0, 1);
			return mesh;
		}

		/// <summary>
		/// Mesh for a cube from (-1, -1, -1) to (1, 1, 1). 
		/// </summary>
		static public MeshGeometry3D CreateCube(int divisions, bool isClosed)
		{
			if (divisions < 1 || divisions > 999)
				return null;

			MeshGeometry3D mesh = new MeshGeometry3D();
			AddTriangles(mesh, divisions, new Point3D(+1, -1, -1), +2 * Math3D.UnitY, +2 * Math3D.UnitZ, 0.00, 0.25);
			AddTriangles(mesh, divisions, new Point3D(+1, +1, -1), -2 * Math3D.UnitX, +2 * Math3D.UnitZ, 0.25, 0.50);
			AddTriangles(mesh, divisions, new Point3D(-1, +1, -1), -2 * Math3D.UnitY, +2 * Math3D.UnitZ, 0.50, 0.75);
			AddTriangles(mesh, divisions, new Point3D(-1, -1, -1), +2 * Math3D.UnitX, +2 * Math3D.UnitZ, 0.75, 1.00);
			AddTriangles(mesh, divisions, new Point3D(-1, -1, +1), +2 * Math3D.UnitX, +2 * Math3D.UnitY, 0.00, 1.00);
			AddTriangles(mesh, divisions, new Point3D(-1, +1, -1), +2 * Math3D.UnitX, -2 * Math3D.UnitY, 0.00, 1.00);
			return mesh;
		}

		/// <summary>
		/// The three points p0, p0 + u and p0 + v make up a triangle. If 'divisions' is 1 and 'doQuad' is false,<para/>
		/// exactly this triangle will be added to the mesh. If 'divisions' is larger than 1, the triangle will be subdivided.<para/>
		/// If 'doQuad' is true, the triangle is expanded to a parallelogram using the fourth point p0 + u + v.
		/// </summary>
		static public void AddTriangles(MeshGeometry3D mesh, int divisions, Point3D p0, Vector3D u, Vector3D v, double tu0, double tu1, bool doQuad = true)
		{
			TextureTransform tt = new TextureTransform(0, divisions, tu0, tu1, 1, 0);
			Vector3D du = u / (double)divisions;
			Vector3D dv = v / (double)divisions;
			Vector3D n = u.Cross(v);
			n.Normalize();

			for (int iv = 0; iv < divisions; iv++)
			{
				int iuMax = doQuad ? divisions : divisions - iv;
				for (int iu = 0; iu < iuMax; iu++)
				{
					Point3D p1 = p0 + iu * du + iv * dv;
					Point3D p2 = p1 + du;
					Point3D p3 = p1 + dv;
					Point3D p4 = p2 + dv;

					mesh.Positions.Add(p1);
					mesh.Normals.Add(n);
					mesh.TextureCoordinates.Add(tt.Transform(iu, iv));

					mesh.Positions.Add(p2);
					mesh.Normals.Add(n);
					mesh.TextureCoordinates.Add(tt.Transform(iu + 1, iv));

					mesh.Positions.Add(p3);
					mesh.Normals.Add(n);
					mesh.TextureCoordinates.Add(tt.Transform(iu, iv + 1));

					int i = mesh.Positions.Count - 3;
					AddTriangleIndices(mesh, i, i + 1, i + 2);

					bool doUpperTriangle = doQuad ? true : iu < iuMax - 1;
					if (doUpperTriangle)
					{
						mesh.Positions.Add(p4);
						mesh.Normals.Add(n);
						mesh.TextureCoordinates.Add(tt.Transform(iu + 1, iv + 1));

						AddTriangleIndices(mesh, i + 1, i + 3, i + 2);
						continue;
					}
				}
			}
		}

		/// <summary>
		/// Add triangles for a disk segment parallel to the xy plane at a certain z value.
		/// If startDegrees > stopDegrees, the triangle faces will go down the negative z axis.
		/// </summary>
		static void AddTriangles(MeshGeometry3D mesh, int divisions, double outerRadius, double innerRadius, double z, double startDegrees, double stopDegrees)
		{
			double start = MathUtils.ToRadians(startDegrees);
			double stop = MathUtils.ToRadians(stopDegrees);
			double dPhi = (stop - start) / divisions;
			double phi0 = start;

			if (IsNotMeantToBeRound(divisions, startDegrees, stopDegrees))
				phi0 = dPhi * 0.5;

			Vector3D n = new Vector3D(0, 0, Math.Sign(dPhi));
			TextureTransform tt = new TextureTransform(-outerRadius, outerRadius, 0, 1, 1, 0);

			int i0 = mesh.Positions.Count;
			if (innerRadius == 0)
			{
				mesh.Positions.Add(new Point3D(0, 0, z));
				mesh.Normals.Add(n);
				mesh.TextureCoordinates.Add(new Point(0.5, 0.5));
			}

			for (int id = 0; id <= divisions; id++)
			{
				double phi = phi0 + id * dPhi;
				double cos = Math.Cos(phi);
				double sin = Math.Sin(phi);
				double x = outerRadius * cos;
				double y = outerRadius * sin;

				mesh.Positions.Add(new Point3D(x, y, z));
				mesh.Normals.Add(n);
				mesh.TextureCoordinates.Add(tt.Transform(x, y));

				if (innerRadius == 0)
				{
					if (id > 0)
					{
						int i = mesh.Positions.Count - 2;
						AddTriangleIndices(mesh, i0, i, i + 1);
					}
					continue;
				}

				x = innerRadius * cos;
				y = innerRadius * sin;

				mesh.Positions.Add(new Point3D(x, y, z));
				mesh.Normals.Add(n);
				mesh.TextureCoordinates.Add(tt.Transform(x, y));

				if (id > 0)
				{
					int i = mesh.Positions.Count - 2;
					AddTriangleIndices(mesh, i, i - 1, i - 2);
					AddTriangleIndices(mesh, i, i + 1, i - 1);
				}
			}
		}

		/// <summary>
		/// Change the alignment of a cylinder to the xy axes when the number of divisions is 4, 6 or 8.
		/// </summary>
		static bool IsNotMeantToBeRound(int divisions, double startDegrees, double stopDegrees)
		{
			if (divisions == 4 || divisions == 6 || divisions == 8)
			{
				if (startDegrees % 360 == 0 && stopDegrees % 360 == 0)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Mesh for a disk (or disk segment) with an outer radius of 1 and the specified inner radius.
		/// </summary>
		/// <param name="divisions">The number of divisions (&gt;= 3).</param>
		/// <param name="innerRadius">The inner radius (&gt;= 0 and &lt; 1).</param>
		static public MeshGeometry3D CreateDiskSegment(int divisions, double innerRadius = 0, double startDegrees = 0, double stopDegrees = 360)
		{
			if (divisions < 3 || divisions > 999 || innerRadius < 0 || innerRadius >= 1)
				return null;

			MeshGeometry3D mesh = new MeshGeometry3D();
			AddTriangles(mesh, divisions, 1.0, innerRadius, 0, startDegrees, stopDegrees);
			return mesh;
		}

		/// <summary>
		/// Mesh for a cylinder (or cylinder segment) in the xy plane from z = 0 to z = 1. 
		/// The lower radius at z = 0 is always 1 and the upper radius at z = 1 can be specified (must be greater 0).
		/// </summary>
		static public MeshGeometry3D CreateCylinderSegment(int divisions, bool isClosed, double upperRadius = 1.0, double startDegrees = 0, double stopDegrees = 360)
		{
			if (divisions < 3 || divisions > 999 || upperRadius <= 0)//--- use CreateCone if upperRadius needs to be 0
				return null;

			MeshGeometry3D mesh = new MeshGeometry3D();
			double start = MathUtils.ToRadians(startDegrees);
			double stop = MathUtils.ToRadians(stopDegrees);
			double dPhi = (stop - start) / divisions;
			double phi0 = start;

			if (IsNotMeantToBeRound(divisions, startDegrees, stopDegrees))
				phi0 = dPhi * 0.5;

			for (int id = 0; id <= divisions; id++)
			{
				double phi = phi0 + id * dPhi;
				double tx = phi / MathUtils.PIx2;
				double x0 = Math.Cos(phi);
				double y0 = Math.Sin(phi);
				double x1 = upperRadius * x0;
				double y1 = upperRadius * y0;
				Vector3D n = new Vector3D(x0, y0, 0);

				mesh.Positions.Add(new Point3D(x0, y0, 0));
				mesh.Normals.Add(n);
				mesh.TextureCoordinates.Add(new Point(tx, 1));

				mesh.Positions.Add(new Point3D(x1, y1, 1));
				mesh.Normals.Add(n);
				mesh.TextureCoordinates.Add(new Point(tx, 0));

				if (id > 0)
				{
					int i = 2 * id;
					AddTriangleIndices(mesh, i, i - 1, i - 2);
					AddTriangleIndices(mesh, i, i + 1, i - 1);
				}
			}

			if (isClosed)
			{
				AddTriangles(mesh, divisions, upperRadius, 0, 1, startDegrees, stopDegrees);
				AddTriangles(mesh, divisions, 1, 0, 0, stopDegrees, startDegrees);

				if (startDegrees != 0 || stopDegrees != 360)
				{
					AddCylinderEndCap(mesh, startDegrees, upperRadius, true);
					AddCylinderEndCap(mesh, stopDegrees, upperRadius, false);
				}
			}

			return mesh;
		}

		static void AddCylinderEndCap(MeshGeometry3D mesh, double angle, double upperRadius, bool isStart)
		{
			double phi = MathUtils.ToRadians(angle);
			double x0 = Math.Cos(phi);
			double y0 = Math.Sin(phi);
			double x1 = upperRadius * x0;
			double y1 = upperRadius * y0;

			int i = mesh.Positions.Count;
			Vector3D n = Math3D.UnitY.Rotate(Math3D.UnitZ, angle);
			double t0 = isStart ? 0 : 1;
			double t1 = phi / MathUtils.PIx2;

			mesh.Positions.Add(new Point3D(0, 0, 0));
			mesh.Normals.Add(n);
			mesh.TextureCoordinates.Add(new Point(t0, 1));

			mesh.Positions.Add(new Point3D(x0, y0, 0));
			mesh.Normals.Add(n);
			mesh.TextureCoordinates.Add(new Point(t1, 1));

			mesh.Positions.Add(new Point3D(x1, y1, 1));
			mesh.Normals.Add(n);
			mesh.TextureCoordinates.Add(new Point(t1, 0));

			mesh.Positions.Add(new Point3D(0, 0, 1));
			mesh.Normals.Add(n);
			mesh.TextureCoordinates.Add(new Point(t0, 0));

			if (isStart)
			{
				AddTriangleIndices(mesh, i, i + 1, i + 2);
				AddTriangleIndices(mesh, i, i + 2, i + 3);
			}
			else
			{
				AddTriangleIndices(mesh, i, i + 2, i + 1);
				AddTriangleIndices(mesh, i, i + 3, i + 2);
			}
		}

		/// <summary>
		/// Mesh for a cone in the xy plane from z = 0 to z = 1.
		/// </summary>
		static public MeshGeometry3D CreateCone(int divisions, bool isClosed)
		{
			if (divisions < 3 || divisions > 999)
				return null;

			TextureTransform tt = new TextureTransform(-1, 1, 0, 1, 1, 0);
			MeshGeometry3D mesh = new MeshGeometry3D();
			mesh.Positions.Add(new Point3D(0, 0, 1));
			mesh.Normals.Add(new Vector3D(0, 0, 1));
			mesh.TextureCoordinates.Add(tt.Transform(0, 0));

			double dPhi = MathUtils.PIx2 / divisions;
			double phi0 = dPhi * 0.5;

			for (int id = 0; id <= divisions; id++)
			{
				double phi = phi0 + id * dPhi;
				double tx = phi / MathUtils.PIx2;
				double x = Math.Cos(phi);
				double y = Math.Sin(phi);

				mesh.Positions.Add(new Point3D(x, y, 0));
				mesh.Normals.Add(new Vector3D(x, y, 0));
				mesh.TextureCoordinates.Add(tt.Transform(x, y));

				if (id > 0)
					AddTriangleIndices(mesh, id, id + 1, 0);
			}

			if (isClosed)
				AddTriangles(mesh, divisions, 1, 0, 0, 360, 0);

			return mesh;
		}

		/// <summary>
		/// Mesh for a sphere located at the origin with radius 1.
		/// </summary>
		static public MeshGeometry3D CreateSphere(int divisions)
		{
			if (divisions < 1 || divisions > 999)
				return null;

			///////////////////////////////////////////////////////////////////
			// Calculate positions
			//
			MeshGeometry3D mesh = new MeshGeometry3D();
			int lastDivision = 2 * divisions;
			double dTheta = Math.PI / lastDivision;

			for (int id = 0; id <= lastDivision; id++)
			{
				double theta = id * dTheta;
				double sinTheta = Math.Sin(theta);
				double cosTheta = Math.Cos(theta);

				int nPoints = GetNumPoints(id, divisions);
				double dPhi = nPoints == 1 ? 0 : MathUtils.PIx2 / (nPoints - 1);

				for (int j = 0; j < nPoints; j++)
				{
					double phi = j * dPhi;
					double tx = phi / MathUtils.PIx2;
					double x = Math.Cos(phi) * sinTheta;
					double y = Math.Sin(phi) * sinTheta;
					double z = cosTheta;

					mesh.Positions.Add(new Point3D(x, y, z));
					mesh.Normals.Add(new Vector3D(x, y, z));
					mesh.TextureCoordinates.Add(new Point(tx, -cosTheta * 0.5 + 0.5));
				}
			}

			///////////////////////////////////////////////////////////////////
			// Calculate triangle indices
			//
			int i1 = 0;
			for (int id = 0; id < divisions; id++, i1++)
			{
				int n1 = GetNumPoints(id, divisions);
				int n2 = GetNumPoints(id + 1, divisions);
				int i2 = i1 + n1;

				for (int j = 0; j < N; j++)
				{
					AddTriangleIndices(mesh, i1, i2, i2 + 1);
					i2++;

					int nPairs = n1 / N;
					for (int n = 0; n < nPairs; n++, i1++, i2++)
					{
						AddTriangleIndices(mesh, i1, i2, i1 + 1);
						AddTriangleIndices(mesh, i1 + 1, i2, i2 + 1);
					}
				}
			}

			lastDivision = 2 * divisions - 1;
			for (int id = divisions; id <= lastDivision; id++, i1++)
			{
				int n1 = GetNumPoints(id, divisions);
				int n2 = GetNumPoints(id + 1, divisions);
				int i2 = i1 + n1;

				for (int j = 0; j < N; j++)
				{
					AddTriangleIndices(mesh, i1, i2, i1 + 1);
					i1++;

					int nPairs = n2 / N;
					for (int n = 0; n < nPairs; n++, i1++, i2++)
					{
						AddTriangleIndices(mesh, i2, i2 + 1, i1);
						AddTriangleIndices(mesh, i2 + 1, i1 + 1, i1);
					}
				}
			}
			return mesh;
		}
		static int N = 4;//or 3, 5, 6, ...

		static int GetNumPoints(int id, int divisions)
		{
			if (id > divisions)
				id = 2 * divisions - id;

			return N * id + 1;
		}
	}
}

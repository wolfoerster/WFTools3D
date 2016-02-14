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
using System.ComponentModel;

#if WFToolsAvailable
using WFTools;
#endif

namespace WFTools3D
{
	/// <summary>
	/// Holds a perspective camera that can be moved and rotated easily. 
	/// </summary>
	public class CameraBox : INotifyPropertyChanged
	{
		#region INotifyPropertyChanged Members

		/// <summary>
		/// Occurs when a property has changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Fires the PropertyChanged event.
		/// </summary>
		protected void FirePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion INotifyPropertyChanged Members

		#region Public Properties

		public PerspectiveCamera Camera = new PerspectiveCamera();

		public Point3D Position
		{
			get { return Camera.Position; }
			set { Camera.Position = value; }
		}

		public Vector3D LookDirection
		{
			get { return Camera.LookDirection; }
			set
			{
				Camera.LookDirection = value;
				if (!MovingDirectionIsLocked)
					MovingDirection = Camera.LookDirection;
			}
		}

		public Vector3D MovingDirection;

		public bool MovingDirectionIsLocked;

		public Vector3D UpDirection
		{
			get { return Camera.UpDirection; }
			set { Camera.UpDirection = value; }
		}

		public double NearPlaneDistance
		{
			get { return Camera.NearPlaneDistance; }
			set { Camera.NearPlaneDistance = value; }
		}

		public double FarPlaneDistance
		{
			get { return Camera.FarPlaneDistance; }
			set { Camera.FarPlaneDistance = value; }
		}

		public double FieldOfView
		{
			get { return Camera.FieldOfView; }
			set { Camera.FieldOfView = MathUtils.Clamp(value, 1, 170); }
		}

		public Vector3D LeftDirection
		{
			get { return Camera.UpDirection.Cross(Camera.LookDirection); }
		}

		public Vector3D RightDirection
		{
			get { return Camera.LookDirection.Cross(Camera.UpDirection); }
		}

		public double RollAngle
		{
			get { return Cut(LeftDirection.AngleTo(Math3D.UnitZ) - 90); }
		}

		public double PitchAngle
		{
			get { return Cut(LookDirection.AngleTo(Math3D.UnitZ) - 90); }
		}

		public int Speed
		{
			get { return speed; }
			set 
			{
				if (speed != value)
				{
					speed = value;
					FirePropertyChanged("Speed");
				}
			}
		}
		private int speed;

		/// <summary>
		/// The scale is used to determine the speed factor and the airplane size.<para/>
		/// If the overall models size is 10 a scale of 1 is appropriate. Otherwise the scale <para/>
		/// might need to be reduced (models size smaller 10) or enlarged (models size larger 10).
		/// </summary>
		public double Scale = 1;
      
		#endregion Public Properties

		#region Public Methods

		/// <summary>
		/// Changes the yaw angle by the specified angle in degrees.
		/// </summary>
		public void ChangeYaw(double angle)
		{
			LookDirection = LookDirection.Rotate(UpDirection, angle);
		}

		/// <summary>
		/// Changes the roll angle by the specified angle in degrees.
		/// </summary>
		public void ChangeRoll(double angle)
		{
			UpDirection = UpDirection.Rotate(LookDirection, angle);
		}

		/// <summary>
		/// Changes the pitch angle by the specified angle in degrees.
		/// </summary>
		public void ChangePitch(double angle)
		{
			Quaternion q = Math3D.Rotation(LeftDirection, angle);
			UpDirection = q.Transform(UpDirection);
			LookDirection = q.Transform(LookDirection);
		}

		/// <summary>
		/// Rotates the camera about the global z axis by the specified angle in degrees. 
		/// This will not affect the roll angle.
		/// </summary>
		public void ChangeHeading(double angle)
		{
			Quaternion q = Math3D.RotationZ(angle);
			UpDirection = q.Transform(UpDirection);
			LookDirection = q.Transform(LookDirection);
		}

		/// <summary>
		/// Moves to the specified direction.
		/// </summary>
		public void Move(Vector3D direction, double amount)
		{
			Position += direction * amount;
		}

		/// <summary>
		/// Rotates the camera about the specified axis by the specified angle in degrees.
		/// </summary>
		public void Rotate(Vector3D axis, double angle)
		{
			Quaternion q = Math3D.Rotation(axis, angle);
			Position = q.Transform(Position);
			UpDirection = q.Transform(UpDirection);
			LookDirection = q.Transform(LookDirection);
		}

		/// <summary>
		/// Rotates the camera about the specified axis and center point by the specified angle in degrees.
		/// </summary>
		public void Rotate(Vector3D axis, double angle, Point3D center)
		{
			if (!center.IsValid())
				center = Math3D.Origin;

			Position = Position.Subtract(center);
			Rotate(axis, angle);
			Position = Position.Add(center);
		}

		/// <summary>
		/// Turns the camera by 180 degrees about the up direction.
		/// </summary>
		public void LookBack()
		{
			if (lookBackAngle != 0)
				return;

			if (Speed == 0)
				ChangeYaw(180);
			else
				lookBackAngle = 1;
		}
		int lookBackAngle;

		/// <summary>
		/// Turns the camera to the origin and changes the up direction accordingly.
		/// </summary>
		public void LookAtOrigin()
		{
			Math3D.LookAtOrigin(Position, ref targetLook, ref targetUp);
			if (Speed == 0)
			{
				UpDirection = targetUp;
				LookDirection = targetLook;
			}
			else
			{
				turnToTarget = true;
			}
		}
		bool turnToTarget;
		Vector3D targetUp, targetLook;

		/// <summary>
		/// Modifies the pitch and roll angle so that the look direction is parallel to the xy plane.
		/// </summary>
		public void FlyParallel(int mode = 0)
		{
			if (speed == 0)
				return;

			targetLook = LookDirection;
			targetLook.Z = 0;
			targetLook.Normalize();
			targetUp = Math3D.UnitZ;

			if (mode != 0)
				targetUp = targetUp.Rotate(targetLook, mode * 15);

			if (speed == 0 || mode != 0)
			{
				UpDirection = targetUp;
				LookDirection = targetLook;
			}
			else
			{
				turnToTarget = true;
			}
		}

		/// <summary>
		/// Stop any turn motion caused by LookAtOrigin() or FlyParallel().
		/// </summary>
		public void StopAnyTurn()
		{
			turnToTarget = false;
		}

		/// <summary>
		/// Call this 30 times a second, e.g. in a timer tick event handler. Moves the camera according to its speed.
		/// </summary>
		public void Update()
		{
			if (Speed == 0)
				return;

			if (turnToTarget)
				TurnToTarget();

			if (lookBackAngle != 0)
			{
				ChangeYaw(6);
				if ((lookBackAngle += 6) > 180)
					lookBackAngle = 0;
			}
			else
			{
				double factor = Math.Log10(Math.Abs(Speed) + 1);
				double angle = MathUtils.ToRadians(RollAngle);
				ChangeHeading(factor * Math.Sin(angle));//--- makes 15 degrees per second at speed 9 and roll angle 30
				Move(MovingDirection, Speed * Scale / 300.0);//--- makes 1 world unit per second at speed 10 and scale 1
			}
		}

		#endregion Public Methods

		#region Private Stuff

		/// <summary>
		/// Cut an angle to 0 degrees if it is smaller than 0.5 degrees.
		/// </summary>
		double Cut(double angleInDegrees)
		{
			return Math.Abs(angleInDegrees) < 0.5 ? 0 : angleInDegrees;
		}

		/// <summary>
		/// Turns the camera smoothly to a target.
		/// </summary>
		void TurnToTarget()
		{
			double len1 = (UpDirection - targetUp).LengthSquared;
			double len2 = (LookDirection - targetLook).LengthSquared;
			double eps = 3e-5;

			if (len1 > eps || len2 > eps)
			{
				eps = 3e-2;
				UpDirection = Math3D.Lerp(UpDirection, targetUp, eps);
				LookDirection = Math3D.Lerp(LookDirection, targetLook, eps);
			}
			else
			{
				UpDirection = targetUp;
				LookDirection = targetLook;
				turnToTarget = false;
			}
		}

		#endregion Private Stuff
	}
}

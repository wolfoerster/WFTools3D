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
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Input;

#if WFToolsAvailable
using WFTools;
#endif

namespace WFTools3D
{
	/// <summary>
	/// A Border with a Viewport3D, a Models container, Lighting, three Cameras and lots of nice features.
	/// </summary>
	public class Scene3D : Border, INotifyPropertyChanged
	{
		public Scene3D()
		{
			IsCached = true;
			Focusable = true;
			IsInteractive = true;
			Background = Brushes.Black;
			Child = Viewport = new Viewport3D();

			AddCamera(-5, -4, 6);
			AddCamera(+5, -4, 6);
			AddCamera(10, 10, 9);
			ActivateCamera(0);

			Viewport.Children.Add(Models = new Object3D());
			Viewport.Children.Add(Lighting = new Lighting());

			//--- timer is required for flight simulation
			timer = new DispatcherTimer(DispatcherPriority.Render);
			timer.Interval = TimeSpan.FromMilliseconds(30);
			timer.Tick += TimerTick;
		}
		DispatcherTimer timer;

		#region Properties

		/// <summary>
		/// Gets or sets the viewport.
		/// </summary>
		public Viewport3D Viewport { get; protected set; }

		/// <summary>
		/// Gets the active camera.
		/// </summary>
		public CameraBox Camera
		{
			get { return Cameras[ccIndex]; }
		}

		/// <summary>
		/// Gets or sets the 3D models container.
		/// </summary>
		public Object3D Models { get; protected set; }

		/// <summary>
		/// Gets or sets the lighting.
		/// </summary>
		public Lighting Lighting { get; protected set; }

		/// <summary>
		/// True if mouse and keyboard events should be processed.
		/// </summary>
		public bool IsInteractive
		{
			get { return isInteractive; }
			set
			{
				if (isInteractive != value)
				{
					isInteractive = value;
					if (!isInteractive)
						RemoveHelperModels();
				}
			}
		}
		private bool isInteractive;

		/// <summary>
		/// If true, the scene is cached for increased rendering performance.
		/// </summary>
		public bool IsCached
		{
			get { return CacheMode != null; }
			set { CacheMode = value ? new BitmapCache() : null; }
		}

		#endregion Properties

		#region Event Handling

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			Focus();
			base.OnMouseDown(e);
			if (!IsInteractive)
				return;

			if (Helpers.IsCtrlDown())
			{
				touchPoint = GetTouchPoint(e.GetPosition(this));

				if (adi != null && Helpers.IsAltDown())
				{
					adi.TargetPoint = touchPoint;
					adi.Update(Camera);
				}
			}
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);
			touchPoint = Math3D.Origin;
			prevPosition.X = double.NaN;
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			touchPoint = Math3D.Origin;
			prevPosition.X = double.NaN;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (!IsInteractive || e.LeftButton != MouseButtonState.Pressed)
				return;

			Point position = e.GetPosition(this);

			if (prevPosition.IsValid())
				HandleMouseMove(prevPosition - position);

			prevPosition = position;
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (!IsInteractive)
				return;

			//--- assume we are handling the key
			e.Handled = true;
			double amount = Helpers.IsShiftDown() ? 1 : 0.2;

			if (Helpers.IsCtrlDown())
			{
				amount *= Helpers.IsAltDown() ? 0.1 : 0.5;
				amount *= Camera.Scale;
				switch (e.Key)
				{
					case Key.Up: Camera.Move(Camera.LookDirection, +amount); return;
					case Key.Down: Camera.Move(Camera.LookDirection, -amount); return;
					case Key.Left: Camera.Move(Camera.LeftDirection, +amount); return;
					case Key.Right: Camera.Move(Camera.LeftDirection, -amount); return;
					case Key.Prior: Camera.Move(Camera.UpDirection, +amount); return;
					case Key.Next: Camera.Move(Camera.UpDirection, -amount); return;
					default: e.Handled = false; return;
				}
			}

			switch (e.Key)
			{
				case Key.Up: Camera.ChangePitch(amount); break;
				case Key.Down: Camera.ChangePitch(-amount); break;
				case Key.Left: if (Camera.Speed == 0) Camera.ChangeYaw(amount); 
								else Camera.ChangeRoll(-amount); break;
				case Key.Right: if (Camera.Speed == 0) Camera.ChangeYaw(-amount); 
								else Camera.ChangeRoll(+amount); break;
				case Key.Prior: Camera.ChangeRoll(-amount); break;
				case Key.Next: Camera.ChangeRoll(+amount); break;
				case Key.W: Camera.Speed++; return;
				case Key.S: Camera.Speed--; return;
				case Key.X: Camera.Speed = 0; return;
				case Key.F: Camera.FlyParallel(); return;
				case Key.A: Camera.FlyParallel(-1); return;
				case Key.D: Camera.FlyParallel(+1); return;
				case Key.T: Camera.LookBack(); return;
				case Key.H: ToggleHelperModels(); return;
				case Key.Space: Camera.LookAtOrigin(); return;
				case Key.D1: ActivateCamera(0); return;
				case Key.D2: ActivateCamera(1); return;
				case Key.D3: ActivateCamera(2); return;
				default: e.Handled = false; return;
			}

			Camera.StopAnyTurn();
		}

		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			base.OnMouseWheel(e);
			Camera.FieldOfView *= e.Delta < 0 ? 1.1 : 1 / 1.1;
		}

		#endregion Event Handling

		#region Public Methods

		/// <summary>
		/// Show/hide airplanes or ADI.
		/// </summary>
		public void ToggleHelperModels()
		{
			if (airplanes == null)
			{
				airplanes = new Airplane[2];
				Models.Children.Add(airplanes[0] = new Airplane());
				Models.Children.Add(airplanes[1] = new Airplane());
			}
			else
			{
				if (adi == null)
				{
					adi = new ADI();
					Models.Children.Add(adi);
				}
				else
				{
					RemoveHelperModels();
				}
			}
			UpdateHelperModels();
		}
		ADI adi;
		Airplane[] airplanes;

		/// <summary>
		/// Remove airplanes and ADI.
		/// </summary>
		public void RemoveHelperModels()
		{
			if (adi != null)
			{
				Models.Children.Remove(adi);
				adi = null;
			}
			if (airplanes != null)
			{
				Models.Children.Remove(airplanes[0]);
				Models.Children.Remove(airplanes[1]);
				airplanes = null;
			}
		}

		public bool ActivateCamera(int index)
		{
			if (!MathUtils.IsValidIndex(index, Cameras.Count))
				return false;

			ccIndex = index;
			Viewport.Camera = Camera.Camera;
			FirePropertyChanged("Camera");

			UpdateHelperModels();
			return true;
		}

		#endregion Public Methods

		#region Private Stuff

		/// <summary>
		/// Adds a camera at the specified position.
		/// </summary>
		void AddCamera(double x, double y, double z)
		{
			CameraBox camera = new CameraBox();
			camera.PropertyChanged += CameraPropertyChanged;
			camera.NearPlaneDistance = 0.1;
			camera.FarPlaneDistance = 1000;
			camera.Position = new Point3D(x, y, z);
			camera.LookAtOrigin();
			Cameras.Add(camera);
		}

		/// <summary>
		/// The list of cameras.
		/// </summary>
		List<CameraBox> Cameras = new List<CameraBox>();

		/// <summary>
		/// The index of the active camera.
		/// </summary>
		int ccIndex = 0;

		/// <summary>
		/// Point in the 3D world which is used as rotation center when ctrl-dragging the mouse.
		/// </summary>
		Point3D touchPoint = Math3D.Origin;

		/// <summary>
		/// Previous mouse position which is used in OnMouseMove.
		/// </summary>
		Point prevPosition = new Point(double.NaN, 0);

		/// <summary>
		/// Rotates the camera according mouse movements.
		/// </summary>
		protected void HandleMouseMove(Vector mouseMove)
		{
			double factor = Helpers.IsShiftDown() ? 0.5 : 0.1;
			double angleX = mouseMove.X * factor;
			double angleY = mouseMove.Y * factor;

			Camera.StopAnyTurn();
			if (Camera.Speed == 0)
			{
				Camera.Rotate(Math3D.UnitZ, 2 * angleX, touchPoint);
				Camera.Rotate(Camera.RightDirection, 2 * angleY, touchPoint);
			}
			else
			{
				if (Camera.MovingDirectionIsLocked)
				{
					Camera.ChangeHeading(angleX);
					Camera.ChangePitch(angleY);
				}
				else
				{
					Camera.ChangeRoll(-angleX);
					Camera.ChangePitch(angleY);
				}
			}
		}

		/// <summary>
		/// Find the 3D model point beneath a 2D viewport point.
		/// </summary>
		Point3D GetTouchPoint(Point pt2D)
		{
			RayMeshGeometry3DHitTestResult htr = Math3D.HitTest(Viewport, pt2D);
			if (htr == null)
				return Math3D.Origin;

			Point3D pt3D = htr.PointHit;//--- in model space
			Matrix3D m = Math3D.GetTransformationMatrix(htr.VisualHit);
			pt3D = m.Transform(pt3D);//--- in global space
			return pt3D;
		}

		/// <summary>
		/// Watch the camera speed to start/stop the timer.
		/// </summary>
		void CameraPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Speed")
			{
				if (Cameras.FirstOrDefault(cam => cam.Speed != 0) == null)
				{
					timer.Stop();
					IsCached = true;
				}
				else if (!timer.IsEnabled)
				{
					IsCached = false;
					timer.Start();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		void UpdateHelperModels()
		{
			//--- the ADI shows the position and orientation of the active camera
			if (adi != null)
				adi.Update(Camera);

			if (airplanes == null)
				return;

			//--- the airplanes show the position and orientation of the other cameras
			for (int airplaneIndex = 0, camIndex = 0; airplaneIndex < 2; airplaneIndex++, camIndex++)
			{
				if (camIndex == ccIndex) camIndex++;
				Matrix3D vm = Math3D.GetViewMatrix(Cameras[camIndex].Camera);
				vm.Invert();

				Matrix3D wm = Matrix3D.Identity;
				double scale = Cameras[camIndex].Scale;
				wm.Scale(new Vector3D(scale, scale, scale));
				wm.Rotate(new Quaternion(Math3D.UnitX, -90));

				airplanes[airplaneIndex].Transform = new MatrixTransform3D(wm * vm);
			}
		}

		/// <summary>
		/// Timer tick event handler. Moves the cameras to their new position.
		/// </summary>
		void TimerTick(object sender, EventArgs e)
		{
			Camera.MovingDirectionIsLocked = Helpers.IsCtrlDown() || Console.CapsLock;

			foreach (var camera in Cameras)
				camera.Update();

			UpdateHelperModels();

			if (TimerTicked != null)
				TimerTicked(sender, e);
		}

		public event EventHandler TimerTicked;

		#endregion Private Stuff

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		protected void FirePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}

//******************************************************************************************
// Copyright © 2016 - 2022 Wolfgang Foerster (wolfoerster@gmx.de)
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

namespace WFTools3D
{
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

    /// <summary>
    /// A Border with a Viewport3D, a Models container, Lighting, three Cameras and lots of nice features.
    /// </summary>
    public class Scene3D : Border, INotifyPropertyChanged
    {
        private readonly DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Render);

        public Scene3D()
        {
            IsCached = true;
            Focusable = true;
            IsInteractive = true;
            Background = Brushes.Black;

            ModelsContainer = new Object3D();
            Models = ModelsContainer.Children;
            Lighting = new Lighting();

            Child = Viewport = new Viewport3D();
            Viewport.Children.Add(ModelsContainer);
            Viewport.Children.Add(Lighting);

            AddCamera(8, -8, 6);
            AddCamera(-8, -8, 6);
            AddCamera(12, 12, 9);
            ActivateCamera(0);

            //--- timer is required for flight simulation
            timer.Interval = TimeSpan.FromMilliseconds(30);
            timer.Tick += TimerTick;
        }

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
        /// Gets the active camera index.
        /// </summary>
        public int CameraIndex
        {
            get { return ccIndex; }
        }

        /// <summary>
        /// Gets or sets the 3D models container.
        /// </summary>
        public Object3D ModelsContainer { get; protected set; }

        /// <summary>
        /// Gets the 3D models collection.
        /// </summary>
        public Visual3DCollection Models { get; protected set; }

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

        /// <summary>
        /// Occurs when the internal timer has ticked.
        /// </summary>
        public event EventHandler TimerTicked;

        #endregion Properties

        #region Event Handling

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            Focus();
            base.OnMouseDown(e);
            if (!IsInteractive)
                return;

            if (WFUtils.IsCtrlDown())
            {
                touchPoint = GetTouchPoint(e.GetPosition(this));

                if (adi != null && WFUtils.IsAltDown())
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
            double amount = WFUtils.IsShiftDown() ? 1 : 0.2;

            if (WFUtils.IsCtrlDown())
            {
                amount *= WFUtils.IsAltDown() ? 0.1 : 0.5;
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
                case Key.Left:
                    if (Camera.Speed == 0) Camera.ChangeYaw(amount);
                    else Camera.ChangeRoll(-amount); break;
                case Key.Right:
                    if (Camera.Speed == 0) Camera.ChangeYaw(-amount);
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
            if (isInteractive)
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
                Models.Add(airplanes[0] = new Airplane());
                Models.Add(airplanes[1] = new Airplane());
            }
            else
            {
                if (adi == null)
                {
                    adi = new ADI();
                    Models.Add(adi);
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
                Models.Remove(adi);
                adi = null;
            }
            if (airplanes != null)
            {
                Models.Remove(airplanes[0]);
                Models.Remove(airplanes[1]);
                airplanes = null;
            }
        }

        /// <summary>
        /// Activates a camera. Valid indices are 0, 1 and 2.
        /// </summary>
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

        /// <summary>
        /// Starts the internal timer.
        /// </summary>
        public void StartTimer(int ms = 30)
        {
            if (timer.IsEnabled)
                return;

            timer.Interval = TimeSpan.FromMilliseconds(ms);
            manualStart = true;
            IsCached = false;
            timer.Start();
        }
        bool manualStart;

        /// <summary>
        /// Stops the internal timer.
        /// </summary>
        public void StopTimer()
        {
            if (!timer.IsEnabled)
                return;

            manualStart = false;
            IsCached = true;
            timer.Stop();
        }

        /// <summary>
        /// Returns true if the internal timer is busy.
        /// </summary>
        public bool IsTimerBusy
        {
            get { return timer.IsEnabled; }
        }

        /// <summary>
        /// Returns the 2D bounding box.
        /// </summary>
        public Rect GetBoundingBox()
        {
            Rect bounds = Rect.Empty;
            AddBoundingBox(ModelsContainer, ref bounds);
            return bounds;
        }

        #endregion Public Methods

        #region Private Stuff

        void AddBoundingBox(Object3D obj, ref Rect bounds)
        {
            if (obj == null)
                return;

            Primitive3D primitive = obj as Primitive3D;
            if (primitive != null)
                bounds.Union(primitive.GetBoundingBox());

            foreach (var child in obj.Children)
                AddBoundingBox(child as Object3D, ref bounds);
        }

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
            double factor = WFUtils.IsShiftDown() ? 0.5 : 0.1;
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
                if (!manualStart)
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
#pragma warning disable CA1416 // Validate platform compatibility
            Camera.MovingDirectionIsLocked = WFUtils.IsCtrlDown() || Console.CapsLock;
#pragma warning restore CA1416 // Validate platform compatibility

            foreach (var camera in Cameras)
                camera.Update();

            UpdateHelperModels();

            if (TimerTicked != null)
                TimerTicked(sender, e);
        }

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

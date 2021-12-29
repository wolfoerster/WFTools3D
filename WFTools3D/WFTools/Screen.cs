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
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Windows;

    /// <summary>
    /// Screen helper class.
    /// </summary>
    public class Screen
    {
        /// <summary>
        /// Gets or sets the name of the screen.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the total screen area.
        /// </summary>
        public Rect ScreenArea { get; set; }

        /// <summary>
        /// Gets or sets the available screen area.
        /// </summary>
        public Rect WorkArea { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is the primary screen.
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// Looks for the screen with the given name.
        /// </summary>
        /// <param name="name">The screen name.</param>
        /// <returns>The matching screen or null.</returns>
        public static Screen LookUpByName(string name)
        {
            List<Screen> screens = GetAllScreens();

            foreach (var screen in screens)
            {
                if (screen.Name == name)
                {
                    return screen;
                }
            }

            return null;
        }

        /// <summary>
        /// Looks for the screen which contains the specified pixel.
        /// </summary>
        /// <param name="pt">The pixel point.</param>
        /// <returns>The matching screen or null.</returns>
        public static Screen LookUpByPixel(Point pt)
        {
            List<Screen> screens = GetAllScreens();

            foreach (var screen in screens)
            {
                if (screen.WorkArea.Contains(pt))
                {
                    return screen;
                }
            }

            return null;
        }

        /// <summary>
        /// Looks for the screen which contains the specified pixel coordinates.
        /// </summary>
        /// <param name="x">The pixel x coordinate.</param>
        /// <param name="y">The pixel y coordinate.</param>
        /// <returns>The matching screen or null.</returns>
        public static Screen LookUpByPixel(double x, double y)
        {
            return LookUpByPixel(new Point(x, y));
        }

        /// <summary>
        /// Looks for the primary screen.
        /// </summary>
        /// <returns>The primary screen or null.</returns>
        public static Screen LookUpPrimary()
        {
            List<Screen> screens = GetAllScreens();

            foreach (var screen in screens)
            {
                if (screen.IsPrimary)
                {
                    return screen;
                }
            }

            return null;
        }

        /****
         *
         * The following code is used to find all screens.
         *
         * ****/

        [DllImport("user32.dll")]
        private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);

        private delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfoEx lpmi);

        // size of a device name string
        private const int CCHDEVICENAME = 32;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MonitorInfoEx
        {
            public int Size;

            public RectStruct Monitor;

            public RectStruct WorkArea;

            public uint Flags; //--- first bit = MONITORINFOF_PRIMARY

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
            public string DeviceName;

            public void Init()
            {
                this.Size = 40 + (2 * CCHDEVICENAME);
                this.DeviceName = string.Empty;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RectStruct
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        /// <summary>
        /// Looks up all screens.
        /// </summary>
        /// <returns>A list of all screens.</returns>
        public static List<Screen> GetAllScreens()
        {
            List<Screen> screens = new List<Screen>();

            EnumDisplayMonitors(
                IntPtr.Zero,
                IntPtr.Zero,
                delegate (IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData)
                {
                    MonitorInfoEx mi = default(MonitorInfoEx);
                    mi.Size = (int)Marshal.SizeOf(mi);
                    bool success = GetMonitorInfo(hMonitor, ref mi);
                    if (success)
                    {
                        Screen screen = new Screen()
                        {
                            ScreenArea = new Rect(mi.Monitor.Left, mi.Monitor.Top, mi.Monitor.Right - mi.Monitor.Left, mi.Monitor.Bottom - mi.Monitor.Top),
                            WorkArea = new Rect(mi.WorkArea.Left, mi.WorkArea.Top, mi.WorkArea.Right - mi.WorkArea.Left, mi.WorkArea.Bottom - mi.WorkArea.Top),
                            IsPrimary = (mi.Flags & 1) == 1,
                            Name = mi.DeviceName,
                        };
                        screens.Add(screen);
                    }

                    return true;
                },
                IntPtr.Zero);

            return screens;
        }
    }
}

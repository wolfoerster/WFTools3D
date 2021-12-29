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
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;

    public static class WFExtensions
    {
        public static bool IsEqualTo(this double value1, double value2, double epsilon = 1e-12)
        {
            return Math.Abs(value1 - value2) <= epsilon;
        }

        public static void Dispatch(this Visual visual, Action action, DispatcherPriority priority = DispatcherPriority.Render)
        {
            visual.Dispatcher.Invoke(action, priority);
            //visual.Dispatcher.BeginInvoke(priority, action);
        }

        public static Point ToDip(this Point pointInPixel, Visual visual)
        {
            var dpi = VisualTreeHelper.GetDpi(visual);
            var pt = pointInPixel;
            pt.X /= dpi.DpiScaleX;
            pt.Y /= dpi.DpiScaleY;
            return pt;
        }

        public static Point ToPixel(this Point pointInDip, Visual visual)
        {
            var dpi = VisualTreeHelper.GetDpi(visual);
            var pt = pointInDip;
            pt.X *= dpi.DpiScaleX;
            pt.Y *= dpi.DpiScaleY;
            return pt;
        }
    }
}

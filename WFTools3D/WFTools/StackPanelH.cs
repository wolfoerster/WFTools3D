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
    using System.Windows;
    using System.Windows.Controls;

    public class StackPanelH : StackPanel
    {
        public StackPanelH()
        {
            Orientation = Orientation.Horizontal;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            double x = 0;

            foreach (UIElement child in base.InternalChildren)
            {
                if (child != null)
                {
                    double width = child.DesiredSize.Width;
                    double height = child.DesiredSize.Height;
                    double y = (arrangeSize.Height - height) * 0.5;

                    Rect rect = new Rect(x, y, width, height);
                    child.Arrange(rect);
                    x += width;
                }
            }

            return arrangeSize;
        }
    }
}

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

    public struct Area
    {
        private Range rangeX;
        private Range rangeY;

        public Area(Range rangeX, Range rangeY)
        {
            this.rangeX = rangeX;
            this.rangeY = rangeY;
        }

        public Area(double left, double top, double right, double bottom)
        {
            rangeX = new Range(left, right);
            rangeY = new Range(bottom, top);
        }

        public Area(Point topLeft, Point bottomRight)
        {
            rangeX = new Range(topLeft.X, bottomRight.X);
            rangeY = new Range(bottomRight.Y, topLeft.Y);
        }

        public Range RangeX
        {
            get { return rangeX; }
            set { rangeX = value; }
        }

        public Range RangeY
        {
            get { return rangeY; }
            set { rangeY = value; }
        }
    }
}

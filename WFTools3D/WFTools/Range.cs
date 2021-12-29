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
    public struct Range
    {
        private double from;
        private double to;

        public Range(double from, double to)
        {
            this.from = from;
            this.to = to;
        }

        public double From
        {
            get { return from; }
            set { from = value; }
        }

        public double To
        {
            get { return to; }
            set { to = value; }
        }

        public double Width
        {
            get { return to - from; }
            set { to = from + value; }
        }

        public double Center
        {
            get { return (from + to) * 0.5; }
            set { Offset(value - Center); }
        }

        public void Offset(double value)
        {
            from += value;
            to += value;
        }
    }
}

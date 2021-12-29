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
    /// <summary>
    /// A linear transformation from a range of doubles to another range of doubles.
    /// </summary>
    public class LinearTransform
    {
        private double slope;
        private double offset;

        public LinearTransform()
        {
            Init(0, 1, 0, 1);
        }

        public LinearTransform(double from1, double from2, double to1, double to2)
        {
            Init(from1, from2, to1, to2);
        }

        public bool Init(double from1, double from2, double to1, double to2)
        {
            if (from1.IsEqualTo(from2) || to1.IsEqualTo(to2))
            {
                slope = 1;
                offset = 0;
                return false;
            }
            else
            {
                slope = (to2 - to1) / (from2 - from1);
                offset = to1 - slope * from1;
                return true;
            }
        }

        public double Slope
        {
            set { slope = value; }
            get { return slope; }
        }

        public double Offset
        {
            set { offset = value; }
            get { return offset; }
        }

        public double Transform(double value)
        {
            return slope * value + offset;
        }

        public double BackTransform(double value)
        {
            return (value - offset) / slope;
        }
    }
}

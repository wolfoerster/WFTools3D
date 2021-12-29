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
    using static MathUtils;

    public class TextureTransform
    {
        private readonly LinearTransform tx = new LinearTransform();
        private readonly LinearTransform ty = new LinearTransform();

        public TextureTransform(double from1, double from2, double tx1, double tx2, double ty1, double ty2)
        {
            tx.Init(from1, from2, tx1, tx2);
            ty.Init(from1, from2, ty1, ty2);
        }

        public Point Transform(double x, double y)
        {
            return new Point(Clamp(tx.Transform(x), 0, 1), Clamp(ty.Transform(y), 0, 1));
        }
    }
}

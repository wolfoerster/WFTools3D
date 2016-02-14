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
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WFTools3D
{
	/// <summary>
	/// 
	/// </summary>
	public class Lighting : ModelVisual3D
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Lighting"/> class.
		/// </summary>
		public Lighting()
		{
			Content = lightingGroup;
			lightingGroup.Children.Add(ambientLight);
			lightingGroup.Children.Add(directionalLight1);
			lightingGroup.Children.Add(directionalLight2);
		}

		public Model3DGroup LightingGroup
		{
			get { return lightingGroup; }
		}
		Model3DGroup lightingGroup = new Model3DGroup();

		/// <summary>
		/// 
		/// </summary>
		public AmbientLight AmbientLight
		{
			get { return ambientLight; }
		}
		AmbientLight ambientLight = new AmbientLight(Colors.White);

		/// <summary>
		/// 
		/// </summary>
		public DirectionalLight DirectionalLight1 
		{
			get { return directionalLight1; }
		}
		DirectionalLight directionalLight1 = new DirectionalLight(Colors.White, new Vector3D(23, 28, -15));

		/// <summary>
		/// 
		/// </summary>
		public DirectionalLight DirectionalLight2
		{
			get { return directionalLight2; }
		}
		DirectionalLight directionalLight2 = new DirectionalLight(Colors.White, new Vector3D(-23, -28, -15));
	}
}

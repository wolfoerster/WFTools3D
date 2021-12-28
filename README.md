# WFTools3D
3D Tools for the Windows Presentation Foundation

The Windows Presentation Foundation (WPF) comes with an easy-to-use 3D framework 
which is essentially a wrapper around DirectX. Although the performance is not as 
high as it would be when using Direct3D directly, it's good enough for simple 
scenarious, and it's definitely fun to play around with it.

The WFTools3D library makes using WPF 3D even more simple and more fun. 
The heart of this library is a WPF FrameworkElement that brings the world of 3D 
into your visual tree: class Scene3D.

The Scene3D has a 'Models' property of type Visual3DCollection. Although you can add
any ModelVisual3D object to this property to populate the scene, it's easier to use
Object3D and Primitive3D objects. 

An Object3D derives from ModelVisual3D and makes it very easy to scale, rotate and 
translate an object in 3D space. Primitive3D is an extension to Object3D and adds 
flesh and bones to the object. Or more correctly: a mesh and materials.

Class Scene3D also has 3 built-in cameras which again can be rotated and translated 
very easily. A highlight for sure is the 'Speed' property of the cameras which makes 
them fly in their look direction. Using the mouse you can now change yaw, pitch and 
roll angles just like in a flight simulator.

<img src="https://i.postimg.cc/gksJQtf3/WFTools3-DDemo.jpg" style="width:880px;">

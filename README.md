# LoadObjTest

A Stride3d sample which loads a Wavefront OBJ file at runtime.

* LoadAsset.cs - This is the asset loading script. It is attached to an empty entity to cause it's "Start()" method to run
* WavefrontOBJLoader.cs - this is the code that parses the Wavefront .OBJ and .MTL files
* Wavefront_VertexSoup_Stride3d.cs - This generates drawable vertex and index buffers with unique "fully configured verticies". In Wavefront files, a face definition specifies two indicies per point, one for position, one for normal. A unique position and unique normal are listed only once per file. In GPU rendering, each face includes one index per-point, of a "fully configured index". If a vertex position is used with two different normals, then it has to be listed twice as two different vertex buffer entries. This code performs this conversion.

Limitations:

- currently only loads the mesh (not materials)... working on it

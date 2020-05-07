# LoadObjTest

A Stride3d sample which dynamically loads a Wavefront OBJ file from the filesystem, and creates a Stride3d mesh object.

You want to Dynamic load when:

The 3d model is not known when your game is built. For example, if you want to import a community created 3d model in OBJ format directly from disk. However, another alternative is having users load models into Stride3d Studio and export asset bundles, and then dynamic loading the asset bundles at runtime. There are tradeoffs between these two methods.

You dont want to Dynamic load when:

The 3d model is a fixed part of the game, known at build-time. In this case, you (generally) want to load the model as an asset in Stride3d Studio, and make it part of the standard asset build process. This allows you to instantiate the model in the editor/designer, and also in code.

Code overview:

* `LoadAsset.cs` - This is the asset loading script. It is attached to an empty entity to cause it's "Start()" method to run
* `WavefrontOBJLoader.cs` - this is the code that parses the Wavefront .OBJ and .MTL files
* `Wavefront_VertexSoup_Stride3d.cs` - This generates drawable vertex and index buffers with unique "fully configured verticies". In Wavefront files, a face definition specifies two indicies per point, one for position, one for normal. A unique position and unique normal are listed only once per file. In GPU rendering, each face includes one index per-point, of a "fully configured index". If a vertex position is used with two different normals, then it has to be listed twice as two different vertex buffer entries. This code performs this conversion.

Limitations:

- currently only loads the mesh (not materials)... working on it

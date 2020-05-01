using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.Extensions;
using Stride.Rendering;
using Stride.Rendering.Materials;
using Stride.Rendering.Materials.ComputeColors;
using Stride.Graphics;
using Stride.Core.Mathematics;

// NOTE: REMEMBER TO CREATE AN EMPTY ENTITY, and add this script as a component!


// https://doc.xenko.com/latest/en/manual/scripts/create-a-model-from-code.html
// https://doc.xenko.com/latest/en/manual/graphics/low-level-api/draw-vertices.html
// https://github.com/profan/XenkoByteSized/tree/master/XenkoByteSized/ProceduralMesh
// https://github.com/stride3d/stride-community-projects

namespace LoadObjectTest
{
    public class LoadAsset : SyncScript
    {
        // Declared public member fields and properties will show in the game studio


        public void LoadAssetTest() {
        
            // Create a new entity and add it to the scene.
            var entity = new Entity();
            
            var rootScene = SceneSystem.SceneInstance.RootScene;
            entity.Transform.Scale = new Vector3(0.2f,0.2f,0.2f);
            entity.Transform.Position = new Vector3(0f,2f,0f);
            entity.Transform.RotationEulerXYZ = new Vector3(0,20,0);
            
            // Create a new model from code
            // https://doc.xenko.com/latest/en/manual/scripts/create-a-model-from-code.html
            
            // Create a model and assign it to the model component.
            var model = new Stride.Rendering.Model();
            entity.GetOrCreate<ModelComponent>().Model = model;  

            // Add one or more meshes using geometric primitives (eg spheres or cubes).
            //var meshDraw = Stride.Graphics.GeometricPrimitives.GeometricPrimitive.Sphere.New(GraphicsDevice).ToMeshDraw();
            //var mesh = new Stride.Rendering.Mesh { Draw = meshDraw }; 
            //model.Meshes.Add(mesh);            

            // create the Mesh
            // https://github.com/stride3d/stride/blob/master/sources/editor/Stride.Assets.Presentation/AssetEditors/Gizmos/LightSpotGizmo.cs#L168

            var CWD = System.IO.Directory.GetCurrentDirectory();

            var assetPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(CWD,@"..\..\..\LoadObjectTest\DynLoadAssets\drone2\Drone2.obj"));

            if (!System.IO.File.Exists(assetPath)) {     
                // not sure why, but DebugText.Print isn't working at the time of testing this...
                DebugText.Print("Cannot find wavefront OBJ file at : " + assetPath, new Int2(50,50));
                return;
            }

            // load the wavefront OBJ file...
            var wfData = new SimpleScene.Util3d.WavefrontObjLoader(assetPath);
            
            // TODO: iterate over materials / multiple materials on the same mesh...
            {
                VertexPositionNormalTexture[] vertices;                
                UInt32[] triIndices;
                Wavefront_VertexSoup_Stride3d.generateDrawIndexBuffer(wfData,wfData.materials[0],out triIndices, out vertices);
                                        
                // convert into a graphics VB / IB pair
                var vertexBuffer = Stride.Graphics.Buffer.Vertex.New(GraphicsDevice, vertices, GraphicsResourceUsage.Dynamic);            
                var indexBuffer = Stride.Graphics.Buffer.Index.New(GraphicsDevice, triIndices);

                // add them to the drawing 
                var customMesh = new Stride.Rendering.Mesh { 
                    Draw = new Stride.Rendering.MeshDraw { 
                            /* Vertex buffer and index buffer setup */ 
                            PrimitiveType = Stride.Graphics.PrimitiveType.TriangleList,
                            DrawCount = triIndices.Length,
                            VertexBuffers = new[] { new VertexBufferBinding(vertexBuffer, VertexPositionNormalTexture.Layout, vertexBuffer.ElementCount) },               
                            IndexBuffer = new IndexBufferBinding(indexBuffer, true, triIndices.Length),                            
                        } };            
                // add the mesh to the model
                model.Meshes.Add(customMesh);
            }


            // Create a material (eg with red diffuse color).
            var materialDescription = new Stride.Rendering.Materials.MaterialDescriptor
            {
                Attributes =
                {
                    DiffuseModel = new MaterialDiffuseLambertModelFeature(),
                    Diffuse = new MaterialDiffuseMapFeature(new ComputeColor { Key = MaterialKeys.DiffuseValue })
                }
            };
            var material = Material.New(GraphicsDevice, materialDescription);
            material.Passes[0].Parameters.Set(MaterialKeys.DiffuseValue, Color.Red);
            model.Materials.Add(material);

           SceneSystem.SceneInstance.RootScene.Entities.Add(entity);

        }
               

        public override void Start()
        {
            // Initialization of the script.
            LoadAssetTest();
             
            //Find camera, I've only got one so this works. If you've got more 
            //than one, I guess you'll need to name them or something...
            // var camera = SceneSystem.SceneInstance.Scene.Entities.First(e => e.Components.ContainsKey(CameraComponent.Key));
            // camera.Add(new ScriptComponent. { Scripts = { new CameraScript()} });
        }

        public override void Update()
        {
            // Do stuff every new frame
        }
    }
}

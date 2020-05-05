using System;
using System.IO;
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


using Stride.UI.Panels;
using Stride.UI.Controls;
using Stride.UI;
using Stride.Rendering.Fonts;
using Stride.Graphics.Font;
using SimpleScene.Util3d;
using System.Data.SqlTypes;


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

        public SpriteFont myFont2;

        public void CreateUI() {
        // var fontSystem = Services.GetService<FontSystem>();
        // var myFont = fontSystem.NewDynamic(10,"Orkney Regular",FontStyle.Regular);
             var startButton = new Button
            {
                Content = new TextBlock {Font = myFont2, Text = "Touch to Start", TextColor = Color.Black, 
                    HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center},                
                Padding = new Thickness(77, 30, 25, 30),
                MinimumWidth = 250f,
            };
            var mainCanvas = new Canvas();
            mainCanvas.Children.Add(startButton);

            mainCanvas.BackgroundColor = Color.Red;
            var mainMenuRoot = new ModalElement{
                HorizontalAlignment = HorizontalAlignment.Stretch,
                // VerticalAlignment = VerticalAlignment.Bottom,
                DefaultHeight = 200,
                Content = mainCanvas 
                };
            

            Entity.Get<UIComponent>().Page = new UIPage { RootElement = mainMenuRoot };
        }

        private Texture textureObjectForWfTex(string textureFilename) {           
            var diffTexStream = System.IO.File.Open(textureFilename,System.IO.FileMode.Open,System.IO.FileAccess.Read);
            var textureObject = Texture.Load(GraphicsDevice,diffTexStream);
         
            return textureObject;
        }

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
            var assetBase = System.IO.Path.GetFullPath(System.IO.Path.Combine(CWD,@"..\..\..\LoadObjectTest\DynLoadAssets\drone2\"));
            var assetPath = (System.IO.Path.Combine(assetBase,"Drone2.obj"));

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
                // customMesh.Draw.GenerateTangentBinormal();

                // set the material index for this mesh
                // customMesh.MaterialIndex = 0;

                // add the mesh to the model                
                model.Meshes.Add(customMesh);
            }

            /*
            this is some sample code of how xenko reads a stream

            Texture texture;
            using (var inStream = game.Content.OpenAsStream(filePath, StreamFlags.None))
                texture = Texture.Load(device, inStream);
        
            var tempStream = new MemoryStream();
            texture.Save(game.GraphicsContext.CommandList, tempStream, intermediateFormat);
            tempStream.Position = 0;
            texture.Dispose();

            using (var inStream = game.Content.OpenAsStream(filePath, StreamFlags.None))
            using (var originalImage = Image.Load(inStream))
            {
                using (var textureImage = Image.Load(tempStream))
                {
                    TestImage.CompareImage(originalImage, textureImage, false, 0, fileName);
                }
            }        
             */


            // load a texture from a file            
            // var diffuseTextureFilename = System.IO.Path.Combine(assetBase,wfData.materials[0].mtl.diffuseTextureResourceName);
            // var diffTexStream = System.IO.File.Open(diffuseTextureFilename,System.IO.FileMode.Open,System.IO.FileAccess.Read);
            // var diffuseTexture = Texture.Load(GraphicsDevice,diffTexStream);

            var diffuseTexture = textureObjectForWfTex(System.IO.Path.Combine(assetBase,wfData.materials[0].mtl.diffuseTextureResourceName));
            var specularTexture = textureObjectForWfTex(System.IO.Path.Combine(assetBase,wfData.materials[0].mtl.specularTextureResourceName));            
            var emissiveTexture = textureObjectForWfTex(System.IO.Path.Combine(assetBase,wfData.materials[0].mtl.ambientTextureResourceName));
            var bumpTexture = textureObjectForWfTex(System.IO.Path.Combine(assetBase,wfData.materials[0].mtl.bumpTextureResourceName));

            var cc = new ComputeColor();
            
            // Create a material (eg with red diffuse color).
            {
                var materialDescription = new Stride.Rendering.Materials.MaterialDescriptor
                {
                    Attributes =
                    {                   
                        DiffuseModel = new MaterialDiffuseLambertModelFeature(),                        
                        // Diffuse = new MaterialDiffuseMapFeature(new ComputeTextureColor(diffuseTexture)),

                        // SpecularModel = new MaterialSpecularMicrofacetModelFeature{ Visibility  = new MaterialSpecularMicrofacetVisibilitySmithBeckmann()} ,
                        SpecularModel = new MaterialSpecularMicrofacetModelFeature{} ,
                        Specular = new MaterialSpecularMapFeature{ SpecularMap = new ComputeTextureColor(specularTexture)},

                        // Emissive = new MaterialEmissiveMapFeature(new ComputeTextureColor(emissiveTexture)),


                        // https://gist.github.com/johang88/3f175b045c8e8b55fb815cc19e6128ba
                        // see TNBExtensions.GenerateTangentBinormal(this MeshDraw meshData)
                        // Surface = new MaterialNormalMapFeature( new ComputeTextureColor(bumpTexture) ),
                       
                        // this is for a solid color rendering...
                        // Diffuse = new MaterialDiffuseMapFeature(new ComputeColor { Key = MaterialKeys.DiffuseValue }),

                    }
                };
                var material = Material.New(GraphicsDevice, materialDescription);            
                material.Passes[0].Parameters.Set(MaterialKeys.EmissiveIntensity,5.0f);                
                
                // this is for solid color rendering...
                // material.Passes[0].Parameters.Set(MaterialKeys.DiffuseValue, Color.Red);     
                model.Materials.Add(material);
           }
            

           SceneSystem.SceneInstance.RootScene.Entities.Add(entity);

        }
               

        public override void Start()
        {
            // Initialization of the script.
            LoadAssetTest();
            CreateUI();
             
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.Graphics;

namespace LoadObjectTest
{
    public class CameraScript : AsyncScript
    {
        // Declared public member fields and properties will show in the game studio
        public Entity Camera;

        public override async Task Execute()
        {
           

            while(Game.IsRunning)
            {   
                await Script.NextFrame();          
                
                #if false
                var deltaTime = (float)Game.UpdateTime.Elapsed.TotalSeconds;

                // Do stuff every new frame
                if (Input.HasKeyboard) {
                    if (Input.IsKeyDown(Keys.X)) {
                        Camera.Transform.Position += new Vector3(0,0.1f * deltaTime ,0);
                    }
                    if (Input.IsKeyDown(Keys.S)) {
                        Camera.Transform.Position += new Vector3(0,-0.1f * deltaTime ,0);
                    }
                    if (Input.IsKeyDown(Keys.A)) {
                        Camera.Transform.Position += new Vector3(0.1f * deltaTime ,0,0);
                    }
                    if (Input.IsKeyDown(Keys.D)) {
                        Camera.Transform.Position += new Vector3(0.1f * deltaTime ,0,0);
                    }
                   
                }
                #endif


                
            }
        }
    }
}

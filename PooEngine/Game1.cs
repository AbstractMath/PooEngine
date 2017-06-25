using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
/*
 * NOTICE: This engine needs a massive rewrite, heres a list of stuff that I need to fix:
 * I primarily need to fix the way VBOs are handled. 
 * Also I need to reorganize things. Like for example, I need to have stuff in separate files. That may help clean up the code.
 */

namespace PooEngine
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        Scene mainScene;
        Cube coob;
        Cube coob1;
        Chunk testChunk;
        Effect shader;
        VertexBuffer CubeBuffer;
        MouseState originalMouseState;
        float camPitch = 0;
        float camYaw = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1024;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            mainScene = new Scene(1024f / 720f);//The scene creates it's own camera. This will probably not last.
            testChunk = new Chunk(shader, GraphicsDevice);
            mainScene.addChild(testChunk);
            //testChunk.initializeBuffer(GraphicsDevice);
            //testChunk.updateVBO();
            //coob = new Cube(shader);
            //coob1 = new Cube(shader);
            //mainScene.addChild(coob);//Just add the new cube as a child of the scene. (This is not technically required just yet, but it will be) Also, in the future you will be able to just set the objects parent.
            //coob.addChild(coob1);
            //mainScene.addChild(coob1);
            //coob.Name = "Cube";
            //coob1.Name = "Cube1";
            //coob.initializeBuffer(GraphicsDevice);
            //coob1.initializeBuffer(GraphicsDevice);
            //CubeBuffer = coob.objBuffer;

            //coob1.position = new Vector3(-4, 0, 0);
            //coob.position = new Vector3(4, 0, 0);

            mainScene.mainCamera.position = new Vector3(0, 0, -4);

            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            originalMouseState = Mouse.GetState();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            shader = Content.Load<Effect>("BasicShader");//Load in the shader.
            testChunk.shader = shader;
            //coob.shader = shader;
            //coob1.shader = shader;
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            //Update the camera, if necessary.
            if (IsActive)//This causes the game to pause if it's not the active window.
            {
                MouseState currentMouseState = Mouse.GetState();
                if (currentMouseState != originalMouseState)
                {
                    float deltaX = currentMouseState.X - originalMouseState.X;
                    float deltaY = currentMouseState.Y - originalMouseState.Y;
                    //Do some math to get the delta pitch and delta yaw correct
                    float tanFov = 2 * (float)System.Math.Tan(mainScene.mainCamera.fieldOfView * 0.00872664625d);
                    float deltaPitch = (float)System.Math.Atan(tanFov * deltaY / GraphicsDevice.Viewport.Height / 2);
                    float deltaYaw = (float)System.Math.Atan(tanFov * deltaX / GraphicsDevice.Viewport.Width / 2);
                    camPitch = camPitch + deltaPitch;
                    camYaw = camYaw + deltaYaw;

                    mainScene.mainCamera.rotation = Matrix.CreateRotationY(camYaw) * Matrix.CreateRotationX(camPitch);

                    Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    int X, Y, Z;
                    X = -(int)mainScene.mainCamera.position.X;
                    Y = -(int)mainScene.mainCamera.position.Y;
                    Z = -(int)mainScene.mainCamera.position.Z;

                    for (int x = -3; x < 3; x++)
                    {
                        for (int y = -3; y < 3; y++)
                        {
                            for (int z = -3; z < 3; z++)
                            {
                                testChunk[X + x, Y + y, Z + z] = 0;
                            }
                        }
                    }
                    
                    testChunk.updateVBO();
                    testChunk.objBuffer = testChunk.getObjectBuffer(GraphicsDevice);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.A))//Movement stuff
                {
                    mainScene.mainCamera.position = mainScene.mainCamera.position + 0.05f * mainScene.mainCamera.getRightVector();
                }

                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    mainScene.mainCamera.position = mainScene.mainCamera.position - 0.05f * mainScene.mainCamera.getRightVector();
                }

                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    mainScene.mainCamera.position = mainScene.mainCamera.position + 0.05f * mainScene.mainCamera.getForwardVector();
                }

                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    mainScene.mainCamera.position = mainScene.mainCamera.position - 0.05f * mainScene.mainCamera.getForwardVector();
                }

                //coob.rotation = coob.rotation * Matrix.CreateFromAxisAngle(Vector3.Normalize(new Vector3(2, 3, -1)), MathHelper.ToRadians(2f));
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            mainScene.drawScene(GraphicsDevice);

            base.Draw(gameTime);
        }
    }
}

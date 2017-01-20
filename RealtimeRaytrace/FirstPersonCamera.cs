using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;


namespace RealtimeRaytrace
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class FirstPersonCamera : Game
    {
        WorldGrid _theEntireWorld = new WorldGrid(1);

        TimeSpan _lastShownFPS;
        //string _fpsString;
        float cycleRadians = 0;

        GraphicsDeviceManager _graphics;
        //SpriteFont _guiFont;
        //SpriteBatch _spriteBatch;

        VertexBuffer _vertexBuffer;
        IndexBuffer _indexBuffer;

        BasicEffect _basicEffect;
        VertexPositionColor[] _vertices;
        Vector2[] _orgVertices;
        int[] _indices;
        int _dist;

        Camera _camera = new Camera(0,new Vector3(0, 0, -3), Vector3.Backward);
        Camera _oldCameraForMovement;
        Vector2 _center;
        Random _rnd = new Random();

        public FirstPersonCamera()
            : base()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = false;
            //get user's primary screen size...
            //_ScreenWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            //_ScreenHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

            //make it full screen... (borderless if you want to is an option as well)
            this.Window.Position = new Point(0, 0);
            this.Window.IsBorderless = true;
            //graphics.PreferredBackBufferWidth = (int)_ScreenWidth;
            //graphics.PreferredBackBufferHeight = (int)_ScreenHeight;

            Content.RootDirectory = "Content";

            IsFixedTimeStep = false; // Setting this to true makes it fixed time step, false is variable time step.
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //_graphics.PreferredBackBufferHeight = 320;
            //_graphics.PreferredBackBufferWidth = 640;
            //_graphics.PreferredBackBufferHeight = 640;
            //_graphics.PreferredBackBufferWidth = 1024;
            //_graphics.PreferredBackBufferHeight = 720;
            //_graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 900;
            _graphics.PreferredBackBufferWidth = 1600;
            //_graphics.PreferredBackBufferHeight = 1050;
            //_graphics.PreferredBackBufferWidth = 1680;
            //_graphics.PreferredBackBufferHeight = 1080;
            //_graphics.PreferredBackBufferWidth = 1920;
#if DEBUG
            _graphics.IsFullScreen = false;
#else
             _graphics.IsFullScreen = true;
#endif
             this.Window.Position = new Point(0, 0);
             _graphics.ApplyChanges();
            _lastShownFPS = TimeSpan.FromSeconds(5);

            GraphicsDevice.Textures[0] = null;
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //_guiFont = Content.Load<SpriteFont>("guiFont");

            // Create a new SpriteBatch, which can be used to draw textures.
            // _spriteBatch = new SpriteBatch(GraphicsDevice);

            _basicEffect = new BasicEffect(GraphicsDevice);

            _center.X = GraphicsDevice.Viewport.Width * 0.5f;
            _center.Y = GraphicsDevice.Viewport.Height * 0.5f;

            //This will create the triangles used for drawing the screen
            TriangleProjectionGrid projGrid = new TriangleProjectionGrid(-_center.X, -_center.Y, _center.X, _center.Y, 3, 2);
            
            //TODO: Check if multiples ar valid
            //TODO: The TriangleHexagonRings should be dynamic to the resolution
            for(int i = 0;i < 900; i += 3)
                projGrid.MakeTriangleHexagonRing(i, 3);

            //for (int i = 40; i < 180; i += 2)
            //    projGrid.MakeTriangleHexagonRing(i, 2);

            //for (int i = 180; i < 120; i += 4)
            //    projGrid.MakeTriangleHexagonRing(i, 4);

            //for (int i = 120; i < 180; i += 6)
            //    projGrid.MakeTriangleHexagonRing(i, 6);

            //for (int i = 180; i < 260; i += 8)
            //    projGrid.MakeTriangleHexagonRing(i, 8);

            _vertices = projGrid.GetTriangleIndex().GetVerticesPositionColor();
            //test av hur det kan se ut
            _orgVertices = new Vector2[_vertices.Length];
            for (int i = 0; i < _vertices.Length; i++ )
               _orgVertices[i] = new Vector2( _vertices[i].Position.X,_vertices[i].Position.Y);

            _vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), _vertices.Length, BufferUsage.WriteOnly);
            _vertexBuffer.SetData<VertexPositionColor>(_vertices);


            _indices = projGrid.GetTriangleIndex().GetIndices();
            _indexBuffer = new IndexBuffer(GraphicsDevice, typeof(int), _indices.Length, BufferUsage.WriteOnly);
            _indexBuffer.SetData(_indices);

            _basicEffect.View = Matrix.CreateLookAt(new Vector3(0, 0, -1), new Vector3(0, 0, 0), Vector3.Down);
            _basicEffect.Projection = Matrix.CreateOrthographic(_center.X * 2, _center.Y * 2, 1, 2);

            Mouse.SetPosition((int)_center.X, (int)_center.Y);

            _theEntireWorld.CreateFlatWorld(40, 40, 40);

            //Not sure if this is wise but a lot of objects were created for building the scene 
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();
            //_spriteBatch.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            _camera.MoveDepth(GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y);
            _camera.MoveSide(GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X);
            _camera.RotateYaw(GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X * 0.2f);
            _camera.RotatePitch(GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y * -0.2f);
            
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                _camera.MoveDepth(0.3f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                _camera.MoveDepth(-0.3f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                _camera.MoveSide(-0.3f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                _camera.MoveSide(0.3f);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                _camera.MoveHeight(0.3f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.C))
            {
                _camera.MoveHeight(-0.3f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.T))
            {
                _graphics.ToggleFullScreen();
                _graphics.ApplyChanges();
            }
#if !DEBUG 
            if (Mouse.GetState().X - (int)_center.X != 0)
            {
                _camera.RotateYaw((Mouse.GetState().X - (int)_center.X) * 0.0002f);
            }
            if (Mouse.GetState().Y - (int)_center.Y != 0)
            {
                _camera.RotatePitch((Mouse.GetState().Y - (int)_center.Y) * 0.0002f);
            }

                if(this.Window != null)
                    Mouse.SetPosition((int)_center.X, (int)_center.Y);
#else

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                _camera.RotateYaw(-0.02f);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                _camera.RotateYaw(0.02f);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                _camera.RotatePitch(-0.02f);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                _camera.RotatePitch(0.02f);
            }

#endif
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            DateTime startTime = DateTime.Now;

            cycleRadians += (float)gameTime.ElapsedGameTime.TotalSeconds /4;

            //TODO: lägg till fler ljuskällor
            Vector3 lightsource = new Vector3(40 * (float)Math.Cos(cycleRadians), 40, 40 * (float)Math.Sin(cycleRadians));
            //ange styrka på ljus
            float lightsourceIntensity = 1450;
            float ambientIntensity = 0.25f;

            for (int l = 0; l < _vertices.Length; l++)
            {
                //Test hur det skulle kunna se ut, positionen kanske inte ska ändra sig bara rayen...
                //Positionen är svår att ändra när trianglarna är av olika storlek. Då de vertex som ligger på en större 
                //triangels långsida måste ta med i beräkningen om den större triangelns vertex har ändrats
                //if (l > 500)
                {
                    _dist = 2;
                    _vertices[l].Position.X = _orgVertices[l].X + _rnd.Next(_dist * 2) - _dist;
                    _vertices[l].Position.Y = _orgVertices[l].Y + _rnd.Next(_dist * 2) - _dist;
                }
                //Ray ray = new Ray(_camera, _vertices[l].Position.X + _rnd.Next(5) - 10, _vertices[l].Position.Y + _rnd.Next(5) - 10);
                Ray ray = new Ray(_camera, _vertices[l].Position.X, _vertices[l].Position.Y);
                Color pixel = Color.Black;
                //TODO: Antialiasing (kan vara sista steget. Gå i genom alla _vertices och jämna ut färgerna. Kan vara svårt att få till eftersom närliggande(x och y) vertices kan ligga långt ifrån varandra i _vertices. Kanske varje varvs start-index kan sparas undan för att användas till detta?)
                //TODO: Eftersläpa färgen på de punkter som har lägre detaljnivå
                //TODO: Skicka med en parameter för detaljnivå (kan användas för att strunta i att göra intersection och bara anta att träff gjordes på de spheres som finns i griden och mixa färgen från de som finns)
                //TODO: Ändra positionerna för _vertices x och y random för varje frame (då blir bilden inte så kantig och statisk)
                Intersection closestIntersection = _theEntireWorld.GetClosestIntersection(ray,200);

                if (closestIntersection.IsHit())
                {
                    //TODO: Lägg till Phong-shading
                    //TODO: Lägg till skugg-kontroll (finns en intersection emellan?)
                    Vector3 direction = lightsource - closestIntersection.GetPosition();
                    float distance = (direction).Length();
                    float factor = Vector3.Dot(closestIntersection.GetNormal(), Vector3.Normalize(direction));
                    //Använd avstånd för att minska ljuset
                    factor *= (float)(lightsourceIntensity / Math.Pow(distance, 2));
                    factor += ambientIntensity;
                    //sätt gränserna till mellan 0 och 1
                    factor = (factor < 0) ? 0 : ((factor > 1) ? 1 : factor);

                    pixel = Color.Multiply(closestIntersection.GetSphere().GetColor(), factor);
                }
                else
                    pixel = Color.Black;

                //Använd cache av första rayen för att se hur mycket som kameran har flyttats eller roterats från förra gången för att avgöra hur 
                //mycket som ska blandas av föregående färg
                float afterBurnFactor = 0.05f + (_vertices[l].Position.Length() * 1.5f) / _graphics.PreferredBackBufferWidth;


                //if(_vertices[l].Color != Color.Black)
                    _vertices[l].Color = Color.Lerp(pixel, _vertices[l].Color, afterBurnFactor);
                //else
                //    _vertices[l].Color = Color.Lerp(pixel, _vertices[l].Color, 0.1f);

                ////TODO: Testar att hålla fps uppe - ändra till något bättre senare
                //if ((DateTime.Now - startTime).TotalMilliseconds > 100)
                //    break;
            }
            _vertexBuffer.SetData<VertexPositionColor>(_vertices);
            _indexBuffer.SetData(_indices);

            _basicEffect.VertexColorEnabled = true;

            GraphicsDevice.SetVertexBuffer(_vertexBuffer);
            GraphicsDevice.Indices = _indexBuffer;

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _vertices.Length, 0, _indices.Length);
            }


            //FPS #################
            //_lastShownFPS = _lastShownFPS.Add(gameTime.ElapsedGameTime);

            //if (_lastShownFPS.TotalSeconds > 1.0)
            //{
            //    _lastShownFPS = TimeSpan.Zero;
            //    _fpsString = string.Format("FPS: {0:F0} Cam: {1}", (1.0 / gameTime.ElapsedGameTime.TotalSeconds), _camera.GetPosition().ToString());
            //}
            //_spriteBatch.Begin();
            //_spriteBatch.DrawString(_guiFont, _fpsString, new Vector2(10, 10), Color.LightGray);
            //_spriteBatch.End();
            //FPS #################

        }
    }
}

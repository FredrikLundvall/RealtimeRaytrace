using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;


namespace RealtimeRaytrace
{
    //Many objects are reused to not trigger the garbagecollection, TODO: consider static pools for some objects (the command for example)...
    public class GameLoop : Game
    {
        GraphicsDeviceManager _graphicsDeviceManager;
        WorldGrid _theEntireWorld;
        IRenderer _renderer;
        ITextRenderer _textRenderer;
        Player _playerOne;
        IInputHandler _inputHandler;
        Queue<IPlayerCommand> _playerCommandQueue;
        //_graphics.PreferredBackBufferHeight = 320;
        //_graphics.PreferredBackBufferWidth = 640;
        //_graphics.PreferredBackBufferHeight = 640;
        //_graphics.PreferredBackBufferWidth = 1024;
        //_graphics.PreferredBackBufferHeight = 720;
        //_graphics.PreferredBackBufferWidth = 1280;
        //_graphics.PreferredBackBufferHeight = 900;
        //_graphics.PreferredBackBufferWidth = 1600;
        //_graphics.PreferredBackBufferHeight = 1050;
        //_graphics.PreferredBackBufferWidth = 1680;
        //_graphics.PreferredBackBufferHeight = 1080;
        //_graphics.PreferredBackBufferWidth = 1920;
        int _screenWidth = 640;
        int _screenHeight = 320;
        bool _wasInactive = true;

        public GameLoop() : base()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            _theEntireWorld = new WorldGrid();

            //make it full screen... (borderless if you want to is an option as well)

#if DEBUG
            _graphicsDeviceManager.IsFullScreen = false;
                this.Window.Position = new Point(0, 0);
                this.Window.IsBorderless = false;
#else
                _graphicsDeviceManager.IsFullScreen = true;
                this.Window.Position = new Point(0, 0);
                this.Window.IsBorderless = true;
            #endif

            Content.RootDirectory = "Content";

            IsFixedTimeStep = false; // Setting this to true makes it fixed time step, false is variable time step.
        }

        protected override void Initialize()
        {           
            base.Initialize();

#if DEBUG
                _playerOne = new Player(_renderer.MainCamera,false);
#else
            _playerOne = new Player(_renderer.MainCamera, true);
            #endif
            _playerCommandQueue = new Queue<IPlayerCommand>();


            //#if !DEBUG
            _inputHandler = new MouseKeybordInputHandler(_screenWidth / 2, _screenHeight / 2);
            //#else
            //_inputHandler = new GamePadInputHandler(PlayerIndex.One);
            //#endif
        }

        protected override void LoadContent()
        {
            _theEntireWorld.CreateCubeWorld(40, 40, 40);
            _renderer = new TriangleRaytraceRenderer(_graphicsDeviceManager, _theEntireWorld, _screenWidth, _screenHeight,
            //new SkyHemisphereTexture(_graphicsDeviceManager, @"Content\skymap_photo8.jpg", HemisphereTextureType.Panorama, false, false, 0.1)
            //new SkyHemisphereTexture(_graphicsDeviceManager, @"Content\mosriver_fisheye220_4k.jpg", HemisphereTextureType.FisheyeVertical,false,false,0.18)
            //new SkySphereTexture(_graphicsDeviceManager, @"Content\mountain.jpg", SphereTextureType.Photo360)
            //new SkySphereTexture(_graphicsDeviceManager, @"Content\lobby.jpg", SphereTextureType.Photo360)
            //new SkySphereTexture(_graphicsDeviceManager, @"Content\angmap23.jpg", SphereTextureType.FisheyeHorizontal)
            //new SkyBoxTexture(_graphicsDeviceManager, @"Content\grimmnight_large.jpg")
            new SkyBoxTexture(_graphicsDeviceManager, @"Content\negz.jpg", @"Content\posz.jpg", @"Content\posy.jpg", @"Content\negy.jpg", @"Content\negx.jpg", @"Content\posx.jpg") 
            );
            _textRenderer = new TrueTypeSharpTextRenderer(_graphicsDeviceManager, @"Content\Anonymous Pro.ttf", _screenHeight / 15);

            //Not sure if this is wise, but a lot of objects was created when building the scene 
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if(this.IsActive)
            {
                if (_wasInactive)
                {
                    _inputHandler.InitiateInput();
                }
                _wasInactive = false;
                _inputHandler.HandleInput(_playerCommandQueue);
            }
            else
            {
                _wasInactive = true;
            }

            while (_playerCommandQueue.Count > 0)
            {
                _playerCommandQueue.Dequeue().Execute(_playerOne, (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (_playerOne.HasFullscreen() != _graphicsDeviceManager.IsFullScreen)
            {
                _graphicsDeviceManager.ToggleFullScreen();
                _graphicsDeviceManager.ApplyChanges();
            }

            if (_playerOne.HasQuit() || Keyboard.GetState().IsKeyDown(Keys.Escape) || GamePad.GetState(0).IsButtonDown(Buttons.Start))
                Exit();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (this.IsActive)
            {
                _renderer.Render(gameTime);
                _textRenderer.SetText(0, "FPS: " + ((int)(1 / gameTime.ElapsedGameTime.TotalSeconds)), new Vector2(5, _screenHeight - 5), Color.CornflowerBlue);
                _textRenderer.Render(gameTime);
            }
            base.Draw(gameTime);
        }
    }
}

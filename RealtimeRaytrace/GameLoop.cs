﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;


namespace RealtimeRaytrace
{
    //Many objects are reused to not trigger the garbagecollection, TODO: consider static pools for some objects (the command for example)...
    public class GameLoop : Game
    {
        GraphicsDeviceManager _graphics;
        WorldGrid _theEntireWorld = new WorldGrid();
        IRenderer _renderer;
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
        bool _wasInactive = false;

        public GameLoop() : base()
        {
            _graphics = new GraphicsDeviceManager(this);
            //make it full screen... (borderless if you want to is an option as well)
            _graphics.IsFullScreen = true;
            this.Window.Position = new Point(0, 0);
            this.Window.IsBorderless = true;

            Content.RootDirectory = "Content";

            IsFixedTimeStep = false; // Setting this to true makes it fixed time step, false is variable time step.
        }

        protected override void Initialize()
        {           
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _theEntireWorld.CreateCubeWorld(40, 40, 40);
            _renderer = new TriangleRaytraceRenderer(_theEntireWorld,_graphics, _screenWidth, _screenHeight);

            _playerOne = new Player(_renderer.MainCamera);
            _playerCommandQueue = new Queue<IPlayerCommand>();

            //#if !DEBUG
            _inputHandler = new MouseKeybordInputHandler(_screenWidth / 2, _screenHeight / 2);
            //#else
            //_inputHandler = new GamePadInputHandler(PlayerIndex.One);
            //#endif

            //Not sure if this is wise, but a lot of objects was created when building the scene 
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            _inputHandler.InitiateInput();
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
            if (_playerOne.HasFullscreen() != _graphics.IsFullScreen)
            {
                _graphics.ToggleFullScreen();
                _graphics.ApplyChanges();
            }

            if (_playerOne.HasQuit() || Keyboard.GetState().IsKeyDown(Keys.Escape) || GamePad.GetState(0).IsButtonDown(Buttons.Start))
                Exit();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            if (this.IsActive)
            {
                _renderer.Render(gameTime);
            }
        }
    }
}

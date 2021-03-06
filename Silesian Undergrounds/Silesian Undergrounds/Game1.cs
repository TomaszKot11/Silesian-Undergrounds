﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Silesian_Undergrounds.Engine.Scene;
using Silesian_Undergrounds.Engine.Utils;
using Silesian_Undergrounds.Engine.Enum;
using Silesian_Undergrounds.Views;
using System;
using System.Collections.Generic;
using Silesian_Undergrounds.Engine.Config;

namespace Silesian_Undergrounds
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public List<String> scenes = new List<String>();
        public int levelCounter = 0;
        public bool isPlayerInMaineMenu = true;

        Scene loadingScene;
        SceneStatusEnum sceneStatus = SceneStatusEnum.Loading;

        Scene scene;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            #region LOAD_CONFIG
            ConfigMgr.LoadConfig();
            #endregion
            #region GRAPHIC_SETTINGS_INIT
            // Window.AllowAltF4 = true;
            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            ResolutionMgr.GameWidth = graphics.PreferredBackBufferWidth;
            graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            ResolutionMgr.GameHeight = graphics.PreferredBackBufferHeight;
            //graphics.ToggleFullScreen();
            graphics.ApplyChanges();

            // Calculate inner unit value
            ResolutionMgr.yAxisUnit = ResolutionMgr.GameHeight / 100.0f;
            ResolutionMgr.xAxisUnit = ResolutionMgr.GameWidth / 100.0f;
            #endregion

            scenes.Add("level_1");
            scenes.Add("level_2");
            scenes.Add("level_3");
            scenes.Add("t");
            scenes.Add("drop");
            scenes.Add("drop2");
            //scenes.Add("drop3");

            TextureMgr.Instance.SetCurrentContentMgr(Content);
            FontMgr.Instance.SetCurrentContentMgr(Content);
            SoundMgr.Instance.SetCurrentContentMgr(Content);

            loadingScene = new Scene(new LoadingView(), true);

            scene = SetMainMenuScene();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Drawer.Initialize(spriteBatch, Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Input.Update();

            if (!scene.isEnd)
                scene.Update(gameTime);
            else
            {
                if(sceneStatus == SceneStatusEnum.Loading)
                {
                    scene = loadingScene;
                    sceneStatus = SceneStatusEnum.Loaded;
                }
                else
                {
                    scene = LevelsManagement();
                    sceneStatus = SceneStatusEnum.Loading;
                }
            }
            // play all enqueued soundeffects
            AudioPlayerMgr.Instance.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Drawer.UpdateGameTime(gameTime);
            GraphicsDevice.Clear(Color.Black);

            scene.Draw();

            base.Draw(gameTime);
        }

        protected Scene LevelsManagement()
        {
            var sceneName = scenes[levelCounter];
            #if DEBUG
            System.Diagnostics.Debug.WriteLine("Current scene: " + sceneName);
            #endif
            levelCounter++;
            Scene sceneToLoad;
            if(levelCounter == scenes.Count)
            {
                sceneToLoad = SceneManager.LoadScene(sceneName, 64);
                sceneToLoad.SetLastScene(true);
                sceneToLoad.SetOnWin(EndGamePlayerWin);
            }
            else
                sceneToLoad = SceneManager.LoadScene(sceneName, 64);

            sceneToLoad.player.SetOnDeath(EndGamePlayerDie);
            sceneToLoad.SetEndGameButtonInPauseMenu(ReturnToMenu);
            if(levelCounter > 1)
                sceneToLoad.DecreaseHungerDropInterval();

            return sceneToLoad;
        }

        protected bool StartGame()
        {
            scene = LevelsManagement();
           
            // start playing the music
            AudioPlayerMgr.Instance.PlayBackgroundMusic("Music/background-game/background_game");
            return true;
        }

        protected bool ExitGame()
        {
            this.Exit();
            return true;
        }

        protected bool EndGamePlayerDie()
        {
            this.scene = SetEndGameScene(EndGameEnum.Lost);
            return true;
        }

        protected bool EndGamePlayerWin()
        {
            this.scene = SetEndGameScene(EndGameEnum.Win);
            return true;
        }

        protected bool StartView()
        {
            this.scene = SetStartView();
            return true;
        }

        protected bool ControlsView()
        {
            this.scene = SetControlsView();
            return true;

        }

        protected bool ReturnToMenu()
        {
            levelCounter = 0;
            SceneManager.ClearPlayerStatistics();
            this.scene = SetMainMenuScene();
            return true;
        }

        protected Scene SetMainMenuScene()
        {
            MainMenuView mainMenu = new MainMenuView();
            AudioPlayerMgr.Instance.PlayBackgroundMusic("Music/menu/menu_theme");
            mainMenu.GetStartGameButton().SetOnClick(StartView);
            mainMenu.GetExitButton().SetOnClick(ExitGame);
            return new Scene(mainMenu);
        }

        protected Scene SetStartView()
        {
            StartView startView = new StartView();
            startView.GetReadyButton().SetOnClick(StartGame);
            startView.GetControlsButton().SetOnClick(ControlsView);
            return new Scene(startView);
        }

        protected Scene SetControlsView()
        {
            ControlsDisplayView controlsDisplayView = new ControlsDisplayView();
            controlsDisplayView.GetNextButton().SetOnClick(StartGame);
            return new Scene(controlsDisplayView);
        }

        protected Scene SetEndGameScene(EndGameEnum endGameEnum)
        {

            if(endGameEnum == EndGameEnum.Lost)
            {
                PlayerDieView endGameWhenPlayerDie = new PlayerDieView();
                endGameWhenPlayerDie.GetReturnToMenuButton().SetOnClick(ReturnToMenu);
                return new Scene(endGameWhenPlayerDie);
            }
            else
            {
                PlayerWinView endGameWhenPlayerWin = new PlayerWinView();
                endGameWhenPlayerWin.GetReturnToMenuButton().SetOnClick(ReturnToMenu);
                return new Scene(endGameWhenPlayerWin);
            }

        }
    }
}

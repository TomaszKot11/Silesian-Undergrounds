﻿using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Silesian_Undergrounds.Engine.Common;
using Silesian_Undergrounds.Engine.Utils;
using Silesian_Undergrounds.Engine.UI;
using Silesian_Undergrounds.Views;
using Silesian_Undergrounds.Engine.Collisions;
using Silesian_Undergrounds.Engine.Enum;
using System.Diagnostics;
using Silesian_Undergrounds.Engine.CommonF;

namespace Silesian_Undergrounds.Engine.Scene
{
    public class Scene
    {

        #region SCENE_VARIABLES
        private List<GameObject> gameObjects;
        private List<GameObject> objectsToDelete;
        private List<GameObject> objectsToAdd;
        private List<GameObject> transitions;

        public Player player;
        private UIArea ui;
        private UIArea pauseMenu;
        public Camera camera { get; private set; }

        public bool isPaused { get; private set; }
        public bool isEnd { get; private set; }
        private readonly bool canUnPause;

        #endregion

        public Scene(PlayerStatistic playerStatistic)
        {
            CollisionSystem.CleanUp();

            gameObjects = new List<GameObject>();
            objectsToDelete = new List<GameObject>();
            objectsToAdd = new List<GameObject>();
            transitions = new List<GameObject>();
            isPaused = false;
            player = new Player(new Vector2(200, 200), new Vector2(ResolutionMgr.TileSize, ResolutionMgr.TileSize), 1, new Vector2(2.5f, 2.5f), playerStatistic);

            TextureMgr.Instance.LoadIfNeeded("minerCharacter");
            player.texture = TextureMgr.Instance.GetTexture("minerCharacter");
            player.Initialize();
            gameObjects.Add(player);

            camera = new Camera(player);
            ui = new InGameUI(player);
            pauseMenu = new UIArea(); // TEMP SET EMPTY PAUSE MENU
            canUnPause = true;
            // TEST CODE
            gameObjects.Add(EnemyFactory.TestEnemyFactory());
        }


        public Scene(UIArea area)
        {
            pauseMenu = ui;
            isPaused = true;
            canUnPause = false;
        }

        #region SCENE_OBJECTS_MANAGMENT_METHODS

        public void AddTransition(GameObject obj)
        {
            transitions.Add(obj);
            objectsToAdd.Add(obj);
        }

        public void AddObject(GameObject obj)
        {
            objectsToAdd.Add(obj);
        }

        public void DeleteObject(GameObject obj)
        {
            objectsToDelete.Add(obj);
        }

        private void AddObjects()
        {
            foreach (var obj in objectsToAdd)
                gameObjects.Add(obj);

            objectsToAdd.Clear();
        }

        private void DeleteObjects()
        {
            foreach (var obj in objectsToDelete)
            {
                obj.RemoveAllComponents();
                gameObjects.Remove(obj);
            }  

            objectsToDelete.Clear();
        }

        public List<GameObject> GameObjects
        {
            get { return gameObjects; }
        }

        #endregion

        public void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                if (isPaused && canUnPause)
                    isPaused = false;
                else
                    isPaused = true;
            }

            if (isPaused)
            {
                pauseMenu.Update(gameTime);
                return;
            }
            
            // Operation of add or remove from gameObjects list has to appear before updating gameObjects
            AddObjects();
            DeleteObjects();

            foreach (var obj in gameObjects)
                obj.Update(gameTime);

            camera.Update(gameTime);

            ui.Update(gameTime);

            DetectPlayerOnTransition();
        }

        public void Draw()
        {
            Drawer.Shaders.DrawGrayScaleEffect((spriteBatch, gameTime) =>
            {
                foreach (var obj in gameObjects)
                {
                    if (obj is Player)
                        continue;

                    if (obj.layer != 3)
                        obj.Draw(spriteBatch);
                }
            }, transformMatrix: camera.Transform);

            Drawer.Draw((spriteBatch, gameTime) =>
            {
                foreach (var obj in gameObjects)
                {
                    if (obj is Player)
                        continue;
                    obj.Draw(spriteBatch);
                }
            }, transformMatrix: camera.Transform);

            Drawer.Shaders.DrawShadowEffect((spriteBatch, gameTime) =>
            {
                foreach (var obj in gameObjects)
                    if (obj.layer == 3)
                        obj.Draw(spriteBatch);
            }, transformMatrix: camera.Transform, lightSource: player.position);
            Drawer.Draw((spriteBatch, gameTime) =>
            {
                player.Draw(spriteBatch);
                // VERY VERY TEST CODE
                //gameObjects[1].Draw(spriteBatch);
            }, transformMatrix: camera.Transform);
            Drawer.Draw((spriteBatch, gameTime) =>
            {
                if (isPaused)
                    pauseMenu.Draw(spriteBatch);
                else
                    ui.Draw(spriteBatch);
            }, null);


            // Draw bright effect for shop items
            //NOTE: moving this lines above causes bug!!!
            Drawer.Shaders.DrawBrightShader((spriteBatch, gameTime) =>
            {
                foreach (var obj in gameObjects)
                {
                    if (obj.layer == (int)LayerEnum.ShopPickables)
                    {
                        obj.Draw(spriteBatch);
                        //obj.color = Color.MediumVioletRed;
                    }

                }

            }, transformMatrix: camera.Transform);

        }

        private void DetectPlayerOnTransition()
        {
            foreach (var transition in transitions)
                if (player.GetTileWhereStanding() == transition.GetTileWhereStanding())
                {
                    foreach (var obj in gameObjects)
                        DeleteObject(obj);

                    foreach (var obj in transitions)
                        DeleteObject(obj);

                    foreach (var obj in objectsToAdd)
                        DeleteObject(obj);

                    DeleteObjects();
                    isEnd = true;
                }
        }
    }
}


﻿using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Silesian_Undergrounds.Engine.Common;
using Silesian_Undergrounds.Engine.Utils;

namespace Silesian_Undergrounds.Engine.Scene
{
    public class Drawer
    {
        private Drawer() {}
        private static SpriteBatch _spriteBatch;
        private static GameTime _gameTime;

        public static class Shaders
        {
            private static Effect _shadowEffect, _grayScaleEffect, _visibilityRadiusShader, _brightEffect, _boosterPickupShader, _shopNeonEffect, _noiseEffect;
            private static Texture2D _brightningTexture, _rainbow;

            public static void DrawShopNeonShader(Action<SpriteBatch, GameTime> drawer, Matrix? transformMatrix = null)
            {
                _spriteBatch.Begin(transformMatrix: transformMatrix, effect: _shopNeonEffect);
                drawer.Invoke(_spriteBatch, _gameTime);
                _spriteBatch.End();
            }

            public static void DrawFoggEffect(Action<SpriteBatch, GameTime> drawer, Matrix? transformMatrix = null)
            {
                _spriteBatch.Begin(SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend, transformMatrix: transformMatrix, effect: _noiseEffect);
                _noiseEffect.Parameters["rnd_time"].SetValue(_gameTime.TotalGameTime.Seconds);
                drawer.Invoke(_spriteBatch, _gameTime);
                _spriteBatch.End();
            }

            public static void DrawBrightShader(Action<SpriteBatch, GameTime> drawer, Matrix? transformMatrix = null)
            {
                _spriteBatch.Begin(SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend, transformMatrix: transformMatrix, effect: _brightEffect);
                _brightEffect.Parameters["lightRaysMask"].SetValue(_brightningTexture);
                drawer.Invoke(_spriteBatch, _gameTime);
                _spriteBatch.End();
            }

            public static void DrawVisibilityRadiusShader(Action<SpriteBatch, GameTime> drawer, Vector2 lightSource, Matrix? transformMatrix = null)
            {
                _visibilityRadiusShader.Parameters["lightSource"].SetValue(new Vector2(960, 540));
                _visibilityRadiusShader.Parameters["gameTime"].SetValue(_gameTime.TotalGameTime.Seconds);
                _spriteBatch.Begin(transformMatrix: transformMatrix, effect: _visibilityRadiusShader);
                drawer.Invoke(_spriteBatch, _gameTime);
                _spriteBatch.End();
            }

            public static void DrawShadowEffect(Action<SpriteBatch, GameTime> drawer, Vector2 lightSource, Matrix? transformMatrix = null)
            {
                _spriteBatch.Begin(blendState: BlendState.AlphaBlend, transformMatrix: transformMatrix, effect: _shadowEffect);
                drawer.Invoke(_spriteBatch, _gameTime);
                _spriteBatch.End();
            }

            public static void DrawGrayScaleEffect(Action<SpriteBatch, GameTime> drawer, Matrix? transformMatrix = null)
            {
                _spriteBatch.Begin(transformMatrix: transformMatrix, effect: _grayScaleEffect);
                drawer.Invoke(_spriteBatch, _gameTime);
                _spriteBatch.End();
            }

            public static void DrawBoosterPickupShader(Action<SpriteBatch, GameTime> drawer, Matrix? transformMatrix = null)
            {
                _boosterPickupShader.Parameters["gameTime"].SetValue(_gameTime.TotalGameTime.Milliseconds);
                _boosterPickupShader.Parameters["rainbow"].SetValue(_rainbow);
                _spriteBatch.Begin(transformMatrix: transformMatrix, effect: _boosterPickupShader);
                drawer.Invoke(_spriteBatch, _gameTime);
                _spriteBatch.End();
            }

            internal static void LoadShaders(ContentManager content)
            {
                _shadowEffect = content.Load<Effect>("ShadowShader");
                _grayScaleEffect = content.Load<Effect>("GrayScaleShader");
                _visibilityRadiusShader = content.Load<Effect>("VisibilityRadiusShader");
                _brightningTexture = TextureMgr.Instance.GetTexture("rays_map");
                _brightEffect = content.Load<Effect>("BrighteningShader");
                _rainbow = TextureMgr.Instance.GetTexture("rainbow");
                _boosterPickupShader = content.Load<Effect>("BoosterPickupShader");
                _shopNeonEffect = content.Load<Effect>("ShopNeonShader");
                _noiseEffect = content.Load<Effect>("NoiseShader");
            }

        }

        public static void Draw(Action<SpriteBatch, GameTime> drawer, Matrix? transformMatrix = null)
        {
            _spriteBatch.Begin(transformMatrix: transformMatrix);
            drawer.Invoke(_spriteBatch, _gameTime);
            _spriteBatch.End();
        }

        public static void UpdateGameTime(GameTime gameTime)
        {
            _gameTime = gameTime;
        }

        public static void Initialize(SpriteBatch spriteBatch, ContentManager content)
        {
            _spriteBatch = spriteBatch;
            Shaders.LoadShaders(content);
        }
    }
}
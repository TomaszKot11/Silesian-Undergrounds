﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Silesian_Undergrounds.Engine.Common;
using Silesian_Undergrounds.Engine.Utils;

namespace Silesian_Undergrounds.Engine.Components
{
    public class Animator : IComponent
    {
        private struct AnimationData
        {
            internal string animationName;
            internal int duration;
            internal bool isRepeatable;
            internal bool setFirstFrameAsTexture;
            internal bool isPermanent;
            internal List<Texture2D> textures;
            internal List<int> timestamps;
        }

        // IComponent inherited
        public Vector2 Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Rectangle Rect { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public GameObject Parent { get; private set; }
        // Animator variables
        private Dictionary<string, AnimationData> animations;
        private AnimationData currentAnimation;
        private TimedEventsScheduler eventsScheduler;
        private Texture2D orginalTexture;
        private Texture2D textureToDraw;

        public bool isPlaying { get; private set; }

        public event EventHandler<string> OnAnimationEnd = delegate { };

        public Animator(GameObject parent)
        {
            animations = new Dictionary<string, AnimationData>();
            eventsScheduler = new TimedEventsScheduler();

            Parent = parent;
            orginalTexture = parent.texture;
            textureToDraw = parent.texture;
        }

        public void CleanUp()
        {
            Parent = null;
            orginalTexture = null;
            textureToDraw = null;
            animations.Clear();
        }

        public void RegisterSelf() { }
        public void UnRegisterSelf() { }

        public void Update(GameTime gameTime)
        {
            eventsScheduler.Update(gameTime);
        }

        public void Draw(SpriteBatch batch)
        {
            // @TODO: remove this haxfix, made in order to render enemy units
            int layer = Parent.layer == 6 ? 1 : Parent.layer;
            batch.Draw(texture: textureToDraw, destinationRectangle: Parent.Rectangle, sourceRectangle: textureToDraw.Bounds, scale: Parent.scale, color: Parent.color, layerDepth: layer);
        }

        public void AddAndPlayAnimation(string name, List<Texture2D> textures, int animDuration, bool repeatable = false, bool useFirstFrameAsTexture = false, bool isPermanent = false)
        {
            this.AddAnimation(name, textures, animDuration, repeatable, useFirstFrameAsTexture, isPermanent);
            this.PlayAnimation(name);
        }

        public void AddAndPlayAnimation(AnimationConfig animationConfig)
        {
            this.AddAnimation(animationConfig.Name, animationConfig.Textures, animationConfig.AnimDuration, animationConfig.Repeatable, animationConfig.UseFirstFrameAsTexture, animationConfig.IsPermanent);
            this.PlayAnimation(animationConfig.Name);
        }

        public void AddAnimation(AnimationConfig animationConfig) =>
          this.AddAnimation(animationConfig.Name, animationConfig.Textures, animationConfig.AnimDuration, animationConfig.Repeatable, animationConfig.UseFirstFrameAsTexture, animationConfig.IsPermanent);
        public void AddAnimation(string name, List<Texture2D> textures, int animDuration, bool repeatable = false, bool useFirstFrameAsTexture = false, bool isPermanent = false)
        {
            if (animations.ContainsKey(name))
                return;

            AnimationData animData = new AnimationData();
            animData.animationName = name;
            animData.duration = animDuration;
            animData.textures = textures;
            animData.timestamps = new List<int>(textures.Count);
            int frameLenght = animDuration / textures.Count;

            for (int i = 0; i < textures.Count; ++i)
                animData.timestamps.Add(frameLenght);

            animData.isRepeatable = repeatable;
            animData.isPermanent = isPermanent;
            animData.setFirstFrameAsTexture = useFirstFrameAsTexture;
            animations.Add(name, animData);
        }

        public void AddAnimation(string name, List<Texture2D> textures, int animDuration, List<int> timestamps, bool repeatable = false, bool useFirstFrameAsTexture = false)
        {
            if (animations.ContainsKey(name))
                return;

            // if amount of timestamps is less than animation frames then fill it with remaining time
            if (timestamps.Count < textures.Count)
            {
                int missingAmount = textures.Count - timestamps.Count;
                int alreadyAssignedTime = 0;
                foreach (var timestamp in timestamps)
                    alreadyAssignedTime += timestamp;

                int possibleToUse = animDuration - alreadyAssignedTime;
                int time = 0;
                if (possibleToUse > 0)
                    time = possibleToUse / missingAmount;

                for (int i = 0; i < missingAmount; ++i)
                    timestamps.Add(time);
            }

            AnimationData animData = new AnimationData();
            animData.animationName = name;
            animData.duration = animDuration;
            animData.textures = textures;
            animData.timestamps = timestamps;
            animData.isRepeatable = repeatable;
            animData.setFirstFrameAsTexture = useFirstFrameAsTexture;

            animations.Add(name, animData);
        }

        public bool PlayAnimation(string name)
        {

            // Do not play new animation if previous was a permanent one
            if (currentAnimation.isPermanent)
                return false;

            if (!animations.ContainsKey(name))
                return false;

            // Do not play same animation as this one which is currently played
            if (name == currentAnimation.animationName)
                return false;


            // stop current animation if there is any
            StopAnimation();

            currentAnimation = animations[name];
            int max = currentAnimation.textures.Count;
            int time = 0;
            this.isPlaying = true;

            // Schedule events to chage texture to draw based on animation frames
            for (int i = 0; i < max; ++i)
            {
                if (i == 0)
                    textureToDraw = currentAnimation.textures[0];

                time += currentAnimation.timestamps[i];
                ScheduleAnimationFrame(i, time, max);
            }

            return true;
        }

        private void ScheduleAnimationFrame(int i, int time, int max)
        {
            eventsScheduler.ScheduleEvent(time, false, () =>
            {
                int index = i + 1;

                if (index < max)
                    textureToDraw = currentAnimation.textures[index];
                else
                    CallAnimationEnd();
            });
        }

        // Stop playing current animation if there is any
        public void StopAnimation()
        {
            isPlaying = false;
            eventsScheduler.ClearAll();
            currentAnimation = new AnimationData();
        }

        // Returns total duration of currently played animation
        // returns 0 if there is no animation played
        public int GetCurrentAnimationLenght()
        {
            return currentAnimation.duration;
        }

        private void CallAnimationEnd()
        {
            if (currentAnimation.setFirstFrameAsTexture && !currentAnimation.isRepeatable)
                textureToDraw = currentAnimation.textures[0];

            this.isPlaying = false;
            OnAnimationEnd.Invoke(this, currentAnimation.animationName);

            if (currentAnimation.isRepeatable)
            {
                string animName = currentAnimation.animationName;
                currentAnimation = new AnimationData();
                PlayAnimation(animName);
            }
            else
                currentAnimation = new AnimationData();
        }
    }
}

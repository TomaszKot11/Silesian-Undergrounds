﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using Silesian_Undergrounds.Engine.Scene;
using Silesian_Undergrounds.Engine.Collisions;

namespace Silesian_Undergrounds.Engine.Common
{
    public class Player : AnimatedGameObject
    {
        public event EventHandler<PropertyChangedArgs<int>> MoneyChangeEvent = delegate { };
        public event EventHandler<PropertyChangedArgs<int>> KeyChangeEvent = delegate { };
        public event EventHandler<PropertyChangedArgs<int>> HungerChangeEvent = delegate { };
        public event EventHandler<PropertyChangedArgs<int>> LiveChangeEvent = delegate { };
        public event EventHandler<PropertyChangedArgs<int>> HungerMaxValueChangeEvent = delegate { };
        public event EventHandler<PropertyChangedArgs<int>> LiveMaxValueChangeEvent = delegate { };

        // determines if the player is in 'attacking' mode (now just digging)
        bool attacking = false;

        private int moneyAmount;
        private int keyAmount;
        private int hungerValue;

        private int maxHungerValue;

        private int HUNGER_DECREASE_INTERVAL_IN_SECONDS = 10;
        private int HUNGER_DECREASE_VALUE = 5;
        private const int LIVE_DECREASE_VALUE_WHEN_HUNGER_IS_ZERO = 20;


        private float timeSinceHungerFall;

        private BoxCollider collider;

        private StatisticHolder statistics;

        public Player(Vector2 position, Vector2 size, int layer, Vector2 scale) : base(position, size, layer, scale)
        {
            FramesPerSecond = 10;

            AddAnimation(5, 25, 313, "Down", 22, 22, new Vector2(0, 0));
            AddAnimation(1, 25, 313, "IdleDown", 22, 22, new Vector2(0, 0));

            //margins abovw and to the right/left are 25
            AddAnimation(5, 25, 25, "Up", 22, 22, new Vector2(0, 0));
            AddAnimation(1, 25, 25, "IdleUp", 22, 22, new Vector2(0, 0));

            AddAnimation(5, 25, 385, "Left", 22, 22, new Vector2(0, 0));
            AddAnimation(1, 25, 385, "IdleLeft", 22, 22, new Vector2(0, 0));

            AddAnimation(5, 25, 169, "Right", 22, 22, new Vector2(0, 0));
            AddAnimation(1, 25, 169, "IdleRight", 22, 22, new Vector2(0, 0));
            //Plays our start animation
            PlayAnimation("IdleDown");

            collider = new BoxCollider(this, 65, 65, -2, -4, false);
            AddComponent(collider);
            statistics = new StatisticHolder(100, 150, 1.0f, 1.0f, 10);
        }

        public bool checkIfEnoughMoney(int cost)
        {
            if (cost > moneyAmount)
                return false;

            return true;
        }

        public override void Update(GameTime gameTime)
        {
            sDirection = Vector2.Zero;

            HandleInput(Keyboard.GetState());

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            HandleHungerDecrasing(deltaTime);

            sDirection *= speed;
            sDirection *= deltaTime;

            collider.Move(sDirection);

            base.Update(gameTime);
        }

        public void Initialize()
        {
            moneyAmount = 200;
            keyAmount = 0;
            hungerValue = 100;
            maxHungerValue = 150;
            timeSinceHungerFall = 0;
        }

        public void AddMoney(int moneyToAdd)
        {
            MoneyAmount += moneyToAdd;
        }

        public void RemoveMoney(int moneyToRemove)
        {
            if (moneyToRemove > moneyAmount)
                MoneyAmount = 0;
            else
                MoneyAmount -= moneyToRemove;
        }

        public void AddKey(int keyNumbers)
        {
            KeyAmount += keyNumbers;
        }

        public void RefilHunger(int hungerValueToRefil)
        {
            if (hungerValue + hungerValueToRefil > maxHungerValue)
                HungerValue += (maxHungerValue - hungerValue);
            else
                HungerValue += hungerValueToRefil;
        }

        public bool CanRefilHunger(int hungerValueToRefil)
        {
            if (hungerValue + hungerValueToRefil > maxHungerValue)
                return false;

            return true;
        }


        public void RefilLive(int liveValueToRefil)
        {
            if (statistics.Health + liveValueToRefil > statistics.MaxHealth)
                LiveValue += (statistics.MaxHealth - statistics.Health);
            else
                LiveValue += liveValueToRefil;
        }

        public bool CanRefilLive(int liveValueToRefil)
        {
            if (statistics.Health + liveValueToRefil > statistics.MaxHealth)
                return false;

            return true;
        }

        public void RemoveKey(int numberKeysToRemove)
        {
            if (numberKeysToRemove > keyAmount)
                KeyAmount = 0;
            else
                KeyAmount -= numberKeysToRemove;
        }

        public void DecreaseHungerValue(int hungerValueToDecrease)
        {
            if(hungerValue > 0)
            {
                if (HungerValue >= hungerValueToDecrease)
                    HungerValue -= hungerValueToDecrease;
                else
                    HungerValue = 0;
            }
            else
            {
                DecreaseLiveValue(LIVE_DECREASE_VALUE_WHEN_HUNGER_IS_ZERO);
            }
        }

        public void DecreaseLiveValue(int liveValueToDecrease)
        {
            if(statistics.Health > 0)
            {
                if (LiveValue >= liveValueToDecrease)
                    LiveValue -= liveValueToDecrease;
                else
                    LiveValue = 0;
            }
            else
            {
                //TODO player die
            }
        }

        public int MaxHungerValue
        {
            get { return maxHungerValue;  }
        }

        public int MaxLiveValue
        {
            get { return statistics.MaxHealth;  }
        }

        public int MoneyAmount
        {
            get { return moneyAmount; }
            private set
            {
                MoneyChangeEvent.Invoke(this, new PropertyChangedArgs<int>(moneyAmount, value));
                moneyAmount = value;
            }
        }

        public int KeyAmount
        {
            get { return keyAmount; }
            private set
            {
                KeyChangeEvent.Invoke(this, new PropertyChangedArgs<int>(keyAmount, value));
                keyAmount = value;
            }
        }

        public int HungerValue
        {
            get { return hungerValue; }
            private set
            {
                HungerChangeEvent.Invoke(this, new PropertyChangedArgs<int>(hungerValue, value));
                hungerValue = value;
            }
        }

        public int LiveValue
        {
            get { return statistics.Health; }
            private set
            {
                LiveChangeEvent.Invoke(this, new PropertyChangedArgs<int>(statistics.Health, value));
                statistics.Health = value;
            }
        }

        public int LiveMaxValue
        {
            get { return statistics.MaxHealth; }
            private set
            {
                LiveMaxValueChangeEvent.Invoke(this, new PropertyChangedArgs<int>(statistics.MaxHealth, value));
                statistics.MaxHealth = value;
            }
        }

        public int HungerMaxValue
        {
            get { return maxHungerValue; }
            private set
            {
                HungerMaxValueChangeEvent.Invoke(this, new PropertyChangedArgs<int>(maxHungerValue, value));
                maxHungerValue = value;
            }
        }

        private void HandleHungerDecrasing(float deltaTime)
        {
            timeSinceHungerFall += deltaTime;

            if (timeSinceHungerFall >= HUNGER_DECREASE_INTERVAL_IN_SECONDS)
            {
                DecreaseHungerValue(HUNGER_DECREASE_VALUE);
                timeSinceHungerFall = 0;
            }
        }

        private void HandleInput(KeyboardState keyState)
        {
            if (!attacking)
            {
                if (keyState.IsKeyDown(Keys.W))
                {
                    sDirection += new Vector2(0, -1);
                    PlayAnimation("Up");
                    currentDirection = movementDirection.up;

                }
                if (keyState.IsKeyDown(Keys.A))
                {
                    sDirection += new Vector2(-1, 0);
                    PlayAnimation("Left");
                    currentDirection = movementDirection.left;

                }
                if (keyState.IsKeyDown(Keys.S))
                {
                    sDirection += new Vector2(0, 1);
                    PlayAnimation("Down");
                    currentDirection = movementDirection.down;

                }
                if (keyState.IsKeyDown(Keys.D))
                {
                    sDirection += new Vector2(1, 0);
                    PlayAnimation("Right");
                    currentDirection = movementDirection.right;

                }
            }

            currentDirection = movementDirection.standstill;
        }

        public override void AnimationDone(string animation)
        {
           if (animation.Contains("Attack"))
           {
               Debug.WriteLine("Attack!");
               attacking = false;
           } else if(IsAnimationMovement(animation))
           {
                currentAnimation = "Idle" + animation;
           }


        }

        // determines if current animation is up/donw/right/left
        private bool IsAnimationMovement(string animation)
        {
            if((animation.Contains("Up") || animation.Contains("Left") || animation.Contains("Down") || animation.Contains("Right")) && !animation.Contains("Idle")) return true;

            return false;
        }

        private void Move()
        {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Left))
                AddForce(-1, 0);
            else if (state.IsKeyDown(Keys.Right))
                AddForce(1, 0);
            if (state.IsKeyDown(Keys.Up))
                AddForce(0, -1);
            else if (state.IsKeyDown(Keys.Down))
                AddForce(0, 1);
        }
    }
}

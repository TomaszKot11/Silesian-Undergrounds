﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Silesian_Undergrounds.Engine;
using Silesian_Undergrounds.Engine.Item;
using Silesian_Undergrounds.Engine.Enum;
using System.Diagnostics;
using System;
using System.Windows.Forms;
using Silesian_Undergrounds.Engine.Common;

namespace Silesian_Undergrounds.Engine.CommonF
{
    public class PickableItem : GameObject
    {
        public Scene.Scene scene;
        protected bool isBuyable { get; set; }
        protected bool wasEntered = false;
        protected bool wasBought = false;
        private double timeSinceBought;
        private const double TIMER = 3;
        private double timer = PickableItem.TIMER;
        private double timeToUpdateFrame = 3;

        public PickableItem(Texture2D texture, Vector2 position, Vector2 size, int layer, Scene.Scene scene, bool isBuyable = false) : base(texture, position, size, layer)
        {
            this.scene = scene;
            this.isBuyable = isBuyable;
        }
        
        public void SetScene(Scene.Scene s)
        {
            scene = s;
        }

        public override void NotifyCollision(GameObject gameobject) {
            if (!wasBought && isBuyable && (gameobject is Player))
            {
                wasBought = true;
                Player player = ((Player)gameobject);
                PerformBuyingOperationForSelf(player);
            }
            
        }

        private void PerformBuyingOperationForSelf(Player player)
        {
            wasEntered = true;
            int itemPrice = GetItemPrice();

            if (player.MoneyAmount >= itemPrice)
            {
                player.RemoveMoney(itemPrice);
                BuyAppripriateProduct(player);
            }
        }

        private int GetItemPrice()
        {
            if (this is Food)
            {
                return (int)ItemPricesEnum.Food;
            }
            else if (this is Heart)
            {
                return (int)ItemPricesEnum.Heart;
            }
            else if (this is Key)
            {
                return (int)ItemPricesEnum.Key;
            }
            else
            {
                #if DEBUG
                Debug.WriteLine("Wrong buyable object!");
                #endif
                // may be risky
                throw new System.Exception();
            }
        }

        private void BuyAppripriateProduct(Player player)
        {
            if (this is Food)
            {
                player.AddToHunger(((Food)this).hungerRefil);
            }
            else if (this is Heart)
            {
                player.AddLiveFromHeart();
            }
            else if (this is Key)
            {
                player.AddKey(1);
            }
        }



        // temporary to show buying mechanics works
        public override void Draw(SpriteBatch spriteBatch)
        {
            // temporary
            if (isBuyable)
                this.color = Color.MediumVioletRed;
       
               

           base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            double elapsed = gameTime.ElapsedGameTime.TotalSeconds;
            timer -= elapsed;
            if(timer < 0)
            {
                timer = PickableItem.TIMER;
                wasBought = false;
            }
            
        }
    }
}

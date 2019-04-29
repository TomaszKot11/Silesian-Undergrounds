﻿using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using Silesian_Undergrounds.Engine.Common;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Silesian_Undergrounds.Engine.Collisions;
using Silesian_Undergrounds.Engine.Enum;
using Silesian_Undergrounds.Engine.CommonF;

namespace Silesian_Undergrounds.Engine.Item {
    public class Food : PickableItem {

        public int hungerRefil { get; private set; }

        public FoodEnum type;

        public Food(Texture2D texture, Vector2 position, Vector2 size, int layer, Scene.Scene scene, FoodEnum foodEnum, bool isBuyable = false) : base(texture, position, size, layer, scene, isBuyable)
        {
            this.type = foodEnum;

            if (this.type == FoodEnum.Meat)
                hungerRefil = 100;
            else
                hungerRefil = 100;

            BoxCollider collider = new BoxCollider(this, 20, 20, 0, 0, true);
            AddComponent(collider);
        }

        //TODO: extract some code to PickableItem class
        public override void NotifyCollision(GameObject obj)
        {
            base.NotifyCollision(obj);

            if (obj is Player && !isBuyable)
            {
                Player pl = (Player)obj;
                if (pl.MaxHungerValue > pl.HungerValue)
                {
                    pl.RefilHunger(this.hungerRefil);
                    this.scene.DeleteObject(this);
                }
            }
        }
    }
}

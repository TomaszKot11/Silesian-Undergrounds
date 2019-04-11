﻿using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using Silesian_Undergrounds.Engine.Common;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Silesian_Undergrounds.Engine.Collisions;
using Silesian_Undergrounds.Engine.Enum;

namespace Silesian_Undergrounds.Engine.Item {
    public class Food : PickableItem {

        public int hunderRefil;

        public FoodEnum type;

        public Food(Texture2D texture, Vector2 position, Vector2 size, int layer, Scene.Scene scene, FoodEnum foodEnum) : base(texture, position, size, layer, scene)
        {
            this.type = foodEnum;

            if (this.type == FoodEnum.Meat)
                hunderRefil = 20;
            else
                hunderRefil = 30;

            BoxCollider collider = new BoxCollider(this, 20, 20, 0, 0, true);
            AddComponent(collider);
        }

        public override void NotifyCollision(GameObject obj)
        {
            if (obj is Player)
            {
                Player pl = (Player)obj;
                this.scene.DeleteObject(this);
            }
        }
    }
}

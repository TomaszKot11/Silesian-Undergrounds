﻿using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Silesian_Undergrounds.Engine.Behaviours
{
    public enum AttackType
    {
        ATTACK_TYPE_MELEE,
        ATTACK_TYPE_RANGED,
    }

    public struct AttackData
    {
        public AttackData(bool isRepeatable, int minDamage, int maxDamage, int attackTimer, AttackType type, float minRange, float maxRange, string pTexture = null, List<Texture2D> particleAnim = null)
        {
            IsRepeatable = isRepeatable;
            MinDamage = minDamage;
            MaxDamage = maxDamage;
            AttackTimer = attackTimer;
            this.type = type;
            MinRange = minRange;
            MaxRange = maxRange;
            this.particleAnim = particleAnim;
            particleTextureName = pTexture;
        }

        public bool IsRepeatable { get; private set; }
        public int MinDamage { get; private set; }
        public int MaxDamage { get; private set; }
        public int AttackTimer { get; private set; }
        public AttackType type { get; private set; }
        public float MinRange { get; private set; }
        public float MaxRange { get; private set; }
        // animation used to play when attack hit target
        public List<Texture2D> particleAnim { get; private set; }
        public string particleTextureName { get; private set; }
    }
}

﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Silesian_Undergrounds.Engine.Common;
using Silesian_Undergrounds.Engine.Components;
using Silesian_Undergrounds.Engine.Collisions;
using Silesian_Undergrounds.Engine.Utils;

namespace Silesian_Undergrounds.Engine.Behaviours
{
    public class HostileBehaviour : IComponent
    {
        // Component inherited
        public Vector2 Position { get; set; }
        public Rectangle Rect { get; set; }

        public GameObject Parent { get; private set; }

        // HostileBehaviour specific
        private bool IsInCombat;
        private GameObject enemy;
        private BoxCollider enemyCollider;
        private CircleCollider aggroArea;
        private BoxCollider collider;
        private bool IsMoveNeeded;
        private TimedEventsScheduler events;
        private float MinDistToEnemy;
        AttackPattern attackPattern;
        private float BonusMoveSpeed;
        private bool IsEvading;

        public HostileBehaviour(GameObject parent, AttackPattern pattern, float bonusMoveSpeed = 0.0f, float minDist = 1)
        {
            Parent = parent;
            Position = new Vector2(0, 0);
            Rect = new Rectangle(0, 0, 0, 0);

            IsInCombat = false;
            enemy = null;
            IsMoveNeeded = false;
            MinDistToEnemy = minDist;
            BonusMoveSpeed = bonusMoveSpeed;

            aggroArea = new CircleCollider(Parent, 70, 0, 0);
            collider = new BoxCollider(Parent, 70, 70, 0, 0, false);
            Parent.AddComponent(collider);
            Parent.AddComponent(aggroArea);
            Parent.OnCollision += NotifyCollision;

            events = new TimedEventsScheduler();
            attackPattern = pattern;
        }

        public void CleanUp()
        {
            DropCombat();
            Parent = null;
            aggroArea = null;
            collider = null;
        }

        public void Draw(SpriteBatch batch) { }

        public void RegisterSelf() { }

        public void UnRegisterSelf() { }

        public void Update(GameTime gameTime)
        {
            if (IsInCombat)
            {
                CheckDistanceToEnemy();
                events.Update(gameTime);
            }
        }

        private void DropCombat()
        {
            events.ClearAll();
            IsInCombat = false;
            IsMoveNeeded = false;
            enemy = null;
            enemyCollider = null;
        }

        public void NotifyCollision(object sender, CollisionNotifyData data)
        {
            if (!IsInCombat && data.source == aggroArea)
            {
                if (data.obj is Player)
                {
                    IsInCombat = true;
                    enemy = data.obj;
                    enemyCollider = enemy.GetComponent<BoxCollider>();
                    CheckDistanceToEnemy();
                    events.ScheduleEvent(50, true, UpdateMovement);
                    PrepareAttackEvents();
                }
            }
        }

        private void UpdateMovement()
        {
            if (!IsMoveNeeded)
                return;

            Vector2 moveForce = new Vector2(0, 0);

            if (enemy.position.X < Parent.position.X)
                moveForce.X = -1;
            else
                moveForce.X = 1;

            if (enemy.position.Y < Parent.position.Y)
                moveForce.Y = -1;
            else
                moveForce.Y = 1;

            moveForce *= (Parent.speed + BonusMoveSpeed);
            collider.Move(moveForce);
        }

        private float GetDistToEnemy()
        {
            float dist = Vector2.Distance(collider.Position, enemyCollider.Position);
            dist -= (collider.Rect.Width / 2);
            // fix me!
            if (enemy != null)
                dist -= (enemyCollider.Rect.Width / 2);
            else
                dist -= (enemy.Rectangle.Width / 2);

            return dist;
        }

        private void CheckDistanceToEnemy()
        {
            float dist = GetDistToEnemy();

            if (dist > MinDistToEnemy)
                IsMoveNeeded = true;
            else
                IsMoveNeeded = false;
        }

        private void PrepareAttackEvents()
        {
            foreach(var attack in attackPattern.attacks)
            {
                events.ScheduleEvent(attack.AttackTimer, attack.IsRepeatable, () =>
                {
                    // Additional check just for safety
                    if (enemy == null)
                        return;

                    AttackData att = attack;
                    // Do not handle melee attack while unit is during movement to enemy
                    if (att.type == AttackType.ATTACK_TYPE_MELEE && IsMoveNeeded)
                        return;

                    // Check distance between unit and enemy in order to validate attack with its data
                    float dist = GetDistToEnemy();
                    // validate attack
                    if (att.MinRange > 0.0f && dist < att.MinRange)
                        return;
                    if (att.MaxRange < dist)
                        return;

                    Random rng = new Random();
                    int dmgValue = rng.Next(att.MinDamage, att.MaxDamage);
                    Player plr = enemy as Player; // TODO: Change it to more flex code via some kind of system
                    plr.DecreaseLiveValue(dmgValue);
                });
            }
        }
    }
}

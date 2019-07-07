﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Silesian_Undergrounds.Engine.Common;
using Silesian_Undergrounds.Engine.Behaviours;
using Silesian_Undergrounds.Engine.Scene;

namespace Silesian_Undergrounds.Engine.Utils
{
  public sealed class EnemyFactory
  {
    public static List<GameObject> GenerateEnemiesForScene(List<GameObject> positionSource)
    {
      List<GameObject> list = new List<GameObject>();

      Random rng = new Random();

      foreach (var pos in positionSource)
      {
        int chance = rng.Next(0, 100);
        if (chance <= 30)
          list.Add(RatFactory(pos.position));
        else if (chance > 30 && chance <= 50)
          list.Add(MinotaurFactory(pos.position));
        else if (chance > 50 && chance <= 90)
          list.Add(WormFactory(pos.position));
      }

      return list;
    }

    public static GameObject MinotaurFactory(Vector2 position)
    {
      TextureMgr.Instance.LoadSingleTextureFromSpritescheet("Monsters/128x80Minotaur_FullSheet", "Monsters/Minotaur", 6, 8, 0, 0, 0, 15);
      Texture2D texture = TextureMgr.Instance.GetTexture("Monsters/Minotaur");

      if (TextureMgr.Instance.LoadAnimationFromSpritesheet("Monsters/128x80Minotaur_FullSheet", "Monsters/Minotaur_Attack", 6, 8, 2, 8, 0, 15, false))
        TextureMgr.Instance.LoadAnimationFromSpritesheet("Monsters/128x80Minotaur_FullSheet", "Monsters/Minotaur_Attack", 6, 8, 3, 8, 0, 15, true);


      GameObject obj = new GameObject(texture, position, new Vector2(ResolutionMgr.TileSize, ResolutionMgr.TileSize), 6);
      AttackPattern attackPattern = new AttackPattern();
      AttackData attackData = new AttackData(true, 15, 30, 3000, AttackType.ATTACK_TYPE_MELEE, 5, 30);
      attackPattern.AddAttack(attackData);

      obj.speed = 2.0f;

      HostileBehaviour behaviour = new HostileBehaviour(obj, attackPattern, 150, 10);

      TextureMgr.Instance.LoadAnimationFromSpritesheet(
        fileName: "Monsters/minotaur_obrocony",
        animName: "Monsters/Minotaur_MoveRight",
        spritesheetRows: 7,
        spritesheetColumns: 8,
        index: 1,
        amount: 8,
        skip: 0,
        spacingX: 0,
        spacingY: 0,
        canAddToExisting: false,
        loadByColumn: false
      );
      TextureMgr.Instance.LoadAnimationFromSpritesheet(
        fileName: "Monsters/minotaur_obrocony",
        animName: "Monsters/Minotaur_Attack",
        spritesheetRows: 7,
        spritesheetColumns: 8,
        index: 3,
        amount: 8,
        skip: 0,
        spacingX: 0,
        spacingY: 0,
        canAddToExisting: false,
        loadByColumn: false
      );
      TextureMgr.Instance.LoadAnimationFromSpritesheet(
        fileName: "Monsters/minotaur_obrocony",
        animName: "Monsters/Minotaur_MoveLeft",
        spritesheetRows: 7,
        spritesheetColumns: 8,
        index: 6,
        amount: 8,
        skip: 0,
        spacingX: 0,
        spacingY: 0,
        canAddToExisting: false,
        loadByColumn: false
    );
      TextureMgr.Instance.LoadAnimationFromSpritesheet(
        fileName: "Monsters/minotaur_obrocony",
        animName: "Monsters/Minotaur_dead",
        spritesheetRows: 7,
        spritesheetColumns: 8,
        index: 5,
        amount: 3,
        skip: 0,
        spacingX: 0,
        spacingY: 0,
        canAddToExisting: false,
        loadByColumn: false
    );

      behaviour.Animator.AddAnimation("MoveRight", TextureMgr.Instance.GetAnimation("Monsters/Minotaur_MoveRight"), 1000);
      behaviour.Animator.AddAnimation("MoveUp", TextureMgr.Instance.GetAnimation("Monsters/Minotaur_MoveRight"), 1000);

      behaviour.Animator.AddAnimation("MoveDown", TextureMgr.Instance.GetAnimation("Monsters/Minotaur_MoveLeft"), 1000);
      behaviour.Animator.AddAnimation("MoveLeft", TextureMgr.Instance.GetAnimation("Monsters/Minotaur_MoveLeft"), 1000);

      behaviour.Animator.AddAnimation("Attack", TextureMgr.Instance.GetAnimation("Monsters/Minotaur_Attack"), 1000);
      behaviour.Animator.AddAnimation("Dead", TextureMgr.Instance.GetAnimation("Monsters/Minotaur_MoveLeft"), 1000);

      obj.AddComponent(behaviour);
      return obj;
    }

    public static GameObject RatFactory(Vector2 position)
    {

      TextureMgr.Instance.LoadSingleTextureFromSpritescheet("Monsters/rat", "Monsters/Rat", 1, 52, 0, 0, 0, 0);
      Texture2D texture = TextureMgr.Instance.GetTexture("Monsters/Rat");

      GameObject obj = new GameObject(texture, position, new Vector2(ResolutionMgr.TileSize, ResolutionMgr.TileSize), 6);
      AttackPattern attackPattern = new AttackPattern();
      AttackData attackData = new AttackData(true, 8, 13, 2000, AttackType.ATTACK_TYPE_MELEE, 5, 20);
      attackPattern.AddAttack(attackData);

      obj.speed = 3.0f;

      HostileBehaviour behaviour = new HostileBehaviour(obj, attackPattern, 70, 5);

      TextureMgr.Instance.LoadAnimationFromSpritesheet(
          fileName: "Monsters/rat",
          animName: "Monsters/Rat_MoveRight",
          spritesheetRows: 1,
          spritesheetColumns: 52,
          index: 0, amount: 4,
          skip: 8,
          spacingX: 0,
          spacingY: 0,
          canAddToExisting: false,
          loadByColumn: false
      );
      TextureMgr.Instance.LoadAnimationFromSpritesheet(
          fileName: "Monsters/rat",
          animName: "Monsters/Rat_Attack",
          spritesheetRows: 1,
          spritesheetColumns: 52,
          index: 0, amount: 10,
          skip: 20,
          spacingX: 0,
          spacingY: 0,
          canAddToExisting: false,
          loadByColumn: false
      );
      TextureMgr.Instance.LoadAnimationFromSpritesheet(
          fileName: "Monsters/rat",
          animName: "Monsters/Rat_MoveLeft",
          spritesheetRows: 1,
          spritesheetColumns: 52,
          index: 0, amount: 4,
          skip: 35,
          spacingX: 0,
          spacingY: 0,
          canAddToExisting: false,
          loadByColumn: false
      );
      TextureMgr.Instance.LoadAnimationFromSpritesheet(
          fileName: "Monsters/rat",
          animName: "Monsters/Rat_dead",
          spritesheetRows: 1,
          spritesheetColumns: 52,
          index: 0, amount: 5,
          skip: 13,
          spacingX: 0,
          spacingY: 0,
          canAddToExisting: false,
          loadByColumn: false
      );

      behaviour.Animator.AddAnimation("MoveRight", TextureMgr.Instance.GetAnimation("Monsters/Rat_MoveRight"), 1000);
      behaviour.Animator.AddAnimation("MoveUp", TextureMgr.Instance.GetAnimation("Monsters/Rat_MoveRight"), 1000);

      behaviour.Animator.AddAnimation("MoveDown", TextureMgr.Instance.GetAnimation("Monsters/Rat_MoveLeft"), 1000);
      behaviour.Animator.AddAnimation("MoveLeft", TextureMgr.Instance.GetAnimation("Monsters/Rat_MoveLeft"), 1000);

      behaviour.Animator.AddAnimation("Attack", TextureMgr.Instance.GetAnimation("Monsters/Rat_Attack"), 1000);
      behaviour.Animator.AddAnimation("Dead", TextureMgr.Instance.GetAnimation("Monsters/Rat_dead"), 1000);

      obj.AddComponent(behaviour);
      return obj;
    }

    public static GameObject WormFactory(Vector2 position)
    {
      TextureMgr.Instance.LoadSingleTextureFromSpritescheet("Monsters/48x48Worm_FullSheet", "Monsters/Worm", 4, 8, 0, 0, 0, 5);
      Texture2D texture = TextureMgr.Instance.GetTexture("Monsters/Worm");

      GameObject obj = new GameObject(texture, position, new Vector2(ResolutionMgr.TileSize, ResolutionMgr.TileSize), 6);
      AttackPattern attackPattern = new AttackPattern();

      AttackData attackData = new AttackData(true, 5, 10, 1000, AttackType.ATTACK_TYPE_MELEE, 5, 15);
      attackPattern.AddAttack(attackData);

      obj.speed = 1.0f;
      HostileBehaviour behaviour = new HostileBehaviour(obj, attackPattern, 40, 1);


      TextureMgr.Instance.LoadAnimationFromSpritesheet("Monsters/worm_obrocony", "Monsters/Worm_MoveRight", 5, 8, 1, 6, 0, 5, false);
      TextureMgr.Instance.LoadAnimationFromSpritesheet("Monsters/worm_obrocony", "Monsters/Worm_Attack", 5, 8, 3, 6, 0, 5, false);
      TextureMgr.Instance.LoadAnimationFromSpritesheet("Monsters/worm_obrocony", "Monsters/Worm_MoveLeft", 5, 8, 4, 6, 0, 5, false);
      TextureMgr.Instance.LoadAnimationFromSpritesheet("Monsters/worm_obrocony", "Monsters/Worm_dead", 5, 8, 2, 6, 0, 5, false);

      behaviour.Animator.AddAnimation("MoveRight", TextureMgr.Instance.GetAnimation("Monsters/Worm_MoveRight"), 1000);
      behaviour.Animator.AddAnimation("MoveUp", TextureMgr.Instance.GetAnimation("Monsters/Worm_MoveRight"), 1000);

      behaviour.Animator.AddAnimation("MoveDown", TextureMgr.Instance.GetAnimation("Monsters/Worm_MoveLeft"), 1000);
      behaviour.Animator.AddAnimation("MoveLeft", TextureMgr.Instance.GetAnimation("Monsters/Worm_MoveLeft"), 1000);

      behaviour.Animator.AddAnimation("Attack", TextureMgr.Instance.GetAnimation("Monsters/Worm_Attack"), 1000);
      behaviour.Animator.AddAnimation("Dead", TextureMgr.Instance.GetAnimation("Monsters/Worm_dead"), 1000);

      obj.AddComponent(behaviour);

      return obj;
    }
  }
}

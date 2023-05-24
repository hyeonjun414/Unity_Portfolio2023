using System.Collections.Generic;
using Manager;
using Newtonsoft.Json.Linq;
using Presenter;
using UniRx;
using UnityEngine;

namespace Model
{
    public enum StatTag
    {
        None,
        Reflect,
        Burn,
        Weak,
        Stun,
        Recovery,
        Slow
    }
    public struct CharacterStat
    {
        public float MaxHp;
        public float CurHp;
        public float Damage;
        public float Speed;
        public float HitRate;
        public float Defence;
    }
    public class CharacterModel
    {
        public string Id;
        public string Name;
        public string Desc;
        public bool IsReady;
        public bool IsDead;
        public float MaxAp;
        public float CurAp;
        public CharacterType CharType;

        public float MaxHp => Stats.MaxHp + BuffStats.MaxHp;
        public float CurHp => Stats.CurHp + BuffStats.CurHp;
        public float Damage => BuffStats.Damage == 0 ? Stats.Damage : Stats.Damage * (1 + BuffStats.Damage);
        public float Speed => BuffStats.Speed == 0 ? Stats.Speed : Stats.Speed * (1 + BuffStats.Speed);
        public float HitRate => BuffStats.HitRate == 0 ? Stats.HitRate : Stats.HitRate * (1 + BuffStats.HitRate);
        public float Defence => Stats.Defence;
        
        public CharacterStat Stats = new();
        public CharacterStat BuffStats = new();
        public Dictionary<StatTag, float> StatTags = new();

        public ReactiveProperty<float> ApRate = new ReactiveProperty<float>();

        public CharacterModel()
        {
        }

        public CharacterModel(MasterEntity me)
        {
            Id = me.Id;
            Name = me.Name;
            Desc = me.Desc;
            Stats.MaxHp = me.Hp;
            Stats.CurHp = me.Hp;
            Stats.Damage = me.Damage;
            Stats.Speed = me.Speed;
            Stats.HitRate = 1;
            
            CurAp = 0;
            MaxAp = 100;
        }
        
        public void TakeDamage(float damage)
        {
            var prevDef = Stats.Defence;
            Stats.Defence = Mathf.Max(Stats.Defence - damage, 0);
            damage = Mathf.Max(damage - prevDef, 0);
            if (damage == 0) return;
            Stats.CurHp -= damage;
            if (Stats.CurHp <= 0)
            {
                Stats.CurHp = 0;
                IsDead = true;
            }
        }


        public void AddAp(float deltaTime)
        {
            CurAp += Speed * deltaTime;
            ApRate.Value = CurAp / MaxAp;
            if (CurAp >= MaxAp)
            {
                IsReady = true;
                CurAp = MaxAp;
            }
        }

        public virtual void UseAp()
        {
            CurAp = 0;
            IsReady = false;
            ApRate.Value = CurAp / MaxAp;
        }

        public void UseAp(float value)
        {
            CurAp = Mathf.Max(CurAp - value, 0);
            IsReady = false;
            ApRate.Value = CurAp / MaxAp;
        }

        public void HpRecover(float value)
        {
            Stats.CurHp = Mathf.Min(Stats.CurHp + value, Stats.MaxHp);
        }

        public void AddBuff(string statName, float value)
        {
            switch (statName)
            {
                case "Damage":
                    BuffStats.Damage += value;
                    break;
                case "HitRate":
                    BuffStats.HitRate += value;
                    break;
                case "Speed":
                    BuffStats.Speed += value;
                    break;
            }
        }

        public void RemoveBuff(string statName, float value)
        {
            switch (statName)
            {
                case "Damage":
                    BuffStats.Damage -= value;
                    break;
                case "HitRate":
                    BuffStats.HitRate -= value;
                    break;
                case "Speed":
                    BuffStats.Speed -= value;
                    break;
            }
        }

        public void AddDefence(int value)
        {
            Stats.Defence += value;
        }

        public void AddTag(StatTag tag, float value)
        {
            if (StatTags.ContainsKey(tag))
                StatTags[tag] += value;
            else
                StatTags.Add(tag, value);
        }

        public void RemoveTag(StatTag tag, float value)
        {
            if (StatTags.ContainsKey(tag) == false) return;

            StatTags[tag] -= value;
            if (StatTags[tag] == 0)
                StatTags.Remove(tag);
        }

        public void MaxHpUp(int value)
        {
            Stats.MaxHp += value;
            Stats.CurHp += value;
        }
    }
    public class EnemyModel : CharacterModel
    {
        public int DropGold;
        public List<JObject> Actions;
        public CharacterAction CurAction;

        public EnemyModel()
        {
        }

        public EnemyModel(MasterEnemy me, float levelValue) : base(me)
        {
            CharType = CharacterType.Enemy;
            Actions = me.Actions;
            DropGold = (int)(me.DropGold * levelValue);
            Stats.Damage = Mathf.Round(Stats.Damage * levelValue);
            Stats.MaxHp = Stats.CurHp = Mathf.Round(Stats.CurHp * levelValue);
            
            if(Actions.Count != 0)
                SetAction();
        }

        public void SetAction()
        {
            var randValue = GameManager.Instance.Rand.Range(0, Actions.Count);
            CurAction = Util.ToObject<CharacterAction>(Actions[randValue]);
            CurAction.Init(this);
        }

        public CharacterAction GetCurAction()
        {
            return CurAction;
        }
    }

    public class AllyModel : CharacterModel
    {
        public List<JObject> Actions;
        public CharacterAction CurAction;
        
        public int Turn;
        public AllyModel(MasterAlly ma, int livingTurn) : base(ma)
        {
            CharType = CharacterType.Ally;
            Actions = ma.Actions;
            Turn = livingTurn;

            if (Actions.Count != 0)
                SetAction();
        }

        public override void UseAp()
        {
            base.UseAp();
            Turn--;
            if (Turn <= 0)
            {
                IsDead = true;
            }
        }

        public void SetAction()
        {
            var randValue = GameManager.Instance.Rand.Range(0, Actions.Count);
            CurAction = Util.ToObject<CharacterAction>(Actions[randValue]);
            CurAction.Init(this);
        }

        public CharacterAction GetCurAction()
        {
            return CurAction;
        }
    }

    public class HeroModel : CharacterModel
    {
        public HeroModel()
        {
        }

        public HeroModel(MasterEntity me) : base(me)
        {
            CharType = CharacterType.Ally;
        }
    }
}

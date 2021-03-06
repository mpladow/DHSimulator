﻿using Engine.Actions;
using Engine.Characters;
using Engine.Characters.CharacterCreation;
using Engine.Characters.HomeWorlds;
using Engine.Skills;
using Engine.Statistics;
using Engine.Utilities;
using Engine.Utilities.Constants;
using Engine.World_Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Engine.Actions.DiceRolls;
using static Engine.Statistics.CharacterStat;

namespace Engine
{
    [Serializable]
    public abstract class Character_base
    {

        public string Name { get; set; }

        public List<CharacterStat> Stats { get; set; }

        public int Wounds { get; set; }

        public List<BodyLocation> BodyLocations { get; set; }
        public int A_Head { get; set; }
        public int A_Body { get; set; }
        public int A_LeftArm { get; set; }
        public int A_RightArm { get; set; }
        public int A_LeftLeg { get; set; }
        public int A_RightLeg { get; set; }

        public List<SkillsWithRank> MovementSkills { get; set; }
        public List<SkillsWithRank> CombatSkills { get; set; }
        public List<SkillsWithRank> GeneralSkills { get; set; }
        public List<SkillsWithRank> InteractionSkills { get; set; }



        public List<string> Aptitudes { get; set; }
        public List<Weapon_base> Weapons { get; set; }


        public HomeWorld HomeWorld { get; set; }
        public Background Background { get; set; }
        public int CurrentPosition { get; set; }

        public int XP { get; set; }

        //constructor
        public Character_base()
        {
            Stats = new List<CharacterStat>();
            Stats.Add(new CharacterStat(Constants.Ws));
            Stats.Add(new CharacterStat(Constants.Bs));
            Stats.Add(new CharacterStat(Constants.Str));
            Stats.Add(new CharacterStat(Constants.T));
            Stats.Add(new CharacterStat(Constants.Ag));
            Stats.Add(new CharacterStat(Constants.Inte));
            Stats.Add(new CharacterStat(Constants.Per));
            Stats.Add(new CharacterStat(Constants.Wp));
            Stats.Add(new CharacterStat(Constants.Fel));
            Stats.Add(new CharacterStat(Constants.Ifl));

            BodyLocations = new List<BodyLocation>()
            { new BodyLocation("A_Head")
                , new BodyLocation("A_Body")
                , new BodyLocation("A_LeftArm")
                , new BodyLocation("A_RightArm")
                , new BodyLocation("A_LeftLeg")
                , new BodyLocation("A_RightLeg")
            };

            MovementSkills = World.MovementSkillsList;
            CombatSkills = World.CombatSkillsList;
            GeneralSkills = World.GeneralSkillsList;
            InteractionSkills = World.InteractionSkillsList;
            Aptitudes = new List<string>();
            Weapons = new List<Weapon_base>();
            SetPlayerMainStatOntoSkill();
        }

        //quickly search for stat by name
        public CharacterStat GetCharacterStatByName(string name)
        {
            return Stats.FirstOrDefault(x => x.Name == name);
        }

        //Character Setup
        public void AllocateValues()
        {
            Cryptorandom rn = new Cryptorandom();
            foreach (var stat in Stats)
            {
                var value = Constants.StartingValue;
                value += RollD10(2, rn).Sum();
                stat.BaseValue = value;
            }
            if (HomeWorld != null)
            {
                Wounds = RollD5(1, rn)[0] + HomeWorld.BaseWounds;
            }
            XP = Constants.StartingExperience;
            SetPlayerMainStatOntoSkill();//sets the base value to the skill once the initial values have been set
        }

        public void SetPlayerMainStatOntoSkill()
        {
            var totalSkills = new List<SkillsWithRank>();
            totalSkills.AddRange(MovementSkills);
            totalSkills.AddRange(GeneralSkills);
            totalSkills.AddRange(InteractionSkills);
            totalSkills.AddRange(CombatSkills);

            foreach (var skill in totalSkills)
            {
                while (skill.MainStat == null)
                {
                    skill.MainStat = skill.Description.Contains("(Toughness)") ? GetCharacterStatByName(Constants.T) : skill.MainStat;
                    skill.MainStat = skill.Description.Contains("(Agility)") ? GetCharacterStatByName(Constants.Ag) : skill.MainStat;
                    skill.MainStat = skill.Description.Contains("(Fellowship)") ? GetCharacterStatByName(Constants.Fel) : skill.MainStat;
                    skill.MainStat = skill.Description.Contains("(Agility)") ? GetCharacterStatByName(Constants.Ag) : skill.MainStat;
                    skill.MainStat = skill.Description.Contains("(Intelligence)") ? GetCharacterStatByName(Constants.Inte) : skill.MainStat;
                    skill.MainStat = skill.Description.Contains("(Willpower)") ? GetCharacterStatByName(Constants.Wp) : skill.MainStat;
                    skill.MainStat = skill.Description.Contains("(Perception)") ? GetCharacterStatByName(Constants.Per) : skill.MainStat;
                    skill.MainStat = skill.Description.Contains("(Strength)") ? GetCharacterStatByName(Constants.Str) : skill.MainStat;
                }
            }
        }

        //
        public void UpdateHomeWorldCharacteristicBonuses()
        {
            Cryptorandom rn = new Cryptorandom();
            var posStats = HomeWorld.StatsAffectedPositive;
            var negStats = HomeWorld.StatsAffectedNegative;
            foreach (var posStat in posStats)
            {
                foreach (var stat in Stats)
                {
                    if (stat.Name == posStat)
                    {
                        stat.BaseValue = Constants.StartingValue + 5;//rests basevaluee to 25
                        int[] roll = RollD10(3, rn);
                        stat.BaseValue += TakeDice(2, roll, true).Sum();
                    }
                }
            }
            foreach (var negStat in negStats)
            {
                foreach (var stat in Stats)
                {
                    if (stat.Name == negStat)
                    {
                        stat.BaseValue = Constants.StartingValue - 5;//resets basevaluee to 25
                        int[] roll = RollD10(3, rn);
                        stat.BaseValue += TakeDice(2, roll, false).Sum();
                    }
                }
            }
            this.Aptitudes.Add(HomeWorld.AptitudeBonus);
        }



        public void SetPlayerStartingSkillsLevel()//this gets set after the user selects their background, which determines their starting skill proficiencies
        {
            var totalSkills = new List<SkillsWithRank>();
            totalSkills.AddRange(MovementSkills);
            totalSkills.AddRange(GeneralSkills);
            totalSkills.AddRange(InteractionSkills);
            totalSkills.AddRange(CombatSkills);

            foreach (var charSkill in totalSkills)
            {
                charSkill.ModifyRank(false);
            }

            foreach (var bgSkill in Background.StartingSkills)
            {
                foreach (var charSkill in totalSkills)
                {
                    if (bgSkill.Name == charSkill.Name)
                    {
                        charSkill.ModifyRank(true);
                    }

                }


            }
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class ParticipantFrames
    {
        public ChampionStats championStats { get; set; }
        public int currentGold { get; set; }
        public DamageStats damageStats { get; set; }
        public int goldPerSecond { get; set; }
        public int jungleMinionsKilled { get; set; }
        public int level { get; set; }
        public int minionsKilled { get; set; }
        public int participantId { get; set; }
        public Position position { get; set; }
        public int timeEnemySpentControlled { get; set; }
        public int totalGold { get; set; }
        public int xp { get; set; }
    }

    public class ChampionStats
    {
        public int abilityHaste { get; set; }
        public int abilityPower { get; set; }
        public int armor { get; set; }
        public int armorPen { get; set; }
        public int armorPenPercent { get; set; }
        public int attackDamage { get; set; }
        public int attackSpeed { get; set; }
        public int bonusArmorPenPercent { get; set; }
        public int bonusMagicPenPercent { get; set; }
        public int ccReduction { get; set; }
        public int cooldownReduction { get; set; }
        public int health { get; set; }
        public int healthMax { get; set; }
        public int healthRegen { get; set; }
        public int lifesteal { get; set; }
        public int magicPen { get; set; }
        public int magicPenPercent { get; set; }
        public int magicResist { get; set; }
        public int movementSpeed { get; set; }
        public int omnivamp { get; set; }
        public int physicalVamp { get; set; }
        public int power { get; set; }
        public int powerMax { get; set; }
        public int powerRegen { get; set; }
        public int spellVamp { get; set; }
    }

    public class DamageStats
    {
        public int magicDamageDone { get; set; }
        public int magicDamageDoneToChampions { get; set; }
        public int magicDamageTaken { get; set; }
        public int physicalDamageDone { get; set; }
        public int physicalDamageDoneToChampions { get; set; }
        public int physicalDamageTaken { get; set; }
        public int totalDamageDone { get; set; }
        public int totalDamageDoneToChampions { get; set; }
        public int totalDamageTaken { get; set; }
        public int trueDamageDone { get; set; }
        public int trueDamageDoneToChampions { get; set; }
        public int trueDamageTaken { get; set; }
    }

    public class Event
    {
        public object realTimestamp { get; set; }
        public int timestamp { get; set; }
        public string type { get; set; }
        public int? itemId { get; set; }
        public int? participantId { get; set; }
        public string levelUpType { get; set; }
        public int? skillSlot { get; set; }
        public int? creatorId { get; set; }
        public string wardType { get; set; }
        public int? level { get; set; }
        public List<int> assistingParticipantIds { get; set; }
        public int? bounty { get; set; }
        public int? killStreakLength { get; set; }
        public int? killerId { get; set; }
        public Position position { get; set; }
        public int? shutdownBounty { get; set; }
        public List<VictimDamageDealt> victimDamageDealt { get; set; }
        public List<VictimDamageReceived> victimDamageReceived { get; set; }
        public int? victimId { get; set; }
        public string killType { get; set; }
        public int? killerTeamId { get; set; }
        public string monsterSubType { get; set; }
        public string monsterType { get; set; }
        public int? afterId { get; set; }
        public int? beforeId { get; set; }
        public int? goldGain { get; set; }
        public string laneType { get; set; }
        public int? teamId { get; set; }
        public string transformType { get; set; }
        public int? multiKillLength { get; set; }
        public string buildingType { get; set; }
        public string towerType { get; set; }
        public int? actualStartTime { get; set; }
        public long? gameId { get; set; }
        public int? winningTeam { get; set; }
    }

    public class Frame
    {
        public List<Event> events { get; set; }
        //public ParticipantFrames participantFrames { get; set; }
        public Dictionary<int, ParticipantFrames> participantFrames {get; set;}
        public int timestamp { get; set; }
    }

    public class TimelineInfo
    {
        public int frameInterval { get; set; }
        public List<Frame> frames { get; set; }
        public long gameId { get; set; }
        public List<TimelineParticipant> participants { get; set; }
    }

    public class TimelineMetadata
    {
        public string dataVersion { get; set; }
        public string matchId { get; set; }
        public List<string> participants { get; set; }
    }

    public class TimelineParticipant
    {
        public int participantId { get; set; }
        public string puuid { get; set; }
    }

    //public class ParticipantFrames
    //{
    //    public _1 _1 { get; set; }
    //    public _2 _2 { get; set; }
    //    public _3 _3 { get; set; }
    //    public _4 _4 { get; set; }
    //    public _5 _5 { get; set; }
    //    public _6 _6 { get; set; }
    //    public _7 _7 { get; set; }
    //    public _8 _8 { get; set; }
    //    public _9 _9 { get; set; }
    //    public _10 _10 { get; set; }
    //}

    public class Position
    {
        public int x { get; set; }
        public int y { get; set; }
    }

    public class RiotAPITimeline
    {
        public TimelineMetadata metadata { get; set; }
        public TimelineInfo info { get; set; }
    }

    public class VictimDamageDealt
    {
        public bool basic { get; set; }
        public int magicDamage { get; set; }
        public string name { get; set; }
        public int participantId { get; set; }
        public int physicalDamage { get; set; }
        public string spellName { get; set; }
        public int spellSlot { get; set; }
        public int trueDamage { get; set; }
        public string type { get; set; }
    }

    public class VictimDamageReceived
    {
        public bool basic { get; set; }
        public int magicDamage { get; set; }
        public string name { get; set; }
        public int participantId { get; set; }
        public int physicalDamage { get; set; }
        public string spellName { get; set; }
        public int spellSlot { get; set; }
        public int trueDamage { get; set; }
        public string type { get; set; }
    }

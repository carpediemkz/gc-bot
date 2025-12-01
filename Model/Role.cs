using System;

namespace gc_bot.Model
{
    /// <summary>
    /// Domain model representing a game role and its resources.
    /// Fields map to: 所属大区 (Region), 服务器 (Server), 所属势力 (Faction), 角色昵称 (Nickname),
    /// 角色等级 (Level) and a set of resource counts.
    /// </summary>
    public sealed class Role
    {
        public Role() { }

        public Role(string region, string server, string faction, string nickname, int level = 1)
        {
            Region = region ?? string.Empty;
            Server = server ?? string.Empty;
            Faction = faction ?? string.Empty;
            Nickname = nickname ?? string.Empty;
            Level = Math.Max(0, level);
        }

        // Identity / basic info
        public string Region { get; init; } = string.Empty;
        public string Server { get; init; } = string.Empty;
        public string Faction { get; init; } = string.Empty;
        public string Nickname { get; init; } = string.Empty;
        public int Level { get; init; } = 1;

        // Resources (counts use long to avoid overflow for large amounts)
        public long Gold { get; set; }           // 金币数量
        public long Silver { get; set; }         // 银币数量
        public long Wood { get; set; }           // 木材
        public long Grain { get; set; }          // 粮食
        public long RefinedIron { get; set; }    // 镔铁
        public long Meteorite { get; set; }      // 陨铁
        public long Silk { get; set; }           // 丝绸
        public long Pouches { get; set; }        // 锦囊
        public long Charcoal { get; set; }       // 木炭
        public long Essence { get; set; }        // 精粹
        public long PreciousOre { get; set; }    // 精金矿
        public long Phantoms { get; set; }       // 幻影数量

        /// <summary>
        /// Adjusts a named resource by the specified delta. Throws if resourceName unknown.
        /// </summary>
        public void AdjustResource(string resourceName, long delta)
        {
            if (string.IsNullOrWhiteSpace(resourceName)) throw new ArgumentNullException(nameof(resourceName));

            switch (resourceName.Trim().ToLowerInvariant())
            {
                case "gold":
                case "金币":
                    Gold = Math.Max(0, Gold + delta);
                    return;
                case "silver":
                case "银币":
                    Silver = Math.Max(0, Silver + delta);
                    return;
                case "wood":
                case "木材":
                    Wood = Math.Max(0, Wood + delta);
                    return;
                case "grain":
                case "粮食":
                    Grain = Math.Max(0, Grain + delta);
                    return;
                case "refinediron":
                case "镔铁":
                    RefinedIron = Math.Max(0, RefinedIron + delta);
                    return;
                case "meteorite":
                case "陨铁":
                    Meteorite = Math.Max(0, Meteorite + delta);
                    return;
                case "silk":
                case "丝绸":
                    Silk = Math.Max(0, Silk + delta);
                    return;
                case "pouches":
                case "锦囊":
                    Pouches = Math.Max(0, Pouches + delta);
                    return;
                case "charcoal":
                case "木炭":
                    Charcoal = Math.Max(0, Charcoal + delta);
                    return;
                case "essence":
                case "精粹":
                    Essence = Math.Max(0, Essence + delta);
                    return;
                case "preciousore":
                case "精金矿":
                    PreciousOre = Math.Max(0, PreciousOre + delta);
                    return;
                case "phantoms":
                case "幻影":
                case "幻影数量":
                    Phantoms = Math.Max(0, Phantoms + delta);
                    return;
                default:
                    throw new ArgumentException($"Unknown resource name: {resourceName}", nameof(resourceName));
            }
        }

        /// <summary>
        /// Returns a short display string for the role.
        /// </summary>
        public override string ToString() =>
            $"{Nickname} (Lv{Level}) @ {Region}/{Server} - Gold:{Gold} Silver:{Silver} Wood:{Wood} Grain:{Grain}";
    }
}
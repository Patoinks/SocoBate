using System;
    public class Hero
    {
        public string name;          // Name of the Hero
        public string spriteId;      // Sprite or Image ID, reference to sprite
        public string rarity;        // Rarity (Common, Rare, Epic, Legendary)
        public int heroId;           // Unique ID for the Hero

        public Hero(int heroId, string name, string spriteId, string rarity)
        {
            this.heroId = heroId;
            this.name = name;
            this.spriteId = spriteId;
            this.rarity = rarity;
        }
    }
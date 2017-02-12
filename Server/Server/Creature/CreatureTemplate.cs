namespace Server.Creature {
    public class CreatureTemplate {
        public int TemplateId;
        public string Name;
        public int Health;
        public int MinLevel;
        public int MaxLevel;

        public CreatureTemplate() {

        }

        public CreatureTemplate(int templateId, string name, int health, int minLevel, int maxLevel) {
            TemplateId = templateId;
            Name = name;
            Health = health;
            MinLevel = minLevel;
            MaxLevel = maxLevel;
        }
    }
}

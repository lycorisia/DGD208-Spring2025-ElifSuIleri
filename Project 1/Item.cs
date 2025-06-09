namespace MysticPets.Items
{
    public class Item
    {
        public string Name { get; set; }             
        public Enums.ItemType ItemType { get; set; }
        public int StatIncrease { get; set; }
        public int UseDuration { get; set; }
        public int Cost { get; set; }                

        public Item() { }

        public Item(string name, Enums.ItemType itemType, int statIncrease, int cost, int useDuration = 1000)
        {
            Name = name;
            ItemType = itemType;
            StatIncrease = statIncrease;
            Cost = cost;
            UseDuration = useDuration;
        }
    }
}
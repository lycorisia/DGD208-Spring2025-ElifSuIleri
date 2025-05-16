using System.Collections.Generic;
using MysticPets.Enums;

namespace MysticPets.Items
{
    public static class ItemDatabase
    {
        public static IEnumerable<Item> GetAllItems()
        {
            return new List<Item>
            {
                new Item { ItemType = ItemType.ManaElixr, StatIncrease = 20, UseDuration = 2000 },
                new Item { ItemType = ItemType.Pets, StatIncrease = 20, UseDuration = 2000 },
                new Item { ItemType = ItemType.EeepyTime, StatIncrease = 20, UseDuration = 2000 }
            };
        }
    }
}
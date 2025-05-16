using System;
using System.Threading.Tasks;
using MysticPets.Pets;
using MysticPets.Items;
using MysticPets.Enums;

namespace MysticPets.Managers
{
    public class ItemManager
    {
        public async Task UseItemOnPetAsync<TPet>(TPet pet, Item item) where TPet : Pet
        {
            if (item.ItemType == ItemType.ManaElixr && pet is not Dragon)
            {
                Console.WriteLine($"Only Dragons can use Mana Elixr.");
                return;
            }

            if (item.ItemType == ItemType.Pets && pet is not Pixie)
            {
                Console.WriteLine($"Only Pixies can use Pets.");
                return;
            }

            if (item.ItemType == ItemType.EeepyTime && pet is not Hydra)
            {
                Console.WriteLine($"Only Hydras can use Eeepy Time.");
                return;
            }

            Console.WriteLine($"Using {item.ItemType} on {pet.Name}...");
            await Task.Delay(item.UseDuration);
            IncreaseStat(pet, item);
            Console.WriteLine($"{item.ItemType} used on {pet.Name}!");
        }

        private void IncreaseStat(Pet pet, Item item)
        {
            switch (item.ItemType)
            {
                case ItemType.ManaElixr:
                    pet.Hunger = Math.Min(pet.Hunger + item.StatIncrease, 100);
                    break;
                case ItemType.Pets:
                    pet.Fun = Math.Min(pet.Fun + item.StatIncrease, 100);
                    break;
                case ItemType.EeepyTime:
                    pet.Sleep = Math.Min(pet.Sleep + item.StatIncrease, 100);
                    break;
            }
        }
    }
}
using System;
using MysticPets.Pets;

namespace MysticPets.Helpers
{
    public static class PetExtensions
    {
        public static bool IsDead(this Pet pet)
        {
            return pet.Hunger <= 0 || pet.Sleep <= 0 || pet.Fun <= 0;
        }

        public static void IncreaseFun(this Pet pet, int amount)
        {
            pet.Fun = Math.Min(pet.Fun + amount, 100);
        }

        public static void IncreaseSleep(this Pet pet, int amount)
        {
            pet.Sleep = Math.Min(pet.Sleep + amount, 100);
        }

        public static string GetName(this Pet pet)
        {
            return pet.Name ?? pet.GetType().Name;
        }
    }
}
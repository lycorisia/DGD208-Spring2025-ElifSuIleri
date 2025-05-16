using System;
using MysticPets.Enums;

namespace MysticPets.Pets
{
    public abstract class Pet
    {
        public string Name { get; set; }
        public PetType PetType { get; private set; }
        public int Hunger { get; set; } = 50;
        public int Sleep { get; set; } = 50;
        public int Fun { get; set; } = 50;

        public event Action<Pet> PetDied;

        protected Pet(string name, PetType petType)
        {
            Name = name;
            PetType = petType;
        }

        public void DecreaseStat(PetStat stat, int amount)
        {
            switch (stat)
            {
                case PetStat.Hunger:
                    Hunger = Math.Max(Hunger - amount, 0);
                    if (Hunger <= 10 && Hunger > 0) Console.WriteLine($"⚠ WARNING: {Name}'s Hunger is critically low! ({Hunger})");
                    break;
                case PetStat.Sleep:
                    Sleep = Math.Max(Sleep - amount, 0);
                    if (Sleep <= 10 && Sleep > 0) Console.WriteLine($"⚠ WARNING: {Name}'s Sleep is critically low! ({Sleep})");
                    break;
                case PetStat.Fun:
                    Fun = Math.Max(Fun - amount, 0);
                    if (Fun <= 10 && Fun > 0) Console.WriteLine($"⚠ WARNING: {Name}'s Fun is critically low! ({Fun})");
                    break;
            }

            if (Hunger == 0 || Sleep == 0 || Fun == 0)
            {
                PetDied?.Invoke(this);
            }
        }
    }
}
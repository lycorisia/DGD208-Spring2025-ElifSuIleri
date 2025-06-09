using System;
using System.Collections.Generic;
using System.Threading;
using MysticPets.Pets;
using MysticPets.Managers;

namespace MysticPets.Managers
{
    public class PetManager
    {
        private static PetManager _instance;
        public static PetManager Instance => _instance ??= new PetManager();

        private List<Pet> pets = new List<Pet>();
        private StatUpdater statUpdater;
        private CancellationTokenSource statUpdaterCts;

        private Pet _activePet; // ✅ NEW: Holds currently selected pet
        public Pet ActivePet => _activePet;
        public void SetActivePet(Pet pet)
        {
            _activePet = pet;
        }

        public IReadOnlyList<Pet> GetAllPets() => pets.AsReadOnly();
        public event Action<Pet> PetRemoved;
        public bool CanAdoptMorePets => pets.Count < 4;

        public bool AdoptPet(Pet pet)
        {
            if (pets.Count >= 4)
                return false;

            pet.PetDied += HandlePetDeath;
            pets.Add(pet);
            return true;
        }

        private void HandlePetDeath(Pet pet)
        {
            pets.Remove(pet);
            PetRemoved?.Invoke(pet);
        }

        public void DeletePet(Pet pet)
        {
            if (pets.Contains(pet))
            {
                pets.Remove(pet);
                PetRemoved?.Invoke(pet);
            }
        }

        public void StartStatUpdates(Action onCoinEarned)
        {
            statUpdaterCts = new CancellationTokenSource();
            statUpdater = new StatUpdater(this);
            _ = statUpdater.StartStatDecreaseLoop(statUpdaterCts.Token, onCoinEarned);
        }

        public void StopStatUpdates()
        {
            if (statUpdaterCts != null)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("[DEBUG] Stopping stat updater loop.");
                Console.ResetColor();

                statUpdaterCts.Cancel();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using MysticPets.Pets;

namespace MysticPets.Managers
{
    public class PetManager
    {
        private List<Pet> pets = new List<Pet>();

        public event Action<Pet> PetRemoved;

        public void AdoptPet(Pet pet)
        {
            pet.PetDied += HandlePetDeath;
            pets.Add(pet);
        }

        private void HandlePetDeath(Pet pet)
        {
            pets.Remove(pet);
            PetRemoved?.Invoke(pet);
        }

        public List<Pet> GetAllPets() => pets;
    }
}
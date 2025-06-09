using System;
using System.Threading;
using System.Threading.Tasks;
using MysticPets.Managers;
using MysticPets.Enums;

namespace MysticPets.Managers
{
    public class StatUpdater
    {
        private readonly PetManager petManager;
        private Action onCoinEarned;

        public StatUpdater(PetManager manager)
        {
            petManager = manager;
        }

        public async Task StartStatDecreaseLoop(CancellationToken token, Action onCoinEarnedCallback)
        {
            onCoinEarned = onCoinEarnedCallback;

            while (!token.IsCancellationRequested)
            {
                var pet = petManager.ActivePet;
                if (pet != null && !pet.IsDead())
                {
                    pet.DecreaseStat(PetStat.Hunger, 1);
                    pet.DecreaseStat(PetStat.Sleep, 1);
                    pet.DecreaseStat(PetStat.Fun, 1);

                    if (pet.Sleep > 50 && pet.Fun > 50)
                    {
                        onCoinEarned?.Invoke();
                    }
                }

                await Task.Delay(3000); // Every 3 seconds
            }
        }
    }
}
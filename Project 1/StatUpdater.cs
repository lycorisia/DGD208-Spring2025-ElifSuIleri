using System.Threading;
using System.Threading.Tasks;
using MysticPets.Managers;
using MysticPets.Enums;

namespace MysticPets.Managers
{
    public class StatUpdater
    {
        private readonly PetManager petManager;

        public StatUpdater(PetManager manager)
        {
            petManager = manager;
        }

        public async Task StartStatDecreaseLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                foreach (var pet in petManager.GetAllPets())
                {
                    pet.DecreaseStat(PetStat.Hunger, 1);
                    pet.DecreaseStat(PetStat.Sleep, 1);
                    pet.DecreaseStat(PetStat.Fun, 1);
                }

                await Task.Delay(3000); // Decrease stats every 3 seconds
            }
        }
    }
}
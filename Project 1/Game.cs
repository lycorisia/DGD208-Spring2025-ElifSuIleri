using System;
using System.Threading;
using System.Threading.Tasks;
using MysticPets.Managers;
using MysticPets.Pets;

namespace MysticPets
{
    public class Game
    {
        private PetManager petManager = new PetManager();
        private ItemManager itemManager = new ItemManager();
        private CancellationTokenSource statUpdaterCts = new CancellationTokenSource();
        private StatUpdater statUpdater;

        public void Start()
        {
            petManager.PetRemoved += OnPetRemoved;
            statUpdater = new StatUpdater(petManager);

            _ = statUpdater.StartStatDecreaseLoop(statUpdaterCts.Token);
            _ = DisplayPetStatsLoop(statUpdaterCts.Token);

            ShowMainMenu();
        }

        private void ShowMainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Mystic Pets ===");
                Console.WriteLine("1. Start Game");
                Console.WriteLine("2. Show Project Info");
                Console.WriteLine("3. Exit");

                Console.Write("\nEnter your choice: ");
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        StartGameSequence();
                        break;
                    case "2":
                        ShowInfo();
                        break;
                    case "3":
                        statUpdaterCts.Cancel();
                        Console.WriteLine("Exiting Mystic Pets... Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid input. Press Enter to try again...");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private void StartGameSequence()
        {
            Console.Clear();
            Console.WriteLine("Welcome to the MysticPets Adoption Centre!");
            Console.WriteLine("I am Liex, I will be your helper today.");
            Console.WriteLine("\nChoose the type of pet you want to adopt:");

            Console.WriteLine("1. Pixie");
            Console.WriteLine("2. Hydra");
            Console.WriteLine("3. Dragon");
            Console.WriteLine("4. Unicorn");
            Console.WriteLine("5. Back to Main Menu");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AdoptPet(new Pixie("Pixie"));
                    break;
                case "2":
                    AdoptPet(new Hydra("Hydra"));
                    break;
                case "3":
                    AdoptPet(new Dragon("Dragon"));
                    break;
                case "4":
                    AdoptPet(new Unicorn("Unicorn"));
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }

            Console.WriteLine("\nPress Enter to return to the main menu...");
            Console.ReadLine();
        }

        private void AdoptPet(Pet pet)
        {
            petManager.AdoptPet(pet);
            Console.WriteLine($"\nYou have adopted {pet.Name} the {pet.PetType}!");
            Console.WriteLine("Their stats will now start decreasing over time.");
        }

        private void ShowInfo()
        {
            Console.Clear();
            Console.WriteLine("Mystic Pets - Project by Elif Su İleri");
            Console.WriteLine("Student Number: 2025XXXXXX");
            Console.WriteLine("\nPress Enter to return to the main menu...");
            Console.ReadLine();
        }

        private void OnPetRemoved(Pet pet)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n!!! ALERT: {pet.Name} the {pet.PetType} has passed away...");
            Console.WriteLine("Your pet is dead oh no!");
            Console.ResetColor();
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        private async Task DisplayPetStatsLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Console.Clear();
                Console.WriteLine("=== Mystic Pets Stat Monitor (Live) ===\n");

                var pets = petManager.GetAllPets();
                if (pets.Count == 0)
                {
                    Console.WriteLine("No pets adopted yet...");
                }
                else
                {
                    foreach (var pet in pets)
                    {
                        Console.WriteLine($"{pet.Name} ({pet.PetType}) - Hunger: {pet.Hunger} | Sleep: {pet.Sleep} | Fun: {pet.Fun}");
                    }
                }

                Console.WriteLine("\n(This screen refreshes automatically every 3 seconds.)");
                await Task.Delay(3000);
            }
        }
    }
}

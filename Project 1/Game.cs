using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MysticPets.Managers;
using MysticPets.Pets;
using MysticPets.Items;
using MysticPets.Helpers;

namespace MysticPets
{
    public class Game
    {
        private PetManager petManager = new PetManager();
        private ItemManager itemManager = new ItemManager();
        private CancellationTokenSource statUpdaterCts = new CancellationTokenSource();
        private StatUpdater statUpdater;
        private int coinCount = 0;
        private bool hasAdopted = false;

        public void Start()
        {
            petManager.PetRemoved += OnPetRemoved;
            statUpdater = new StatUpdater(petManager);
            _ = statUpdater.StartStatDecreaseLoop(statUpdaterCts.Token, OnCoinEarned);
            StartGameSequence();
        }

        public void Continue()
        {
            StartGameSequence();
        }

        private void StartGameSequence()
        {
            if (!hasAdopted)
            {
                Console.Clear();
                Console.WriteLine("Welcome to the MysticPets Adoption Centre!");
                Console.WriteLine("I am Liex, I will be your helper today.");
                Console.WriteLine("\nChoose the type of pet you want to adopt:");

                Console.WriteLine("1. Pixie");
                Console.WriteLine("2. Hydra");
                Console.WriteLine("3. Dragon");
                Console.WriteLine("4. Unicorn");

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
                    default:
                        Console.WriteLine("Invalid choice.");
                        return;
                }

                hasAdopted = true;
                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
            }

            while (!petManager.GetAllPets().First().IsDead())
            {
                Console.Clear();
                Console.WriteLine("=== MysticPets In-Game Menu ===\n");
                Console.WriteLine("1. Feed");
                Console.WriteLine("2. Play");
                Console.WriteLine("3. Sleep");
                Console.WriteLine("4. Shop");
                Console.WriteLine("5. Exit to Main Menu");
                Console.WriteLine("6. View Stats (Live)");
                Console.WriteLine($"\nCurrent Coins: {coinCount}");
                Console.Write("\nChoose an action: ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        FeedPet();
                        break;
                    case "2":
                        petManager.GetAllPets().First().IncreaseFun(10);
                        break;
                    case "3":
                        petManager.GetAllPets().First().IncreaseSleep(10);
                        break;
                    case "4":
                        OpenShop();
                        break;
                    case "5":
                        return;
                    case "6":
                        ShowLiveStats().Wait();
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Press Enter to try again.");
                        Console.ReadLine();
                        break;
                }
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nYour pet is dead oh no!");
            Console.ResetColor();
            statUpdaterCts.Cancel();
            Console.WriteLine("Press Enter to return to the main menu...");
            Console.ReadLine();
        }

        private void AdoptPet(Pet pet)
        {
            petManager.AdoptPet(pet);
            Console.WriteLine($"\nYou have adopted {pet.GetName()} the {pet.PetType}!");
        }

        private void FeedPet()
        {
            var pet = petManager.GetAllPets().First();
            var inventory = itemManager.GetInventory();

            if (!inventory.Any())
            {
                Console.WriteLine("\nYou don't have any food. Visit the shop first.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("\nYour Food Inventory:");
            for (int i = 0; i < inventory.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {inventory[i].Name} (+{inventory[i].StatIncrease} Hunger)");
            }

            Console.Write("Choose item to feed: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= inventory.Count)
            {
                var item = inventory[index - 1];
                _ = itemManager.UseItemOnPetAsync(pet, item);
                itemManager.RemoveFromInventory(item);
            }
            else
            {
                Console.WriteLine("Invalid choice.");
            }

            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        private void OpenShop()
        {
            var shopItems = itemManager.GetAvailableItems();

            Console.WriteLine("\nWelcome to the Shop! Your coins: " + coinCount);
            for (int i = 0; i < shopItems.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {shopItems[i].Name} - {shopItems[i].Cost} coins (+{shopItems[i].StatIncrease} Hunger)");
            }

            Console.Write("Choose item to buy: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= shopItems.Count)
            {
                var item = shopItems[index - 1];
                if (coinCount >= item.Cost)
                {
                    coinCount -= item.Cost;
                    itemManager.AddToInventory(item);
                    Console.WriteLine("Item purchased successfully!");
                }
                else
                {
                    Console.WriteLine("Not enough coins!");
                }
            }

            Console.WriteLine("Press Enter to return...");
            Console.ReadLine();
        }

        private void OnPetRemoved(Pet pet)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n!!! ALERT: {pet.GetName()} the {pet.PetType} has passed away...");
            Console.WriteLine("Your pet is dead oh no!");
            Console.ResetColor();
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        private async Task ShowLiveStats()
        {
            Console.Clear();
            Console.WriteLine("Press ENTER any time to stop viewing stats.\n");

            using CancellationTokenSource viewCts = new();
            Task statsTask = DisplayPetStatsLoop(viewCts.Token);

            Console.ReadLine();
            viewCts.Cancel();

            await statsTask;
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
                        Console.WriteLine($"{pet.GetName()} ({pet.PetType}) - Hunger: {pet.Hunger} | Sleep: {pet.Sleep} | Fun: {pet.Fun}");
                    }
                }

                Console.WriteLine($"\nTotal Coins: {coinCount}");
                Console.WriteLine("\n(This screen refreshes automatically every 3 seconds.)");
                await Task.Delay(3000);
            }
        }

        private void OnCoinEarned()
        {
            coinCount++;
        }
    }
}

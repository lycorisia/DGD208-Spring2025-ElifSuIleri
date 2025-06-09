using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MysticPets.Enums;
using MysticPets.Managers;
using MysticPets.Pets;
using MysticPets.Items;
using MysticPets.Helpers;

namespace MysticPets
{
    public class Game
    {
        private Pet _activePet;

        public Game(Pet pet)
        {
            _activePet = pet;
        }

        public Game()
        {
            
        }
        
        private PetManager petManager = PetManager.Instance;
        private ItemManager itemManager = new ItemManager();
        private int coinCount = 0;
        private bool hasAdopted = false;
        private DateTime lastFeedTime = DateTime.MinValue;
        private DateTime lastPlayTime = DateTime.MinValue;
        private DateTime lastSleepTime = DateTime.MinValue;
        private readonly TimeSpan actionCooldown = TimeSpan.FromSeconds(3);


        public void Start()
        {
            petManager.PetRemoved += OnPetRemoved;
            petManager.StartStatUpdates(OnCoinEarned);

            if (_activePet == null)
            {
                // No pet passed in → run normal adoption flow
                StartGameSequence(); // this includes AdoptPet(), etc.
            }
            else
            {
                // A pet was passed in → skip adoption and go to gameplay
                Console.Clear();
                Console.WriteLine($"🐾 Welcome back, {_activePet.Name} the {_activePet.PetType}!");
                RunPetLoop(_activePet);
            }
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
                Console.WriteLine("Let's go and find your perfect Mystical Pet!.");
                Console.WriteLine("\nChoose the type of pet you want to adopt:");

                Console.WriteLine("1. Pixie");
                Console.WriteLine("2. Hydra");
                Console.WriteLine("3. Dragon");
                Console.WriteLine("4. Unicorn");

                var choice = Console.ReadLine();

                Console.Write("\nGive your new pet a name: ");
                string petName = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        AdoptPet(new Pixie(petName));
                        break;
                    case "2":
                        AdoptPet(new Hydra(petName));
                        break;
                    case "3":
                        AdoptPet(new Dragon(petName));
                        break;
                    case "4":
                        AdoptPet(new Unicorn(petName));
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
                var pet = petManager.GetAllPets().First();
                DisplayPetAscii(pet);

                Console.WriteLine(@"
 _  _  ____  __ _  _  _ 
( \/ )(  __)(  ( \/ )( \
/ \/ \ ) _) /    /) \/ (
\_)(_/(____)\_)__)\____/                      
");
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
                        if (DateTime.Now - lastFeedTime < actionCooldown)
                        {
                            Console.WriteLine("⏳ You need to wait before feeding again.");
                            Console.ReadLine();
                        }
                        else
                        {
                            FeedPet();
                            lastFeedTime = DateTime.Now;
                        }
                        break;
                    case "2":
                        if (DateTime.Now - lastPlayTime < actionCooldown)
                        {
                            Console.WriteLine("⏳ You need to wait before playing again.");
                            Console.ReadLine();
                        }
                        else
                        {
                            petManager.GetAllPets().First().IncreaseFun(10);
                            lastPlayTime = DateTime.Now;
                        }
                        break;
                    case "3":
                        if (DateTime.Now - lastSleepTime < actionCooldown)
                        {
                            Console.WriteLine("⏳ You need to wait before sleeping again.");
                            Console.ReadLine();
                        }
                        else
                        {
                            petManager.GetAllPets().First().IncreaseSleep(10);
                            lastSleepTime = DateTime.Now;
                        }
                        break;
                    case "4":
                        OpenShop();
                        break;
                    case "5":
                        petManager.StopStatUpdates(); // ✅ important!
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
            petManager.StopStatUpdates();
            Console.WriteLine("Press Enter to return to the main menu...");
            Console.ReadLine();
        }

        private void AdoptPet(Pet pet)
        {
            petManager.AdoptPet(pet);
            PetManager.Instance.SetActivePet(pet);
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
            Console.WriteLine($"Rest in Peace, {pet.GetName()}.");
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
        private void DisplayPetAscii(Pet pet)
        {
            Console.WriteLine();

            switch (pet.PetType)
            {
                case PetType.Pixie:
                    Console.WriteLine(@"
.'.         .'.
|  \       /  |
'.  \  |  /  .'
  '. \\|// .'
    '-- --'
    .'/|\'.
   '..'|'..'                     
");
                    break;
                case PetType.Hydra:
                    Console.WriteLine(@"
 .             _.--._       /|
        .    .'()..()`.    / /
            ( `-.__.-' )  ( (    .
   .         \        /    \ \
       .      \      /      ) )        .
            .' -.__.- `.-.-'_.'
 .        .'  /-____-\  `.-'       .
          \  /-.____.-\  /-.
           \ \`-.__.-'/ /\|\|           .
          .'  `.    .'  `.
          |/\/\|    |/\/\|                   
");
                    break;
                case PetType.Dragon:
                    Console.WriteLine(@"
            \||/
                |  @___oo
      /\  /\   / (__,,,,|
     ) /^\) ^\/ _)
     )   /^\/   _)
     )   _ /  / _)
 /\  )/\/ ||  | )_)
<  >      |(,,) )__)
 ||      /    \)___)\
 | \____(      )___) )___
  \______(_______;;; __;;;                     
");
                    break;
                case PetType.Unicorn:
                    Console.WriteLine(@"
       /
               ,.. /
             ,'   ';
  ,,.__    _,' /';  .
 :','  ~~~~    '. '~
:' (   )         )::,
'. '. .=----=..-~  .;'
 '  ;'  ::   ':.  '""
   (:   ':    ;)
    \\   '""  ./
     '""      '""                     
");
                    break;
            }

            Console.WriteLine();
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
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write($"{pet.GetName()} ");
                        Console.ResetColor();
                        Console.WriteLine($"({pet.PetType}) - Hunger: {pet.Hunger} | Sleep: {pet.Sleep} | Fun: {pet.Fun}");

                        List<string> lowStats = new();

                        if (pet.Hunger < 10) lowStats.Add("Hunger");
                        if (pet.Sleep < 10) lowStats.Add("Sleep");
                        if (pet.Fun < 10) lowStats.Add("Fun");

                        if (lowStats.Count > 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"⚠ WARNING: Low {string.Join(", ", lowStats)}");
                            Console.ResetColor();
                        }

                    }

                }

                Console.WriteLine($"\nTotal Coins: {coinCount}");
                Console.WriteLine("\n(This screen refreshes automatically every 3 seconds.)");
                await Task.Delay(3000);
            }
        }
private void RunPetLoop(Pet pet)
{
    while (!pet.IsDead())
    {
        Console.Clear();
        DisplayPetAscii(pet);

        Console.WriteLine(@"
 _  _  ____  __ _  _  _ 
( \/ )(  __)(  ( \/ )( \
/ \/ \ ) _) /    /) \/ (
\_)(_/(____)\_)__)\____/                      
");
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
                if (DateTime.Now - lastFeedTime < actionCooldown)
                {
                    Console.WriteLine("⏳ You need to wait before feeding again.");
                    Console.ReadLine();
                }
                else
                {
                    FeedPet();
                    lastFeedTime = DateTime.Now;
                }
                break;

            case "2":
                if (DateTime.Now - lastPlayTime < actionCooldown)
                {
                    Console.WriteLine("⏳ You need to wait before playing again.");
                    Console.ReadLine();
                }
                else
                {
                    pet.IncreaseFun(10);
                    lastPlayTime = DateTime.Now;
                }
                break;

            case "3":
                if (DateTime.Now - lastSleepTime < actionCooldown)
                {
                    Console.WriteLine("⏳ You need to wait before sleeping again.");
                    Console.ReadLine();
                }
                else
                {
                    pet.IncreaseSleep(10);
                    lastSleepTime = DateTime.Now;
                }
                break;

            case "4":
                OpenShop();
                break;

            case "5":
                petManager.StopStatUpdates();
                return;

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
    petManager.StopStatUpdates();
    Console.WriteLine("Press Enter to return to the main menu...");
    Console.ReadLine();
    petManager.StopStatUpdates();
 

}


        private void OnCoinEarned()
        {
            coinCount++;
        }
    }
}

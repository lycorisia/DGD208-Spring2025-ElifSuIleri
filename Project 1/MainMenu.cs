using System;
using MysticPets.Managers;
using static MysticPets.Game;


namespace MysticPets.UI
{
    public class MainMenu
    {
        private readonly Game _game;
        private bool _gameStarted;

        public MainMenu(Game game)
        {
            _game = game;
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                DisplayTitle();
                DisplayOptions();

                Console.Write("\nChoose an option: ");
                string input = Console.ReadLine()?.Trim().ToLower();

                switch (input)
                {
                    case "1":
                        if (!_gameStarted)
                        {
                            _gameStarted = true;
                            _game.Start();
                        }
                        else
                        {
                            ShowContinueMenu();
                        }
                        break;

                    case "2":
                        ShowProjectInfo();
                        break;

                    case "3":
                        Console.WriteLine("Goodbye!");
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Press any key to try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void DisplayTitle()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
 ██████   ██████ █████ █████  █████████  ███████████ █████   █████████ 
░░██████ ██████ ░░███ ░░███  ███░░░░░███░█░░░███░░░█░░███   ███░░░░░███
 ░███░█████░███  ░░███ ███  ░███    ░░░ ░   ░███  ░  ░███  ███     ░░░ 
 ░███░░███ ░███   ░░█████   ░░█████████     ░███     ░███ ░███         
 ░███ ░░░  ░███    ░░███     ░░░░░░░░███    ░███     ░███ ░███         
 ░███      ░███     ░███     ███    ░███    ░███     ░███ ░░███     ███
 █████     █████    █████   ░░█████████     █████    █████ ░░█████████ 
░░░░░     ░░░░░    ░░░░░     ░░░░░░░░░     ░░░░░    ░░░░░   ░░░░░░░░░  
                                                                       
 ███████████  ██████████ ███████████  █████████                        
░░███░░░░░███░░███░░░░░█░█░░░███░░░█ ███░░░░░███                       
 ░███    ░███ ░███  █ ░ ░   ░███  ░ ░███    ░░░                        
 ░██████████  ░██████       ░███    ░░█████████                        
 ░███░░░░░░   ░███░░█       ░███     ░░░░░░░░███                       
 ░███         ░███ ░   █    ░███     ███    ░███                       
 █████        ██████████    █████   ░░█████████                        
░░░░░        ░░░░░░░░░░    ░░░░░     ░░░░░░░░░                         
");
            Console.ResetColor();
        }

        private void DisplayOptions()
        {
            Console.WriteLine("1. " + (_gameStarted ? "Continue" : "Start Game"));
            Console.WriteLine("2. Project Info");
            Console.WriteLine("3. Exit");
        }

        private void ShowProjectInfo()
        {
            Console.Clear();
            Console.WriteLine("MysticPets – A virtual pet care simulation game created for DGD208 Game Programming by Elif Su İleri - 2305041041.");
            Console.WriteLine("Take care of magical pets by feeding, playing, and letting them sleep. Earn coins, buy items, and keep your pet alive");
            Console.WriteLine("And don't you dare let them die");
            Console.WriteLine("\nCredits: r/csharp, Chat GPT (Sadly), hhhhertz and ApexNebulae on x/Twitter, Microsoft's C# Tutorials and Olcay Kalyoncuoğlu's Class on C# at Udemy.com");
            Console.ReadKey();
        }

        private void ShowContinueMenu()
        {
            while (true)
            {
                Console.Clear();
                var pets = PetManager.Instance.GetAllPets();

                Console.WriteLine("=== Your Pets ===\n");

                for (int i = 0; i < pets.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {pets[i].Name} the {pets[i].PetType}");
                }

                int adoptOption = pets.Count + 1;
                int deleteOption = pets.Count + 2;
                int backOption = pets.Count + 3;

                if (pets.Count < 4)

                Console.WriteLine($"{adoptOption}. [ + ] Adopt a New Pet");
                Console.WriteLine($"{deleteOption}. [ x ] Delete a Pet");
                Console.WriteLine($"{backOption}. [ <- ] Back to Main Menu");


                Console.Write("\nChoose an option: ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out int option))
                {
                    if (option >= 1 && option <= pets.Count)
                    {
                        var selectedPet = pets[option - 1];
                        PetManager.Instance.SetActivePet(selectedPet);
                        Game game = new Game(selectedPet);
                        game.Start();
                        return;
                    }
                    else if (option == adoptOption && pets.Count < 4)
                    {
                        Game game = new Game(); // triggers adoption
                        game.Start();
                        return;
                    }
                    else if (option == deleteOption)
                    {
                        DeletePetMenu();
                    }
                    else if (option == backOption)
                    {
                        return;
                    }
                    else
                    {
                        Console.WriteLine("❌ Invalid choice. Press Enter to try again.");
                        Console.ReadLine();
                    }
                }
                else
                {
                    Console.WriteLine("❌ Invalid input. Press Enter to try again.");
                    Console.ReadLine();
                }
            }
        }


        private void DeletePetMenu()
        {
            var pets = PetManager.Instance.GetAllPets();

            if (pets.Count == 0)
            {
                Console.WriteLine("You have no pets to delete. Press Enter to return.");
                Console.ReadLine();
                return;
            }

            Console.Clear();
            Console.WriteLine("🗑️ Choose a pet to delete:\n");

            for (int i = 0; i < pets.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {pets[i].Name} the {pets[i].PetType}");
            }

            Console.WriteLine($"{pets.Count + 1}. Cancel");

            Console.Write("\nEnter your choice: ");
            string input = Console.ReadLine();

            if (int.TryParse(input, out int index) && index >= 1 && index <= pets.Count)
            {
                var petToRemove = pets[index - 1];
                PetManager.Instance.DeletePet(petToRemove);
                Console.WriteLine($"{petToRemove.Name} has been deleted. Press Enter to continue.");
            }
            else
            {
                Console.WriteLine("Cancelled. Press Enter to return.");
            }

            Console.ReadLine();
        }
    }
}

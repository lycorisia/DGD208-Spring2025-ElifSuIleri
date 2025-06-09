using System;
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
                            _game.Continue(); 
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
            Console.WriteLine("MysticPets – A virtual pet simulation game created for DGD208 Game Programming by Elif Su İleri.");
            Console.WriteLine("Take care of magical pets by feeding, playing, and letting them sleep. Earn coins, buy items, and keep your pet alive");
            Console.WriteLine("Earn coins, buy items, and keep your pet alive!");
            Console.WriteLine("\nCredits: r/csharp, Chat GPT, hhhhertz and ApexNebulae on x/Twitter and Olcay Kalyoncuoğlu's Class on C# at Udemy.com");
            Console.ReadKey();
        }
    }
}

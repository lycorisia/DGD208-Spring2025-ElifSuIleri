using System;
using MysticPets.UI;
using static MysticPets.Game;

namespace MysticPets
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            MainMenu mainMenu = new MainMenu(game);
            mainMenu.Run();
        }
    }
}
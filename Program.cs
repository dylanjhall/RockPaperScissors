using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;


namespace RockPaperScissorsGame
{
    internal class Program
    {
        public static List<Player> Players = new List<Player>();
        public static OptionType? LastWin;
        private static int UserId = 0;
        private static bool isFirstPlayer = true;

        static void Main(string[] args)
        {
            Console.Clear();
            var mode = PlayMode.Computer;
            var hasWinner = false;
            string playerName = string.Empty;
            Console.WriteLine("Welcome Player 1!");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Please enter your name: ");
            playerName = Console.ReadLine();
            Console.WriteLine($"Thank You,{playerName}");
            var player1 = CreatePlayer(playerName, PlayerType.Human);
            player1.UserConsoleColor = ConsoleColor.Yellow;
            Players.Add(player1);

            Console.WriteLine($"{playerName} Do you want to play against the computer? Enter Y or N (Default is Yes for Computer)");
            Console.WriteLine();
            ValidateModeResponse(ref mode);
            Console.WriteLine();
            Console.WriteLine($"You chose {mode} mode");
            Console.WriteLine();
            Console.WriteLine("Let's play!");
            Console.WriteLine();
            Console.WriteLine();
            Player player2;
            if (mode == PlayMode.Computer)
            {
                player2 = CreatePlayer("Hal3000", PlayerType.Computer);
                player2.UserConsoleColor = ConsoleColor.DarkMagenta;
                Players.Add(player2);

            }
            else
            {
                Console.WriteLine("Please enter your name: ");
                playerName = Console.ReadLine();
                Console.WriteLine($"Thank You,{playerName}");
                player2 = CreatePlayer(playerName, PlayerType.Human);
                Players.Add(player2);
            }

            var round = new Round();
            while (!hasWinner)
            {


                while (player1.Wins < 3 && player2.Wins < 3)
                {

                    while (round.Turns < 2)
                    {
                        var p = ToggleTurn() ? Players.Single(pl => pl.Id == 1) : Players.Single(pl => pl.Id == 2);
                        Console.ForegroundColor = (ConsoleColor)p.UserConsoleColor;
                        var ans = QueryPlayer(p);
                        if (!ValidAnswer(ans))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Invalid value, try again");
                            Console.WriteLine();
                            continue;
                        }
                        Console.WriteLine($"{p.Name} chose {ans}");
                        _ = (p.Id == 1) ? player1.currentSelection = ans : player2.currentSelection = ans;
                        round.Turns++;
                        Console.WriteLine();
                    }
                    round.Turns = 0;

                    DetermineWinner(ref player1, ref player2);
                    Console.WriteLine();

                }
                Console.WriteLine("******************");
                hasWinner = true;
            }

            Console.WriteLine("************* Game Over ******************");
            Console.ResetColor();
            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey(true);

        }

        private static bool ValidAnswer(OptionType ans)
        {
            return Enum.IsDefined(typeof(OptionType), ans);
        }

        private static Player CreatePlayer(string playerName, PlayerType playerType)
        {
            var player = new Player()
            {
                Id = GetNewUserId(),
                Name = playerName,
                playerType = playerType
            };

            return player;
        }

        static OptionType QueryPlayer(Player player)
        {
            var answer = 0;
            //Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{player.Name}, Please choose from the following options by selecting a number:");
            Console.WriteLine(String.Format("|{0,5}|{1,5}|{2,5}|", OptionType.Rock, OptionType.Paper, OptionType.Scissors));
            Console.WriteLine(String.Format("|{0,5}|{1,5}|{2,5}|", (int)OptionType.Rock, (int)OptionType.Paper, (int)OptionType.Scissors));
            Console.ForegroundColor = ConsoleColor.Blue;
            if (player.playerType == PlayerType.Computer)
            {
                if (LastWin == null)
                {
                    answer = GenerateRandomNumber();
                }
                else
                {
                    answer = (int)LastWin;
                }
            }
            else
            {
                Console.Write("Answer: ");
                answer = Int32.Parse(Console.ReadLine());
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            return (OptionType)answer;
        }

        static void ValidateModeResponse(ref PlayMode mode)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            var response = Console.ReadKey().KeyChar;
            while (!(char.ToUpper(response) == 'Y' || char.ToUpper(response) == 'N'))
            {
                Console.Error.WriteLine("Error: Please choose 'Y' for Yes or 'N' for No");
                Console.WriteLine();
                response = Console.ReadKey().KeyChar;
            }

            mode = char.ToUpper(response) == 'Y' ? PlayMode.Computer : PlayMode.PlayerToPlayer;

        }

        static int GenerateRandomNumber()
        {
            var n = RandomNumberGenerator.GetInt32(1,4);
            return n;
        }

        static bool ToggleTurn()
        {
            isFirstPlayer = !isFirstPlayer;
            return isFirstPlayer;
        }

        static void DetermineWinner(ref Player player1, ref Player player2)
        {
            // If tied, no one wins this round
            if (player1.currentSelection == player2.currentSelection)
            {
                Console.WriteLine("It's a tie! Try again");
                return;
            }

            switch (player1.currentSelection)
            {
                case OptionType.Rock:

                    if (player2.currentSelection == OptionType.Paper) // "PAPER"
                    {
                        Console.WriteLine($"{player2.Name} Wins!");
                        LastWin = player2.currentSelection;
                        player2.Wins++;                        
                    }
                    else
                    {
                        Console.WriteLine($"{player1.Name} Wins!");
                        LastWin = player1.currentSelection;
                        player1.Wins++;
                    }
                    break;
                case OptionType.Paper:
                    if (player2.currentSelection == OptionType.Rock)
                    {
                        Console.WriteLine($"{player1.Name} Wins!");
                        LastWin = player1.currentSelection;
                        player1.Wins++;
                    }
                    else
                    {
                        Console.WriteLine($"{player2.Name} Wins!");
                        LastWin = player2.currentSelection;
                        player2.Wins++;
                    }
                    break;
                case OptionType.Scissors:
                    if (player2.currentSelection == OptionType.Rock) // rock
                    {
                        Console.WriteLine($"{player2.Name} Wins!");
                        LastWin = player2.currentSelection;
                        player2.Wins++;
                    }
                    else
                    {
                        Console.WriteLine($"{player1.Name} Wins!");
                        LastWin = player1.currentSelection;
                        player1.Wins++;
                    }

                    break;
            }


        }

        private static int GetNewUserId() => ++UserId;

    }

    internal class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public PlayerType playerType { get; set; }

        public OptionType currentSelection { get; set; }

        public int Wins { get; set; }

        public ConsoleColor UserConsoleColor { get; set; }

    }




    internal class Round
    {
        public int Turns { get; set; } = 0;
    }

    #region Enums
    internal enum PlayerType
    {
        Computer,
        Human
    }

    internal enum OptionType
    {
        Rock = 1,
        Paper = 2,
        Scissors = 3
    }

    internal enum PlayMode
    {
        PlayerToPlayer,
        Computer
    }
    #endregion
}

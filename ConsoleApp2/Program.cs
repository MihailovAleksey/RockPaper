using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No parameters passed.");
                Console.WriteLine("Please provide not even count of strings.");
                return;
            }

            if (args.Length % 2 != 1)
            {
                Console.WriteLine("Strings count is even.");
                Console.WriteLine("Please provide not even count of strings.");
                return;
            }

            if (args.Distinct().Count() != args.Length)
            {
                Console.WriteLine("Not all strings are unique.");
                Console.WriteLine("Please provide unique strings");
                return;
            }

            int pcChoice = MakeDecision(args.Length - 1);
            string hmacKey;
            byte[] secretKey = new byte[128];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(secretKey);
                using (HMACSHA256 hmac = new HMACSHA256(secretKey))
                {
                    var resultHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(args[pcChoice]));
                    hmacKey = BitConverter.ToString(hmac.Key).Replace("-", string.Empty);
                    Console.WriteLine("HMAC: " + BitConverter.ToString(resultHash).Replace("-", string.Empty));
                }
            }

            int playerChoice = -1;
            while (playerChoice == -1)
            {
                ShowMenu(args);
                playerChoice = ReadKey(args.Length);
            }

            if (playerChoice == 0)
            {
                return;
            }

            Console.WriteLine($"Your move: {args[playerChoice - 1]}");
            Console.WriteLine($"Computer move: {args[pcChoice]}");
            CalculateWin(playerChoice - 1, pcChoice, args.Length);
            Console.WriteLine($"HMAC key: {hmacKey}");
        }

        public static void CalculateWin(int playerChoice, int pcChoice, int count)
        {
            if (playerChoice == pcChoice)
            {
                Console.WriteLine("Tie");
                return;
            }

            int result = (playerChoice + (count / 2)) % count;

            if (IsPcBetween(playerChoice, pcChoice, result))
            {
                Console.WriteLine("You lose");
            }
            else
            {
                Console.WriteLine("You win");
            }
        }

        private static bool IsPcBetween(int playerChoice, int pcChoice, int end)
        {
            if (end < playerChoice)
            {
                if ((pcChoice > playerChoice && pcChoice >= end) || (pcChoice < playerChoice && pcChoice <= end))
                {
                    return true;
                }
            }
            else
            {
                if (pcChoice > playerChoice && pcChoice <= end)
                {
                    return true;
                }
            }

            return false;
        }

        private static void ShowMenu(string[] turns)
        {
            for (int i = 0; i < turns.Length; i++)
            {
                Console.WriteLine($"{i + 1} - {turns[i]}");
            }
            Console.WriteLine("0 - Exit");
            Console.Write("Enter your move: ");
        }

        private static int ReadKey(int maxValue)
        {
            if (!int.TryParse(Console.ReadLine(), out int userChoice))
            {
                Console.WriteLine("Invalid input.");
                return -1;
            }
            else if (userChoice < 0 || userChoice > maxValue)
            {
                Console.WriteLine($"Number not in range [{0};{maxValue}]");
                return -1;
            }

            return userChoice;
        }

        private static int MakeDecision(int max)
        {
            Random random = new Random();
            int turnNumber = random.Next(0, max);
            return turnNumber;
        }
    }
}

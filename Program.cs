namespace consoleui
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            Console.Title = "testing consoleui";
            var cui = ConsoleUI.Instance;
            try
            {
                while (true)
                {
                    var c = Console.ReadKey(true);
                    switch (c.Key)
                    {
                        case ConsoleKey.Escape:
                            return;
                        case ConsoleKey.R:
                            cui.Clear("red", ConsoleColor.DarkRed);
                            break;
                        case ConsoleKey.B:
                            cui.Clear("blue", ConsoleColor.DarkBlue);
                            break;
                        case ConsoleKey.G:
                            cui.Clear("green", ConsoleColor.DarkGreen);
                            break;
                        case ConsoleKey.Z:
                            cui.Clear("");
                            break;
                        case ConsoleKey.D0:
                            Console.ResetColor();
                            break;
                        case ConsoleKey.P:
                            for (int i = 0; i < 20; ++i)
                                Console.WriteLine($"line {i}");
                            break;
                        case ConsoleKey.D:
                            Console.BufferHeight *= 2;
                            break;
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.PageUp:
                        case ConsoleKey.PageDown:
                        case ConsoleKey.DownArrow:
                            cui.GoUp(c.Key);
                            break;
                    }
                }
            }
            finally
            {
                Console.Write("press key to end program");
                Console.ReadKey(true);
            }
        }
    }
}
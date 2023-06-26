namespace consoleui
{
    public class ConsoleUI
    {
        private static Lazy<ConsoleUI> _instance = new Lazy<ConsoleUI>();
        private int maxHeight = 8000;
        private int maxIdle = 50;

        public static ConsoleUI Instance { get => _instance.Value; }


        public void Clear(string indictor, ConsoleColor bgColor = ConsoleColor.Black, ConsoleColor fgColor = ConsoleColor.Gray)
        {
            Console.BufferWidth = Console.WindowWidth;
            Console.CursorLeft = 0;
            if (Console.BufferHeight > maxHeight)
                Console.BufferHeight = maxHeight;
            var oldCursorTop = Console.CursorTop;
            var oldWindowTop = Console.WindowTop;
            try
            {
                int newHeight = Console.CursorTop + Console.WindowHeight + 2;
                if (Console.BufferHeight < newHeight)
                    Console.BufferHeight = newHeight;
            }
            catch
            {
                Console.WriteLine("no new BufferHeight");
            }
            Console.BackgroundColor = bgColor;
            Console.ForegroundColor = fgColor;
            var es = new String(' ', Console.WindowWidth * Console.WindowHeight);
            Console.Write(es);
            if (!string.IsNullOrWhiteSpace(indictor))
            {
                Console.CursorLeft = Console.WindowWidth - indictor.Length;
                Console.Write(indictor);
            }
            Console.CursorLeft = 0;
            Console.CursorTop = oldCursorTop;
        }

        internal void GoUp(ConsoleKey key)
        {
            int oldCursorTop = Console.CursorTop;
            int wh = Console.WindowHeight;
            int idle = 0;
            bool stop=false;
            while (idle < maxIdle)
            {
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        if (Console.CursorTop > 0)
                            Console.CursorTop -= 1;
                        break;
                    case ConsoleKey.DownArrow:
                        if (Console.CursorTop < Console.BufferHeight - 1)
                            Console.CursorTop += 1;
                        break;
                    case ConsoleKey.PageUp:
                        if (Console.CursorTop > wh / 2)
                            Console.CursorTop -= wh / 2;
                        break;
                    case ConsoleKey.PageDown:
                        if (Console.CursorTop < Console.BufferHeight - wh / 2 - 1)
                            Console.CursorTop += wh / 2;
                        break;
                    default:
                        stop= true;
                        break;
                }
                if (stop)
                    break;
                for (idle = 0; idle < maxIdle; idle++)
                {
                    if (Console.KeyAvailable)
                    {
                        key = Console.ReadKey(true).Key;
                        break;
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            Console.CursorTop = oldCursorTop;
            Console.CursorLeft = 0;
        }
    }
}
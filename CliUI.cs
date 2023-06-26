namespace consoleui
{
    public class CliUI
    {
        private static Lazy<CliUI> _instance = new Lazy<CliUI>();
        private int maxHeight = 8000;
        private int maxIdle = 50;

        /// <summary>
        /// Contains all possible commands
        /// </summary>
        private static Dictionary<string, CliAction> commands = new Dictionary<string, CliAction>();
        /// <summary>
        /// recent commands or recently added are on the top of the list
        /// </summary>
        private static int maxPreference = 0;
        /// <summary>
        /// also those characters are added to the search string
        /// </summary>
        private static Char[] okChars = new char[] { ' ', '.', '_' };
        public static CliUI Instance { get => _instance.Value; }

        /// <summary>
        /// keeping track of writings, may need to write another header if something else wrote in the mean time
        /// </summary>
        private static uint consolePos = 0;
        public static uint ConsolePos { get => consolePos; }

        private static Queue<DateTime> outputDelay = new Queue<DateTime>();

        internal static int Add(string cmd, Action action, int p)
        {
            if (string.IsNullOrWhiteSpace(cmd))
                return p;
            if (p == 0)
                p = ++maxPreference;
            else if (p == 1)
                p = maxPreference;
            lock (commands)
            {
                commands[cmd] = new CliAction(action, p);
            }
            return p;
        }

        internal void Remove(string cmd)
        {
            lock (commands)
            {
                commands.Remove(cmd);
            }
        }

        /// <summary>
        /// sorts the commands according to preference (higher first), then alphabetically
        /// </summary>
        /// <param name="cmd1"></param>
        /// <param name="cmd2"></param>
        /// <returns>-1,0,1 like Comparison</returns>
        private static int SortCommands(string cmd1, string cmd2)
        {
            var ca1 = commands[cmd1];
            var ca2 = commands[cmd2];
            if (ca1.p > ca2.p)
                return -1;
            if (ca2.p > ca1.p)
                return 1;
            return cmd1.CompareTo(cmd2);
        }

        public static void Write(string msg, int delay = 2000, bool newLine = true, ConsoleColor fg = ConsoleColor.Gray, ConsoleColor bg = ConsoleColor.Black)
        {
            lock (Console.Out)
            {
                ++consolePos;
                var wh = Console.WindowHeight - 3;
                while(outputDelay.Count >= wh)
                {
                    var d=outputDelay.Dequeue();
                    while(DateTime.Now < d)
                    {
                        if (Console.KeyAvailable)
                            break;
                        Thread.Sleep(100);
                    }
                }
                outputDelay.Enqueue(DateTime.Now.AddMilliseconds(delay));
                Console.ForegroundColor = fg;
                Console.BackgroundColor = bg;
                if (newLine)
                    Console.WriteLine(msg);
                else
                    Console.Write(msg);
            }
        }

        public void Clear(string indictor, ConsoleColor bgColor = ConsoleColor.Black, ConsoleColor fgColor = ConsoleColor.Gray)
        {
            lock (Console.Out)
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
        }

        internal void GoUp(ConsoleKey key)
        {
            lock (Console.Out)
            {
                int oldCursorTop = Console.CursorTop;
                int wh = Console.WindowHeight;
                int idle = 0;
                bool stop = false;
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
                            stop = true;
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
                    // if no key was pressed, then idle==maxIdle
                }
                Console.CursorTop = oldCursorTop;
                Console.CursorLeft = 0;
            }
        }

        /// <summary>
        /// Creates a list of matching positions
        /// </summary>
        /// <param name="heystack">the full string to match</param>
        /// <param name="keystrokes">the needle to find</param>
        /// <returns>a list of matching positions</returns>
        private static List<int> CheckString(string heystack, string keystrokes)
        {
            var l = new List<int>();
            int pos = -1;
            foreach (char c in keystrokes)
            {
                pos = heystack.IndexOf(c, pos + 1);
                if (pos < 0)
                    return l;
                l.Add(pos);
            }
            return l;
        }

    }
    /// <summary>
    /// datastructure that holds the action and preference value
    /// </summary>
    internal class CliAction
    {
        internal Action action;
        internal int p;

        public CliAction(Action action, int p)
        {
            this.action = action;
            this.p = p;
        }
    }
}
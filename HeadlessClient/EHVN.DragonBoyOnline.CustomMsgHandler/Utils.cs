namespace EHVN.DragonBoyOnline.CustomMsgHandler
{
    internal static class Utils
    {
        internal static void LogException(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ERROR] {ex}");
            Console.ResetColor();
        }
    }
}

// See https://aka.ms/new-console-template for more information
using ConsoleApp1.src.Model;
using static ConsoleApp1.src.Utils.Constants;
public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Starting the application! Click anywhere to get the details");
        MouseHook mouseHook = new MouseHook(WH_MOUSE_LL, WM_LBUTTONDOWN);
    }
}
using System;
using Gtk;

namespace Game1
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main(string[] args)
        {
            Application.Init();
            var win = new MainWindow();
            win.Show();
            Application.Run();
        }
    }
}

// Xamarin is the definition of retarded so this is needed.
namespace Mono.Unix
{
    public static class Catalog
    {
        public static string GetString(string value)
        {
            return value;
        }
    }
}
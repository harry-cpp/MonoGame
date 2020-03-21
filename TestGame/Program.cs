using System;
using WebAssembly;
using WebGLDotNET;

namespace TestGame
{
    class Program
    {
        static async void Main()
        {
            var g = new Game1();
            g.Run();
        }
    }
}

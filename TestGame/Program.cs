using System;
using WebAssembly;
using WebGLDotNET;

namespace Platformer2D
{
    class Program
    {
        static async void Main()
        {
            var g = new PlatformerGame();
            g.Run();
        }
    }
}

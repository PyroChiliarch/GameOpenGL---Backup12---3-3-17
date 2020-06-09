using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace GameOpenGL
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game(500, 500);
            game.Run();
        }
    }
}

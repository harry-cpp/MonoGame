using static Retyped.dom;
using Microsoft.Xna.Framework;

namespace Platformer2D
{
    public class App
    {
        private static Game game;

        public static void Main()
        {
            var div = new HTMLDivElement();
            div.style.width = "800px";
            div.style.height = "480px";
            document.body.appendChild(div);

            var button = new HTMLButtonElement();
            button.innerHTML = "Click on game area to start it!";
            button.style.width = "100%";
            button.style.height = "100%";
            button.style.backgroundColor = "#6495ED";
            button.style.color = "#ffffff";
            button.style.fontSize = "20px";
            div.appendChild(button);

            button.onclick = (ev) => 
            {
                div.removeChild(button);

                var canvas = new HTMLCanvasElement();
                canvas.style.width = "100%";
                canvas.style.height = "100%";
                canvas.id = "monogamecanvas";
                div.appendChild(canvas);

                game = new PlatformerGame();
                game.Run();
            };
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TestGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _tex;
        private Song _song;
        bool loading = true;
        SpriteFont font;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            LoadContentAsync();
        }

        private async void LoadContentAsync()
        {
            _song = await Content.LoadAsync<Song>("awake");
            _tex = await Content.LoadAsync<Texture2D>("hacker");
            font = await Content.LoadAsync<SpriteFont>("font");

            MediaPlayer.Play(_song);

            loading = false;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        int frame = 0;
        int frameCounter = 0;
        int _lastTime = 0;

        protected override void Draw(GameTime gameTime)
        {
            if (loading)
            {
                GraphicsDevice.Clear(Color.YellowGreen);
                return;
            }

            if (gameTime.TotalGameTime.Seconds > _lastTime)
            {
                _lastTime = gameTime.TotalGameTime.Seconds;
                frame = frameCounter;
                frameCounter = 0;
            }
            else
            {
                frameCounter++;
            }

            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _spriteBatch.Draw(_tex, Vector2.Zero, Color.White);
            _spriteBatch.DrawString(font, "Fps: " + frame + System.Environment.NewLine + "Well spritefonts are working as well...", Vector2.Zero, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

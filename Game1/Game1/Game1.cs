using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using Microsoft.Xna.Framework.Media;

namespace Game1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        //Calling all our variables for the entirety of the code (dowsn't include object variables, only the object themselves)
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Rectangle fenetre;
        Rectangle background;
        Texture2D wallpaper;
        GameObject heros;
        GameObject ennemi = new GameObject();
        GameObject projectile;
        GameObject missile;

        public Game1()
        {
            //Get gpu
            graphics = new GraphicsDeviceManager(this);
            //Set content folder to "content"
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            //Get les valeurs de la fenetre + fullscreen
            this.graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.DisplayMode.Width;
            this.graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.DisplayMode.Height;
            this.graphics.ToggleFullScreen();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //set window values
            fenetre = graphics.GraphicsDevice.Viewport.Bounds;
            fenetre.Width = graphics.GraphicsDevice.Viewport.Width;
            fenetre.Height = graphics.GraphicsDevice.Viewport.Height;

            //Calling our hero object
            heros = new GameObject();
            heros.estVivant = true;
            heros.vitesse = 10; //Change speed here
            heros.sprite = Content.Load<Texture2D>("Mario.png"); //Change texture here
            heros.position = heros.sprite.Bounds;
            heros.position.Offset((fenetre.Width / 2), (fenetre.Height / 2));

            //calling our ennemy object
            ennemi = new GameObject();
            ennemi.estVivant = true;
            ennemi.vitesse = 20; //Change speed here
            ennemi.sprite = Content.Load<Texture2D>("Cloud.png");
            ennemi.position = ennemi.sprite.Bounds;

            //calling our projectile object
            projectile = new GameObject();
            projectile.estVivant = true;
            projectile.vitesse = 20; //Change speed here
            projectile.sprite = Content.Load<Texture2D>("Shell.png"); //Change texture here
            projectile.position = projectile.sprite.Bounds;

            //Calling our missile object
            missile = new GameObject();
            missile.estVivant = false;
            missile.vitesse = 20; //Change speed here
            missile.sprite = Content.Load<Texture2D>("GreenShell.png"); //Change texture here
            missile.position = missile.sprite.Bounds;
            missile.position.Offset(-64,0);

            //setting wallpaper
            background = new Rectangle(0, 0, fenetre.Width, fenetre.Height);
            wallpaper = Content.Load<Texture2D>("background.png"); //Change texture here
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //User input
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                heros.position.X += heros.vitesse;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                heros.position.X -= heros.vitesse;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                heros.position.Y += heros.vitesse;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                heros.position.Y -= heros.vitesse;
            }

            //Calling our functions below
            UpdateMissile();
            UpdateProjectile();
            UpdateEnnemi();
            UpdateHeros();
            base.Update(gameTime);
        }

        /// <summary>
        /// Hero update code
        /// </summary>
        protected void UpdateHeros()
        {
            //only do this if hero is alive
            if (heros.estVivant == true)
            {
                //if projectile touches hero sprite, render hero "dead"
                if (heros.position.Intersects(projectile.position))
                {
                    heros.estVivant = false;
                }

                //Bound for the bottom of the screen
                if (heros.position.X > fenetre.Width - heros.position.Width)
                {
                    heros.position.X = fenetre.Width - heros.position.Width;
                }

                //Bound for the top of the screen
                if (heros.position.Y > fenetre.Height - heros.position.Height)
                {
                    heros.position.Y = fenetre.Height - heros.position.Height;
                }

                //Bound for the Right side of screen
                if (heros.position.X < 0)
                {
                    heros.position.X = 0;
                }

                //Bound for bottom of the screen
                if (heros.position.Y < 0)
                {
                    heros.position.Y = 0;
                }
            }
            else
            {
                //if hero is dead, draw him off screen
                heros.position.X = -500;
                heros.position.Y = 0;
            }
        }

        /// <summary>
        /// Ennemy update code
        /// </summary>
        protected void UpdateEnnemi()
        {
            //only do this if ennemy is alive
            if (ennemi.estVivant == true)
            {
                //if missile touches ennemy sprite, render ennemy "dead"
                if (ennemi.position.Intersects(missile.position))
                {
                    ennemi.estVivant = false;
                }

                //Giving ennemy the movement
                ennemi.position.X += (int)ennemi.vitesse;

                int maxX = fenetre.Width - (ennemi.sprite.Width);
                int maxY = fenetre.Height - (ennemi.sprite.Height);

                //Bounce on contact of either wall
                if (ennemi.position.X > maxX || ennemi.position.X < 0)
                {
                    ennemi.vitesse = -(ennemi.vitesse);
                }
            }
            else
            {
                //if ennemy is dead, draw him off screen
                ennemi.position.X = -500;
                ennemi.position.Y = 0;
            }
        }

        /// <summary>
        /// Projectile code
        /// </summary>
        protected void UpdateProjectile()
        {
            //if the projectile hits the bottom of the screen, send another one at the ennemy's position's feet
            if (projectile.position.Y > fenetre.Bottom)
            {
                projectile.position.X = ennemi.position.X;
                projectile.position.Y = ennemi.position.Y + ennemi.sprite.Height;
            }

            //Giving the projectile its speed (on the "y" axis)
            projectile.position.Y += projectile.vitesse;
        }

        /// <summary>
        /// Missile code
        /// </summary>
        protected void UpdateMissile()
        {
            //if space is pressed shoot a projectile from mario to the top of the window
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                missile.estVivant = true;
                //Give the missile its original position
                missile.position = heros.position;
                //if the missile touches the top of the window make it "disappear"
                if (missile.position.Y < fenetre.Top)
                {
                    missile.estVivant = false;
                }
            }

            //Give the missile its speed
            missile.position.Y -= missile.vitesse;
        }

        protected override void Draw(GameTime gameTime)
        {
            //Solid background color **optionnal**
            GraphicsDevice.Clear(Color.Gray);

            //Begin drawing our game
            spriteBatch.Begin();
            spriteBatch.Draw(wallpaper, background, Color.White);

            //Do draw hero if alive
            if (heros.estVivant == true)
            {
                spriteBatch.Draw(heros.sprite, heros.position, Color.White);
            }

            //Draw our ennemy if alive
            if (ennemi.estVivant == true)
            {
                spriteBatch.Draw(ennemi.sprite, ennemi.position, Color.White);
            }

            //Draw projectile if alive
            if (projectile.estVivant == true)
            {
                spriteBatch.Draw(projectile.sprite, projectile.position, Color.White);
            }

            //Draw missile if alive
            if (missile.estVivant == true)
            {
                spriteBatch.Draw(missile.sprite, missile.position, Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

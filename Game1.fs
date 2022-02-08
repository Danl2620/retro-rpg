namespace RetroRpg

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open Microsoft.Xna.Framework.Content

type Game1 () as this =
    inherit Game()

    let graphics = new GraphicsDeviceManager(this)
    let mutable spriteBatch = Unchecked.defaultof<_>
    let mutable Tex : Texture2D option = None

    override this.Initialize() =
        // TODO: Add your initialization logic here
        let contentPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Content")
        this.Content.RootDirectory <- contentPath
        this.IsMouseVisible <- true
        
        base.Initialize()

    override this.LoadContent() =
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)
        
        // TODO: use this.Content to load your game content here
        Tex <- Some (this.Content.Load("Basic1"))

    override this.Update (gameTime) =
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back = ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        then this.Exit();

        // TODO: Add your update logic here

        base.Update(gameTime)

    override this.Draw (gameTime) =
        this.GraphicsDevice.Clear Color.CornflowerBlue

        // TODO: Add your drawing code here
        base.Draw(gameTime)

        spriteBatch.Begin()
        spriteBatch.Draw(Tex.Value, Vector2(10.f,60.f), Color.White)
        spriteBatch.End()


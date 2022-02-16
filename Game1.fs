namespace RetroRpg

open System
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open Microsoft.Xna.Framework.Content
open MonoGame.Extended
open MonoGame.Extended.Tiled
open MonoGame.Extended.Tiled.Renderers
open MonoGame.Extended.ViewportAdapters

type Map =
    { tileMap: TiledMap
      tileMapRenderer: TiledMapRenderer }

    member this.Update(gameTime: GameTime) =
        this.tileMapRenderer.Update(gameTime)
    
    member this.Draw(viewMatrix: Matrix) =
        this.tileMapRenderer.Draw(viewMatrix)

    member this.Size() : int*int =
        (this.tileMap.WidthInPixels, this.tileMap.HeightInPixels)

    member this.PlayerPosition() : Vector2 =
        let objectLayer = this.tileMap.ObjectLayers.[0]
        let p = objectLayer.Objects.[0].Position
        p

    static member create(graphicsDevice: GraphicsDevice, content: ContentManager) =
        //let map = content.Load<TiledMap>("example/samplemap")
        let map = content.Load<TiledMap>("oryx/TMX/oryx_16-bit_fantasy_test")
        { tileMap = map
          tileMapRenderer = new TiledMapRenderer(graphicsDevice, map) }


type Game1 () as this =
    inherit Game()

    let graphics = new GraphicsDeviceManager(this)
    let mutable map: Map option = None
    let mutable spriteBatch = Unchecked.defaultof<_>
    let mutable Tex : Texture2D option = None
    let mutable camera : OrthographicCamera option = None
    let mutable cameraPosition: Vector2 = Vector2.Zero
    let mutable playerPosition: Vector2 = Vector2.Zero

    // private functions
    let getMovementDirection() : Vector2 =
        let state = Keyboard.GetState()

        if (state.IsKeyDown(Keys.Down)) then
            Vector2.UnitY
        elif (state.IsKeyDown(Keys.Up)) then
            -Vector2.UnitY
        elif (state.IsKeyDown(Keys.Left)) then
            -Vector2.UnitX
        elif (state.IsKeyDown(Keys.Right)) then
            Vector2.UnitX
        else
            Vector2.Zero

    // methods
    override this.Initialize() =
        // TODO: Add your initialization logic here
        //let contentPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Content/bin/DesktopGL")
        this.Content.RootDirectory <- "Content"
        this.IsMouseVisible <- true

        let size = (800,600)
        match size with
        | (w,h) ->
            let viewportAdapter = new BoxingViewportAdapter(this.Window, this.GraphicsDevice, w, h)
            camera <- OrthographicCamera(viewportAdapter) |> Some
            playerPosition <- Vector2((float32 w)/2.0f,(float32 h)/2.0f)
            playerPosition.ToString() |> printfn "player position: %s"
        base.Initialize()

    override this.LoadContent() =
        let sampleMap = Map.create(this.GraphicsDevice, this.Content)

        match sampleMap.Size() with
        | (w,h) ->
            cameraPosition <- Vector2((float32 w)/2.0f, (float32 h)/2.0f)

        //playerPosition <- sampleMap.PlayerPosition()
        //playerPosition.ToString() |> printfn "player position: %s"
        map <- Some sampleMap
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)
        
        // TODO: use this.Content to load your game content here
        Tex <- Some (this.Content.Load("Basic1"))

    override this.Update (gameTime) =
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back = ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        then this.Exit();

        // TODO: Add your update logic here
        map |> Option.iter (fun m -> m.Update(gameTime))

        // update camera
        let speed: float32 = 200.0f
        let seconds = gameTime.GetElapsedSeconds()
        let movementDirection = getMovementDirection()
        cameraPosition <- cameraPosition + speed * movementDirection * seconds
        camera |> Option.iter (fun c -> c.LookAt(cameraPosition))

        base.Update(gameTime)

    override this.Draw (gameTime) =
        this.GraphicsDevice.Clear Color.CornflowerBlue

        // TODO: Add your drawing code here
        map |> Option.iter (fun m -> m.Draw(camera.Value.GetViewMatrix()))
        base.Draw(gameTime)

        spriteBatch.Begin()
        let p = playerPosition
        let sp = p // camera.Value.WorldToScreen(p)
        spriteBatch.Draw(Tex.Value, sp, Color.White)
        spriteBatch.End()


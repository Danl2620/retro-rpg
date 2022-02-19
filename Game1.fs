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
        let map = content.Load<TiledMap>("example/samplemap")
        //let map = content.Load<TiledMap>("oryx/TMX/oryx_16-bit_fantasy_test")
        { tileMap = map
          tileMapRenderer = new TiledMapRenderer(graphicsDevice, map) }

type Character =
    { sprite: Texture2D
      mutable position: Vector2
      mutable destination: Vector2 }

    member this.Update(gameTime: GameTime) =
        // figure out how to curve interp from one tile to the next using the current position and the destination and passage of time.
        // (should allow move input to queue ahead by 1 or 2)
        // need gameTime for moves too
        ()

    member this.Draw(camera: Camera<Vector2>, spriteBatch: SpriteBatch) =
        let p = this.position
        let sp = camera.WorldToScreen(p)
        spriteBatch.Draw(this.sprite, sp, Color.White)

    member this.Move(dir: Vector2, map: Map) =
        let buildingLayer = map.tileMap.GetLayer("building") :?> TiledMapTileLayer
        let newPosition = this.position + dir
        let w,h = buildingLayer.TileWidth, buildingLayer.TileHeight
        let tile = buildingLayer.GetTile(uint16 (newPosition.X/(float32 w)), uint16 (newPosition.Y/(float32 h)))
        let tileId = tile.GlobalIdentifier
        if tileId = 0 then
            this.position <- newPosition

type Game1 () as this =
    inherit Game()

    let graphics = new GraphicsDeviceManager(this)
    let mutable map: Map option = None
    let mutable spriteBatch = Unchecked.defaultof<_>
    let mutable camera : OrthographicCamera option = None
    let mutable cameraPosition: Vector2 = Vector2.Zero
    let mutable player: Character option = None

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
            //playerPosition <- Vector2((float32 w)/2.0f,(float32 h)/2.0f)
            //playerPosition.ToString() |> printfn "player position: %s"
        base.Initialize()

    override this.LoadContent() =
        let sampleMap = Map.create(this.GraphicsDevice, this.Content)

        match sampleMap.Size() with
        | (w,h) ->
            cameraPosition <- Vector2((float32 w)/2.0f, (float32 h)/2.0f)

        map <- Some sampleMap
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)
        
        // TODO: use this.Content to load your game content here
        player <- Some {
            sprite = this.Content.Load("Basic1")
            position = sampleMap.PlayerPosition()
        }

        //playerPosition.ToString() |> printfn "player position: %s"

    override this.Update (gameTime) =
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back = ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        then this.Exit();

        // TODO: Add your update logic here
        map |> Option.iter (fun m -> m.Update(gameTime))
        player |> Option.iter (fun p -> p.Update(gameTime))

        // update camera
        let speed: float32 = 200.0f
        let seconds = gameTime.GetElapsedSeconds()
        let movementDirection = getMovementDirection()
        player |> Option.iter (fun p -> p.Move(movementDirection, map.Value))
        //cameraPosition <- cameraPosition + speed * movementDirection * seconds
        //camera |> Option.iter (fun c -> c.LookAt(cameraPosition))

        let p = player.Value.position
        camera |> Option.iter (fun c -> c.LookAt(p))

        base.Update(gameTime)

    override this.Draw (gameTime) =
        this.GraphicsDevice.Clear Color.CornflowerBlue

        // TODO: Add your drawing code here
        map |> Option.iter (fun m -> m.Draw(camera.Value.GetViewMatrix()))
        base.Draw(gameTime)

        spriteBatch.Begin()
        player |> Option.iter (fun p -> p.Draw(camera.Value, spriteBatch))
        spriteBatch.End()


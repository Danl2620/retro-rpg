namespace RetroRpg

module Program =

    open System
    open Microsoft.Xna.Framework

    [<EntryPoint>]
    let main argv =
        printfn "Hello World from F#!"
        use game = new Game1()
        game.Run()
        0 // return an integer exit code

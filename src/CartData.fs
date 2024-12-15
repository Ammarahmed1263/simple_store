namespace Store.CartData

open System.IO
open Newtonsoft.Json
open Store.Domain

module CartData =
    let cartFilePath: string = "cart.json"

    let loadCart () =
        try
            if File.Exists(cartFilePath) then
                File.ReadAllText(cartFilePath) |> JsonConvert.DeserializeObject<Cart>
            else
                { Items = []; Total = 0M }
        with (ex: exn) ->
            failwithf "Error loading cart data: %s" ex.Message

    let saveCart (cart: Cart) =
        let json: string = JsonConvert.SerializeObject(cart, Formatting.Indented)
        File.WriteAllText(cartFilePath, json)

namespace Store.ProductData

open System.IO
open Newtonsoft.Json
open Store.Domain

module ProductData =
    let productFilePath: string = "storeCatalog.json"

    let loadProducts () =
        try
            if File.Exists(productFilePath) then
                let json: string = File.ReadAllText(productFilePath)
                JsonConvert.DeserializeObject<Product list>(json)
            else
                []
        with (ex: exn) ->
            failwithf "Error loading products: %s" ex.Message
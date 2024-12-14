namespace Store.Data

open System.IO
open Newtonsoft.Json
open Store.Domain

module ProductData =
    let productFilePath = "storeCatalog.json"

    let saveProducts (products: Product list) =
        try
            let json = JsonConvert.SerializeObject(products, Formatting.Indented)
            File.WriteAllText(productFilePath, json)
        with ex -> failwithf "Error saving products: %s" ex.Message

    let loadProducts () =
        try
            if File.Exists(productFilePath) then
                let json = File.ReadAllText(productFilePath)
                JsonConvert.DeserializeObject<Product list>(json)
            else
                []
        with ex -> failwithf "Error loading products: %s" ex.Message
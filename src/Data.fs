namespace Store.Data

open System.IO
open Newtonsoft.Json
open Store.Domain

module ProductData =
    let productFilePath = "storeCatalog.json"
    let totalFilePath = "totalValue.json"

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

    let saveTotal (total: decimal) =
        try
            let json = JsonConvert.SerializeObject(total, Formatting.Indented)
            File.WriteAllText(totalFilePath, json)
        with ex -> failwithf "Error saving total value: %s" ex.Message

    let loadTotal () =
        try
            if File.Exists(totalFilePath) then
                let json = File.ReadAllText(totalFilePath)
                JsonConvert.DeserializeObject<decimal>(json)
            else
                0.00M
        with ex -> failwithf "Error loading total value: %s" ex.Message

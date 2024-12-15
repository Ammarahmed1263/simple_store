namespace Store.Business

open Store.Domain

module ProductManager =

    let searchProductByName products name =
        products |> List.tryFind (fun product -> product.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase))

    let addToCart products name =
        products
        |> List.map (fun product ->
            if product.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase) then
                { product with IsInCart = true }
            else
                product)

    let removeFromCart products name =
        products
        |> List.map (fun product ->
            if product.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase) then
                { product with IsInCart = false }
            else
                product)

    let calculateTotal products =
        products
        |> List.filter (fun product -> product.IsInCart)
        |> List.sumBy (fun product -> product.Price)

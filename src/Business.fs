namespace Store.Business

open Store.Domain

module CartManager =
    let addToCart (cart: Cart) (product: Product) =
        if cart.Items |> List.exists (fun (p: Product) -> p.Name = product.Name) then
            cart
        else
            let updatedItems: Product list = product :: cart.Items
            let updatedTotal: decimal = updatedItems |> List.sumBy (fun (p: Product) -> p.Price)

            { Items = updatedItems
              Total = updatedTotal }

    let removeFromCart (cart: Cart) (product: Product) =
        let updatedItems: Product list = cart.Items |> List.filter (fun (p: Product) -> p.Name <> product.Name)
        let updatedTotal: decimal = updatedItems |> List.sumBy (fun (p: Product) -> p.Price)
        { Items = updatedItems
          Total = updatedTotal }
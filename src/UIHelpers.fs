namespace Store.UI

open System.Windows.Forms
open Store.Domain

module UIHelpers =

    let updateCartListBox (cartListBox: ListBox) (products: Product list) =
        cartListBox.Items.Clear()
        products
        // |> List.filter (fun p -> p.IsInCart)
        |> List.iter (fun p -> 
            cartListBox.Items.Add(sprintf "%s - $%.2f" p.Name p.Price) |> ignore)

    let updateProductNameTextBox (source: ListBox) (other: ListBox) (textBox: TextBox) =
        if source.SelectedItem <> null then
            other.ClearSelected()
            textBox.Text <- source.SelectedItem.ToString().Split('-').[0].Trim() // Clear the text box if nothing is selected

namespace Store.UI
open System
open System.Windows.Forms
open Store.Domain

module UIHelpers =

    let updateCartListBox (cartListBox: ListBox) (products: Product list) =
        cartListBox.Items.Clear()
        products
        |> List.iter (fun (p: Product) -> 
            cartListBox.Items.Add(sprintf "%s - $%.2f" p.Name p.Price) |> ignore)

    let updateProductNameTextBox (source: ListBox) (other: ListBox) (textBox: TextBox) =
        if source.SelectedItem <> null then
            other.ClearSelected()
            textBox.Text <- source.SelectedItem.ToString().Split('-').[0].Trim()

    let styleButton (button: Button) (backColor: Drawing.Color) (textColor: Drawing.Color) =
        button.BackColor <- backColor
        button.ForeColor <- textColor
        button.Font <- new Drawing.Font("Arial", 13.0f, Drawing.FontStyle.Bold)
        button.FlatStyle <- FlatStyle.Flat
        button.Height <- 30

    let styleLabel (label: Label) =
        label.Font <- new Drawing.Font("Arial Rounded MT Bold", 18.0f, Drawing.FontStyle.Regular)
        label.AutoSize <- true
        label.TextAlign <- Drawing.ContentAlignment.MiddleCenter
        label.Dock <- DockStyle.Fill

    let styleListBox (listBox: ListBox) =
        listBox.Font <- new Drawing.Font("Arial Rounded MT", 11.0f)
        listBox.BackColor <- Drawing.Color.WhiteSmoke

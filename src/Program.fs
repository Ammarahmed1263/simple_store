open System
open System.Windows.Forms
open Store.Domain
open Store.ProductData
open Store.CartData
open Store.Business
open Store.UI.UIHelpers

[<EntryPoint>]
let main _ =
    let products: Product list = ProductData.loadProducts ()
    let cart: Cart = CartData.loadCart ()

    let form: Form =
        new Form(Text = "Store Simulator", WindowState = FormWindowState.Maximized)

    let table: TableLayoutPanel =
        new TableLayoutPanel(Dock = DockStyle.Fill, ColumnCount = 2)


    table.RowCount <- 4
    table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80.0f)) |> ignore
    table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20.0f)) |> ignore

    let nameLabel = new Label(Text = "Enter Product Name")
    styleLabel nameLabel

    let productNameTextBox =
        new TextBox(
            Dock = DockStyle.Fill,
            Font = new Drawing.Font("Arial Rounded MT", 12.0f),
            BackColor = Drawing.Color.LightGray,
            Height = 35,
            Multiline = true
        )

    let addToCartButton = new Button(Text = "Add to Cart", Dock = DockStyle.Fill)
    styleButton addToCartButton Drawing.Color.Green Drawing.Color.White

    let removeFromCartButton =
        new Button(Text = "Remove from Cart", Dock = DockStyle.Fill)
    styleButton removeFromCartButton Drawing.Color.IndianRed Drawing.Color.White

    let storeCatalog = new ListBox(Dock = DockStyle.Fill)
    styleListBox storeCatalog

    let cartLabel = new Label(Text = "Your Cart")
    styleLabel cartLabel

    let checkoutButton = new Button(Text = "Checkout", Dock = DockStyle.Fill)
    styleButton checkoutButton Drawing.Color.DodgerBlue Drawing.Color.White

    let cartTotalLabel = new Label(Dock = DockStyle.Bottom, Visible = false)
    styleLabel cartTotalLabel

    let cartListBox = new ListBox(Dock = DockStyle.Fill)
    styleListBox cartListBox

    products
    |> List.iter (fun product ->
        storeCatalog.Items.Add(sprintf "%s - %.2f$" product.Name product.Price)
        |> ignore)

    cart.Items
    |> List.iter (fun item -> cartListBox.Items.Add(sprintf "%s - %.2f$" item.Name item.Price) |> ignore)

    addToCartButton.Click.Add(fun _ ->
        let productName = productNameTextBox.Text.Trim().ToLower()
        let cart = CartData.loadCart ()

        if productName <> "" then
            match products |> List.tryFind (fun p -> p.Name.ToLower() = productName) with
            | Some p ->
                if
                    cart.Items
                    |> List.tryFind (fun p -> p.Name.ToLower() = productName)
                    |> Option.isSome
                then
                    MessageBox.Show(
                        $"'{productName}' is already in the cart.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    )
                    |> ignore
                else
                    let updatedCart = CartManager.addToCart cart p
                    CartData.saveCart updatedCart

                    updateCartListBox cartListBox updatedCart.Items

                    MessageBox.Show(
                        $"'{productName}' added to cart.",
                        "Info",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    )
                    |> ignore
            | None ->
                MessageBox.Show(
                    $"Product '{productName}' not found.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                )
                |> ignore
        else
            MessageBox.Show("Please enter a product name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            |> ignore)

    removeFromCartButton.Click.Add(fun _ ->
        let productName = productNameTextBox.Text.Trim().ToLower()
        let cart = CartData.loadCart ()

        if productName <> "" then
            match products |> List.tryFind (fun p -> p.Name.ToLower() = productName) with
            | Some p ->
                if
                    cart.Items
                    |> List.tryFind (fun p -> p.Name.ToLower() = productName)
                    |> Option.isSome
                then

                    let updatedCart = CartManager.removeFromCart cart p
                    CartData.saveCart updatedCart

                    updateCartListBox cartListBox updatedCart.Items

                    MessageBox.Show(
                        $"'{productName}' removed from cart.",
                        "Info",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    )
                    |> ignore
                else
                    MessageBox.Show(
                        $"'{productName}' is not in the cart.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    )
                    |> ignore
            | None ->
                MessageBox.Show(
                    $"Product '{productName}' not found.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                )
                |> ignore
        else
            MessageBox.Show("Please enter a product name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            |> ignore)

    checkoutButton.Click.Add(fun _ ->
        let cart = CartData.loadCart ()

        if cart.Total <= 0.0M then
            MessageBox.Show(
                "Your cart is empty. Add products before checkout.",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            )
            |> ignore
        else
            cartTotalLabel.Text <- sprintf "Total: %.2f$" cart.Total
            cartTotalLabel.Visible <- true

            let timer = new Timer(Interval = 5000)

            timer.Tick.Add(fun _ ->
                cartTotalLabel.Visible <- false
                timer.Stop()
                timer.Dispose())

            timer.Start())

    storeCatalog.SelectedIndexChanged.Add(fun _ -> updateProductNameTextBox storeCatalog cartListBox productNameTextBox)
    cartListBox.SelectedIndexChanged.Add(fun _ -> updateProductNameTextBox cartListBox storeCatalog productNameTextBox)

    table.Controls.Add(nameLabel, 0, 0)
    table.Controls.Add(productNameTextBox, 0, 1)
    table.Controls.Add(addToCartButton, 0, 2)
    table.Controls.Add(removeFromCartButton, 0, 3)
    table.Controls.Add(storeCatalog, 0, 4)
    table.Controls.Add(cartLabel, 1, 0)
    table.Controls.Add(checkoutButton, 1, 1)
    table.SetRowSpan(checkoutButton, 2)
    table.Controls.Add(cartListBox, 1, 4)
    table.Controls.Add(cartTotalLabel, 1, 3)

    table.CellBorderStyle <- TableLayoutPanelCellBorderStyle.Single // Add border to enhance table appearance
    table.BackColor <- Drawing.Color.AliceBlue // Light background color for the table

    form.Controls.Add(table)

    Application.Run(form)
    0

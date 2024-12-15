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

    // Load cart on app start
    let form: Form =
        new Form(Text = "Store Simulator", WindowState = FormWindowState.Maximized)

    let table: TableLayoutPanel = new TableLayoutPanel(Dock = DockStyle.Fill, ColumnCount = 2)
    table.RowCount <- 4
    table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60.0f)) |> ignore
    table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30.0f)) |> ignore

    // Reusable function to style buttons
    let styleButton (button: Button) (backColor: Drawing.Color) (textColor: Drawing.Color) =
        button.BackColor <- backColor
        button.ForeColor <- textColor
        button.Font <- new Drawing.Font("Arial", 13.0f, Drawing.FontStyle.Bold)
        button.FlatStyle <- FlatStyle.Flat
        button.FlatAppearance.BorderColor <- Drawing.Color.LightGray
        button.FlatAppearance.BorderSize <- 3
        button.Height <- 50 // Increase button height

    // Reusable function to style labels
    let styleLabel (label: Label) =
        label.Font <- new Drawing.Font("Arial", 11.0f, Drawing.FontStyle.Regular)
        label.AutoSize <- true

    // Reusable function to style list boxes
    let styleListBox (listBox: ListBox) =
        listBox.Font <- new Drawing.Font("Arial", 11.0f)
        listBox.BackColor <- Drawing.Color.WhiteSmoke

    // UI elements
    let nameLabel =
        new Label(Text = "Enter Product Name")
    styleLabel nameLabel
    nameLabel.ForeColor <- Drawing.Color.DarkSlateGray

    let productNameTextBox = new TextBox(Dock = DockStyle.Fill)
    productNameTextBox.BackColor <- Drawing.Color.LightGray
    productNameTextBox.Font <- new Drawing.Font("Arial", 11.0f)

    let addToCartButton = new Button(Text = "Add to Cart", Dock = DockStyle.Fill)
    styleButton addToCartButton Drawing.Color.DarkGreen Drawing.Color.White

    let removeFromCartButton =
        new Button(Text = "Remove from Cart", Dock = DockStyle.Fill)
    styleButton removeFromCartButton Drawing.Color.Red Drawing.Color.White

    let storeCatalog = new ListBox(Dock = DockStyle.Fill)
    styleListBox storeCatalog

    let cartListBox = new ListBox(Dock = DockStyle.Fill)
    styleListBox cartListBox

    let checkoutButton = new Button(Text = "Checkout", Dock = DockStyle.Fill)
    styleButton checkoutButton Drawing.Color.DodgerBlue Drawing.Color.White

    let cartTotalLabel =
        new Label(Dock = DockStyle.Bottom, Visible = false)
    styleLabel cartTotalLabel

    // Populate store catalog
    products
    |> List.iter (fun product ->
        storeCatalog.Items.Add(sprintf "%s - $%.2f" product.Name product.Price)
        |> ignore)

    cart.Items
    |> List.iter (fun item -> cartListBox.Items.Add(sprintf "%s - $%.2f" item.Name item.Price) |> ignore)

    // Button handlers
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
            ) |> ignore
        else
            cartTotalLabel.Text <- sprintf "Total: $%.2f" cart.Total
            cartTotalLabel.Visible <- true

            // Use a timer to hide the label after 10 seconds
            let timer = new Timer(Interval = 5000) // 10 seconds in milliseconds
            timer.Tick.Add(fun _ ->
                cartTotalLabel.Visible <- false
                timer.Stop()
                timer.Dispose()
            )
            timer.Start()
    )

    // Subscribe to both list boxes' selection change events
    storeCatalog.SelectedIndexChanged.Add(fun _ -> updateProductNameTextBox storeCatalog cartListBox productNameTextBox)
    cartListBox.SelectedIndexChanged.Add(fun _ -> updateProductNameTextBox cartListBox storeCatalog productNameTextBox)

    // Layout
    table.Controls.Add(nameLabel, 0, 0)
    table.Controls.Add(productNameTextBox, 0, 1)
    table.Controls.Add(addToCartButton, 0, 2)
    table.Controls.Add(removeFromCartButton, 0, 3)
    table.Controls.Add(storeCatalog, 0, 4)
    table.Controls.Add(checkoutButton, 1, 1)
    table.SetRowSpan(checkoutButton, 2)
    table.Controls.Add(cartListBox, 1, 4)
    table.Controls.Add(cartTotalLabel, 1, 3)

    table.CellBorderStyle <- TableLayoutPanelCellBorderStyle.Single // Add border to enhance table appearance
    table.BackColor <- Drawing.Color.AliceBlue // Light background color for the table

    form.Controls.Add(table)

    Application.Run(form)
    0

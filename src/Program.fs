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
    table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70.0f)) |> ignore
    table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30.0f)) |> ignore

    // UI elements with color improvements
    let nameLabel = new Label(Text = "Enter Product Name", AutoSize = true, Font = new Drawing.Font("Arial", 20.0f))
    nameLabel.ForeColor <- System.Drawing.Color.DarkSlateGray  // Changed color
    let nameTextBox = new TextBox(Dock = DockStyle.Fill)
    nameTextBox.BackColor <- System.Drawing.Color.LightGray  // Changed background color
    let addToCartButton = new Button(Text = "Add to Cart", Dock = DockStyle.Fill)
    addToCartButton.BackColor <- System.Drawing.Color.LightGreen  // Changed background color
    addToCartButton.ForeColor <- System.Drawing.Color.White  // Changed text color
    let removeFromCartButton = new Button(Text = "Remove from Cart", Dock = DockStyle.Fill)
    removeFromCartButton.BackColor <- System.Drawing.Color.IndianRed  // Changed background color
    removeFromCartButton.ForeColor <- System.Drawing.Color.White  // Changed text color
    let storeCatalog = new ListBox(Dock = DockStyle.Fill)
    storeCatalog.BackColor <- System.Drawing.Color.WhiteSmoke  // Changed background color
    let cartListBox = new ListBox(Dock = DockStyle.Fill)
    cartListBox.BackColor <- System.Drawing.Color.WhiteSmoke  // Changed background color
    let checkoutButton = new Button(Text = "Checkout", Dock = DockStyle.Fill)
    checkoutButton.BackColor <- System.Drawing.Color.DodgerBlue  // Changed background color
    checkoutButton.ForeColor <- System.Drawing.Color.White  // Changed text color

    let cartTotalLabel =
        new Label(Dock = DockStyle.Bottom, Font = new Drawing.Font("Arial Rounded MT Bold", 14.0f), Visible = false)

    // Populate store catalog
    products |> List.iter (fun product -> 
        storeCatalog.Items.Add(sprintf "%s ($%.2f)" product.Name product.Price) |> ignore)

    // Update cart and total
    let updateUI () =
        cartListBox.Items.Clear()
        products
        |> List.filter (fun p -> p.IsInCart)
        |> List.iter (fun p -> cartListBox.Items.Add(sprintf "%s - $%.2f" p.Name p.Price) |> ignore)
        cartTotalLabel.Text <- sprintf "Total: $%.2f" total

    // Initial update
    updateUI() // Update UI immediately after loading products and total

    // Button handlers with validation (no changes)
    addToCartButton.Click.Add(fun _ ->
        let name = nameTextBox.Text.Trim().ToLower() // Convert input to lowercase
        if name <> "" then
            let product = products |> List.tryFind (fun p -> p.Name.ToLower() = name) // Convert product names to lowercase for comparison
            match product with
            | Some p when not p.IsInCart -> 
                // Only add the product if it is not already in the cart
                products <- ProductManager.addToCart products name
                total <- ProductManager.calculateTotal products
                ProductData.saveProducts products
                ProductData.saveTotal total
                MessageBox.Show($"'{name}' added to cart.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
                updateUI()
            | Some _ -> 
                MessageBox.Show($"'{name}' is already in the cart.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
            | None -> 
                MessageBox.Show($"Product '{name}' not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
        else
            MessageBox.Show("Please enter a product name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore)

    removeFromCartButton.Click.Add(fun _ ->
        let name = nameTextBox.Text.Trim().ToLower() // Convert input to lowercase
        if name <> "" then
            let product = products |> List.tryFind (fun p -> p.Name.ToLower() = name) // Convert product names to lowercase for comparison
            match product with
            | Some p when p.IsInCart -> 
                // Only remove the product if it is in the cart
                products <- ProductManager.removeFromCart products name
                total <- ProductManager.calculateTotal products
                ProductData.saveProducts products
                ProductData.saveTotal total
                MessageBox.Show($"'{name}' removed from cart.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
                updateUI()
            | Some _ -> 
                MessageBox.Show($"'{name}' is not in the cart.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
            | None -> 
                MessageBox.Show($"Product '{name}' not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
        else
            MessageBox.Show("Please enter a product name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore)

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
                cartTotalLabel.Text <- sprintf "Total: $%.2f" cart.Total
                cartTotalLabel.Visible <- true

                // Use a timer to hide the label after 10 seconds
                let timer = new Timer(Interval = 5000) // 10 seconds in milliseconds

                timer.Tick.Add(fun _ ->
                    cartTotalLabel.Visible <- false
                    timer.Stop()
                    timer.Dispose())

                timer.Start())
    // Layout
    table.Controls.Add(nameLabel, 0, 0)
    table.Controls.Add(nameTextBox, 0, 1)
    table.Controls.Add(addToCartButton, 0, 2)
    table.Controls.Add(removeFromCartButton, 0, 3)
    table.Controls.Add(storeCatalog, 0, 4)
    table.Controls.Add(cartListBox, 1, 0)
    table.Controls.Add(checkoutButton, 1, 2)
    table.Controls.Add(cartTotalLabel, 1, 3)

    form.Controls.Add(table)

    Application.Run(form)
    0

// Program.fs

open System
open System.Windows.Forms
open Store.Domain
open Store.Data
open Store.Business
open Store.UI.UIHelpers

[<EntryPoint>]
let main _ =
    // ملف المنتجات
    let productFilePath = "products.txt"

    // تحميل المنتجات من الملف عند بداية التطبيق
    let mutable products = ProductData.loadProducts productFilePath
    let mutable total = 0.0m  // المتغير لتخزين قيمة التوتال

    // إنشاء النموذج (Form) للتطبيق
    let form: Form =
        new Form(Text = "Store Simulator", WindowState = FormWindowState.Maximized)

    let table = new TableLayoutPanel(Dock = DockStyle.Fill, ColumnCount = 2)
    table.RowCount <- 4

    // إضافة زر للـ Checkout
    let checkoutButton = new Button(Text = "Checkout", Dock = DockStyle.Fill)

    checkoutButton.Click.Add(fun _ ->
        // حساب الـ total عند الضغط على الـ Checkout
        total <- ProductManager.calculateTotal products
        MessageBox.Show($"Total: ${total:F2}", "Checkout", MessageBoxButtons.OK)
        |> ignore)

    // إضافة باقي العناصر إلى الجدول
    table.Controls.Add(checkoutButton, 0, 3)

    // إضافة الجدول إلى الفورم
    form.Controls.Add(table)

    // بدء التطبيق
    Application.Run(form)
    0

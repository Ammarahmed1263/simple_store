namespace Store.Domain

type Product = {
  Name: string
  Price: decimal
  Description: string
}

type Cart = {
  Items: Product list
  Total: decimal
}

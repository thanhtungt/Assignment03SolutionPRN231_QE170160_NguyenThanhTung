namespace eStore.Models
{
    // ShoppingCart.cs
    public class ShoppingCart
    {
        public List<ShoppingCartItem> _items { get; set; }

        public ShoppingCart()
        {
            _items = new List<ShoppingCartItem>();
        }

        public IReadOnlyList<ShoppingCartItem> Items => _items.AsReadOnly();

        public void AddToCart(int productId, string productName, decimal unitPrice, string categoryName)
        {
            var existingItem = _items.FirstOrDefault(item => item.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                _items.Add(new ShoppingCartItem
                {
                    ProductId = productId,
                    ProductName = productName,
                    UnitPrice = unitPrice,
                    Quantity = 1,
                    CategoryName = categoryName
                });
            }
        }

        public void RemoveFromCart(int productId)
        {
            var item = _items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                _items.Remove(item);
            }
        }
    }

}

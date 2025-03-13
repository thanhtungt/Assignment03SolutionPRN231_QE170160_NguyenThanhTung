namespace eStore.Models
{
    public class ShoppingCartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string CategoryName { get; set; }  // Lưu danh mục sản phẩm

    }

}

namespace Scripts
{
    public class Store : BaseBuilding
    {
        public void AddOutputStock(ProductData product, int amount)
        {
            if (outputStock.ContainsKey(product))
                outputStock[product] += amount;
            else
                outputStock.Add(product, amount);

        }

        protected override void UpdateStockText()
        {
            base.UpdateStockText();
        }
    }
}
public class ItemFactory : BaseFactory
{
    void Start()
    {
        RegisterPrefabs();
    }
    private void RegisterPrefabs()
    {
        var loader = GameLoader.GameLoaderInstance;
        loader.HandleWhenItemPrefabsLoaded(() =>
        {
            RegisterProducts();
        });
    }
    private void RegisterProducts()
    {
        var loader = GameLoader.GameLoaderInstance;
        var itemPrefabs = loader.GetItemPrefabs();
        foreach (var item in itemPrefabs)
        {
            RegisterProduct(item.ItemName, item);
        }
    }
}
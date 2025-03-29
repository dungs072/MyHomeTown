using UnityEngine;

public class PropertyFactory : BaseFactory
{
    private PropertyBase currentCreatingProduct;
    void Awake()
    {
        Loader.OnAssetLoaded += OnAssetLoaded;
    }
    void OnDestroy()
    {
        Loader.OnAssetLoaded -= OnAssetLoaded;
    }
    private void OnAssetLoaded()
    {
        RegisterPrefabs();
    }
    private void RegisterPrefabs()
    {
        var productPrefabs = Loader.Instance.PropList;
        for (int i = 0; i < productPrefabs.Count; i++)
        {
            string productName = productPrefabs[i].ProductName;
            RegisterProduct(productName, productPrefabs[i]);
        }
    }

    public override BaseProduct GetFreeProduct(string productName)
    {
        var product = base.GetFreeProduct(productName);
        currentCreatingProduct = product as PropertyBase;
        return currentCreatingProduct;
    }


    public void HandleOnCreatingProperty()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 worldPosition = hit.point;
            currentCreatingProduct.transform.position = new Vector3(worldPosition.x, 0, worldPosition.z);
        }

    }

    public void HandleOnFinishCreatingProperty()
    {
        if (currentCreatingProduct != null)
        {
            currentCreatingProduct.gameObject.SetActive(true);
            currentCreatingProduct = null;
        }
    }


}

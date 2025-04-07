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
        if (currentCreatingProduct.TryGetComponent(out Occupier occupier))
        {
            occupier.StartMove();
        }
        if (currentCreatingProduct.TryGetComponent(out PropertyBase property))
        {
            property.SetPropState(PropState.Selected);
        }

        return currentCreatingProduct;
    }


    public void HandleOnCreatingProperty()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000, LayerMask.GetMask(LayerConstant.PLACABLE_LAYER)))
        {
            Vector3 worldPosition = hit.point;

            if (currentCreatingProduct.TryGetComponent(out Occupier occupier))
            {
                occupier.HandleMovingOnGrid(worldPosition);
            }
        }

    }

    public void HandleOnFinishCreatingProperty()
    {
        if (currentCreatingProduct.TryGetComponent(out PropertyBase property))
        {
            property.SetPropState(PropState.Free);
        }
        if (currentCreatingProduct.TryGetComponent(out Occupier occupier))
        {
            occupier.StopMove();
            if (occupier.IsOverlap)
            {
                currentCreatingProduct.gameObject.SetActive(false);
                return;
            }
            else
            {
                occupier.SetOccupiedSlots();
            }
        }
        if (currentCreatingProduct.TryGetComponent(out WorkContainer workContainer))
        {
            var workContainerManager = ManagerSingleton.Instance.WorkContainerManager;
            workContainerManager.AddWorkContainer(workContainer);
        }

        if (currentCreatingProduct != null)
        {
            currentCreatingProduct.gameObject.SetActive(true);
            currentCreatingProduct = null;
        }
    }


}

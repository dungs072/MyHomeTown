using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseProduct : MonoBehaviour
{
    public virtual string ProductName { get; }
}

public class BaseFactory : MonoBehaviour
{
    protected Dictionary<string, BaseProduct> productDictionary = new Dictionary<string, BaseProduct>();

    protected Dictionary<string, List<BaseProduct>> productListDictionary = new Dictionary<string, List<BaseProduct>>();
    
    public virtual void RegisterProduct(string productName, BaseProduct product)
    {
        if (!productDictionary.ContainsKey(productName))
        {
            productDictionary.Add(productName, product);
        }
        else
        {
            Debug.LogWarning($"Product {productName} is already registered.");
        }
    }
    public virtual BaseProduct CreateProduct(string productName)
    {
        if (productDictionary.TryGetValue(productName, out BaseProduct prefab))
        {
            BaseProduct product = Instantiate(prefab);
            product.name = productName;
            if (productListDictionary.ContainsKey(productName))
            {
                productListDictionary[productName].Add(product);
            }
            else
            {
                productListDictionary[productName] = new List<BaseProduct> { product };
            }
            return product;
        }
        else
        {
            Debug.LogWarning($"Product {productName} is not registered.");
            return null;
        }
    }

    public virtual BaseProduct GetFreeProduct(string productName)
    {
        if (productListDictionary.TryGetValue(productName, out List<BaseProduct> productList) && productList.Count > 0)
        {
            foreach (var product in productList)
            {
                if (!product.isActiveAndEnabled)
                {
                    product.gameObject.SetActive(true);
                    return product;
                }
            }
        }

        var newProduct = CreateProduct(productName);
        return newProduct;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
namespace GamePlayContainerElements
{
    public class Footer : MonoBehaviour
    {
        [SerializeField] private GameObject propHolder;
        [SerializeField] private Prop prop;

        private List<Prop> props = new List<Prop>();

        private PropertyFactory propertyFactory;

        void Start()
        {
            InitComponents();
        }
        private void InitComponents()
        {
            if (!propertyFactory) return;
            propertyFactory = ManagerSingleton.EmpireInstance.PropertyFactory;
        }
        public void CreateProp(string name, Sprite sprite)
        {
            Prop newProp = Instantiate(prop, propHolder.transform);
            props.Add(newProp);
            newProp.SetName(name);
            RegisterEventForPropUI(newProp);
            if (!sprite) return;
            newProp.SetBackground(sprite);
        }

        public void RegisterEventForPropUI(Prop prop)
        {
            var productName = prop.PropName;
            void startCreateProp()
            {
                ManagerSingleton.EmpireInstance.PropertyFactory.GetFreeProduct(productName);
            }
            void creatingProp()
            {
                ManagerSingleton.EmpireInstance.PropertyFactory.HandleOnCreatingProperty();
            }
            void finishCreateProp()
            {
                ManagerSingleton.EmpireInstance.PropertyFactory.HandleOnFinishCreatingProperty();
            }
            prop.RegisterButtonClickEvent(startCreateProp, creatingProp, finishCreateProp);
        }
    }

}

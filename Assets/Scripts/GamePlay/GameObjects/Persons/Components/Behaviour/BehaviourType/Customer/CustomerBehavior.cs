using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class CustomerBehavior : BaseBehavior
{
    public CustomerBehavior(Person person) : base(person)
    {
        this.person = person;
    }
}

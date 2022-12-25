using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceProvider : MonoBehaviour
{
    static public ResourceProvider i;

    [SerializeField]
    private InputController _inputController;

    public InputController inputController
    {
        get { return _inputController; }
    }

    void Awake()
    {
        i = this;
    }

}

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

    [SerializeField]
    private CharacterAnimationController animationController;

    public bool isGoal
    {
        get { return animationController.isGoal; }
    }

    [SerializeField]
    private GameManager _gameManager;

    public GameManager gameManager
    {
        get { return _gameManager; }
    }

    void Awake()
    {
        i = this;
    }

}

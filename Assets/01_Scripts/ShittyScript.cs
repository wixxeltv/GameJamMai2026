using System;
using UnityEngine;

public class ShittyScript : MonoBehaviour
{
    [SerializeField] private GameObject help;

    private void Start()
    {
        help.SetActive(true);
    }
}

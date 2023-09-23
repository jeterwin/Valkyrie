using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private float xModifier;
    [SerializeField] private float yModifier;
    private Vector3 startPos;
    private void Start()
    {
        startPos = transform.position;
    }
    void Update()
    {
        Vector3 mousePosition = camera.ScreenToViewportPoint(Input.mousePosition);
        transform.position = new Vector3(startPos.x + (xModifier * mousePosition.x),
            startPos.y + (yModifier * mousePosition.y), 0);
    }
}

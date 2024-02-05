using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Psuedo3DManager : MonoBehaviour
{
    public RectTransform depthDot;

    public float backgroundScale = .3f;

    public float backgroundDepth = -20;

    public float sizeScale = 0.15f;

    public float scalingRatio = 2;

    public float gravity = 1;

    public GameObject beanBagPrefab;

    public List<GameObject> beanBagObjects = new List<GameObject>();


    public float groundLevel = -55;

    public Beanbag selectedBeanbag = null;

    public Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject currentBean = Instantiate(beanBagPrefab, canvas.transform);

            beanBagObjects.Add(currentBean);

            currentBean.transform.localPosition = new Vector2(-85 + 85 * i, -187);

        }



    }

    // Update is called once per frame
    void Update()
    {
        if (beanBagObjects.Count == 0) {
            Debug.Log("You've lost! Reloading room...");
            
            

        
        }
    }
}

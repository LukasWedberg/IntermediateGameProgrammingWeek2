using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Beanbag : MonoBehaviour
{
    public Psuedo3DManager psuedo3dManager;


    [SerializeField]
    private Vector3 _3dPos;

    private Vector3 foregroundScale;

    //I thought it might be fun to try practicing getters/setters 
    //by making the 3D-to-2D math automatically happen when you change the 3D position.
    //This could make things a lot easier, so yay!

    [SerializeField]
    public Vector3 ThreeDeePosition { 
        get { return _3dPos;}
        set 
        {
            //Debug.Log("Now we're scaling!");
        
            _3dPos = value; 
            
            float depthAlpha = 1-Mathf.Lerp(1f, psuedo3dManager.backgroundScale, Mathf.Clamp(_3dPos.z,-20,0)/psuedo3dManager.backgroundDepth);

            //Debug.Log("DEPTH ALPHA:"+ depthAlpha );

            //Using the depthAlpha value, we will scale
            //the size and position to make it look
            //like it's moving in 3d!

            _2dPositioning.anchoredPosition = new Vector2(
                Mathf.Lerp(_3dPos.x, psuedo3dManager.depthDot.anchoredPosition.x, depthAlpha)
                , Mathf.Lerp(_3dPos.y,psuedo3dManager.depthDot.anchoredPosition.y, depthAlpha)
                );

            

            _2dPositioning.localScale = foregroundScale * Mathf.Lerp(1f, psuedo3dManager.backgroundScale/2, depthAlpha);


        }
    }


    //Make beanState Enum, to track behavior like being dragged and being thrown!
    public enum BeanState { 
        Idle,
        Dragging,
        Freefall

    }




    private RectTransform _2dPositioning;


    //Since we're making a beanbag toss game, we'll need to check the velocity
    //of the beanbag when its being dragged. If it's high enough when the player
    //lets go, then that means they want to throw it!
    [SerializeField]
    private Vector3 currentDraggingVelocity;





    // Start is called before the first frame update
    void Start()
    {
        _2dPositioning = GetComponent<RectTransform>();

        psuedo3dManager = GameObject.Find("Psuedo3DManager").GetComponent<Psuedo3DManager>();

        foregroundScale = _2dPositioning.localScale;

        ThreeDeePosition = _2dPositioning.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        //ThreeDeePosition += new Vector3(0,0,-0.1f);
    }
}

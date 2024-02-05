using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Beanbag : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public Psuedo3DManager psuedo3dManager;

    float requiredThrowStrength = 0.01f;

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
            
            float depthAlphaPos = 1-Mathf.Lerp(1f, psuedo3dManager.backgroundScale, Mathf.Clamp(_3dPos.z,psuedo3dManager.backgroundDepth, 0)/psuedo3dManager.backgroundDepth);
            float depthAlphaScale = Mathf.Lerp(1f, psuedo3dManager.sizeScale, Mathf.Clamp(_3dPos.z,psuedo3dManager.backgroundDepth, 0)/psuedo3dManager.backgroundDepth);

            //Debug.Log("DEPTH ALPHA:"+ depthAlpha );

            //Using the depthAlpha value, we will scale
            //the size and position to make it look
            //like it's moving in 3d!

            _2dPositioning.localPosition = new Vector2(
                Mathf.Lerp(_3dPos.x, psuedo3dManager.depthDot.anchoredPosition.x, depthAlphaPos)
                , Mathf.Lerp(_3dPos.y,psuedo3dManager.depthDot.anchoredPosition.y, depthAlphaPos)
                );



            _2dPositioning.localScale = foregroundScale * depthAlphaScale; //* Mathf.Lerp(1f, psuedo3dManager.sizeScale, depthAlphaScale * psuedo3dManager.scalingRatio);


        }
    }


    //Make beanState Enum, to track behavior like being dragged and being thrown!
    public enum BeanState { 
        Idle,
        Dragging,
        Freefall,
        Landed

    }

    BeanState currentBeanState = BeanState.Idle;



    private RectTransform _2dPositioning;


    //Since we're making a beanbag toss game, we'll need to check the velocity
    //of the beanbag when its being dragged. If it's high enough when the player
    //lets go, then that means they want to throw it!
    [SerializeField]
    private Vector3 currentDraggingVelocity;
    private Vector3 previousDraggingPos;

    private float verticalVelocity = 0f;
    private float horizontalVelocity = 0f;
    private float ZVelocity = 0f;



    private Vector3 startPos;



    // Start is called before the first frame update
    void Start()
    {
        _2dPositioning = GetComponent<RectTransform>();

        psuedo3dManager = GameObject.Find("Psuedo3DManager").GetComponent<Psuedo3DManager>();

        foregroundScale = _2dPositioning.localScale;

        ThreeDeePosition = _2dPositioning.anchoredPosition;

        startPos = _2dPositioning.anchoredPosition;

        Invoke("AllowSelect", 1);
    }

    // Update is called once per frame
    void Update()
    {
        //ThreeDeePosition += new Vector3(0,0,-0.1f);

        switch (currentBeanState){
        
            case BeanState.Idle:
                ThreeDeePosition = Vector3.Lerp(_2dPositioning.anchoredPosition, startPos + new Vector3(0, 2f * Mathf.Sin( (.5f * Time.realtimeSinceStartup + startPos.x) ),0) , 0.1f);
                //Debug.Log("IDLE");
                
                break;
            case BeanState.Dragging:
                //Debug.Log("CURRENTLY DRAGGING!");

                break;

            case BeanState.Freefall:

                if (ThreeDeePosition.y > psuedo3dManager.groundLevel)
                {
                    Debug.Log("WHEEEEEE: " + Vector3.Distance(ThreeDeePosition, psuedo3dManager.goalPosition.position));
                    //ThreeDeePosition += currentDraggingVelocity + new Vector3(0, 0, -currentDraggingVelocity.magnitude);
                    ThreeDeePosition += new Vector3(horizontalVelocity, verticalVelocity, -.8f) * Time.deltaTime * currentDraggingVelocity.magnitude * 6;

                    verticalVelocity -= psuedo3dManager.gravity * Time.deltaTime;


                    //currentDraggingVelocity = new Vector3(currentDraggingVelocity.x * .9f, currentDraggingVelocity.y - 0.1f, 0);

                    if (Vector3.Distance(ThreeDeePosition, psuedo3dManager.goalPosition.position) < 120 && ThreeDeePosition.z < -30)
                    {
                        psuedo3dManager.beanBagObjects.Remove(this.gameObject);
                        Debug.Log("You scored a point, congrats!");
                        psuedo3dManager.points++;

                        Destroy(gameObject);

                    }
                }
                else {
                    currentBeanState = BeanState.Landed;
                
                }

                break;
            case BeanState.Landed:
                if (psuedo3dManager.beanBagObjects.Contains(this.gameObject) )
                {
                    psuedo3dManager.beanBagObjects.Remove(this.gameObject);
                }

                break;


        }


    }

    //bean selection status
    bool mouseRollOver = false;
    bool allowSelect = false;
    bool isDragging = false;


    //Mouse Roll Over 
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (psuedo3dManager.selectedBeanbag == null)
        {
            mouseRollOver = true;

            //cardManager.RelocateAllCards(this);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseRollOver = false;
        //rectTrans.localScale = Vector3.one;

        //cardManager.RelocateAllCards();
    }

    //Drag Detection
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (allowSelect && eventData.button == PointerEventData.InputButton.Left && currentBeanState == BeanState.Idle)
        {
            if (!isDragging)
            {
                isDragging = true;

                currentBeanState = BeanState.Dragging;
                //cardManager.currentSelectedCard = this;
                psuedo3dManager.selectedBeanbag = this;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (allowSelect && eventData.button == PointerEventData.InputButton.Left)
        {
            if (isDragging)
            {
                
                //rectTrans.localScale = selectedScale;
                Vector3 globalMousePos;
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_2dPositioning, eventData.position, eventData.pressEventCamera, out globalMousePos))
                {
                    _2dPositioning.position = globalMousePos;

                    //previousDraggingPos = globalMousePos;

                    //Debug.Log("CURRENTLY DRAGGING!");

                    currentDraggingVelocity = transform.position - previousDraggingPos;

                    previousDraggingPos = transform.position;

                    Debug.Log(currentDraggingVelocity.magnitude);


                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (allowSelect && eventData.button == PointerEventData.InputButton.Left)
        {
            if (isDragging)
            {
                //End Drag
                isDragging = false;

                currentBeanState = BeanState.Idle;

                //allowSelect = false;
                //Invoke("AllowSelect", 0.5f);
                //cardManager.currentSelectedCard = null;

                psuedo3dManager.selectedBeanbag = null;
                
                
                if (currentDraggingVelocity.magnitude > requiredThrowStrength)
                {
                    ThreeDeePosition = new Vector3(transform.position.x, transform.position.y,0);

                    verticalVelocity = currentDraggingVelocity.y;

                    horizontalVelocity = currentDraggingVelocity.x;

                    currentBeanState = BeanState.Freefall;

                }


            }
        }
    }

    void AllowSelect()
    {
        allowSelect = true;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    private LineRenderer line;
    private Vector3 previousPosition;
    public Image image;
    private Bounds bounds;
    private float minX;
    private float maxX;
    private float minY;
    private float maxY;

    [SerializeField]
    private float minDistance=0.1f;
    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 1;
        previousPosition = transform.position;
        
        var min = Vector3.positiveInfinity;
        var max = Vector3.negativeInfinity;

        var v = new Vector3[4];
        image.rectTransform.GetWorldCorners(v);

        minX = Mathf.Infinity;
        maxX = Mathf.NegativeInfinity;
        minY = Mathf.Infinity;
        maxY = Mathf.NegativeInfinity;

        // update min and max
        foreach (var vector3 in v)
        {
            //Debug.Log(vector3);
            if(vector3.x > maxX){
                maxX = vector3.x;
            }
            if(vector3.x < minX){
                minX = vector3.x;
            }
            if(vector3.y > maxY){
                maxY = vector3.y;
            }
            if(vector3.y < minY){
                minY = vector3.y;
            }
            min = Vector3.Min(min, vector3);
            max = Vector3.Max(max, vector3);
        }

        // create the bounds
        bounds = new Bounds();
        bounds.SetMinMax(min, max);

        //Debug.Log(minX);
        //Debug.Log(maxX);
        //Debug.Log(minY);
        //Debug.Log(maxY);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0)){
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentPosition.z = 0.0f;
            bool inBounds = false;

            if(currentPosition.x <= maxX && currentPosition.x >= minX && currentPosition.y <= maxY && currentPosition.y >= minY){
                inBounds = true;
            }

            //Debug.Log(inBounds);

            if(Vector3.Distance(currentPosition, previousPosition) > minDistance && inBounds){

                if(previousPosition == transform.position){
                    line.SetPosition(0, currentPosition);
                }
                else{
                    line.positionCount++;
                    line.SetPosition(line.positionCount - 1, currentPosition);
                }
                previousPosition = currentPosition;
            }
        }
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageScript : MonoBehaviour
{

    GameObject objFerrisWheel;


    // Start is called before the first frame update
    void Start()
    {
        objFerrisWheel = GameObject.Find("FerrisWheel").gameObject;

   
    }

    // Update is called once per frame
    void Update()
    {

        objFerrisWheel.transform.RotateAround(Vector3.left, 0.2f * Time.deltaTime);


        GameObject.Find("gondola1").gameObject.transform.position = GameObject.Find("axis1").gameObject.transform.position + new Vector3(0,0,0);
        GameObject.Find("gondola2").gameObject.transform.position = GameObject.Find("axis2").gameObject.transform.position + new Vector3(0,0,0);
        GameObject.Find("gondola3").gameObject.transform.position = GameObject.Find("axis3").gameObject.transform.position + new Vector3(0,0,0);
        GameObject.Find("gondola4").gameObject.transform.position = GameObject.Find("axis4").gameObject.transform.position + new Vector3(0,0,0);
        GameObject.Find("gondola5").gameObject.transform.position = GameObject.Find("axis5").gameObject.transform.position + new Vector3(0,0,0);
        GameObject.Find("gondola6").gameObject.transform.position = GameObject.Find("axis6").gameObject.transform.position + new Vector3(0,0,0);
        GameObject.Find("gondola7").gameObject.transform.position = GameObject.Find("axis7").gameObject.transform.position + new Vector3(0,0,0);
        GameObject.Find("gondola8").gameObject.transform.position = GameObject.Find("axis8").gameObject.transform.position + new Vector3(0,0,0);

    }




}

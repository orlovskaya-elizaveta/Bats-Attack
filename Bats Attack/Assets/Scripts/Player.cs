using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    float speed = 5;
    float camSpeed = 200;
    private Image damageImage;
    private float timer;
    private bool isGetDamage;
    private float fadeTime = 1.5f;
    private float redImageBright = 0.3f;

    private void Awake()
    {
        damageImage = GameObject.Find("Canvas").GetComponentInChildren<Image>();
    }
    void Start () {

        Instantiate(Resources.Load("prefab_bat_01"));
        Instantiate(Resources.Load("prefab_bat_02"));
        Instantiate(Resources.Load("prefab_bat_03"));
    }
	
	void Update () {

        MoveAndRotate();

        if (timer <= 1.5f && isGetDamage)
        {
            //плавное снижение яркости красной подсветки после получения урона
            float colA = redImageBright - timer / fadeTime * redImageBright;
            damageImage.color = new Color(damageImage.color.r, damageImage.color.g, damageImage.color.b, colA);
            timer += 1 * Time.deltaTime;
        }
        else
        {
            isGetDamage = false;
            timer = 0;
        }
            
    }

    void OnTriggerEnter(Collider other)
    {
        //подсветка красным при получении урона
        damageImage.color = new Color(damageImage.color.r, damageImage.color.g, damageImage.color.b, 0.3f);
        isGetDamage = true;
    }


    private void MoveAndRotate()
    {
        Vector3 Movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * speed * Time.deltaTime;
        Vector3 WorldMovement = transform.TransformPoint(Movement);
        WorldMovement.y = 0f;
        Movement = transform.InverseTransformPoint(WorldMovement);

        transform.Translate(Movement);


        var rotY = Input.GetAxis("HorizontalCam") * Time.deltaTime * camSpeed;
        var rotX = Input.GetAxis("VerticalCam") * Time.deltaTime * -camSpeed;

        transform.Rotate(new Vector3(0, rotY, 0), Space.World);
        transform.Rotate(new Vector3(rotX, 0, 0), Space.Self);
    }
}

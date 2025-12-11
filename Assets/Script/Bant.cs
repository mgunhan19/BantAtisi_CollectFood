using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bant : MonoBehaviour
{
    [SerializeField] Renderer _Renderer;// Inspector�da d�zenlenebilir hale getirir.
    float BantHizi= .3f;
    // Update is called once per frame
    void Update()
    {
        if (Time.deltaTime != 0)
        //Band�n hareketini sa�lad�m burada
            _Renderer.material.SetTextureOffset("_BaseMap", new Vector2(0, -Time.time * BantHizi));
        }
    private void OnTriggerStay(Collider other)
    {
        if (Time.timeScale != 0)
            //�zerinden bulunan t�m objelere g�� uygulamas� i�in
            other.transform.Translate((BantHizi-(BantHizi/3))*Time.deltaTime*Vector3.right,Space.World);
    }
}

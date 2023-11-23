using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FishNet.Connection; // Librerias Fishnet para conexiones
using FishNet.Object;     // Librerias Fishnet para objetos de red



public class Player : NetworkBehaviour  // Clase player de red
{

    [SerializeField]
    private GameObject Guerrero;
     [SerializeField]
    private bool test; // boleano para chequeo offline

    [SerializeField]
    private float velo; // velocidad variable para el jugador
    [SerializeField]
    private Rigidbody rig;
    public float veloR; // velocidad del rigidbody    
   
   
    [SerializeField]
    private float vidas; // cantidad de vidas
    [SerializeField]
    private float energia; // porcentaje de energia
    [SerializeField]
    private float poder;  // cantidad de poder
    [SerializeField] 
    private GameObject cam; // guarda la camara main para acceder a ella una vez spawneado como Cliente


[SerializeField]
    private bool local;
    

 public override void OnStartClient() // Ejecuto antes del start el check si soy dueño o remoto
    {
        base.OnStartClient();
                    rig=GetComponent<Rigidbody>();
        if (base.IsOwner)
        {
            local=true;
            cam=GameObject.FindWithTag("MainCamera");
            cam.GetComponent<Cam>().target=gameObject.transform;

             }
        else
        {
           local=false;
   Destroy(gameObject.GetComponent<Rigidbody>());
        }
    }
 


    




    void Start()
    {
        if(test)
        local=true;
    }

    // Update is called once per frame
    void Update()
    {



        if(local==true){
            Local();
        }else{
            Remoto();
        }

    }


    void Local(){
        float rotay=Input.GetAxis("Horizontal");
        float muevez=velo*Input.GetAxis("Vertical");

    
       transform.Translate(Vector3.forward * muevez *Time.deltaTime);  
       veloR=rig.velocity.magnitude;
       
       

        if(muevez>0 ){
                        Guerrero.GetComponent<Animator>().SetInteger("mueve",1);
                    }else if(muevez<0 && veloR<0.4f){
                        Guerrero.GetComponent<Animator>().SetInteger("mueve",-1);
                    }else{
                        Guerrero.GetComponent<Animator>().SetInteger("mueve",0);
                    }
    
      //  if(rotay!=0)
     //  transform.Rotate(transform.rotation.x,transform.rotation.y+rotay*10,transform.rotation.z);     
        if(Input.GetButton("Fire1")){
                                    Guerrero.GetComponent<Animator>().SetInteger("ataque",1);
                                    }else if(Input.GetButton("Fire2")){
                                    Guerrero.GetComponent<Animator>().SetInteger("ataque",2);
                                    }else{
                                    Guerrero.GetComponent<Animator>().SetInteger("ataque",0);
                                    }
    }


     void Remoto(){
       

    }
}

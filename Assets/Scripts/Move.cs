using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
 using FishNet.Object.Synchronizing;
//This is made by Bobsi Unity - Youtube
public class Move : NetworkBehaviour
{
    [Header("Parametros Mueve")]   // Parametros basicos para el character controller
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
 
    CharacterController characterController; // Obj character con..
    Vector3 moveDirection = Vector3.zero;
  
 
    [HideInInspector]
    public bool canMove = true;
 
    [Header("Parametros grales")]      // parametros generales 
    [SerializeField]private bool local;
    [SerializeField]private Animator anim;
    
    [Header("Parametros Lucha")]
    [SerializeField]private int energia;
    [SerializeField]private int vidas;
    [SerializeField]private GameObject energiaBar;
    [SerializeField]private float golpeRate;
    [SerializeField]private bool muerto;
    
    private bool shot;

    public override void OnStartClient() // se da prioridad a esta ejecucion
    {
        base.OnStartClient();  // cuando se inicia nuestro cliente
        
        if(base.IsServer)         // Si soy server check
        Debug.Log("SERVER");
        if (base.IsOwner)           // si soy dueño guardo en variable y adquiero el char contr
        {
             local=true;
             characterController = GetComponent<CharacterController>();
       //      Cursor.lockState = CursorLockMode.Locked;                // a modificar dinamicamente
       //      Cursor.visible = false;
        }
        else
        {
            local=false;   // soy remoto

        }
    }
 

    void Start()
    {
       
    }
 
    void Update()
    {
       // envio a funciones si soy local o remoto
       if(local)
       Local();
       else
       Remoto();
       
        
 
    }



void Local(){

                         



// MOVIMIENTOS ::::::::::::::::
        float Del=Input.GetAxis("Vertical");  // guardo datos de los inputs
        float Lat=Input.GetAxis("Horizontal");


        // inicializacion de datos para movi
        bool isRunning = false;                                   
        isRunning = Input.GetKey(KeyCode.LeftShift);               
       
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
 
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Del : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Lat : 0;
        float movementDirectionY = moveDirection.y;
        
        // se calcula todo el mov...
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
 
                        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
                        {
                            moveDirection.y = jumpSpeed;
                        }
                        else
                        {
                            moveDirection.y = movementDirectionY;
                        }
                
                        if (!characterController.isGrounded)
                        {
                            moveDirection.y -= gravity * Time.deltaTime;
                        }
                // se envia al char contr.
        characterController.Move(moveDirection * Time.deltaTime);
    //roto con teclado horizontal
        if (canMove )
        {
                transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Horizontal") * lookSpeed, 0);
        }
  //::::::::::::::::::::::::::::::::


 // ANIMACION :::::::::::::::::::::
 if(Del>0)    // envio parametros al animator para animacion de mov // la anim se sincroniza con componente fishnet anim
 anim.SetInteger("mueve",1);
 else if(Del<0)
 anim.SetInteger("mueve",-1);
 else
 anim.SetInteger("mueve",0);
  //::::::::::::::::::::::::::::::::


// GOLPE :::::::::::::::::::::::::::
if(Input.GetButtonDown("Fire1") && shot==false ){
                            anim.SetInteger("ataque",1);  // envio anim de golpe
                            RaycastHit hit;
                            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 1.7f)){  // raycast para check de enemigo

                                if(hit.collider.tag=="Player"){  
                                         hit.collider.SendMessage("RemotoHit",1);
                                                            // si encuentro enemigo le envio un mensaje a funcion RemotoHit
                                             }
                                
                            }
 shot=true;

}
else
anim.SetInteger("ataque",0);
shot=false;

//::::::::::::::::::::::::::::::::::
 

}





void Remoto(){}  // Temporal

public void RemotoHit(int golpe){
                                      // recibo mensaje con golpe
                            if(local==false){                 
                                               // le resto el golpe a la energia  (esto se hace en cliente pero la idea tambien es enviar la accion al Sv y calcular ahi)
                                                            
                                                                            // envio al sv resultados
                                                            if(energia<1){
                                                                            muerto=true;
                                                                            rpcMuerteSV(muerto,energia);  // 


                                                            }else{
                                                                            energia-=golpe;
                                                                            rpcMuerteSV(false,energia);

                                                            }


                                            }
}


[ServerRpc(RequireOwnership = false)]               // Si soy Sv recibo para calcular y enviar a clientes el resultado (aca se armaria la autoridad del Sv)
    public void rpcMuerteSV(bool muerte,int ener)
    {
        rpcMuerte(muerte,ener);
    }
 

    [ObserversRpc]                                  // si soy client recibo del Sv los resultados , animo resto energia etc.
    public void rpcMuerte(bool muerte,int ener)
    {
      
      if(muerte==true){
      anim.SetBool("muerte",muerte);
      
      }

      energia=ener;
      if(energia>=0)
      energiaBar.transform.localScale=new Vector3(0.08f,0.08f,ener*0.01f);

    }



}
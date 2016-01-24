﻿
using UnityEngine;
using System.Collections;
using IsoTools;
using UnityEngine.UI;
using System;

namespace Caapora {
    [System.Serializable]
    public class Caapora : CharacterBase {


	
	public GameObject go;
	public IsoObject caapora;
	public static bool isPlayingAnimation;
	public static Caapora _instance;
    public string _moveDirection;
    public Sprite baldeCheio;
    public bool canFillBucket;
    private Vector3 direction;
    private float currentXp;
    private    Text StatusHP;
    private    GameObject balde;
    private bool _running = false;
    private Image CaaporaLifeBar;
    private Text Altura;
    private GameObject FakeRigidbody;


       
        public void Awake()
        {
           

            if (_instance == null)
            {

                DontDestroyOnLoad(this);

   
                _instance = this;

            }

            else
            {
              
                if (this != _instance)
                    Destroy(gameObject);
            }



            canFillBucket = false;

            Altura = GameObject.Find("Altura").GetComponent<Text>();

            CaaporaLifeBar = GameObject.Find("CaaporaStatus/life").GetComponent<Image>();

            StatusHP = GameObject.Find("CaaporaStatus/Status/hp").GetComponent<Text>();
            balde = GameObject.Find("baldeVazioPrefab");

           
           

          
        }


        public static Caapora instance
        {
            get
            {
                if (_instance == null)
                {


                    DontDestroyOnLoad(_instance);

                    _instance = FindObjectOfType<Caapora>() as Caapora;
                }

                return _instance;
            }
        }



        public override void Start()
        {
   
            base.Start();


            _animator = GetComponentInChildren<Animator>();

            iso_object = GetComponent<IsoObject>();
            iso_rigidyBody = GetComponent<IsoRigidbody>();

            //Hack para o isoTool
            FakeRigidbody = GameObject.Find("_FakePlayer");
            DontDestroyOnLoad(FakeRigidbody);

        }


        public override void Update()
        {

            base.Update();


            Altura.text = String.Format("{0:0.00}" , iso_object.positionZ);

            if (canFillBucket)
            {
                StartCoroutine(FillBucketSlowly());
               // SoundManager.instance.PlaySingle(SoundManager.instance.river);

            }
     
         
             

            UpdateCaaporaStatus();

            if (_running)
                run();
            else
                walk();

            currentLevel = StatsController.GetCurrentLevel();
            currentXp = StatsController.GetCurrentXp();

            AutomaticMovement(this);



            if (!GameManager.isAnimating)
                StatusHP.text = _life.ToString();



            if (!Inventory.isEmpty())
            {

                if (Balde.instance.waterPercent <= 0.0f)
                    AdviceSimple.showAdvice("Ande próximo ao lago para encher o balde com água!");
            }


            if (Inventory.isEmpty())
            {


                if (canCatch(balde))
                {

                    // Exibe a dica de tela
                    Advice.ShowAdvice(true);

                    // Checa por entrada de dados
                    if (Input.GetKeyDown(KeyCode.A) || InputController.instance.AClick)
                    {


                        AddItemToInventory(balde);

                        // Migra a animação para a do balde
                        _animator.SetTrigger("Catch");

                        balde.SetActive(false);



                    }

                }

            } // End IsEmpty


        }  //End Update()
 

        private void OnIsoCollisionEnter(IsoCollision iso_collision)
        {

            base.OnIsoCollisionEnter(iso_collision);

            if (iso_collision.gameObject.tag == "Water")
            {

                if (isPlayerWithBucket())
                {
                    GetComponent<IsoRigidbody>().velocity = Vector3.one;

                    canFillBucket = true;


                }


            }


        }


        private void OnIsoCollisionExit(IsoCollision iso_collision)
        {
            base.OnIsoCollisionExit(iso_collision);

            if (iso_collision.gameObject.tag == "Water")
            {
                canFillBucket = false;
     
            }

        }

        private void UpdateCaaporaStatus()
        {
            CaaporaLifeBar.fillAmount = _life / 1000;

        }

        public void run()
        {
      
            instance.speed = 3f;

        }



        public void walk()
        {

            instance.speed = 2f;
        }

        public static bool running
        {
            get
            {
                return instance._running;
            }
            set
            {
                instance._running = value;
            }
        }


      

        void CheckProximity()
        {


            // create a ray going into the scene from the screen location the user clicked at
            Vector3 direction = (GameObject.Find("centerRef").transform.position - transform.position).normalized;

            Debug.DrawLine(transform.position, GameObject.Find("centerRef").transform.position);

            Ray ray = new Ray(transform.position, direction);

            // the raycast hit info will be filled by the Physics.Raycast() call further
            RaycastHit hit;

            // perform a raycast using our new ray. 
            // If the ray collides with something solid in the scene, the "hit" structure will
            // be filled with collision information
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Encontrou a posicao alvo");
                Debug.DrawLine(hit.point, ray.origin);
            }

        }

      
        void AddItemToInventory(GameObject item)
        {
            Inventory.instance.itemList.Add(item);

            // Temporário
            GameObject.Find("item1").GetComponent<Image>().sprite = Resources.Load("Sprites/balde", typeof(Sprite)) as Sprite;

        }

        
        bool canCatch(GameObject go)
        {


            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {



                    if (go != null)
                        if (Mathf.RoundToInt(go.GetComponent<IsoObject>().positionX) == Mathf.RoundToInt(iso_object.positionX + x) &&
                                Mathf.RoundToInt(go.GetComponent<IsoObject>().positionY) == Mathf.RoundToInt(iso_object.positionY + y))
                        {

                            return true;
                        }


                }
            }


            return false;


        }



        public override void moveLeft()
        {


           iso_rigidyBody.velocity = new Vector3(-instance.speed, instance.speed, 0);

            _animator.SetTrigger("Caapora-left");

        }

        public override void moveRight()
        {

      
            instance.iso_rigidyBody.velocity = new Vector3(instance.speed, -instance.speed, 0);
            _animator.SetTrigger("Caapora-right");

        }

        public override void moveDown()
        {

            iso_rigidyBody.velocity = new Vector3(-instance.speed, -instance.speed, 0);
            _animator.SetTrigger("Caapora-Sul");

        }


        public override void moveUp()
        {

            instance.iso_rigidyBody.velocity = new Vector3(instance.speed, instance.speed, 0);
            _animator.SetTrigger("Caapora-Norte");

        }



        public void moveUpLeft()
        {

            instance.iso_rigidyBody.velocity = new Vector3(0, instance.speed, 0);
            _animator.SetTrigger("Caapora-Norte");

        }

        public void Jump()
        {
            iso_rigidyBody.velocity = new Vector3(0, 0, instance.speed);


        }



        public IEnumerator FillBucketSlowly()
        {


            var balde = Inventory.getItem().GetComponent<Balde>();

         
            _animator.SetTrigger("Catch");

            balde.FillBucket();


            yield return new WaitForSeconds(0.1f);



        }


       

        bool isPlayerWithBucket()
        {

            return !Inventory.isEmpty();

        }


 
	    IEnumerator RemoveBalloon() {
		
		    yield return new WaitForSeconds(2);
		    ConversationPanel.ActivePanel (false);
		

	    }


       
        public static IEnumerator AnimateCaapora(string direction, int steps){
		
			instance.caapora = instance.gameObject.GetComponent<IsoObject> ();


			isPlayingAnimation = true;
			

			for (int i = 0; i < steps; i++)
            {

                if (direction == "left")
                    moveDirection = "left";

                if (direction == "right")
                    moveDirection = "right";

                if (direction == "up")
                    moveDirection = "up";

                if (direction == "down")
                    moveDirection = "down";

                if (direction == "jump")
                    InputController.instance.AClick = true;

                isPlayingAnimation = (i == steps - 1) ? false : true;
				
				yield return new WaitForSeconds(.08f);
			}

        }



        public static string moveDirection
        {
            get
            {
                return instance._moveDirection;
            }

            set
            {
                instance._moveDirection = value;
            }
        }



      
        public static IEnumerator ShakePlayer(){

			instance.caapora = instance.gameObject.GetComponent<IsoObject> ();
	
			for (int i = 0; i < 10; i++) {

				
				instance.iso_rigidyBody.velocity = new Vector3 ( 0, 0, 0.5f);

				yield return new WaitForSeconds(.08f);	

			
			}
		
	    }




    }

}
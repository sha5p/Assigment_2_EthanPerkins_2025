using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarBoard : MonoBehaviour
{
    [Header("Camera")]
    public Camera MainCamera;
    public CinemachineCamera Camera;
    [Header("Inputs")]
    private PlayerControl playerControls;
    public InputActionReference actionReference;
    private InputAction action;

    
    [Header("Player")]
    [SerializeField] public GameObject player;
    [SerializeField] public GameObject player_main;
    [SerializeField] public GameObject player_cam;
    [SerializeField] public Movment_Basic player_script_1;
    [SerializeField] public PlayerMovement player_script_2 ;

    [Header("Car")]
    [SerializeField] public CarController_player CarController ;
    [SerializeField] public GameObject car;
    public GameObject text;
    public bool incar=false;
    void Awake()
    {
        playerControls = new PlayerControl();
        action = actionReference.action;
        action.Enable();
        action.performed += OnQuestPerformed;
    }
    void RefreshReferences()
    {
        car = GameObject.FindWithTag("YourCar");
        player_main = GameObject.FindWithTag("Player");
        // Re-fetch other references if needed, e.g. scriptOne
        if (car != null)
        {
            scriptOne = car.GetComponent<InteractableObject>();
            CarController = car.GetComponent<CarController_player>();
        }
    }
    public void Start()
    {
        RefreshReferences();
        carbought();
    }
    public void carbought()
    {
        if (PlayerPrefs.GetInt("HasBoughtCar", 0) == 1)
        {
            scriptOne.ItemName = "Click X to board car RT to drift";
        }
    }
    void OnEnable()
    {
        playerControls.Enable();
    }

    void OnDisable()
    {
        playerControls.Disable();
    }
    public InteractableObject scriptOne;
    private void OnQuestPerformed(InputAction.CallbackContext context)
    {
        car = GameObject.FindWithTag("YourCar");
        player_main = GameObject.FindWithTag("Player");
        if (incar)
        {
            GetOutOfCar();
        }
        

        else if ((Vector3.Distance(car.transform.position, player_main.transform.position) < 5f)&& car !=null)
        {
            if (PlayerPrefs.GetInt("HasBoughtCar", 0) == 1)
            {
                scriptOne.ItemName = "Click X to board car RT to drift";
                getintoCar();
            }
            else
            {
                Debug.Log("You need to buy this car first!");
            }
        }
    }

    void GetOutOfCar()
    {
        if (text != null)
        {
            scriptOne.enabled = true;
            text.SetActive(true);
            incar = false;
            player.SetActive(true);
            player_main.transform.position = car.transform.position + car.transform.TransformDirection(Vector3.left);
            CarController.enabled = false;
            player_script_1.enabled = true;
            player_script_2.enabled = true;
            MainCamera.transform.SetParent(player_main.transform);
            MainCamera.transform.localPosition = new Vector3(0, 2.25f, -4f);
            MainCamera.transform.localEulerAngles = new Vector3(17.354f, 0f, 0f);
            Camera.Follow = player_cam.transform;
            Rigidbody carRigidbody = car.GetComponent<Rigidbody>();
            carRigidbody.linearVelocity = Vector3.zero;
            carRigidbody.angularVelocity = Vector3.zero;
        }

    }

    void getintoCar()
    {
        if (text != null)
        {
            text.SetActive(false);
            scriptOne.enabled = false;
            incar = true;
            player.SetActive(false);
            player_main.transform.position = car.transform.position + car.transform.TransformDirection(Vector3.left);
            CarController.enabled = true;
            player_script_1.enabled = false;
            player_script_2.enabled = false;
            MainCamera.transform.SetParent(car.transform);
            MainCamera.transform.localPosition = new Vector3(0, 2.25f, -4f);
            MainCamera.transform.localEulerAngles = new Vector3(17.354f, 0f, 0f);

            Camera.Follow = car.transform;
        }
        
    }
}
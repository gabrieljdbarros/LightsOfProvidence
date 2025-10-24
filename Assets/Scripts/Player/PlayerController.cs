using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]



public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3f;
    public float crouchSpeed = 2f;
    public float sprintSpeed = 5f;
    public float gravity = -9.81f;

    [Header("Controller Heights")]
    public float standHeight = 1.8f;
    public float crouchHeight = 1.0f;

    [Header("Mouse Look")]
    public Transform cameraPivot;
    public float mouseSensitivity = 0.1f;
    public float lookClamp = 85f;

    [Header("Crouch Camera")]
    public float cameraStandY = 0.8f;
    public float cameraCrouchY = 0.4f;
    public float crouchTransitionSpeed = 5f;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float minStaminaToStartSprint = 10f;
    public float drainSprintPerSec = 20f;
    public float regenIdlePerSec = 25f;
    public float regenWalkPerSec = 15f;
    public float regenCrouchPerSec = 30f;
    public Slider staminaBar;

    [Header("Noise")]
    [Range(0f, 1f)] public float noiseWalk = 0.6f;
    [Range(0f, 1f)] public float noiseCrouch = 0.2f;
    [Range(0f, 1f)] public float noiseSprint = 1.0f;
    public float maxNoiseRadius = 12f;


    public float CurrentNoise { get; private set; }
    public float NoiseRadius => CurrentNoise * maxNoiseRadius;

    // Internos
    [SerializeField] private InputActionReference sprintAction;
    private CharacterController cc;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private float pitch;
    private bool isCrouched;
    private bool isSprinting;
    private float stamina;
    private PlayerInput pi;

    void OnEnable()  { if (sprintAction) sprintAction.action.Enable(); }
    void OnDisable() { if (sprintAction) sprintAction.action.Disable(); }
    void Start()
    {


        cc = GetComponent<CharacterController>();
        pi = GetComponent<PlayerInput>();
        if (!cameraPivot) cameraPivot = Camera.main ? Camera.main.transform : null;

        // Alturas e câmera
        cc.height = standHeight;
        cc.center = new Vector3(0f, cc.height / 2f, 0f);
        if (cameraPivot) cameraPivot.localPosition = new Vector3(0f, cameraStandY, 0f);

        // Cursor travado
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        stamina = maxStamina;
        UpdateStaminaUI();
    }


    void OnMove(InputValue v) => moveInput = v.Get<Vector2>();
    void OnLook(InputValue v) => lookInput = v.Get<Vector2>();

    void OnCrouch(InputValue v)
    {
        if (v.isPressed)
        {
            isCrouched = !isCrouched;
            if (isCrouched) isSprinting = false; // agachar cancela sprint

            cc.height = isCrouched ? crouchHeight : standHeight;
            cc.center = new Vector3(0f, cc.height / 2f, 0f);
        }
    }

    // Sprint ativo enquanto o botão estiver pressionado (ex.: LeftShift)
    void OnSprint(InputValue v)
    {
        // só pode iniciar sprint se não estiver agachado e tiver stamina suficiente
        if (v.isPressed)
            isSprinting = !isCrouched && stamina >= minStaminaToStartSprint;
        else
            isSprinting = false;
    }

    void OnInteract(InputValue v)
    {
        if (v.isPressed)
        {
            // Aqui você colocará a lógica futura de interação (raycast, pegar item, abrir porta, etc.)
            Debug.Log("Interagir acionado!");
        }
    }

    void Update()
    {
        //Velocidade atual
        bool hasMoveInput = moveInput.sqrMagnitude > 0.0001f;

        // Sprint via InputActionReference (arraste no Inspector)
        bool sprintHeld = sprintAction && sprintAction.action.IsPressed();

        // Só pode sprintar se tiver stamina suficiente
        bool canStartSprint = stamina >= minStaminaToStartSprint;

        // Regras de sprint
        if (isCrouched) 
        isSprinting = false;
        else 
        isSprinting = sprintHeld && (isSprinting || canStartSprint);

        if (isSprinting && stamina <= 0.01f) 
        isSprinting = false;

        // Define velocidade atual
        float currentSpeed = isCrouched ? crouchSpeed : (isSprinting ? sprintSpeed : walkSpeed);


        //  Movimento no plano
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        cc.Move(move * currentSpeed * Time.deltaTime);

        //  Gravidade
        if (cc.isGrounded && velocity.y < 0f) velocity.y = -2f; // mantém colado no chão
        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);

        //  Look (yaw/pitch) 
        float yaw = lookInput.x * mouseSensitivity;
        float deltaPitch = -lookInput.y * mouseSensitivity;
        transform.Rotate(0f, yaw, 0f);
        pitch = Mathf.Clamp(pitch + deltaPitch, -lookClamp, lookClamp);
        if (cameraPivot) cameraPivot.localEulerAngles = new Vector3(pitch, 0f, 0f);

        //  Transição da câmera com crouch 
        if (cameraPivot)
        {
            float targetY = isCrouched ? cameraCrouchY : cameraStandY;
            Vector3 lp = cameraPivot.localPosition;
            lp.y = Mathf.Lerp(lp.y, targetY, Time.deltaTime * crouchTransitionSpeed);
            cameraPivot.localPosition = lp;
        }

        //  Stamina: dreno/regen
        float delta = 0f;
        if (isSprinting && hasMoveInput && !isCrouched)
        {
            delta = -drainSprintPerSec * Time.deltaTime;
        }
        else
        {
            if (!hasMoveInput) delta = regenIdlePerSec * Time.deltaTime;
            else if (isCrouched) delta = regenCrouchPerSec * Time.deltaTime;
            else delta = regenWalkPerSec * Time.deltaTime;
        }

        stamina = Mathf.Clamp(stamina + delta, 0f, maxStamina);
        UpdateStaminaUI();

        //  Noise (para os inimigos - NÃO IMPLEMENTADO AINDA / ainda precisamos colocar o input do mic) 
        if (!hasMoveInput) CurrentNoise = 0f;
        else if (isCrouched) CurrentNoise = noiseCrouch;
        else if (isSprinting) CurrentNoise = noiseSprint;
        else CurrentNoise = noiseWalk;
    if (Time.frameCount % 20 == 0) Debug.Log($"stam:{stamina:0.0}");

    }

    private void UpdateStaminaUI()
    {
        if (staminaBar)
        {
            staminaBar.maxValue = maxStamina;
            staminaBar.value = stamina;
        }
    }
    
}

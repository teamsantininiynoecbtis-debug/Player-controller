using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 5.0f;
    public float runSpeed = 9.0f;
    public float sensitivity = 2.0f;
    public float multiplicadorVelocidad = 1.0f; // NUEVO: Para penalización de peso
    
    [Header("Estamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaDrain = 20f;
    public float staminaRegen = 15f;
    public float exhaustedDelay = 2.0f; 
    
    public Image staminaBar;
    private CanvasGroup staminaCanvasGroup;
    private CharacterController controller;
    private float rotationX = 0f;
    private float timer = 0;
    private float cooldownTimer = 0f; 
    
    public Transform weapon;
    private Vector3 weaponDefaultPos;
    private bool isExhausted = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentStamina = maxStamina;
        weaponDefaultPos = weapon.localPosition;
        Cursor.lockState = CursorLockMode.Locked;

        if (staminaBar != null)
        {
            staminaCanvasGroup = staminaBar.GetComponent<CanvasGroup>();
            if (staminaCanvasGroup == null) staminaCanvasGroup = staminaBar.gameObject.AddComponent<CanvasGroup>();
            staminaCanvasGroup.alpha = 0;
        }
    }

    void Update()
    {
        // 1. ROTACIÓN
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        Camera.main.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // 2. LÓGICA DE CARRERA
        bool wantsToRun = Input.GetKey(KeyCode.LeftShift);
        bool isMoving = controller.velocity.magnitude > 0.1f;
        
        if (isExhausted && currentStamina >= 20f) isExhausted = false;

        bool canRun = wantsToRun && !isExhausted && currentStamina > 0 && isMoving;
        // Se aplica el multiplicador de peso aquí
        float currentSpeed = (canRun ? runSpeed : walkSpeed) * multiplicadorVelocidad;

        if (canRun)
        {
            currentStamina -= staminaDrain * Time.deltaTime;
            if (currentStamina <= 0) { currentStamina = 0; isExhausted = true; cooldownTimer = exhaustedDelay; }
        }
        else
        {
            if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;
            else currentStamina += staminaRegen * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        if (staminaCanvasGroup != null)
        {
            if (currentStamina < maxStamina) staminaCanvasGroup.alpha = 1; 
            else staminaCanvasGroup.alpha -= Time.deltaTime * 2f; 
        }
        if (staminaBar != null) staminaBar.fillAmount = currentStamina / maxStamina;

        // 3. MOVIMIENTO
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // 4. BALANCEO
        if (move.magnitude > 0.1f)
        {
            float speedMultiplier = canRun ? 1.5f : 1f;
            timer += Time.deltaTime * (10f * speedMultiplier);
            weapon.localPosition = new Vector3(
                weaponDefaultPos.x + Mathf.Cos(timer / 2) * 0.05f,
                weaponDefaultPos.y + Mathf.Sin(timer) * 0.05f,
                weaponDefaultPos.z
            );
        }
    }
}
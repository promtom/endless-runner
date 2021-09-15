using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveController : MonoBehaviour
{    
    private Rigidbody2D rig;

    [Header("Movement")]
    public float moveAccel;
    public float maxSpeed;
    
    [Header("Jump")]
    public float jumpAccel;
    private bool isJumping; // pake raycast and Layer
    private bool isOnGround; // pake raycast and Layer
    
    [Header("Ground Raycast")]
    public float groundRaycastDistance;
    public LayerMask groundLayerMask;
    [Header("Scoring")]
    public ScoreController score;
    public float scoringRatio;
    private float lastPositionX;
       
    [Header("GameOver")]
    public GameObject gameOverScreen;
    public float fallPositionY;

    [Header("Camera")]
    public CameraMoveController gameCamera;
    
    private Animator anim;

    private CharacterSoundController sound;
    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sound = GetComponent<CharacterSoundController>();
    }
    // Update is called once per frame
    private void Update()
    {
        // read input
        //Input.GetMouseButtonDown(0) = BUAT KLIK KIRI PADA MOUSE
        if (Input.GetKeyDown("space"))
        {
            if (isOnGround)
            {
                isJumping = true;
                sound.PlayJump();
            }
        }
        // change animation
        anim.SetBool("isOnGround", isOnGround);
        // calculate score
        int distancePassed = Mathf.FloorToInt(transform.position.x - lastPositionX);
        int scoreIncrement = Mathf.FloorToInt(distancePassed / scoringRatio);

        if (scoreIncrement > 0)
        {
            score.IncreaseCurrentScore(scoreIncrement);
            lastPositionX += distancePassed;
        }
        // game over
        if (transform.position.y < fallPositionY)
        {
            GameOver();
        }
    }

    //Fungsi ini dijalankan tiap waktu yang fixed. Waktu ini diset sebagai dari physics engine (biasanya pada 0.02 detik). Pemanggilan fungsi ini tidak terpengaruh oleh frame-rate dan akan selalu stabil.
    //fungsi ini hanya dianjurkan untuk kalkulasi physics saja (karena lambat)
    private void FixedUpdate()
    {
        // raycast ground
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundRaycastDistance, groundLayerMask);
        if (hit)
        {
            if (!isOnGround && rig.velocity.y <= 0)
            {
                isOnGround = true;
            }
        }
        else
        {
            isOnGround = false;
        }

        // calculate velocity vector
        Vector2 velocityVector = rig.velocity;

        if (isJumping)
        {
            velocityVector.y += jumpAccel;
            isJumping = false;
        }

        velocityVector.x = Mathf.Clamp(velocityVector.x + moveAccel * Time.deltaTime, 0.0f, maxSpeed);

        rig.velocity = velocityVector;
    }

    private void GameOver()
    {
        // set high score
        score.FinishScoring();

        // stop camera movement
        gameCamera.enabled = false;

        // show gameover
        gameOverScreen.SetActive(true);

        // disable this too
        this.enabled = false;
    }

    /*DEBUG*/    
    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + (Vector3.down * groundRaycastDistance), Color.white);
    }
}

using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;


public class BallScript : MonoBehaviour
{
    public enum GameState
    {
        Playing,
        GameOver,
        Win
    }
    public float gravityScale = 3f;
    public float tiltSpeed = 10f;
    public Vector2 direction = new(0, 0);
    Vector3 initialPosition = new(0, 0, 0);

    public GameObject directionArrow;
    public GameObject verticalTiltArrow;
    public GameObject horizontalTiltArrow;

    public Rigidbody2D rigidBody2D;
    public TMP_Text gameStateText;
    public TMP_Text timerText;

    private float initialTime = 0f;
    private float timeElapsed = 0f;

    public GameState currentGameState = GameState.Playing;

    void Start()
    {
        initialTime = 0f;
        initialPosition = transform.position;
        gameStateText.text = "Playing";
    }

    private void Update()
    {
        UpdateGameState(currentGameState);
        UpdateTimer();
        ManageInput();
        DisplayGravityArrow();
    }

    void FixedUpdate()
    {
        Move();
    }

    void UpdateTimer()
    {
        if (currentGameState == GameState.Playing)
        {
            timeElapsed = Time.timeSinceLevelLoad - initialTime;
            int minutes = Mathf.FloorToInt(timeElapsed / 60f);
            int seconds = Mathf.FloorToInt(timeElapsed % 60f);
            timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
        }
    }

    void ManageInput()
    {
        if (Input.GetKey(KeyCode.D))
        {
            direction.x += tiltSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction.x -= tiltSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.W))
        {
            direction.y += tiltSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction.y -= tiltSpeed * Time.deltaTime;
        }
        direction = Vector3.ClampMagnitude(direction, 1f);

        if (currentGameState != GameState.Playing && Input.GetKeyDown(KeyCode.Space))
        {
            Restart();
        }
    }

    void Move()
    {
        if (currentGameState == GameState.Playing)
        {
            rigidBody2D.AddForce(direction * gravityScale);
        }
        else
        {
            rigidBody2D.linearVelocity = Vector2.zero;
        }
    }

    void DisplayGravityArrow()
    {
        directionArrow.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        directionArrow.transform.localScale = 10 * direction.magnitude * new Vector3(1, 1, 1);
        horizontalTiltArrow.transform.localScale =  new Vector3(10, 10 * direction.x, 1);
        verticalTiltArrow.transform.localScale = new Vector3(10, 10 * direction.y, 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Hazard"))
        {
            UpdateGameState(GameState.GameOver);
        }
        else if (collision.gameObject.CompareTag("Goal"))
        {
            UpdateGameState(GameState.Win);
        }
    }

    void UpdateGameState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Playing:
                currentGameState = GameState.Playing;
                gameStateText.text = "Playing";
                break;
            case GameState.GameOver:
                currentGameState = GameState.GameOver;
                gameStateText.text = "Game Over: Press SPACE to Restart";
                break;
            case GameState.Win:
                currentGameState = GameState.Win;
                gameStateText.text = "You Win!: Press SPACE to Restart";
                break;
        }
    }

    void Restart()
    {
        currentGameState = GameState.Playing;
        initialTime = Time.timeSinceLevelLoad;
        gameStateText.text = "Playing";
        direction = Vector2.zero;
        transform.position = initialPosition;
        rigidBody2D.linearVelocity = Vector2.zero;
    }

        IEnumerator WaitAndRestart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

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
    public float tiltSpeed = .1f;
    public Vector2 direction = new(0, 0);
    public GameObject directionArrow;
    public Rigidbody2D rigidBody2D;
    public TMP_Text gameStateText;
    public GameState currentGameState = GameState.Playing;
    void FixedUpdate()
    {
        UpdateGameState(currentGameState);
        ManageInput();
        Move();
        DisplayGravityArrow();
    }

    void ManageInput()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            direction.x += tiltSpeed;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            direction.x -= tiltSpeed;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            direction.y += tiltSpeed;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            direction.y -= tiltSpeed;
        }
        direction = Vector3.ClampMagnitude(direction, 1f);

        if (currentGameState != GameState.Playing && Input.GetKeyDown(KeyCode.Space))
        {
           StartCoroutine(WaitAndRestart(0.1f));
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

    IEnumerator WaitAndRestart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

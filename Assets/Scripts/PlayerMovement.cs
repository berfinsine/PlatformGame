using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rgb;
    Vector3 velocity;

    float speedAmount = 3f;
    float jumpAmount = 5f;
    public Animator animator;
    private Vector3 respawnPoint;
    public GameObject fallDetector;

    private int jumpCount = 0;
    private int maxJumpCount = 2;

    public TextMeshProUGUI scoreText;
    private int collectedCherries = 0;
    private int cherriesToCollect = 5;
    private bool canEnterDoor = false;

    // Start is called before the first frame update
    void Start()
    {
        rgb = GetComponent<Rigidbody2D>();
        respawnPoint = transform.position;
        scoreText.text = "Score: " + Score.totalScore;
    }

    // Update is called once per frame
    void Update()
    {
        velocity = new Vector3(Input.GetAxis("Horizontal"), 0f);
        transform.position += velocity * speedAmount * Time.deltaTime;
        animator.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Horizontal")));

        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount)
        {
            AudioManager.instance.Play("Jump");
            rgb.velocity = new Vector2(rgb.velocity.x, jumpAmount);
            jumpCount++;
            animator.SetBool("isJumping", true);
        }

        if (Mathf.Approximately(rgb.velocity.y, 0))
        {
            animator.SetBool("isJumping", false);
            jumpCount = 0;
        }

        if (Input.GetAxisRaw("Horizontal") == -1)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else if (Input.GetAxisRaw("Horizontal") == 1)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "FallDetector")
        {
            transform.position = respawnPoint;
        }
        else if (collision.tag == "Cherry")
        {
            Score.totalScore += 150;
            scoreText.text = "Score: " + Score.totalScore;
            collision.gameObject.SetActive(false);
            AudioManager.instance.Play("Cherry");
            collectedCherries++;
            CheckCherryCount();
        }
        else if (collision.tag == "Star")
        {
            Score.totalScore += 250;
            scoreText.text = "Score: " + Score.totalScore;
            collision.gameObject.SetActive(false);
            AudioManager.instance.Play("Cherry");
        }
        else if (collision.tag == "NextLevel" && canEnterDoor)
        {
            AudioManager.instance.Play("NextCompleted");
            LoadNextLevel();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Enemy")
        {
            // Check if player is above the enemy
            if (rgb.velocity.y < 0 && transform.position.y > collision.transform.position.y + 0.5f)
            {
                Destroy(collision.gameObject);
                Score.totalScore += 300;
                scoreText.text = "Score: " + Score.totalScore;
            }
            else
            {
                transform.position = respawnPoint;
            }
        }
    }

    void CheckCherryCount()
    {
        if (collectedCherries >= cherriesToCollect)
        {
            canEnterDoor = true;
        }
    }

    void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
            respawnPoint = transform.position;
        }
    }
}



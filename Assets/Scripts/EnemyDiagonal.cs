using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDiagonal : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f;
    private float _horizontalSpeed = 2.5f;
    private Player _player;
    private Animator _anim;
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        if (_player == null)
        {
            Debug.LogError("The Player is NULL.");
        }

        _anim = GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.LogError("The Animator is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        EnemyMovement();
        
    }

    void EnemyMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        transform.Translate(Vector3.right * _horizontalSpeed * Time.deltaTime);

        if (transform.position.y <= -9f)
        {
            float randomX = Random.Range(5f, -5f);
            transform.position = new Vector3(randomX, 9, 0);
        }
        
        if (transform.position.x >= 8)
        {
            _horizontalSpeed *= -1;
        }
        if (transform.position.x <= -8)
        {
            _horizontalSpeed *= -1;
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Collided");
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null) 
            {
                player.AddScore(10);
                player.Damage(1);
            }
            _anim.SetTrigger("OnEnemyDeath");                            // can also pass OnEnemyDeath ID
            _speed = 0;
            _horizontalSpeed = 0;
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.5f);
        }
        
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            if(_player != null)
            {
                _player.AddScore(10);
            }
            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _horizontalSpeed = 0;
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.5f);
        }
    }
}

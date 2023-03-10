using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{   //public or private reference
    //date type (int, float, bool, string)
    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _seekingLaserPrefab;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private bool _isTripleShotActive = false;
    [SerializeField]
    private GameObject _enhancedTripleShotPrefab;
    [SerializeField]
    private bool _seekingLaser = false;
    [SerializeField]
    private bool _isEnhancedTripleShotActive = false;
    [SerializeField]
    private bool _moreSpeed = false;
    private int _speedMultiplier = 2;
    [SerializeField]
    private bool _shieldActive = false;
    [SerializeField]
    private bool _shieldActive1 = false;
    [SerializeField]
    private bool _shieldActive2 = false;
    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private GameObject _shieldVisualizer1;
    [SerializeField]
    private GameObject _shieldVisualizer2;
    [SerializeField]
    private int _score;
    private UIManager _uiManager;

    [SerializeField]
    private GameObject _rightEngine, _leftEngine;

    [SerializeField]
    private AudioClip _laserSoundClip;
    [SerializeField]
    private AudioSource _audioSource;
    private float _fasterSpeed = 10f;

    private int _ammoCount = 10;

    [SerializeField]
    private GameObject _smallThruster;
    [SerializeField]
    private GameObject _bigThruster;
    [SerializeField]
    private bool _shiftSpeedActive = false;
    private string _chargingText = "l";
    private bool _isDischarging = false;
    private Camera _camera;
    //private Enemy _enemy;
    //private Powerup _powerup;

    // Enemies
    [SerializeField]
    private GameObject _enemyDiagonal;
    [SerializeField]
    private GameObject _enemyHorizontal;

    

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpeedCharging());
        transform.position = new Vector3(0,-4,0);
        _leftEngine.SetActive(false);
        _rightEngine.SetActive(false);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>(); //could also put in Spawn Manager tag
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        _camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        //_enemy = GameObject.Find("Enemy").GetComponent<Enemy>();
        //_powerup = FindAnyObjectByType<Powerup>();

        if (_audioSource == null)
        {
            Debug.LogError("The player's audio source is NULL");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }
        if (_spawnManager == null)
        {
            Debug.LogError("The spawn Manager is NULL");
        }
        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL");
        }
        if (_camera == null)
        {
            Debug.LogError("The camera is NULL");
        }
        //if (_enemy == null)
        //{
        //    Debug.LogError("The enemy is NULL");
        //}
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        FireLaser();
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal"); //move left, right with a, d
        float verticalInput = Input.GetAxis("Vertical"); //move up, down with w, s

        //new Vector3(1,0,0) * input * _speed * real time
        //transform.Translate(Vector3.right * horizontalInput * _speed * Time.deltaTime);
        //transform.Translate(Vector3.up * verticalInput * _speed * Time.deltaTime);

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        // 2D Game Part III ---------------------------------------------------------------------------------------------------------
        if (Input.GetKey(KeyCode.LeftShift) && _shiftSpeedActive == true)
        {
            transform.Translate(direction * _fasterSpeed * Time.deltaTime);
            _smallThruster.gameObject.SetActive(false);
            _bigThruster.gameObject.SetActive(true);
            if (_isDischarging != true)
            {
                StartCoroutine(SpeedDecharging());
            }
            
        } 
        else
        {
            transform.Translate(direction * _speed * Time.deltaTime);
            _bigThruster.gameObject.SetActive(false);
            _smallThruster.gameObject.SetActive(true);
        }
        // 2D Game Part III ---------------------------------------------------------------------------------------------------------


        if (transform.position.y >= 3)
        {
            transform.position = new Vector3(transform.position.x, 3, 0);
        }
        else if (transform.position.y <= -3.8f)
        {
            transform.position = new Vector3(transform.position.x, -3.8f, 0);
        }
        

        //transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x <= -10)
        {
            transform.position = new Vector3(10, transform.position.y, 0);
        }
        else if (transform.position.x >= 10)
        {
            transform.position = new Vector3(-10, transform.position.y, 0);
        }
    }

    void FireLaser()
    {

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire && _ammoCount > 0)                                 //GetKey for continuous fire
        {
            _canFire = Time.time + _fireRate;
            _audioSource.Play();
            if (_isTripleShotActive == true)
            {
                Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
                AmmoDisplay();
            }
            else if (_isEnhancedTripleShotActive == true)
            {
                Instantiate(_enhancedTripleShotPrefab, transform.position, Quaternion.identity);
                AmmoDisplay();
            }
            else if (_seekingLaser == true)
            {
                Instantiate(_seekingLaserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);   //enemy.transform.scale = new Vector3()  // for rotation, instead of Quat.iden, use 
                AmmoDisplay();
            }
            else
            {
                Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
                AmmoDisplay();
            }       
        }       
    }

    public void Damage(int livesChange) 
    {
        if (_shieldActive == true)
        {
            _shieldActive = false;
            _shieldVisualizer.SetActive(false);
            ShieldActive1();                            // part III
            return;
        }
        if (_shieldActive1 == true)
        {
            _shieldActive1 = false;
            _shieldVisualizer1.SetActive(false);
            ShieldActive2();
            return;
        }
        if (_shieldActive2 == true)
        {
            _shieldActive2 = false;
            _shieldVisualizer2.SetActive(false);
            return;
        }
        if (_lives < 6 && livesChange < 0)
        {
            _lives -= livesChange;
        }
        else if (livesChange > 0)
        {
            _lives -= livesChange;
            StartCoroutine(_camera.CameraShake());
        }



        if (_lives == 2)
        {
            _leftEngine.SetActive(true);
            _rightEngine.SetActive(false);
        }
        if (_lives == 1)
        {
            _rightEngine.SetActive(true);
        }
        if (_lives > 2)
        {
            _leftEngine.SetActive(false);
            _rightEngine.SetActive(false);
        }
        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();

            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        while (_isTripleShotActive == true)
        {
            yield return new WaitForSeconds(5f);
            _isTripleShotActive = false;
        }
    }

    public void MoreSpeedActive()
    {
        _moreSpeed = true;
        _speed *= _speedMultiplier;
        StartCoroutine(MoreSpeedPowerDownRoutine());
    }

    IEnumerator MoreSpeedPowerDownRoutine()
    {
        while (_moreSpeed == true)
        {
            yield return new WaitForSeconds(5f);
            _moreSpeed = false;
            _speed /= _speedMultiplier;
        }
    }

    public void ShieldActive()
    {
        _shieldActive = true;
        _shieldVisualizer.SetActive(true);
        _shieldActive1 = false;
        _shieldVisualizer1.SetActive(false);
        _shieldActive2 = false;
        _shieldVisualizer2.SetActive(false);
    }
// 2D Game Part III -------------------------------------------------------------------------------
    public void ShieldActive1()
    {
        _shieldActive1 = true;
        _shieldVisualizer1.SetActive(true);
    }
    public void ShieldActive2()
    {
        _shieldActive2 = true;
        _shieldVisualizer2.SetActive(true);
    }
// 2D Game Part III -------------------------------------------------------------------------------

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
        _spawnManager.SpawnController(_score);
    }

    public void AmmoDisplay()
    {
        _ammoCount -= 1;
        _uiManager.UpdateAmmo(_ammoCount);
    }

    public void MoreAmmoActive(int ammo)
    {
        if (_ammoCount < 10)
        {
            _ammoCount += ammo;
            if (_ammoCount > 10)
            {
                _ammoCount = 10;
            }
        }
        else
        {
            return;
        }
        _uiManager.UpdateAmmo(_ammoCount);
    }

    public void EnhancedTripleShotActive()
    {
        _isEnhancedTripleShotActive = true;
        StartCoroutine(EnhancedTripleShotPowerDownRoutine());
    }

    IEnumerator EnhancedTripleShotPowerDownRoutine()
    {
        while (_isEnhancedTripleShotActive == true)
        {
            yield return new WaitForSeconds(5f);
            _isEnhancedTripleShotActive = false;
        }
    }

    IEnumerator SpeedDecharging()
    {
        _isDischarging = true;    
        while (_shiftSpeedActive == true)
        {
            yield return new WaitForSeconds(0.1f);
            _chargingText = _chargingText.Remove(_chargingText.Length-1);
            _uiManager.Charger(_chargingText);
            if (_chargingText == "l")
            {
                _shiftSpeedActive = false;
                StartCoroutine(SpeedCharging());
            }
        }
        _isDischarging = false;
    }

    IEnumerator SpeedCharging()
    {
        while (_shiftSpeedActive == false)
        {
            yield return new WaitForSeconds(0.1f);
            _chargingText += "l";
            _uiManager.Charger(_chargingText);
            if (_chargingText == "llllllllllllllllllll")
            {
                _shiftSpeedActive = true;
                
            }
        }
    }

    public void SeekingLaserActive()
    {
        _seekingLaser = true;
        StartCoroutine(SeekingLaserPowerDownRoutine());
    }

    IEnumerator SeekingLaserPowerDownRoutine()
    {
        while (_seekingLaser == true)
        {
            yield return new WaitForSeconds(5f);
            _seekingLaser = false;
        }
    }
}



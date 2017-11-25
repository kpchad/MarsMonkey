
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    [SerializeField] float rotThrust = 200f;
    [SerializeField] float forwardThrust = 800f;
    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
	}

    // Update is called once per frame
    void Update() {
        // todo: stop sound on death
        if (state == State.Alive) {
        Thrust();
        Rotate();
        }
        else{
            
        }
	}

    void OnCollisionEnter(Collision collision){

        if (state != State.Alive){return;}

        switch (collision.gameObject.tag){
            case "Friendly":
                break;
            case "Finish":
                state = State.Transcending;
                Invoke("LoadNextScene", 1f); // parameterize time
                break;
            default:
                print("dead");
                state = State.Dying;
                Invoke("LoadFirstScene", 1f); // parameterize time
                break;
        }
    }

    private void LoadFirstScene() {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene() {
        SceneManager.LoadScene(1); // todo: allow for more than 2 levels
    }

    private void Thrust() {
        if (Input.GetKey(KeyCode.Space)) { // can thrust while rotating
            float thrustThisFrame = forwardThrust * Time.deltaTime;
            rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
            if (!audioSource.isPlaying) { // so it doesnt layer
                audioSource.Play();
            }
        }
        else {
            audioSource.Stop();
        }
    }

    private void Rotate() {
        rigidBody.freezeRotation = true; // take manual control of rotation

        float rotationThisFrame = rotThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A)) {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D)) {
            transform.Rotate(Vector3.back * rotationThisFrame);
        }

        rigidBody.freezeRotation = false; // resume physics control of rotaiton
    }
}

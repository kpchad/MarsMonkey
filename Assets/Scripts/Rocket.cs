﻿
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] float rotThrust = 200f;
    [SerializeField] float forwardThrust = 800f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip success;
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem successParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;
    bool collisionsDisabled = false;

	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
	}

    // Update is called once per frame
    void Update() {
        // todo: stop sound on death
        if (state == State.Alive) {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        else{
        }
        if (Debug.isDebugBuild){
            RespondToDebugKeys(); 
        }
	}

    void OnCollisionEnter(Collision collision){

        if (state != State.Alive || collisionsDisabled){return;}

        switch (collision.gameObject.tag){
            case "Friendly":
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartSuccessSequence() {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        Invoke("LoadNextScene", levelLoadDelay);
    }

    private void StartDeathSequence() {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("LoadFirstScene", levelLoadDelay);
    }

    private void LoadFirstScene() {
        print("load first scene"); 
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene() {
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = buildIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings){ // change for max number of levels
            nextSceneIndex = 0; // loop back to start
        }
        SceneManager.LoadScene(nextSceneIndex); 
    }

    private void RespondToThrustInput() {
        if (Input.GetKey(KeyCode.Space)) { // can thrust while rotating
            ApplyThrust();
        }
        else {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust() {
        float thrustThisFrame = forwardThrust * Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
        if (!audioSource.isPlaying) { // so it doesnt layer
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    private void RespondToRotateInput() {
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

    private void RespondToDebugKeys() {
        if (Input.GetKeyDown(KeyCode.L)) {
            LoadNextScene();
        }
        else if (Input.GetKeyDown(KeyCode.C)) {
            collisionsDisabled = !collisionsDisabled;
            print(collisionsDisabled);
        }
    }
}

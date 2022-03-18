using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using Unity.MLAgents;
using UnityEngine;

public class Area : MonoBehaviour
{
    public enum eLesson
    {
        lesson0 = 30, lesson1 = 40, lesson2 = 50, lesson3 = 60
    }
    public GameObject pipeBodyPrefab;
    public float gapSize = 50f;     //45
    public BirdAgent agent;
    private float span = 2.4f;      //1.8f
    private float delta;
    [Header("Lesson 테스트용입니다")]
    public eLesson lesson = eLesson.lesson0;
    public NNModel birdBrain;

    private const float PIPE_SPAWN_X_POSITION = 100f;
    private const float CAMERA_ORTHO_SIZE = 50f;
    private const float PIPE_WIDTH = 7.8f;

    List<GameObject> pipes = new List<GameObject>();
    EnvironmentParameters resetParams;

    private void Start()
    {
        //Debug.Log("[Area] Start");
        Application.runInBackground = true;

        this.agent.onEndEpisode = () => {

            for (int i = 0; i < this.pipes.Count; i++) {
                var pipe = this.pipes[i];
                Destroy(pipe);
            }

            pipes.Clear();
        };

        this.agent.onEpisodeBegin = (resetParams) => {
            this.resetParams = resetParams;
        };
    }

    void Update()
    {
        this.delta += Time.deltaTime;
        if (this.delta >= this.span) {
            this.delta = 0;
            this.CreatePipe();
        }
    }

    private void CreatePipe()
    {
        var lessonHeight = this.resetParams.GetWithDefault("pipe_height", 30);
        lessonHeight = 100;

        if (lessonHeight <= 0) {
            return;
        }

        Transform pipeBodyBottom = Instantiate(this.pipeBodyPrefab, this.transform).transform;
        Transform pipeBodyTop = Instantiate(this.pipeBodyPrefab, this.transform).transform;

        this.pipes.Add(pipeBodyBottom.gameObject);
        this.pipes.Add(pipeBodyTop.gameObject);

        //bottom 위치 
        pipeBodyBottom.position = new Vector3(this.transform.position.x + PIPE_SPAWN_X_POSITION, -CAMERA_ORTHO_SIZE, 0);

        //top 위치 
        pipeBodyTop.position = new Vector3(this.transform.position.x + PIPE_SPAWN_X_POSITION, +CAMERA_ORTHO_SIZE, 0);
        pipeBodyTop.localScale = new Vector3(1, -1, 1);


        float bottmHeight = 0;
        float topHeight = 0;

        if (lessonHeight == 100)
        {
            var randomHeight = Random.Range(30, 70);
            //bottom sprite height
            bottmHeight = (int)randomHeight - gapSize * .5f;
            pipeBodyBottom.GetComponent<SpriteRenderer>().size = new Vector2(PIPE_WIDTH, bottmHeight);

            //top sprite height
            topHeight = CAMERA_ORTHO_SIZE * 2f - (int)randomHeight - gapSize * .5f;
            //float topHeight = (int)lessonHeight - gapSize * .5f;
            pipeBodyTop.GetComponent<SpriteRenderer>().size = new Vector2(PIPE_WIDTH, topHeight);
        }
        else {
            bottmHeight = (int)lessonHeight - gapSize * .5f;
            pipeBodyBottom.GetComponent<SpriteRenderer>().size = new Vector2(PIPE_WIDTH, bottmHeight);

            //top sprite height
            //topHeight = CAMERA_ORTHO_SIZE * 2f - (int)randomHeight - gapSize * .5f;
            topHeight = (int)lessonHeight - gapSize * .5f;
            pipeBodyTop.GetComponent<SpriteRenderer>().size = new Vector2(PIPE_WIDTH, topHeight);
        }


        BoxCollider2D pipeBodyBoxColliderBottom = pipeBodyBottom.GetComponent<BoxCollider2D>();
        pipeBodyBoxColliderBottom.size = new Vector2(PIPE_WIDTH * .6f, bottmHeight);
        pipeBodyBoxColliderBottom.offset = new Vector2(0f, bottmHeight * .5f);


        BoxCollider2D pipeBodyBoxColliderTop = pipeBodyTop.GetComponent<BoxCollider2D>();
        pipeBodyBoxColliderTop.size = new Vector2(PIPE_WIDTH * .6f, topHeight);
        pipeBodyBoxColliderTop.offset = new Vector2(0f, topHeight * .5f);

        pipeBodyBottom.GetComponent<Pipe>().OnPassPipe = () => {
            this.agent.AddReward(1.0f);
        };

        this.agent.SetModel("FlappyBird", this.birdBrain);

    }
}

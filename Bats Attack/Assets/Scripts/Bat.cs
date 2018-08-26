#define Debug
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour
{
    private Player player;
    private GameObject nextPoint;
    Vector3 AB;
    private bool isMovingToPlayer = false;
    private bool isMovingToDest = false;
    private float timer;
    private float pauseTime;

    private float speed = 5f;

    private int subdivs = 20;

    private float[] _speedsByChordsLengths;
    private float _totalLength;

    private Vector3 _p0;
    private Vector3 _p1;
    private Vector3 _p2;
    
    private float _t;

#if Debug
    private Vector3 _prevPos;
    private List<Vector3> _testPositions = new List<Vector3>();
#endif

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    void Start()
    {
        transform.position = GetRandCoords();
        pauseTime = Random.Range(1, 10);
    }

    void Update()
    {
        //если никуда не движемся - ждем некоторое время и задаем точку, в которую будем двигаться
        if (!isMovingToPlayer && !isMovingToDest)
        {
            timer += 1 * Time.deltaTime;
            if (timer >= pauseTime)
            {                
                SetNewDest();
                isMovingToPlayer = true;
                SetPointToMove();
                timer = 0;
            }                        
        }

        if (isMovingToPlayer)
        {
            //если достигли точки, в которой стоял игрок, то летим в точку назначения
            if (Vector3.Distance(transform.position, _p2) <= 0.5f)
            {
                isMovingToPlayer = false;
                isMovingToDest = true;
                SetPointToMove();
            }
            else
                Move();
        }

        if (isMovingToDest)
        {
            //если достигли точки назначения, переходим в состояние "никуда не движемся"
            if (Vector3.Distance(transform.position, nextPoint.transform.position) <= 0.5f)
            {
                isMovingToDest = false;
            }
            else
                Move();
        }

#if Debug
        DrawPoints();
#endif
    }

    Vector3 GetQuadBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float invT = 1 - t;
        return (invT * invT * p0) + (2 * invT * t * p1) + (t * t * p2);
    }

    private void PrepareCoords()
    {
        _speedsByChordsLengths = new float[subdivs];

        Vector3 prevPos = _p0;
        for (int i = 0; i < subdivs; i++)
        {
            Vector3 curPos = GetQuadBezierPoint(_p0, _p1, _p2, (i + 1) / (float)subdivs);
            float length = Vector3.Magnitude(curPos - prevPos);

#if Debug
            Debug.DrawLine(curPos, prevPos, Color.yellow, 30f);
#endif

            _speedsByChordsLengths[i] = length;
            _totalLength += length;
            prevPos = curPos;
        }

        for (int i = 0; i < subdivs; i++)
        {
            _speedsByChordsLengths[i] = _speedsByChordsLengths[i] / _totalLength * subdivs;
        }
    }

    float GetSpeedByCoordLength(float t)
    {
        int pos = (int)(t * subdivs) - 1;
        pos = Mathf.Clamp(pos, 0, subdivs - 1);
        return _speedsByChordsLengths[pos];
    }

#if Debug
    void DrawPoint(Vector3 p)
    {
        Debug.DrawRay(p + Vector3.down * 0.5f * 0.1f, Vector3.up * 0.1f);
        Debug.DrawRay(p + -Vector3.forward * 0.5f * 0.1f, Vector3.forward * 0.1f);
    }

    void DrawPoints()
    {
        _testPositions.Add(transform.position);

        foreach (Vector3 t in _testPositions)
        {
            DrawPoint(t);
        }
    }
#endif

    private void Move()
    {
        _t += Time.deltaTime * speed / _totalLength / GetSpeedByCoordLength(_t);
        _t = Mathf.Clamp01(_t);

        Vector3 b = GetQuadBezierPoint(_p0, _p1, _p2, _t);
        transform.LookAt(b);
        transform.position = b;
    }

    private void SetNewDest()
    {
        //удаляем старую точку назначения и создаем новую
        if (nextPoint) Destroy(nextPoint);
        nextPoint = Instantiate(Resources.Load("Sphere")) as GameObject;
        nextPoint.transform.position = GetRandCoords();

        AB = nextPoint.transform.position - transform.position;
        pauseTime = Random.Range(1, 10);
    }

    private void SetPointToMove()
    {
        _p0 = transform.position;

        if (isMovingToPlayer)
        {
            _p1 = player.transform.localPosition - AB / 2;
            _p2 = player.transform.position;
        }
        if (isMovingToDest)
        {
            _p1 = player.transform.localPosition + AB / 2;
            _p2 = nextPoint.transform.position;
        }
        PrepareCoords();
        _t = 0;
    }

    private Vector3 GetRandCoords()
    {
        float newX = Random.Range(20f, 100f) * (Random.Range(-1f, 1f) > 0f ? 1 : -1);
        float newZ = Random.Range(20f, 100f) * (Random.Range(-1f, 1f) > 0f ? 1 : -1);
        return new Vector3(newX, Random.Range(15f, 35f), newZ);
    }
}

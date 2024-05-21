using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX;

public class TestRandMove : MonoBehaviour
{
    [SerializeField]
    int Count = 40;

    [SerializeField, Range(1, 5)]
    int _fractalLevel = 2;

    List<RandomWalk> _list = new List<RandomWalk>(20);
    // Start is called before the first frame update
    void Start()
    {
        _speed = 1f;
        _strenght = 1f;

        Random.InitState(System.DateTime.Now.Millisecond);
        for (int i = 0; i < Count; i++)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = new Vector3(
                Map(Random.value, 0, 1, -1, 1), 
                Map(Random.value, 0, 1, -1, 1), 
                Map(Random.value, 0, 1, -1, 1));

            go.transform.localScale = Vector3.one * 0.1f;
            var move = go.AddComponent<RandomWalk>();
            move.Strength = _strenght;
            move._fractalLevel = _fractalLevel;
            move.Speed = _speed;
            _list.Add(move);
        }
    }

    float Map(float v,float fromMin,float fromMax, float toMin, float toMax)
    {
        return v / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }

    private void Update()
    {
        foreach (var i in _list)
        {
            i.Strength = _strenght;
            i.Speed = _speed;
        }
    }

    float _speed = 0.5f;
    float _strenght = 0.5f;

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, 0, Screen.width / 8, Screen.height));
        GUILayout.BeginVertical();
        if (GUILayout.Button("趋于目标"))
        {
            foreach (var i in _list)
            {
                i.SetTrendPos(transform.position, new RandomWalk.DefaultTrendRater(0.1f, 200f).GetRate);
            }
        }
        if (GUILayout.Button("清除趋于目标"))
        {
            foreach (var i in _list)
            {
                i.ClearTrend();
            }
        }
        GUILayout.Label("速度（0-2）");
        _speed = GUILayout.HorizontalSlider(_speed, 0, 2);
        GUILayout.Label("强度（0-10）");
        _strenght = GUILayout.HorizontalSlider(_strenght, 0, 10);

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}

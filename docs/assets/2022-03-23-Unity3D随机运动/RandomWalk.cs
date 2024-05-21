using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public class RandomWalk : MonoBehaviour
    {
        public const float StepTime = 0.002f;

        public float Strength = 1;
        public float Speed = 1;
        [Range(1, 5)]
        public int _fractalLevel = 1;

        private float _t;
        private float _rx, _ry, _rz;
        private Transform _tran;

        private bool _trend = false;
        private Vector3 _tar;
        private System.Func<Vector3, Vector3, float> _rateFunc;

        public void SetTrendPos(Vector3 tar, System.Func<Vector3, Vector3, float> rateFunc)
        {
            _trend = true;
            _tar = tar;
            _rateFunc = rateFunc;
        }

        public void ClearTrend() { _trend = false; }

        void Start()
        {
            _tran = transform;
            PrepareNoise();
        }

        Vector3 delta = Vector3.zero;
        /// <summary>
        /// 上一个update剩余的不足一次步进模拟的时间
        /// </summary>
        float _retainedTime = 0;

        void Update()
        {
            // 计算要模拟多少步
            float cnt = 1;
            var ft = Time.deltaTime;
            ft += _retainedTime;
            ft *= Speed;
            cnt = ft / StepTime;
            _retainedTime = (cnt - Mathf.Floor(cnt)) * StepTime;
            cnt = Mathf.Clamp(cnt, 0, 100);

            for (int i = 0; i < cnt; i++)
            {
                Step();
            }
        }

        private void Step()
        {
            _t += StepTime;
            var x = Perlin.Fbm(_rx, _t, _fractalLevel);
            var y = Perlin.Fbm(_ry, _t, _fractalLevel);
            var z = Perlin.Fbm(_rz, _t, _fractalLevel);

            delta.x = x;
            delta.y = y;
            delta.z = z;
            delta *= StepTime;

            if (_trend)
            {
                var r = GetTrendRate();
                var v = GetTrendDelta();
                _tran.position += delta * Strength * (1 - r) + v * r;
            }
            else
            {
                _tran.position += delta * Strength;
            }
        }

        private float GetTrendRate()
        {
            return _rateFunc(_tar, _tran.position);
        }

        private Vector3 GetTrendDelta()
        {
            var dis = Vector3.Distance(_tar, _tran.position);
            if (dis < 0.001f)
                return Vector3.zero;
            else
            {
                var d = Speed * StepTime;
                d = Mathf.Min(d, dis);
                return (_tar - _tran.position).normalized * d;
            }
        }

        private void PrepareNoise()
        {
            // perlin noise有效输入范围是[0-255]
            _rx = Random.value * 255;
            _ry = Random.value * 255;
            _rz = Random.value * 255;
            _t = Random.value * 255;
        }

        public class DefaultTrendRater
        {
            float _mid, _high;
            public DefaultTrendRater(float mid, float high)
            {
                _mid = mid;
                _high = high;
            }

            public float GetRate(Vector3 a, Vector3 b)
            {
                var dis = Vector3.Distance(a, b);
                if (dis > _high)
                    return 0;
                else if (dis > _mid)
                    return 0.5f;
                else
                    return 1;
            }

        }
    }
}
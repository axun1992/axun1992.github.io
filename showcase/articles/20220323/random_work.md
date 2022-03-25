[toc]

# Unity3D随机运动
## 简介
这里介绍在Unity3D的三维空间中作随机运动的计算方法。
这是对“随机游走”术语概念在三维移动上的具体应用。
## 效果
[![Watch the video](./random_move.png)](./random_move.mp4)
以上视频演示了随机运动和趋于某个点运动。
## 主要思路
本文的实现主要基于柏林噪声（Perlin Noise）。柏林噪声本质上是一个随机数生成器，只不过它和完全随机不同，在输入连续的时候，输出也是较为连续的。
这就意味着，当我们用它来决定运动方向时，不会出现上一帧向前而下一帧马上向后的情况，总会有一段时间的过渡。
关于柏林噪声算法，直接使用了github上的开源代码：
`https://github.com/keijiro/PerlinNoise`
## 需要哪些控制参数
开始之前，先想一想我们需要哪些可控的参数？
首先应该需要随机运动的强度，也就是随机运动时的大致范围。
其次，需要控制运动快慢。也就是在保持同样运行轨迹前提下，用多少帧完成轨迹。

实际也是如此，我提供了参数：
```
public float Strength = 1;
public float Speed = 1;
```
另外，不同频率柏林噪声经过多层叠加后效果会更好，因此又提供了一个参数控制叠加次数（或称fbm，分形布朗运动）：
```
[Range(1, 5)]
public int _fractalLevel = 1;
```
## 柏林噪声用法
柏林噪声分为一维、二维、三维，也就是输入参数的数量，注意相同的输入参数会得到相同的输出结果。因此给输入参数增加一些动态，输出就随机了，通常输入结合时间变化。
下面这个方法有第四个参数，就是多个频率噪声的叠加次数。一般5次以内。
```
public static float Fbm(float x, float y, float z, int octave)
```
出于性能考虑，我们只使用二维版本：
```
public static float Fbm(float x, float y, int octave)
```
在三维空间中每一次模拟调用三次该方法。在x、y、z方向各初始化一个随机值一直使用，传入其x参数（保证在三个轴向上的随机值是不一样的）。时间值传入其y参数。
三次调用得到的三个值分别作用在x、y、z坐标上，大概就是这样的思路。

## 速度控制
速度控制实质是这样一种要求：同样的任务用不同的时间完成。
在这里就是指我们需要保证在不同速度时总的移动量是相等的。
因此先定义模拟一个基本时间片的方法。基本时间片一定要比较小，这样在一个Update中才可以执行多次，才可以乘以Speed缩放后减少执行次数。这里把时间片设置为0.002秒。
一个Update中应该步进模拟的次数就是：
```
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
```
这个和unity引擎的Update与FixedUpdate之间的关系很像。

## 强度控制
强度控制很简单，把从柏林噪声中取得的值乘以强度参数就可以了。

完整代码：
[TestRandMove.cs](./TestRandMove.cs)
[RandomWalk.cs](./RandomWalk.cs)
[Perlin.cs](./Perlin.cs)
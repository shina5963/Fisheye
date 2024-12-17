using UnityEngine;

public class SatelliteOrbit : MonoBehaviour
{
    // 軌道傾斜角(度)
    [SerializeField] private float inclinationAngle = 51.6f;

    // 軌道周期(秒) = 90分 = 5400秒
    [SerializeField] private float orbitPeriod = 5400f/60/60;

    // 1周360度をorbitPeriod秒で回るための1秒あたり回転度数
    private float degreesPerSecond;

    public GameObject Satellite;
    void Start()
    {
        // 軌道面の傾斜設定
        // 初期状態は本初子午線上に衛星が位置し、赤道面上とするとY軸周りの回転が基本軌道面。
        // 傾斜をZ軸回りで付与すると、X-Z平面が傾くイメージとなる。

        // 現在、衛星は(-6771,0,0)におり、これは本初子午線上。
        // 傾斜をZ軸で与える（X軸が赤道、Z軸で傾斜を付ける）
        Satellite.transform.localRotation = Quaternion.Euler(0f, 0f, inclinationAngle);

        // 回転速度計算
        // 360度 / orbitPeriod秒 = deg/sec
        degreesPerSecond = 360f / orbitPeriod;
    }

    void Update()
    {
        // 親オブジェクトをY軸回りで回転させる（Z軸で傾斜を加えた上でY回転することで傾斜軌道実現）
        Satellite.transform.Rotate(0f, degreesPerSecond * Time.deltaTime, 0f, Space.Self);
    }
}

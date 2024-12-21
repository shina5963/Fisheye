using UnityEngine;
using UnityEngine.UI; // UIを使用する場合必要

public class EarthController : MonoBehaviour
{
    public GameObject Earth;

    // デフォルト24時間: 86400秒(1日) / 3600 = 24。 
    // 現在は1秒=1時間換算するため24でOK。
    [SerializeField]
    public float rotationPeriod =1440;

    // 現在のシミュレーション内時刻(時間単位)
    private float simulatedTimeHours = 0f;

    // 現在時刻を表示するUIテキスト
    public Text timeText;

    private void Start()
    {
        // simulatedTimeHoursを0から開始
        simulatedTimeHours = 0f;
    }
    public string timeString;
    private void Update()
    {
        // 地球回転処理
        float degreesPerSecond = 360f / rotationPeriod;
        Earth.transform.Rotate(Vector3.up, -1*degreesPerSecond * Time.deltaTime, Space.Self);

        // シミュレーション内時刻の進行
        // ここでは1秒ごとに1時間が経過すると仮定
        simulatedTimeHours += Time.deltaTime*(24f / rotationPeriod);

        // 24時間を超えたら0に戻す(1日サイクル)
        if (simulatedTimeHours >= 24f)
        {
            simulatedTimeHours -= 24f;
        }

        // 現在時刻のフォーマット
        int hours = Mathf.FloorToInt(simulatedTimeHours);
        float fractionalHour = simulatedTimeHours - hours;
        int minutes = Mathf.FloorToInt(fractionalHour * 60);
        float fractionalMinute = (fractionalHour * 60) - minutes;
        int seconds = Mathf.FloorToInt(fractionalMinute * 60);

        // "HH:MM:SS"形式でテキスト表示
        //string timeString = string.Format("Time {0:D2}:{1:D2}", hours, minutes);
        timeString = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
        if (timeText != null)
        {
            timeText.text = timeString;
        }
    }
}

using UnityEngine;
using UnityEngine.UI; // UI用

public class SatelliteController : MonoBehaviour
{

    public EarthController earthController;
    // 軌道傾斜角(度)
    [SerializeField] private float inclinationAngle = 51.6f;

    // 軌道周期(秒) = 90分 = 5400秒
    [SerializeField] private float orbitPeriod = 5400f / 60 / 60;

    // 1周360度をorbitPeriod秒で回るための1秒あたり回転度数
    private float degreesPerSecond;

    public Vector2 rollSpeedRange = new Vector2(-0.5f, 0.5f);
    public Vector2 pitchSpeedRange = new Vector2(-0.5f, 0.5f);
    public Vector2 yawSpeedRange = new Vector2(-0.5f, 0.5f);

    public float rollSpeed=1;
    public float pitchSpeed=1;
    public float yawSpeed=1;

    public Vector3 satellitePower=new Vector3(1,1,1);

    // 更新間隔
    [SerializeField] private float randomizeInterval = 2.0f;
    private float timeSinceLastUpdate = 0f;

    public GameObject Satellite;
    public GameObject SatelliteParent;

    // UI Textオブジェクトの参照
    [Header("UI Text References")]
    [SerializeField] private Text rollText;
    [SerializeField] private Text pitchText;
    [SerializeField] private Text yawText;

    // UI Textオブジェクトの参照
    [Header("UI Text References")]
    [SerializeField] private Text rollTorqueText;
    [SerializeField] private Text pitchTorqueText;
    [SerializeField] private Text yawTorqueText;

    void Start()
    {

        // 軌道面の傾斜設定
       // SatelliteParent.transform.localRotation = Quaternion.Euler(0f, 0f, inclinationAngle);

        //SatelliteParent.transform.localRotation = Quaternion.Euler(inclinationAngle,-90f ,0f );


        // 初期ランダム回転速度設定
        //UpdateRandomSpeeds();
    }

    void Update()
    {


        orbitPeriod = earthController.rotationPeriod / 16f;//earthController.rotationPeriod=24時間の場合、90分

        // 回転速度計算
        degreesPerSecond = 360f / orbitPeriod;

        // 軌道回転処理
        //SatelliteParent.transform.Rotate(0f, -1 * degreesPerSecond * Time.deltaTime, 0f, Space.Self);
        //SatelliteParent.transform.Rotate(0f, 0, -1 * degreesPerSecond * Time.deltaTime, Space.Self);
        Vector3 localEulerAngles = SatelliteParent.transform.localRotation.eulerAngles;

        // 現在のローカル回転角を取得し、z軸を減少させる
        localEulerAngles.z += degreesPerSecond * Time.deltaTime;

        // 新しい回転を適用する
        SatelliteParent.transform.localRotation = Quaternion.Euler(localEulerAngles);
        // ランダム姿勢回転
        //Satellite.transform.Rotate(rollSpeed * Time.deltaTime, pitchSpeed * Time.deltaTime, yawSpeed * Time.deltaTime, Space.Self);
        Satellite.transform.Rotate(satellitePower.x* Time.deltaTime, satellitePower.y* Time.deltaTime, satellitePower.z* Time.deltaTime, Space.Self);

        // ランダム速度更新
      //  timeSinceLastUpdate += Time.deltaTime;
        //if (timeSinceLastUpdate >= randomizeInterval)
       // {
            //UpdateRandomSpeeds();
       //     timeSinceLastUpdate = 0f;
       // }

        // テキスト更新
       // UpdateUIText();
    }

    // ランダムな速度を生成する関数
    private void UpdateRandomSpeeds()
    {
        rollSpeed = Random.Range(rollSpeedRange.x, rollSpeedRange.y);
        pitchSpeed = Random.Range(pitchSpeedRange.x, pitchSpeedRange.y);
        yawSpeed = Random.Range(yawSpeedRange.x, yawSpeedRange.y);
    }

    // テキストオブジェクトに表示を更新する関数
    private void UpdateUIText()
    {
        if (rollText != null) rollTorqueText.text = $"Roll: {rollSpeed:F2}";
        if (pitchText != null) pitchTorqueText.text = $"Pitch: {pitchSpeed:F2}";
        if (yawText != null) yawTorqueText.text = $"Yaw: {yawSpeed:F2}";

        Vector3 eulerRotation = Satellite.transform.localEulerAngles;

        if (rollText != null) rollText.text = $"Roll: {eulerRotation.z:F2}°";
        if (pitchText != null) pitchText.text = $"Pitch: {eulerRotation.x:F2}°";
        if (yawText != null) yawText.text = $"Yaw: {eulerRotation.y:F2}°";
    }
}

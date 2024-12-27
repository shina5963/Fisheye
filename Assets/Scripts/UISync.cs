using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UISync : MonoBehaviour
{
    public UIDocument UIRight;
    public UIDocument UILeft;
    public SatelliteController satelliteController;
    public EarthController earthController;
    public FrustumToEarthProjection frustumToEarthProjection;
    public  SatellitePositionTracker satellitePositionTracker;
    public AngleCalculator angleCalculator;
     
        public GameObject targetObject; // 回転を同期する対象のオブジェクト
    public GameObject SatelliteHeight;
    public GameObject SatelliteParent;
    private TextField timeStringField;

    public Camera FisheyeCamera;
    public Volume volume;
    private FloatField DayTime;
    private FloatField Sight;
    private Slider FOV;
    private Slider FisheyeRatio;
    private Slider Declination;

    private Vector3Field SatelliteRotation;
    private Vector3Field SatellitePower;
    private Vector3Field SatellitePos;

    private LensDistortion lensDistortion;

    private Button executeProjectionButton;
    private Button SaveButton;

    private Toggle timeControlToggle;

    private void OnEnable()
    {
        // UIDocumentのルート要素を取得
        var root = UIRight.GetComponent<UIDocument>().rootVisualElement;
        var rootL = UILeft.GetComponent<UIDocument>().rootVisualElement;


        // トグルを取得（例: トグルのラベルが "Time Control" の場合）
        timeControlToggle = rootL.Query<Toggle>().Where(v => v.label == "時間停止（Space）").First();

        DayTime = rootL.Query<FloatField>().Where(v => v.label == "1 日の長さ[秒]").First();
        Sight= root.Query<FloatField>().Where(v => v.label == "視線角[°]").First();

        SatelliteRotation = root.Query<Vector3Field>().Where(v => v.label == "衛星回転（ヨー[°], ピッチ[°], ロール[°]）").First();
        SatellitePower = root.Query<Vector3Field>().Where(v => v.label == "衛星トルク（ヨー, ピッチ, ロール）").First();
        SatellitePos = root.Query<Vector3Field>().Where(v => v.label == "衛星位置（緯度[°], 経度[°], 高度[km]）").First();
      
        // print(SatellitePos.inputUssClassName);
        /* SatellitePos.xLabel = "緯度 (°)";
         SatellitePos.yLabel = "経度 (°)";
         SatellitePos.zLabel = "高度 (m)";*/



        FOV = rootL.Query<Slider>().Where(v => v.label == "視野角[°]").First();
        FisheyeRatio = rootL.Query<Slider>().Where(v => v.label == "魚眼率").First();
        Declination= root.Query<Slider>().Where(v => v.label == "近点経度[°]").First();
        // TextFieldを取得
        timeStringField = rootL.Query<TextField>().Where(v => v.label == "時刻").First();

        // 初期値を設定
        if (targetObject != null)
        {
            SatelliteRotation.value = RoundVector3(targetObject.transform.localRotation.eulerAngles, 2);
        }

        SatellitePower.value = RoundVector3(satelliteController.satellitePower, 2);

        Vector2 LatLon = satellitePositionTracker.latLon;
    SatellitePos.value = RoundVector3(new Vector3(LatLon.x, LatLon.y, -Mathf.Round(SatelliteHeight.transform.localPosition.x + 6371)), 2);

        DayTime.value = Mathf.Round(earthController.rotationPeriod);
        Sight.value = angleCalculator.angle;//Mathf.Round(angleCalculator.angle);
        FOV.value = Mathf.Round(FisheyeCamera.fieldOfView);


        // トグルの値変更イベントを登録
        timeControlToggle.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue)
            {
                Time.timeScale = 0;
            }
            else
            {
                // トグルがオフになった場合
                // トグルがオンになった場合
                Time.timeScale = 1;
              
            }

            // トグルのテキストを更新
            UpdateToggleText(evt.newValue);
        });

        // Lens Distortionエフェクトを取得
        //var  = FindObjectOfType<Volume>();
        if (volume && volume.profile.TryGet(out lensDistortion))
        {
            FisheyeRatio.value = Round(lensDistortion.intensity.value, 2) ; // 初期値を設定
        }
        else
        {
            Debug.LogError("Lens Distortion is not set in the Volume profile.");
        }

     
        Declination.value = Round(SatelliteParent.transform.localRotation.eulerAngles.z, 2); // 初期値を設定

        // Vector3Fieldの値変更イベントを登録
        SatelliteRotation.RegisterValueChangedCallback(evt =>
        {
            if (targetObject != null)
            {
                // 値を丸めてTransformに反映
                var roundedValue = RoundVector3(evt.newValue, 2);
                targetObject.transform.localRotation = Quaternion.Euler(roundedValue);

                // 丸めた値をフィールドに再代入（フィールドを更新）
                SatelliteRotation.SetValueWithoutNotify(roundedValue);
            }
        });

        SatellitePower.RegisterValueChangedCallback(evt =>
        {
            // 値を丸めてTransformに反映
            var roundedValue = RoundVector3(evt.newValue, 2);
            satelliteController.satellitePower = roundedValue;

            // 丸めた値をフィールドに再代入（フィールドを更新）
            SatellitePower.SetValueWithoutNotify(roundedValue);
        });

        SatellitePos.RegisterValueChangedCallback(evt =>
        {
            // 値を丸めてTransformに反映
            var roundedValue = RoundVector3(evt.newValue, 2);
            if (roundedValue.z < 1)
                roundedValue.z = 1;
            SatelliteHeight.transform.localPosition = -new Vector3(roundedValue.z + 6371, 0, 0);

            //satelliteController.satellitePower = roundedValue.z;

            // 丸めた値をフィールドに再代入（フィールドを更新）
            SatellitePos.SetValueWithoutNotify(roundedValue);
        });

        DayTime.RegisterValueChangedCallback(evt =>
        {
            // 値を丸めてTransformに反映
            var roundedValue = Mathf.Round(evt.newValue);
            if (roundedValue < 1)
                roundedValue = 1;
            earthController.rotationPeriod = roundedValue;

            // 丸めた値をフィールドに再代入（フィールドを更新）
            DayTime.SetValueWithoutNotify(roundedValue);
        });

      

        FOV.RegisterValueChangedCallback(evt =>
        {
            // 値を丸めてTransformに反映
            var roundedValue = Mathf.Round(evt.newValue);

            FisheyeCamera.fieldOfView = roundedValue;

            // 丸めた値をフィールドに再代入（フィールドを更新）
            FOV.SetValueWithoutNotify(roundedValue);
        });

        FisheyeRatio.RegisterValueChangedCallback(evt =>
        {
            if (lensDistortion != null)
            {
                // 値を丸めて反映
                var roundedValue = Round(evt.newValue,2);
                lensDistortion.intensity.value = roundedValue;

                // 丸めた値をフィールドに再代入（フィールドを更新）
                FisheyeRatio.SetValueWithoutNotify(roundedValue);
            }
        });

        Declination.RegisterValueChangedCallback(evt =>
        {
            if (SatelliteParent != null)
            {
                // 値を丸めて反映
                var roundedValue = Round(evt.newValue, 2);

                // 現在のローカル回転角を取得
                Vector3 localEulerAngles = SatelliteParent.transform.localRotation.eulerAngles;

                // 入力された値を-180°~180°に正規化
                localEulerAngles.z = NormalizeAngle(roundedValue);

                // 新しい回転を適用
                SatelliteParent.transform.localRotation = Quaternion.Euler(localEulerAngles);

                // 丸めた値をフィールドに再代入（通知を送らないで更新）
                Declination.SetValueWithoutNotify(roundedValue);
            }
        });

        // ボタンを取得
        executeProjectionButton = rootL.Query<Button>().Where(v => v.text == "ダウンレンジ表示").First();
        SaveButton = rootL.Query<Button>().Where(v => v.text == "撮影（S）").First();

        // ボタンのクリックイベントを登録
        executeProjectionButton.RegisterCallback<ClickEvent>(evt =>
        {
            // FrustumToEarthProjection.Projection() を実行
            frustumToEarthProjection.Projection();
           // Debug.Log("Projection executed.");
        });

        // ボタンのクリックイベントを登録
        SaveButton.RegisterCallback<ClickEvent>(evt =>
        {
            // FrustumToEarthProjection.Projection() を実行
            webGLRenderTextureSaver.SaveRenderTextureAsCroppedPNG();
            // Debug.Log("Projection executed.");
        });

    }
    public WebGLRenderTextureSaver webGLRenderTextureSaver;
        

    // トグルのテキストを更新するヘルパーメソッド
    private void UpdateToggleText(bool isOn)
    {
        if (isOn)
        {
            timeControlToggle.text = "停止中";
        }
        else
        {
            timeControlToggle.text = "";
        }
    }
    private void Update()
    {

        // Spaceキーでトグルのオンオフを切り替える
        if (Input.GetKeyDown(KeyCode.Space) && timeControlToggle != null)
        {
            // 現在のトグルの値を反転
            bool newValue = !timeControlToggle.value;
            timeControlToggle.value = newValue;

            // トグルのテキストを更新
            UpdateToggleText(newValue);

            // 時間制御の処理を実行
            if (newValue)
            {
                Time.timeScale = 0; // 停止
            }
            else
            {
                Time.timeScale = 1; // 再開
            }
        }
        // オブジェクトのTransformが変更された場合、Vector3Fieldを更新
        if (targetObject != null)
        {
            var roundedValue = RoundVector3(targetObject.transform.localRotation.eulerAngles, 2);
            if (SatelliteRotation.value != roundedValue)
            {
                SatelliteRotation.value = roundedValue;
            }
        }
        // オブジェクトのTransformが変更された場合、Vector3Fieldを更新
        if (SatelliteParent != null)
        {
            // 現在のローカル回転角を取得し、-180°~180°に変換
            float rawAngle = SatelliteParent.transform.localRotation.eulerAngles.z;
            float normalizedAngle = NormalizeAngle(rawAngle);

            // 小数点以下2桁に丸める
            var roundedValue = Round(normalizedAngle, 2);

            if (Declination.value != roundedValue)
            {
                Declination.value = roundedValue;
            }
        }
        if(angleCalculator!=null)
            Sight.value = angleCalculator.angle;

        if (satellitePositionTracker != null) {
            Vector2 LatLon = satellitePositionTracker.latLon;
            SatellitePos.value = RoundVector3(new Vector3(LatLon.x, LatLon.y, -Mathf.Round(SatelliteHeight.transform.localPosition.x + 6371)), 2);
        }

        if (earthController != null && timeStringField != null)
        {
            // EarthControllerのtimeStringをTextFieldに反映
            timeStringField.SetValueWithoutNotify(earthController.timeString);
        }
    }
    // 角度を-180°~180°に正規化するメソッド
    float NormalizeAngle(float angle)
    {
        // 360°ごとにループ
        angle = angle % 360;
        // -180°~180°の範囲に変換
        if (angle > 180) angle -= 360;
        return angle;
    }

   
    // Vector3 の各成分を指定した小数点以下の桁数で丸める
    private Vector3 RoundVector3(Vector3 value, int decimalPlaces)
    {
        float multiplier = Mathf.Pow(10, decimalPlaces);
        return new Vector3(
            Mathf.Round(value.x * multiplier) / multiplier,
            Mathf.Round(value.y * multiplier) / multiplier,
            Mathf.Round(value.z * multiplier) / multiplier
        );
    }
    private float Round(float value, int decimalPlaces)
    {
        float multiplier = Mathf.Pow(10, decimalPlaces);
        return Mathf.Round(value * multiplier) / multiplier;
    }
}

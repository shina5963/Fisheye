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
    public GameObject targetObject; // 回転を同期する対象のオブジェクト
    public GameObject SatelliteHeight;
    private TextField timeStringField;

    public Camera FisheyeCamera;
    public Volume volume;
    private FloatField DayTime;
    private FloatField Height;
    private Slider FOV;
    private Slider FisheyeRatio;

    private Vector3Field SatelliteRotation;
    private Vector3Field SatellitePower;

    private LensDistortion lensDistortion;

    private Button executeProjectionButton;
  
    private void OnEnable()
    {
        // UIDocumentのルート要素を取得
        var root = UIRight.GetComponent<UIDocument>().rootVisualElement;
        var rootL = UILeft.GetComponent<UIDocument>().rootVisualElement;

        DayTime = root.Query<FloatField>().Where(v => v.label == "1 日の長さ（秒）").First();
        Height= root.Query<FloatField>().Where(v => v.label == "衛星高さ（km）").First();

        SatelliteRotation = root.Query<Vector3Field>().Where(v => v.label == "Satellite Rotation").First();
        SatellitePower = root.Query<Vector3Field>().Where(v => v.label == "Satellite Power").First();
        FOV = root.Query<Slider>().Where(v => v.label == "視野角（度）").First();
        FisheyeRatio = root.Query<Slider>().Where(v => v.label == "魚眼率").First();
        // TextFieldを取得
        timeStringField = rootL.Query<TextField>().Where(v => v.label == "時刻").First();

        // 初期値を設定
        if (targetObject != null)
        {
            SatelliteRotation.value = RoundVector3(targetObject.transform.localRotation.eulerAngles, 2);
        }

        SatellitePower.value = RoundVector3(satelliteController.satellitePower, 2);

        DayTime.value = Mathf.Round(earthController.rotationPeriod);
        Height.value = -Mathf.Round(SatelliteHeight.transform.localPosition.x+6371);
        FOV.value = Mathf.Round(FisheyeCamera.fieldOfView);

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

        Height.RegisterValueChangedCallback(evt =>
        {
            // 値を丸めてTransformに反映
            var roundedValue = Mathf.Round(evt.newValue);
            if (roundedValue < 1)
                roundedValue = 1;
            SatelliteHeight.transform.localPosition = -new Vector3( roundedValue+6371,0,0);

            // 丸めた値をフィールドに再代入（フィールドを更新）
            Height.SetValueWithoutNotify(roundedValue);
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

        // ボタンを取得
        executeProjectionButton = root.Query<Button>().Where(v => v.text == "ダウンレンジ表示").First();

        // ボタンのクリックイベントを登録
        executeProjectionButton.RegisterCallback<ClickEvent>(evt =>
        {
            // FrustumToEarthProjection.Projection() を実行
            frustumToEarthProjection.Projection();
           // Debug.Log("Projection executed.");
        });

    }

    private void Update()
    {
        // オブジェクトのTransformが変更された場合、Vector3Fieldを更新
        if (targetObject != null)
        {
            var roundedValue = RoundVector3(targetObject.transform.localRotation.eulerAngles, 2);
            if (SatelliteRotation.value != roundedValue)
            {
                SatelliteRotation.value = roundedValue;
            }
        }
        if (earthController != null && timeStringField != null)
        {
            // EarthControllerのtimeStringをTextFieldに反映
            timeStringField.SetValueWithoutNotify(earthController.timeString);
        }
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

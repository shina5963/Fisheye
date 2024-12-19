using UnityEngine;
using UnityEngine.UIElements;

public class TransformRotationSync : MonoBehaviour
{
    public SatelliteController satelliteController;
    public GameObject targetObject; // 回転を同期する対象のオブジェクト
   
    private Vector3Field SatelliteRotation;
    private Vector3Field SatellitePower;


    private void OnEnable()
    {
        // UIDocumentのルート要素を取得
        var root = GetComponent<UIDocument>().rootVisualElement;

        SatelliteRotation = root.Query<Vector3Field>().Where(v => v.label == "Satellite Rotation").First();
        SatellitePower= root.Query<Vector3Field>().Where(v => v.label == "Satellite Power").First();


        // 初期値を設定
        if (targetObject != null)
        {
            SatelliteRotation.value = RoundVector3(targetObject.transform.rotation.eulerAngles, 2);
        }

       
            SatellitePower.value = RoundVector3(satelliteController.satellitePower  , 2);
        


        // Vector3Fieldの値変更イベントを登録
        SatelliteRotation.RegisterValueChangedCallback(evt =>
        {
            if (targetObject != null)
            {
                // 値を丸めてTransformに反映
                var roundedValue = RoundVector3(evt.newValue, 2);
                targetObject.transform.rotation = Quaternion.Euler(roundedValue);

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
    }

    private void Update()
    {
        // オブジェクトのTransformが変更された場合、Vector3Fieldを更新
        if (targetObject != null)
        {
            var roundedValue = RoundVector3(targetObject.transform.rotation.eulerAngles, 2);
            if (SatelliteRotation.value != roundedValue)
            {
                SatelliteRotation.value = roundedValue;
            }
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
}

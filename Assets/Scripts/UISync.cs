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
    public GameObject targetObject; // ��]�𓯊�����Ώۂ̃I�u�W�F�N�g
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
        // UIDocument�̃��[�g�v�f���擾
        var root = UIRight.GetComponent<UIDocument>().rootVisualElement;
        var rootL = UILeft.GetComponent<UIDocument>().rootVisualElement;

        DayTime = root.Query<FloatField>().Where(v => v.label == "1 ���̒����i�b�j").First();
        Height= root.Query<FloatField>().Where(v => v.label == "�q�������ikm�j").First();

        SatelliteRotation = root.Query<Vector3Field>().Where(v => v.label == "Satellite Rotation").First();
        SatellitePower = root.Query<Vector3Field>().Where(v => v.label == "Satellite Power").First();
        FOV = root.Query<Slider>().Where(v => v.label == "����p�i�x�j").First();
        FisheyeRatio = root.Query<Slider>().Where(v => v.label == "���ᗦ").First();
        // TextField���擾
        timeStringField = rootL.Query<TextField>().Where(v => v.label == "����").First();

        // �����l��ݒ�
        if (targetObject != null)
        {
            SatelliteRotation.value = RoundVector3(targetObject.transform.localRotation.eulerAngles, 2);
        }

        SatellitePower.value = RoundVector3(satelliteController.satellitePower, 2);

        DayTime.value = Mathf.Round(earthController.rotationPeriod);
        Height.value = -Mathf.Round(SatelliteHeight.transform.localPosition.x+6371);
        FOV.value = Mathf.Round(FisheyeCamera.fieldOfView);

        // Lens Distortion�G�t�F�N�g���擾
        //var  = FindObjectOfType<Volume>();
        if (volume && volume.profile.TryGet(out lensDistortion))
        {
            FisheyeRatio.value = Round(lensDistortion.intensity.value, 2) ; // �����l��ݒ�
        }
        else
        {
            Debug.LogError("Lens Distortion is not set in the Volume profile.");
        }

        // Vector3Field�̒l�ύX�C�x���g��o�^
        SatelliteRotation.RegisterValueChangedCallback(evt =>
        {
            if (targetObject != null)
            {
                // �l���ۂ߂�Transform�ɔ��f
                var roundedValue = RoundVector3(evt.newValue, 2);
                targetObject.transform.localRotation = Quaternion.Euler(roundedValue);

                // �ۂ߂��l���t�B�[���h�ɍđ���i�t�B�[���h���X�V�j
                SatelliteRotation.SetValueWithoutNotify(roundedValue);
            }
        });

        SatellitePower.RegisterValueChangedCallback(evt =>
        {
            // �l���ۂ߂�Transform�ɔ��f
            var roundedValue = RoundVector3(evt.newValue, 2);
            satelliteController.satellitePower = roundedValue;

            // �ۂ߂��l���t�B�[���h�ɍđ���i�t�B�[���h���X�V�j
            SatellitePower.SetValueWithoutNotify(roundedValue);
        });

        DayTime.RegisterValueChangedCallback(evt =>
        {
            // �l���ۂ߂�Transform�ɔ��f
            var roundedValue = Mathf.Round(evt.newValue);
            if (roundedValue < 1)
                roundedValue = 1;
            earthController.rotationPeriod = roundedValue;

            // �ۂ߂��l���t�B�[���h�ɍđ���i�t�B�[���h���X�V�j
            DayTime.SetValueWithoutNotify(roundedValue);
        });

        Height.RegisterValueChangedCallback(evt =>
        {
            // �l���ۂ߂�Transform�ɔ��f
            var roundedValue = Mathf.Round(evt.newValue);
            if (roundedValue < 1)
                roundedValue = 1;
            SatelliteHeight.transform.localPosition = -new Vector3( roundedValue+6371,0,0);

            // �ۂ߂��l���t�B�[���h�ɍđ���i�t�B�[���h���X�V�j
            Height.SetValueWithoutNotify(roundedValue);
        });

        FOV.RegisterValueChangedCallback(evt =>
        {
            // �l���ۂ߂�Transform�ɔ��f
            var roundedValue = Mathf.Round(evt.newValue);

            FisheyeCamera.fieldOfView = roundedValue;

            // �ۂ߂��l���t�B�[���h�ɍđ���i�t�B�[���h���X�V�j
            FOV.SetValueWithoutNotify(roundedValue);
        });

        FisheyeRatio.RegisterValueChangedCallback(evt =>
        {
            if (lensDistortion != null)
            {
                // �l���ۂ߂Ĕ��f
                var roundedValue = Round(evt.newValue,2);
                lensDistortion.intensity.value = roundedValue;

                // �ۂ߂��l���t�B�[���h�ɍđ���i�t�B�[���h���X�V�j
                FisheyeRatio.SetValueWithoutNotify(roundedValue);
            }
        });

        // �{�^�����擾
        executeProjectionButton = root.Query<Button>().Where(v => v.text == "�_�E�������W�\��").First();

        // �{�^���̃N���b�N�C�x���g��o�^
        executeProjectionButton.RegisterCallback<ClickEvent>(evt =>
        {
            // FrustumToEarthProjection.Projection() �����s
            frustumToEarthProjection.Projection();
           // Debug.Log("Projection executed.");
        });

    }

    private void Update()
    {
        // �I�u�W�F�N�g��Transform���ύX���ꂽ�ꍇ�AVector3Field���X�V
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
            // EarthController��timeString��TextField�ɔ��f
            timeStringField.SetValueWithoutNotify(earthController.timeString);
        }
    }

    // Vector3 �̊e�������w�肵�������_�ȉ��̌����Ŋۂ߂�
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

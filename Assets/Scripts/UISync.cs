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
    private Vector3Field SatellitePos;

    private LensDistortion lensDistortion;

    private Button executeProjectionButton;
    private Button SaveButton;

    private Toggle timeControlToggle;

    private void OnEnable()
    {
        // UIDocument�̃��[�g�v�f���擾
        var root = UIRight.GetComponent<UIDocument>().rootVisualElement;
        var rootL = UILeft.GetComponent<UIDocument>().rootVisualElement;


        // �g�O�����擾�i��: �g�O���̃��x���� "Time Control" �̏ꍇ�j
        timeControlToggle = root.Query<Toggle>().Where(v => v.label == "���Ԓ�~�iSpace�j").First();

        DayTime = root.Query<FloatField>().Where(v => v.label == "1 ���̒����i�b�j").First();
        Height= root.Query<FloatField>().Where(v => v.label == "�q�������ikm�j").First();

        SatelliteRotation = root.Query<Vector3Field>().Where(v => v.label == "�q����]�i���[[��], �s�b�`[��], ���[��[��]�j").First();
        SatellitePower = root.Query<Vector3Field>().Where(v => v.label == "�q���g���N�i���[, �s�b�`, ���[���j").First();
        SatellitePos = root.Query<Vector3Field>().Where(v => v.label == "�q���ʒu�i�ܓx[��], �o�x[��], ���x[km]�j").First();
      
        // print(SatellitePos.inputUssClassName);
        /* SatellitePos.xLabel = "�ܓx (��)";
         SatellitePos.yLabel = "�o�x (��)";
         SatellitePos.zLabel = "���x (m)";*/



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
        SatellitePos.value = RoundVector3(new Vector3(0,0, -Mathf.Round(SatelliteHeight.transform.localPosition.x + 6371)), 2);

        DayTime.value = Mathf.Round(earthController.rotationPeriod);
        Height.value = -Mathf.Round(SatelliteHeight.transform.localPosition.x+6371);
        FOV.value = Mathf.Round(FisheyeCamera.fieldOfView);


        // �g�O���̒l�ύX�C�x���g��o�^
        timeControlToggle.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue)
            {
                Time.timeScale = 0;
            }
            else
            {
                // �g�O�����I�t�ɂȂ����ꍇ
                // �g�O�����I���ɂȂ����ꍇ
                Time.timeScale = 1;
              
            }

            // �g�O���̃e�L�X�g���X�V
            UpdateToggleText(evt.newValue);
        });

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

        SatellitePos.RegisterValueChangedCallback(evt =>
        {
            // �l���ۂ߂�Transform�ɔ��f
            var roundedValue = RoundVector3(evt.newValue, 2);
            if (roundedValue.z < 1)
                roundedValue.z = 1;
            SatelliteHeight.transform.localPosition = -new Vector3(roundedValue.z + 6371, 0, 0);

            //satelliteController.satellitePower = roundedValue.z;

            // �ۂ߂��l���t�B�[���h�ɍđ���i�t�B�[���h���X�V�j
            SatellitePos.SetValueWithoutNotify(roundedValue);
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
        SaveButton = root.Query<Button>().Where(v => v.text == "�B�e�iS�j").First();

        // �{�^���̃N���b�N�C�x���g��o�^
        executeProjectionButton.RegisterCallback<ClickEvent>(evt =>
        {
            // FrustumToEarthProjection.Projection() �����s
            frustumToEarthProjection.Projection();
           // Debug.Log("Projection executed.");
        });

        // �{�^���̃N���b�N�C�x���g��o�^
        SaveButton.RegisterCallback<ClickEvent>(evt =>
        {
            // FrustumToEarthProjection.Projection() �����s
            webGLRenderTextureSaver.SaveRenderTextureAsCroppedPNG();
            // Debug.Log("Projection executed.");
        });

    }
    public WebGLRenderTextureSaver webGLRenderTextureSaver;
        

    // �g�O���̃e�L�X�g���X�V����w���p�[���\�b�h
    private void UpdateToggleText(bool isOn)
    {
        if (isOn)
        {
            timeControlToggle.text = "��~��";
        }
        else
        {
            timeControlToggle.text = "";
        }
    }
    private void Update()
    {

        // Space�L�[�Ńg�O���̃I���I�t��؂�ւ���
        if (Input.GetKeyDown(KeyCode.Space) && timeControlToggle != null)
        {
            // ���݂̃g�O���̒l�𔽓]
            bool newValue = !timeControlToggle.value;
            timeControlToggle.value = newValue;

            // �g�O���̃e�L�X�g���X�V
            UpdateToggleText(newValue);

            // ���Ԑ���̏��������s
            if (newValue)
            {
                Time.timeScale = 0; // ��~
            }
            else
            {
                Time.timeScale = 1; // �ĊJ
            }
        }
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

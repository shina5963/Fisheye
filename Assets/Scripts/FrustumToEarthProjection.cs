using UnityEngine;
using System.Collections.Generic; // List���g�p���邽�߂ɕK�v
using System.Collections; // �R���[�`�����g�p���邽�߂ɕK�v
public class FrustumToEarthProjection : MonoBehaviour
{
    public Camera camera; // �ΏۂƂȂ�J����
    public float earthScale = 6371f * 2f; // �n���̃X�P�[���i���a�ɍ��킹��j
    public float satelliteAltitude = 400f; // ���x 400 km
    public float sphereRadius = 50f; // ��_�ɔz�u����X�t�B�A�̃T�C�Y

    public float stepSize = 0.1f; // ���C��i�߂�X�e�b�v�T�C�Y
    public float tolerance = 0.01f; // ����̋��e�덷
    float earthRadius = 6371; // �n���̔��a


    private List<GameObject> createdObjects = new List<GameObject>(); // �������ꂽ�I�u�W�F�N�g��ێ����郊�X�g
    void ClearCreatedObjects()
    {
        // ���X�g���̑S�I�u�W�F�N�g���폜
        foreach (GameObject obj in createdObjects)
        {
            Destroy(obj);
        }

        // ���X�g���N���A
        createdObjects.Clear();
    }
    // float satelliteAltitude = 400; // ���x400km
    public void Projection()
    {
        // �����̃I�u�W�F�N�g���폜
        ClearCreatedObjects();

        // �J�����ʒu�̐ݒ�
        // camera.transform.position = new Vector3(0, earthRadius + satelliteAltitude, 0);
        // camera.transform.LookAt(Vector3.zero);

        // �J�����ݒ�
        //camera.farClipPlane = earthRadius + 1000; // �K�؂Ȕ͈͂ɐݒ�
        // camera.fieldOfView = 90; // ����p��90�x�ɐݒ�

        Debug.Log($"Camera Position: {camera.transform.position}");
        Debug.Log($"Camera Far Clip Plane: {camera.farClipPlane}");
        Debug.Log($"Earth Radius: {earthRadius}");

        // ������̃R�[�i�[���v�Z
        Vector3[] frustumCorners = new Vector3[4];
        camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);

        // �f�o�b�O: Camera Space
        for (int i = 0; i < frustumCorners.Length; i++)
        {
            Debug.Log($"Frustum Corner {i} (Camera Space): {frustumCorners[i]}");

           // PlaceSphereAt0(frustumCorners[i]);
        }
        Vector3 worldSpaceCorner;
        Vector3 cameraToEarthDirection= Vector3.zero;
        Vector3 surfaceOrigin = Vector3.zero;

        for (int i = 0; i < frustumCorners.Length; i++)
        {
            // ���[�J�����W�����[���h���W�ɕϊ�
            worldSpaceCorner = camera.transform.TransformPoint(frustumCorners[i]);

            // �J�����̈ʒu�ƒn�����S�iVector3.zero�j����ɖ@���������v�Z
            cameraToEarthDirection = (Vector3.zero - camera.transform.position).normalized;

            // �n�����a�������J���������ɕ␳
           
            // �X�P�[���_�E���i5000�Ŋ���j
            worldSpaceCorner /= 5000f;

            worldSpaceCorner -= cameraToEarthDirection * 6371f; // �n�����a���i6371�j�����␳


            // �X�t�B�A��z�u���Ċm�F
          //  PlaceSphereAt0_2(worldSpaceCorner);
            frustumCorners[i] = worldSpaceCorner;
            // �f�o�b�O�o��
            Debug.Log($"Frustum Corner {i} (Corrected World Space): {worldSpaceCorner}");
            surfaceOrigin += frustumCorners[i];
        }

        surfaceOrigin = surfaceOrigin / 4;
        // ���[���h���W�ɕϊ��Bscale5000�Ŋ���
        //  for (int i = 0; i < frustumCorners.Length; i++)
        //{
        //     frustumCorners[i] = camera.transform.TransformPoint(frustumCorners[i])/5000f;
        //     PlaceSphereAt0_2(frustumCorners[i]);
        //     Debug.Log($"Frustum Corner {i} (World Space): {frustumCorners[i]}");
        // }

        Vector3[] intersections = new Vector3[frustumCorners.Length];


        // �e�R�[�i�[����n���Ɍ��������C���������i�߂Ēn���̉������o
        for (int i = 0; i < frustumCorners.Length; i++)
        {


            // �n�����S����ɂ��������x�N�g�����v�Z�i������̒�ʂ̊e���_����n�����S�ւ̃x�N�g���j
            Vector3 vertex = Vector3.zero;



            if (i + 1 < frustumCorners.Length)
            {
                vertex = (frustumCorners[i] + frustumCorners[i + 1]) / 2;
              //  PlaceSphereAt2(vertex);
            }

            else
            {
                vertex = (frustumCorners[i] + frustumCorners[0]) / 2;
              //  PlaceSphereAt2(vertex);
            }

           // PlaceSphereAt2(-cameraToEarthDirection * 6371);
           // PlaceSphereAt2(surfaceOrigin);
            Vector3 directionToEarth = (surfaceOrigin - vertex).normalized;
            // ������̔��Α��Ɍ������������v�Z
            // Vector3 oppositeDirection = (camera.transform.position - worldSpaceCorner).normalized;

            // ���C���쐬�i������̒�ʂ̒��_����n�������Ɍ�����j
            Ray ray = new Ray(vertex, directionToEarth);



            Vector3 rayOrigin = vertex;
            Vector3 rayDirection = (surfaceOrigin - rayOrigin).normalized;
            intersections[i]= FindEarthEdge(rayOrigin, rayDirection);

          //  if (edgePoint != Vector3.zero)
          //  {
           //     Debug.Log($"Edge {i}: {edgePoint}");
           //     PlaceSphereAt(edgePoint); // �X�t�B�A��z�u���Ċm�F
           // }
           // else
           // {
           //     Debug.LogWarning($"No edge found for corner {i}");
           // }
        }



        /*
                // �n���\�ʂƂ̌�_���v�Z
                Debug.Log("Calculating intersections with Earth...");
                Vector3[] intersections = new Vector3[frustumCorners.Length];
                for (int i = 0; i < frustumCorners.Length; i++)
                {
                    // ���[�J�����W�����[���h���W�ɕϊ�

                    // �n�����S����ɂ��������x�N�g�����v�Z�i������̒�ʂ̊e���_����n�����S�ւ̃x�N�g���j
                    Vector3 vertex = Vector3.zero;



                    if (i + 1 < frustumCorners.Length)
                    {
                        vertex = (frustumCorners[i] + frustumCorners[i + 1]) / 2;
                        PlaceSphereAt2(vertex);
                    }

                    else
                    {
                        vertex = (frustumCorners[i] + frustumCorners[0]) / 2;
                        PlaceSphereAt2(vertex);
                    }

                    PlaceSphereAt2(-cameraToEarthDirection * 6371);
                    PlaceSphereAt2(surfaceOrigin);
                    Vector3 directionToEarth = (surfaceOrigin - vertex).normalized;
                    // ������̔��Α��Ɍ������������v�Z
                   // Vector3 oppositeDirection = (camera.transform.position - worldSpaceCorner).normalized;

                    // ���C���쐬�i������̒�ʂ̒��_����n�������Ɍ�����j
                    Ray ray = new Ray(vertex, directionToEarth);

                    // �n���\�ʂƂ̌�_���v�Z
                    if (SphereIntersection(ray, earthRadius, out float intersectionDistance))
                    {
                        Vector3 intersectionPoint = ray.GetPoint(intersectionDistance);

                        // ��_�ɃX�t�B�A��z�u
                        PlaceSphereAt(intersectionPoint);

                        // �f�o�b�O�o��
                        Debug.Log($"Intersection {i}: {intersectionPoint}");

                        Ray ray2 = new Ray(camera.transform.position, (intersectionPoint- camera.transform.position).normalized);
                        if (SphereIntersection(ray2, earthRadius, out float intersectionDistance2))
                        {
                            Vector3 intersectionPoint2 = ray2.GetPoint(intersectionDistance2);

                            // ��_�ɃX�t�B�A��z�u
                            PlaceSphereAt3(intersectionPoint2);

                            // �f�o�b�O�o��
                            Debug.Log($"Intersection {i}: {intersectionPoint2}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"No intersection for corner {i}");
                    }
                }*/

        // �n���\�ʏ�̌ʂ̒����i�_�E�������W�j���v�Z
        float arcLength1 = CalculateArcLength(intersections[0], intersections[2], earthRadius);
        float arcLength2 = CalculateArcLength(intersections[1], intersections[3], earthRadius);

     
        // �ʂ̒������e�L�X�g�I�u�W�F�N�g�ŕ\��
        DisplayArcLength(intersections[0], intersections[2], arcLength1, Color.red);
        DisplayArcLength(intersections[1], intersections[3], arcLength2, Color.cyan    );
        // �ʂ̒������f�o�b�O���O�ŕ\��
        Debug.Log($"Arc Length between Intersections[0] and Intersections[2]: {arcLength1} km");
        Debug.Log($"Arc Length between Intersections[1] and Intersections[3]: {arcLength2} km");

        // �n����ɐ�������
        DrawArcOnEarth(intersections[0], intersections[2], Color.red, earthRadius); // �Ίp1�̌ʁi�ԁj
        DrawArcOnEarth(intersections[1], intersections[3], Color.cyan     , earthRadius); // �Ίp2�̌ʁi�j

        // �|���S���ʐς��v�Z
        Debug.Log("Calculating spherical polygon area...");
       float area = CalculateSphericalPolygonArea(intersections, earthRadius);
        Debug.Log($"Projected Area: {area} km^2");

        // Projection�����I�����5�b��AClearCreatedObjects�����s
        StartCoroutine(ClearObjectsAfterDelay(5f));
    }
    private IEnumerator ClearObjectsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // �w�肳�ꂽ���Ԃ����҂�
        ClearCreatedObjects(); // �I�u�W�F�N�g���폜
        Debug.Log("Objects cleared after delay.");
    }
    float CalculateArcLength(Vector3 pointA, Vector3 pointB, float radius)
    {
        // 2�_�Ԃ̊p�x�i���W�A���j���v�Z
        float angle = Vector3.Angle(pointA.normalized, pointB.normalized) * Mathf.Deg2Rad;

        // �ʂ̒������v�Z
        return radius * angle;
    }

    Vector3 LatLonToCartesian(float latitude, float longitude, float radius)
    {
        float latRad = latitude * Mathf.Deg2Rad;
        float lonRad = longitude * Mathf.Deg2Rad;

        float x = radius * Mathf.Cos(latRad) * Mathf.Cos(lonRad);
        float y = radius * Mathf.Sin(latRad);
        float z = radius * Mathf.Cos(latRad) * Mathf.Sin(lonRad);

        return new Vector3(x, y, z);
    }
    void DisplayArcLength(Vector3 start, Vector3 end, float arcLength, Color color)
    {
        // �ʂ̒��ԓ_���v�Z
        Vector3 midPoint = (start + end) / 2;

        // �e�L�X�g�I�u�W�F�N�g�𐶐�
        GameObject textObj = new GameObject("ArcLengthText");
        TextMesh textMesh = textObj.AddComponent<TextMesh>();

        // �e�L�X�g���e���ʂ̒����ɐݒ�
        textMesh.text = $"{arcLength:F2} km"; // �����_�ȉ�2���ŕ\��
        textMesh.fontSize = 500; // �t�H���g�T�C�Y��傫���ݒ�
        textMesh.characterSize = 0.1f; // �e�L�X�g�̃X�P�[���𒲐�
        textMesh.color = color; // �e�L�X�g�̐F���ʂ̐F�Ɉ�v������

        // �e�L�X�g�̈ʒu���X�t�B�A�̏�����ɔz�u
        Vector3 offset = camera.transform.up * (earthRadius *1); // ������ւ̃I�t�Z�b�g
        textObj.transform.position = ( start + 3 * end) / 4;

        // �e�L�X�g�̃X�P�[���𒲐�
        textObj.transform.localScale = new Vector3(35, 35, 35);

        // �e�L�X�g���J�����̕����Ɍ�����
        textObj.transform.LookAt(camera.transform.position);
        textObj.transform.Rotate(0, 180, 0); // �e�L�X�g�𔽓]���Đ�����������

        // �e�L�X�g�����_�ɕ��s�ɔz�u�i�J�����̉�]�Ɉ�v������j
        textObj.transform.rotation = camera.transform.rotation;

        // ���������e�L�X�g�I�u�W�F�N�g�����X�g�ɒǉ����ĊǗ�
        createdObjects.Add(textObj);
    }


    void DrawArcOnEarth(Vector3 start, Vector3 end, Color color, float radius)
    {
        int segmentCount = 50; // �ʂ𕪊�����Z�O�����g��
        Vector3 startNormalized = start.normalized;
        Vector3 endNormalized = end.normalized;

        // LineRenderer�p�̃Q�[���I�u�W�F�N�g���쐬
        GameObject lineObj = new GameObject("ArcRenderer");
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();

        // LineRenderer�̐ݒ�
        lineRenderer.startWidth = 50f; // ���̕��i�n�_�j
        lineRenderer.endWidth = 50f;   // ���̕��i�I�_�j
        lineRenderer.positionCount = segmentCount + 1; // �ʂ̒��_��
        lineRenderer.useWorldSpace = true; // ���[���h���W���g�p
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // �V���v���ȃ}�e���A��
        lineRenderer.startColor = color; // ���̐F�i�n�_�j
        lineRenderer.endColor = color;   // ���̐F�i�I�_�j

        // �ʂ��Z�O�����g�ŕ`��
        for (int i = 0; i <= segmentCount; i++)
        {
            float t = (float)i / segmentCount; // �i������
            Vector3 pointOnArc = Vector3.Slerp(startNormalized, endNormalized, t) *( radius+50); // ���ʐ��`���
            lineRenderer.SetPosition(i, pointOnArc); // ���_��ݒ�
        }

        // ���������������X�g�ɒǉ����āA��ō폜�\��
        createdObjects.Add(lineObj);
    }



    public float maxRayDistance = 10000f; // ���C�̍ő勗��
    public LayerMask layerMask; // ���C���[�}�X�N�i�C�ӂ̃��C���[��ݒ�\�j

    // �n���\�ʂɋ߂Â��|�C���g��T������
    Vector3 FindEarthEdge(Vector3 rayOrigin, Vector3 rayDirection)
    {
        Vector3 currentPoint = rayOrigin;
        float currentDistance = Vector3.Distance(Vector3.zero, currentPoint);

        int loopCount = 0; // ���[�v�񐔂��J�E���g

        // ���C��i�߂Ēn���\�ʂ�T��
        while (true)
        {
            loopCount++;
           // Debug.Log($"Loop {loopCount}: Current Point = {currentPoint}, Distance to Earth Center = {currentDistance - earthRadius}");

            // ���[�v��100��𒴂�����I��
            if (loopCount > 1000)
            {
                Debug.LogWarning("Exceeded maximum loop count. Exiting to prevent infinite loop.");
                break;
            }

            // �X�e�b�v�T�C�Y�����i�߂�
            currentPoint += rayDirection * stepSize;
            currentDistance = Vector3.Distance(Vector3.zero, currentPoint);
            //PlaceSphereAt2(currentPoint);


           // Vector3 directionToEarth = (surfaceOrigin - vertex).normalized;
            // ������̔��Α��Ɍ������������v�Z
            // Vector3 oppositeDirection = (camera.transform.position - worldSpaceCorner).normalized;

            // ���C���쐬�i������̒�ʂ̒��_����n�������Ɍ�����j
            Ray ray = new Ray(camera.transform.position, (currentPoint- camera.transform.position).normalized);
            //Debug.Log($"Loop {loopCount}: Current Point = {currentPoint}, camera.transform.position = {camera.transform.position}");
           // PlaceSphereAt2(camera.transform.position+(currentPoint - camera.transform.position).normalized*1000);
            // �n���\�ʂƂ̌�_���v�Z
            /*if (SphereIntersection(ray, earthRadius, out float intersectionDistance))
            {
                Vector3 intersectionPoint = ray.GetPoint(intersectionDistance);
                Debug.LogWarning(intersectionPoint);

                // ��_�ɃX�t�B�A��z�u
                PlaceSphereAt(intersectionPoint);

                // �f�o�b�O�o��
                return intersectionPoint;

            }
            */
            if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, layerMask))
            {
                // �Փ˓_�̍��W���擾
                Vector3 hitPoint = hit.point;

                // �f�o�b�O���O�ŏՓ˓_��\��
                Debug.Log($"Ray hit at: {hitPoint}");

                // �Փ˓_�ɃX�t�B�A��z�u���Ċm�F
                PlaceSphereAt(hitPoint);

                return hitPoint;


            }


            // �n���\�ʂɓ��B�����ꍇ
            //if (Mathf.Abs(currentDistance - earthRadius) <= tolerance)
            //{
            //    Debug.Log($"Reached Earth surface at Loop {loopCount}: {currentPoint}");
            //    break;
            // }

            // �i�݂������ꍇ
            // if (currentDistance < earthRadius)
            // {
            //     Debug.LogWarning($"Overshot the Earth surface at Loop {loopCount}: {currentPoint}");
            //     break;
            // }
        }//
        // �n���\�ʂ��������Ȃ������ꍇ
        return Vector3.zero;
    }

    // ���̂Ƃ̌����_���v�Z
    bool SphereIntersection(Ray ray, float radius, out float intersectionDistance)
    {
        Vector3 originToCenter = -ray.origin; // �n�����S�� (0, 0, 0)
        float b = Vector3.Dot(ray.direction, originToCenter);
        float c = originToCenter.sqrMagnitude - radius * radius;
        float discriminant = b * b - c;

        if (discriminant < 0)
        {
            intersectionDistance = 0;
            return false; // �����Ȃ�
        }

        intersectionDistance = b - Mathf.Sqrt(discriminant);
        return intersectionDistance > 0; // �������J�����̑O���ɂ���ꍇ�̂ݗL��
    }

    // �J�[�gesian���W���ܓx�E�o�x�ɕϊ�
    /*  Vector2 CartesianToLatLon(Vector3 position, float radius)
      {
          float latitude = Mathf.Asin(position.y / radius) * Mathf.Rad2Deg; // �ܓx (�x)
          float longitude = Mathf.Atan2(position.z, position.x) * Mathf.Rad2Deg; // �o�x (�x)
          return new Vector2(latitude, longitude);
      }*/

    Vector2 CartesianToLatLon(Vector3 position, float radius)
    {
        Vector3 earthRotation = earthTransform.rotation.eulerAngles;
        print(earthRotation.y + 180);


        // �J�����̉q���V���p�i�O���X�Ίp 51.6 �x�j���l��
        Quaternion orbitInclination = Quaternion.Euler(0, 0, -23.4f);

        // �q���ʒu��n���̃��[�J����ɕϊ�
        Vector3 localPosition = Quaternion.Inverse(orbitInclination) * position;

        // ���[�J�����W�ňܓx�E�o�x���v�Z
        float latitude = Mathf.Asin(localPosition.y / radius) * Mathf.Rad2Deg; // �ܓx (�x)
        float longitude = Mathf.Atan2(localPosition.z, localPosition.x) * Mathf.Rad2Deg; // �o�x (�x)

        longitude = longitude + 180 + earthRotation.y;
        if (latitude > 90) latitude = 90;
       else if (latitude < -90) latitude = -90;

        // �o�x�� -180�`180 �x�ɐ��K��
        if (longitude > 180) longitude -= 360;
       else if (longitude < -180) longitude += 360;
        return new Vector2(latitude, longitude);
    }
    public Transform earthTransform;
    /*Vector2 CartesianToLatLon(Vector3 position, float radius)
    {-8.128
        // �n���̉�]���擾
        //Quaternion earthRotation = earthTransform.rotation;
        Vector3 earthRotation = earthTransform.localRotation.eulerAngles;

        // ���[���h���W��n���̃��[�J�����W�ɕϊ�
        //Vector3 localPosition = Quaternion.Inverse(camera.transform.rotation) * position;

        // �J�����̉q���V���p�i�O���X�Ίp 51.6 �x�j���l��
        Quaternion orbitInclination = Quaternion.Euler(0, 0, 51.6f);

        // �q���ʒu��n���̃��[�J����ɕϊ�
        Vector3 localPosition = Quaternion.Inverse(orbitInclination) * position;

        // ���[�J�����W�ňܓx�E�o�x���v�Z
        float latitude = Mathf.Asin(localPosition.y / radius) * Mathf.Rad2Deg; // �ܓx (�x)
         float longitude = Mathf.Atan2(localPosition.z, localPosition.x) * Mathf.Rad2Deg; // �o�x (�x)
                                                                                          // float latitude = Mathf.Asin(position.y / radius) * Mathf.Rad2Deg; // �ܓx (�x)
                                                                                          //float longitude = Mathf.Atan2(position.z, position.x) * Mathf.Rad2Deg; // �o�x (�x)
                                                                                          // �n���̉�]���l��
        longitude -= earthRotation.y;

        // �ܓx�� -90�`90 �x�ɐ��K��
        if (latitude > 90) latitude = 90;
        if (latitude < -90) latitude = -90;

        // �o�x�� -180�`180 �x�ɐ��K��
        if (longitude > 180) longitude -= 360;
        if (longitude < -180) longitude += 360;

        // �n���̌X�� (23.4�x) ���l��
        latitude += 23.4f;

        // �ܓx���ēx -90�`90 �x�Ɏ��߂�
        latitude = Mathf.Clamp(latitude, -90, 90);

        return new Vector2(latitude, longitude);
    }*/

    // ���ʃ|���S���̖ʐς��v�Z
    float CalculateSphericalPolygonArea(Vector3[] points, float radius)
    {
        int n = points.Length;
        float totalExcess = 0f; // ���ʎO�p�`�̉ߏ�p�̍��v
        for (int i = 0; i < n; i++)
        {
            Vector3 a = points[i].normalized;
            Vector3 b = points[(i + 1) % n].normalized;
            Vector3 c = points[(i + 2) % n].normalized;

            // ���ʗ]���藝�ɂ��p�x�v�Z
            float angle = Mathf.Acos(Vector3.Dot(Vector3.Cross(a, b), Vector3.Cross(b, c)) /
                                      (Vector3.Cross(a, b).magnitude * Vector3.Cross(b, c).magnitude));

            totalExcess += angle;
        }

        // ���ʃ|���S���ʐ� (����: A = R^2 * (���v�p�x - (n-2)*��))
        float polygonArea = radius * radius * (totalExcess - (n - 2) * Mathf.PI);
        return Mathf.Abs(polygonArea); // �ʐς͐��l��Ԃ�
    }

    void PlaceSphereAt(Vector3 position)
    {
        // �X�t�B�A�𐶐�
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = Vector3.one * sphereRadius; // �X�t�B�A�̃T�C�Y�𒲐�
        sphere.GetComponent<Renderer>().material.color = Color.red; // ���o�I�ɕ�����₷�����邽�߂ɐF��ύX

        // ���������X�t�B�A�����X�g�ɒǉ�
        createdObjects.Add(sphere);

        // �e�L�X�g�I�u�W�F�N�g�𐶐�
        GameObject textObj = new GameObject("CoordinateText");
        TextMesh textMesh = textObj.AddComponent<TextMesh>();

        // �ܓx�o�x���v�Z
        Vector2 latLon = CartesianToLatLon(position, earthRadius);

      
        // �e�L�X�g�̓��e���ܓx�o�x�ɐݒ�
        textMesh.text = $"Lat: {latLon.x:F2}��, Lon: {latLon.y:F2}��";
        // �e�L�X�g�̓��e�����W�ɐݒ�
       // textMesh.text = $"({position.x:F2}, {position.y:F2}, {position.z:F2})";
        textMesh.fontSize = 500; // �t�H���g�T�C�Y��傫���ݒ�
        textMesh.characterSize = 0.1f; // �e�L�X�g�̃X�P�[���𒲐�
        textMesh.color = Color.white; // �e�L�X�g�̐F��ݒ�

        // �e�L�X�g�̈ʒu���X�t�B�A�̏�����ɔz�u�i�J�����������l���j
        Vector3 offset = camera.transform.up * (sphereRadius * 1.5f); // ������ւ̃I�t�Z�b�g
        textObj.transform.position = position + offset;

        // �e�L�X�g�̃X�P�[���𒲐�
        textObj.transform.localScale = new Vector3(35, 35, 35);

        // �e�L�X�g���J�����̕����Ɍ�����
        textObj.transform.LookAt(camera.transform.position);
        textObj.transform.Rotate(0, 180, 0); // �e�L�X�g�𔽓]���Đ�����������

        // �e�L�X�g�����_�ɕ��s�ɔz�u�i�J�����̉�]�Ɉ�v������j
        textObj.transform.rotation = camera.transform.rotation;

        // ���������e�L�X�g�I�u�W�F�N�g�����X�g�ɒǉ�
        createdObjects.Add(textObj);
    }

   

    void PlaceSphereAt0(Vector3 position)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = Vector3.one * sphereRadius; // �X�t�B�A�̃T�C�Y�𒲐�
        sphere.GetComponent<Renderer>().material.color = Color.blue; // ���o�I�ɕ�����₷�����邽�߂ɐF��ύX

    }
        void PlaceSphereAt0_2(Vector3 position)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = position;
            sphere.transform.localScale = Vector3.one * sphereRadius; // �X�t�B�A�̃T�C�Y�𒲐�
            sphere.GetComponent<Renderer>().material.color = Color.green; // ���o�I�ɕ�����₷�����邽�߂ɐF��ύX
        }
    void PlaceSphereAt2(Vector3 position)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = Vector3.one * sphereRadius; // �X�t�B�A�̃T�C�Y�𒲐�
        sphere.GetComponent<Renderer>().material.color = Color.white; // ���o�I�ɕ�����₷�����邽�߂ɐF��ύX
    }
    void PlaceSphereAt3(Vector3 position)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = Vector3.one * sphereRadius; // �X�t�B�A�̃T�C�Y�𒲐�
        sphere.GetComponent<Renderer>().material.color = Color.gray; // ���o�I�ɕ�����₷�����邽�߂ɐF��ύX
    }

}

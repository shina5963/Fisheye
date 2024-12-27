using UnityEngine;
using System.Collections.Generic; // Listを使用するために必要
using System.Collections; // コルーチンを使用するために必要
public class FrustumToEarthProjection : MonoBehaviour
{
    public Camera camera; // 対象となるカメラ
    public float earthScale = 6371f * 2f; // 地球のスケール（直径に合わせる）
    public float satelliteAltitude = 400f; // 高度 400 km
    public float sphereRadius = 50f; // 交点に配置するスフィアのサイズ

    public float stepSize = 0.1f; // レイを進めるステップサイズ
    public float tolerance = 0.01f; // 判定の許容誤差
    float earthRadius = 6371; // 地球の半径


    private List<GameObject> createdObjects = new List<GameObject>(); // 生成されたオブジェクトを保持するリスト
    void ClearCreatedObjects()
    {
        // リスト内の全オブジェクトを削除
        foreach (GameObject obj in createdObjects)
        {
            Destroy(obj);
        }

        // リストをクリア
        createdObjects.Clear();
    }
    // float satelliteAltitude = 400; // 高度400km
    public void Projection()
    {
        // 既存のオブジェクトを削除
        ClearCreatedObjects();

        // カメラ位置の設定
        // camera.transform.position = new Vector3(0, earthRadius + satelliteAltitude, 0);
        // camera.transform.LookAt(Vector3.zero);

        // カメラ設定
        //camera.farClipPlane = earthRadius + 1000; // 適切な範囲に設定
        // camera.fieldOfView = 90; // 視野角を90度に設定

        Debug.Log($"Camera Position: {camera.transform.position}");
        Debug.Log($"Camera Far Clip Plane: {camera.farClipPlane}");
        Debug.Log($"Earth Radius: {earthRadius}");

        // 視錐台のコーナーを計算
        Vector3[] frustumCorners = new Vector3[4];
        camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);

        // デバッグ: Camera Space
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
            // ローカル座標をワールド座標に変換
            worldSpaceCorner = camera.transform.TransformPoint(frustumCorners[i]);

            // カメラの位置と地球中心（Vector3.zero）を基準に法線方向を計算
            cameraToEarthDirection = (Vector3.zero - camera.transform.position).normalized;

            // 地球半径分だけカメラ方向に補正
           
            // スケールダウン（5000で割る）
            worldSpaceCorner /= 5000f;

            worldSpaceCorner -= cameraToEarthDirection * 6371f; // 地球半径分（6371）だけ補正


            // スフィアを配置して確認
          //  PlaceSphereAt0_2(worldSpaceCorner);
            frustumCorners[i] = worldSpaceCorner;
            // デバッグ出力
            Debug.Log($"Frustum Corner {i} (Corrected World Space): {worldSpaceCorner}");
            surfaceOrigin += frustumCorners[i];
        }

        surfaceOrigin = surfaceOrigin / 4;
        // ワールド座標に変換。scale5000で割る
        //  for (int i = 0; i < frustumCorners.Length; i++)
        //{
        //     frustumCorners[i] = camera.transform.TransformPoint(frustumCorners[i])/5000f;
        //     PlaceSphereAt0_2(frustumCorners[i]);
        //     Debug.Log($"Frustum Corner {i} (World Space): {frustumCorners[i]}");
        // }

        Vector3[] intersections = new Vector3[frustumCorners.Length];


        // 各コーナーから地球に向かうレイを少しずつ進めて地球の縁を検出
        for (int i = 0; i < frustumCorners.Length; i++)
        {


            // 地球中心を基準にした方向ベクトルを計算（視錐台の底面の各頂点から地球中心へのベクトル）
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
            // 視錐台の反対側に向かう方向を計算
            // Vector3 oppositeDirection = (camera.transform.position - worldSpaceCorner).normalized;

            // レイを作成（視錐台の底面の頂点から地球方向に向ける）
            Ray ray = new Ray(vertex, directionToEarth);



            Vector3 rayOrigin = vertex;
            Vector3 rayDirection = (surfaceOrigin - rayOrigin).normalized;
            intersections[i]= FindEarthEdge(rayOrigin, rayDirection);

          //  if (edgePoint != Vector3.zero)
          //  {
           //     Debug.Log($"Edge {i}: {edgePoint}");
           //     PlaceSphereAt(edgePoint); // スフィアを配置して確認
           // }
           // else
           // {
           //     Debug.LogWarning($"No edge found for corner {i}");
           // }
        }



        /*
                // 地球表面との交点を計算
                Debug.Log("Calculating intersections with Earth...");
                Vector3[] intersections = new Vector3[frustumCorners.Length];
                for (int i = 0; i < frustumCorners.Length; i++)
                {
                    // ローカル座標をワールド座標に変換

                    // 地球中心を基準にした方向ベクトルを計算（視錐台の底面の各頂点から地球中心へのベクトル）
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
                    // 視錐台の反対側に向かう方向を計算
                   // Vector3 oppositeDirection = (camera.transform.position - worldSpaceCorner).normalized;

                    // レイを作成（視錐台の底面の頂点から地球方向に向ける）
                    Ray ray = new Ray(vertex, directionToEarth);

                    // 地球表面との交点を計算
                    if (SphereIntersection(ray, earthRadius, out float intersectionDistance))
                    {
                        Vector3 intersectionPoint = ray.GetPoint(intersectionDistance);

                        // 交点にスフィアを配置
                        PlaceSphereAt(intersectionPoint);

                        // デバッグ出力
                        Debug.Log($"Intersection {i}: {intersectionPoint}");

                        Ray ray2 = new Ray(camera.transform.position, (intersectionPoint- camera.transform.position).normalized);
                        if (SphereIntersection(ray2, earthRadius, out float intersectionDistance2))
                        {
                            Vector3 intersectionPoint2 = ray2.GetPoint(intersectionDistance2);

                            // 交点にスフィアを配置
                            PlaceSphereAt3(intersectionPoint2);

                            // デバッグ出力
                            Debug.Log($"Intersection {i}: {intersectionPoint2}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"No intersection for corner {i}");
                    }
                }*/

        // 地球表面上の弧の長さ（ダウンレンジ）を計算
        float arcLength1 = CalculateArcLength(intersections[0], intersections[2], earthRadius);
        float arcLength2 = CalculateArcLength(intersections[1], intersections[3], earthRadius);

     
        // 弧の長さをテキストオブジェクトで表示
        DisplayArcLength(intersections[0], intersections[2], arcLength1, Color.red);
        DisplayArcLength(intersections[1], intersections[3], arcLength2, Color.cyan    );
        // 弧の長さをデバッグログで表示
        Debug.Log($"Arc Length between Intersections[0] and Intersections[2]: {arcLength1} km");
        Debug.Log($"Arc Length between Intersections[1] and Intersections[3]: {arcLength2} km");

        // 地球上に線を引く
        DrawArcOnEarth(intersections[0], intersections[2], Color.red, earthRadius); // 対角1の弧（赤）
        DrawArcOnEarth(intersections[1], intersections[3], Color.cyan     , earthRadius); // 対角2の弧（青）

        // ポリゴン面積を計算
        Debug.Log("Calculating spherical polygon area...");
       float area = CalculateSphericalPolygonArea(intersections, earthRadius);
        Debug.Log($"Projected Area: {area} km^2");

        // Projection処理終了後に5秒後、ClearCreatedObjectsを実行
        StartCoroutine(ClearObjectsAfterDelay(5f));
    }
    private IEnumerator ClearObjectsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 指定された時間だけ待つ
        ClearCreatedObjects(); // オブジェクトを削除
        Debug.Log("Objects cleared after delay.");
    }
    float CalculateArcLength(Vector3 pointA, Vector3 pointB, float radius)
    {
        // 2点間の角度（ラジアン）を計算
        float angle = Vector3.Angle(pointA.normalized, pointB.normalized) * Mathf.Deg2Rad;

        // 弧の長さを計算
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
        // 弧の中間点を計算
        Vector3 midPoint = (start + end) / 2;

        // テキストオブジェクトを生成
        GameObject textObj = new GameObject("ArcLengthText");
        TextMesh textMesh = textObj.AddComponent<TextMesh>();

        // テキスト内容を弧の長さに設定
        textMesh.text = $"{arcLength:F2} km"; // 小数点以下2桁で表示
        textMesh.fontSize = 500; // フォントサイズを大きく設定
        textMesh.characterSize = 0.1f; // テキストのスケールを調整
        textMesh.color = color; // テキストの色を弧の色に一致させる

        // テキストの位置をスフィアの少し上に配置
        Vector3 offset = camera.transform.up * (earthRadius *1); // 上方向へのオフセット
        textObj.transform.position = ( start + 3 * end) / 4;

        // テキストのスケールを調整
        textObj.transform.localScale = new Vector3(35, 35, 35);

        // テキストをカメラの方向に向ける
        textObj.transform.LookAt(camera.transform.position);
        textObj.transform.Rotate(0, 180, 0); // テキストを反転して正しい向きに

        // テキストを視点に平行に配置（カメラの回転に一致させる）
        textObj.transform.rotation = camera.transform.rotation;

        // 生成したテキストオブジェクトをリストに追加して管理
        createdObjects.Add(textObj);
    }


    void DrawArcOnEarth(Vector3 start, Vector3 end, Color color, float radius)
    {
        int segmentCount = 50; // 弧を分割するセグメント数
        Vector3 startNormalized = start.normalized;
        Vector3 endNormalized = end.normalized;

        // LineRenderer用のゲームオブジェクトを作成
        GameObject lineObj = new GameObject("ArcRenderer");
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();

        // LineRendererの設定
        lineRenderer.startWidth = 50f; // 線の幅（始点）
        lineRenderer.endWidth = 50f;   // 線の幅（終点）
        lineRenderer.positionCount = segmentCount + 1; // 弧の頂点数
        lineRenderer.useWorldSpace = true; // ワールド座標を使用
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // シンプルなマテリアル
        lineRenderer.startColor = color; // 線の色（始点）
        lineRenderer.endColor = color;   // 線の色（終点）

        // 弧をセグメントで描画
        for (int i = 0; i <= segmentCount; i++)
        {
            float t = (float)i / segmentCount; // 進捗割合
            Vector3 pointOnArc = Vector3.Slerp(startNormalized, endNormalized, t) *( radius+50); // 球面線形補間
            lineRenderer.SetPosition(i, pointOnArc); // 頂点を設定
        }

        // 生成した線をリストに追加して、後で削除可能に
        createdObjects.Add(lineObj);
    }



    public float maxRayDistance = 10000f; // レイの最大距離
    public LayerMask layerMask; // レイヤーマスク（任意のレイヤーを設定可能）

    // 地球表面に近づくポイントを探索する
    Vector3 FindEarthEdge(Vector3 rayOrigin, Vector3 rayDirection)
    {
        Vector3 currentPoint = rayOrigin;
        float currentDistance = Vector3.Distance(Vector3.zero, currentPoint);

        int loopCount = 0; // ループ回数をカウント

        // レイを進めて地球表面を探す
        while (true)
        {
            loopCount++;
           // Debug.Log($"Loop {loopCount}: Current Point = {currentPoint}, Distance to Earth Center = {currentDistance - earthRadius}");

            // ループが100回を超えたら終了
            if (loopCount > 1000)
            {
                Debug.LogWarning("Exceeded maximum loop count. Exiting to prevent infinite loop.");
                break;
            }

            // ステップサイズだけ進める
            currentPoint += rayDirection * stepSize;
            currentDistance = Vector3.Distance(Vector3.zero, currentPoint);
            //PlaceSphereAt2(currentPoint);


           // Vector3 directionToEarth = (surfaceOrigin - vertex).normalized;
            // 視錐台の反対側に向かう方向を計算
            // Vector3 oppositeDirection = (camera.transform.position - worldSpaceCorner).normalized;

            // レイを作成（視錐台の底面の頂点から地球方向に向ける）
            Ray ray = new Ray(camera.transform.position, (currentPoint- camera.transform.position).normalized);
            //Debug.Log($"Loop {loopCount}: Current Point = {currentPoint}, camera.transform.position = {camera.transform.position}");
           // PlaceSphereAt2(camera.transform.position+(currentPoint - camera.transform.position).normalized*1000);
            // 地球表面との交点を計算
            /*if (SphereIntersection(ray, earthRadius, out float intersectionDistance))
            {
                Vector3 intersectionPoint = ray.GetPoint(intersectionDistance);
                Debug.LogWarning(intersectionPoint);

                // 交点にスフィアを配置
                PlaceSphereAt(intersectionPoint);

                // デバッグ出力
                return intersectionPoint;

            }
            */
            if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, layerMask))
            {
                // 衝突点の座標を取得
                Vector3 hitPoint = hit.point;

                // デバッグログで衝突点を表示
                Debug.Log($"Ray hit at: {hitPoint}");

                // 衝突点にスフィアを配置して確認
                PlaceSphereAt(hitPoint);

                return hitPoint;


            }


            // 地球表面に到達した場合
            //if (Mathf.Abs(currentDistance - earthRadius) <= tolerance)
            //{
            //    Debug.Log($"Reached Earth surface at Loop {loopCount}: {currentPoint}");
            //    break;
            // }

            // 進みすぎた場合
            // if (currentDistance < earthRadius)
            // {
            //     Debug.LogWarning($"Overshot the Earth surface at Loop {loopCount}: {currentPoint}");
            //     break;
            // }
        }//
        // 地球表面を見つけられなかった場合
        return Vector3.zero;
    }

    // 球体との交差点を計算
    bool SphereIntersection(Ray ray, float radius, out float intersectionDistance)
    {
        Vector3 originToCenter = -ray.origin; // 地球中心は (0, 0, 0)
        float b = Vector3.Dot(ray.direction, originToCenter);
        float c = originToCenter.sqrMagnitude - radius * radius;
        float discriminant = b * b - c;

        if (discriminant < 0)
        {
            intersectionDistance = 0;
            return false; // 交差なし
        }

        intersectionDistance = b - Mathf.Sqrt(discriminant);
        return intersectionDistance > 0; // 交差がカメラの前方にある場合のみ有効
    }

    // カートesian座標を緯度・経度に変換
    /*  Vector2 CartesianToLatLon(Vector3 position, float radius)
      {
          float latitude = Mathf.Asin(position.y / radius) * Mathf.Rad2Deg; // 緯度 (度)
          float longitude = Mathf.Atan2(position.z, position.x) * Mathf.Rad2Deg; // 経度 (度)
          return new Vector2(latitude, longitude);
      }*/

    Vector2 CartesianToLatLon(Vector3 position, float radius)
    {
        Vector3 earthRotation = earthTransform.rotation.eulerAngles;
        print(earthRotation.y + 180);


        // カメラの衛星天頂角（軌道傾斜角 51.6 度）を考慮
        Quaternion orbitInclination = Quaternion.Euler(0, 0, -23.4f);

        // 衛星位置を地球のローカル基準に変換
        Vector3 localPosition = Quaternion.Inverse(orbitInclination) * position;

        // ローカル座標で緯度・経度を計算
        float latitude = Mathf.Asin(localPosition.y / radius) * Mathf.Rad2Deg; // 緯度 (度)
        float longitude = Mathf.Atan2(localPosition.z, localPosition.x) * Mathf.Rad2Deg; // 経度 (度)

        longitude = longitude + 180 + earthRotation.y;
        if (latitude > 90) latitude = 90;
       else if (latitude < -90) latitude = -90;

        // 経度を -180～180 度に正規化
        if (longitude > 180) longitude -= 360;
       else if (longitude < -180) longitude += 360;
        return new Vector2(latitude, longitude);
    }
    public Transform earthTransform;
    /*Vector2 CartesianToLatLon(Vector3 position, float radius)
    {-8.128
        // 地球の回転を取得
        //Quaternion earthRotation = earthTransform.rotation;
        Vector3 earthRotation = earthTransform.localRotation.eulerAngles;

        // ワールド座標を地球のローカル座標に変換
        //Vector3 localPosition = Quaternion.Inverse(camera.transform.rotation) * position;

        // カメラの衛星天頂角（軌道傾斜角 51.6 度）を考慮
        Quaternion orbitInclination = Quaternion.Euler(0, 0, 51.6f);

        // 衛星位置を地球のローカル基準に変換
        Vector3 localPosition = Quaternion.Inverse(orbitInclination) * position;

        // ローカル座標で緯度・経度を計算
        float latitude = Mathf.Asin(localPosition.y / radius) * Mathf.Rad2Deg; // 緯度 (度)
         float longitude = Mathf.Atan2(localPosition.z, localPosition.x) * Mathf.Rad2Deg; // 経度 (度)
                                                                                          // float latitude = Mathf.Asin(position.y / radius) * Mathf.Rad2Deg; // 緯度 (度)
                                                                                          //float longitude = Mathf.Atan2(position.z, position.x) * Mathf.Rad2Deg; // 経度 (度)
                                                                                          // 地球の回転を考慮
        longitude -= earthRotation.y;

        // 緯度を -90～90 度に正規化
        if (latitude > 90) latitude = 90;
        if (latitude < -90) latitude = -90;

        // 経度を -180～180 度に正規化
        if (longitude > 180) longitude -= 360;
        if (longitude < -180) longitude += 360;

        // 地軸の傾き (23.4度) を考慮
        latitude += 23.4f;

        // 緯度を再度 -90～90 度に収める
        latitude = Mathf.Clamp(latitude, -90, 90);

        return new Vector2(latitude, longitude);
    }*/

    // 球面ポリゴンの面積を計算
    float CalculateSphericalPolygonArea(Vector3[] points, float radius)
    {
        int n = points.Length;
        float totalExcess = 0f; // 球面三角形の過剰角の合計
        for (int i = 0; i < n; i++)
        {
            Vector3 a = points[i].normalized;
            Vector3 b = points[(i + 1) % n].normalized;
            Vector3 c = points[(i + 2) % n].normalized;

            // 球面余弦定理による角度計算
            float angle = Mathf.Acos(Vector3.Dot(Vector3.Cross(a, b), Vector3.Cross(b, c)) /
                                      (Vector3.Cross(a, b).magnitude * Vector3.Cross(b, c).magnitude));

            totalExcess += angle;
        }

        // 球面ポリゴン面積 (公式: A = R^2 * (合計角度 - (n-2)*π))
        float polygonArea = radius * radius * (totalExcess - (n - 2) * Mathf.PI);
        return Mathf.Abs(polygonArea); // 面積は正値を返す
    }

    void PlaceSphereAt(Vector3 position)
    {
        // スフィアを生成
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = Vector3.one * sphereRadius; // スフィアのサイズを調整
        sphere.GetComponent<Renderer>().material.color = Color.red; // 視覚的に分かりやすくするために色を変更

        // 生成したスフィアをリストに追加
        createdObjects.Add(sphere);

        // テキストオブジェクトを生成
        GameObject textObj = new GameObject("CoordinateText");
        TextMesh textMesh = textObj.AddComponent<TextMesh>();

        // 緯度経度を計算
        Vector2 latLon = CartesianToLatLon(position, earthRadius);

      
        // テキストの内容を緯度経度に設定
        textMesh.text = $"Lat: {latLon.x:F2}°, Lon: {latLon.y:F2}°";
        // テキストの内容を座標に設定
       // textMesh.text = $"({position.x:F2}, {position.y:F2}, {position.z:F2})";
        textMesh.fontSize = 500; // フォントサイズを大きく設定
        textMesh.characterSize = 0.1f; // テキストのスケールを調整
        textMesh.color = Color.white; // テキストの色を設定

        // テキストの位置をスフィアの少し上に配置（カメラ方向を考慮）
        Vector3 offset = camera.transform.up * (sphereRadius * 1.5f); // 上方向へのオフセット
        textObj.transform.position = position + offset;

        // テキストのスケールを調整
        textObj.transform.localScale = new Vector3(35, 35, 35);

        // テキストをカメラの方向に向ける
        textObj.transform.LookAt(camera.transform.position);
        textObj.transform.Rotate(0, 180, 0); // テキストを反転して正しい向きに

        // テキストを視点に平行に配置（カメラの回転に一致させる）
        textObj.transform.rotation = camera.transform.rotation;

        // 生成したテキストオブジェクトをリストに追加
        createdObjects.Add(textObj);
    }

   

    void PlaceSphereAt0(Vector3 position)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = Vector3.one * sphereRadius; // スフィアのサイズを調整
        sphere.GetComponent<Renderer>().material.color = Color.blue; // 視覚的に分かりやすくするために色を変更

    }
        void PlaceSphereAt0_2(Vector3 position)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = position;
            sphere.transform.localScale = Vector3.one * sphereRadius; // スフィアのサイズを調整
            sphere.GetComponent<Renderer>().material.color = Color.green; // 視覚的に分かりやすくするために色を変更
        }
    void PlaceSphereAt2(Vector3 position)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = Vector3.one * sphereRadius; // スフィアのサイズを調整
        sphere.GetComponent<Renderer>().material.color = Color.white; // 視覚的に分かりやすくするために色を変更
    }
    void PlaceSphereAt3(Vector3 position)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = Vector3.one * sphereRadius; // スフィアのサイズを調整
        sphere.GetComponent<Renderer>().material.color = Color.gray; // 視覚的に分かりやすくするために色を変更
    }

}

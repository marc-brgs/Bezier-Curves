using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class PointManager : MonoBehaviour
{
    public GameObject controlPointPrefab; 
    private List<Vector3> controlPoints = new List<Vector3>(); 
    private bool inputEnabled = true;
    private LineRenderer lineRenderer;
    private bool polygonClosed = false;
    public Button cButton;
    public Button pButton; 
    
    public GameObject pointManagerObject;
    
    // Déplacer un point
    private List<GameObject> controlPointsObjects = new List<GameObject>();
    private GameObject bezierLine;
    private bool isHold = false;
    private GameObject closestPoint;
    private int closestIndex = 0;
    private bool isDrawned = false;
    private string lastMethod = "casteljau";
    
    // Lissage
    private int step = 100;
    public int heightES = 5;
    public float scaleES = 1f;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        
        // Utilise la référence pour appeler les méthodes appropriées
        pButton.onClick.AddListener(() =>
        {
            ClearBezier();
            GeneratePascale(controlPoints);
            lastMethod = "pascale";
            isDrawned = true;
        });
        
        cButton.onClick.AddListener(() =>
        {
            ClearBezier();
            GenerateCasteljau(controlPoints);
            lastMethod = "casteljau";
            isDrawned = true;
        });
    }

    private void Update()
    {
        // Vérifie si la détection des clics est activée
        if (inputEnabled)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 screenPosition = Input.mousePosition;

                // Convertit la position du clic de l'écran à la position dans le monde
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 10f));

                // Ajoute le point de contrôle à la liste
                controlPoints.Add(worldPosition);
                controlPointsObjects.Add(CreateControlPoint(worldPosition));
                UpdateLineRenderer();
            }
            if (Input.GetMouseButtonDown(1))
            {
                inputEnabled = false;
                cButton.gameObject.SetActive(true);
                pButton.gameObject.SetActive(true);
                ClosePolygon();
            }
        }

        // Déplacement d'un point
        if (isDrawned)
        {
            // Ajout d'un point
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
            {
                Vector3 screenPosition = Input.mousePosition;
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 10f));
                
                float minDistance = 1000f;
                float minDistance2 = 1000f;
                
                int closestIndex2 = 0;
                int i = 0;
                foreach (var controlPointObj in controlPointsObjects)
                {
                    if (Vector3.Distance(controlPointObj.transform.position, worldPosition) < minDistance)
                    {
                        closestIndex2 = closestIndex;
                        minDistance2 = minDistance;
                        closestIndex = i;
                        minDistance = Vector3.Distance(controlPointObj.transform.position, worldPosition);
                    }
                    else if (Vector3.Distance(controlPointObj.transform.position, worldPosition) < minDistance2)
                    {
                        closestIndex2 = i;
                        minDistance2 = Vector3.Distance(controlPointObj.transform.position, worldPosition);
                    }
                    i++;
                }

                int minIndex = closestIndex < closestIndex2 ? closestIndex : closestIndex2;
                if (minIndex == 0 && (closestIndex == controlPoints.Count-1 || closestIndex2 == controlPoints.Count-1))
                    minIndex = controlPoints.Count-1; // point entre le premier et le dernier
                controlPoints.Insert(minIndex+1, worldPosition);
                controlPointsObjects.Insert(minIndex+1, CreateControlPoint(worldPosition));
                LiveRefresh();
            }
            else if (Input.GetMouseButtonDown(0) && !isHold)
            {
                Vector3 screenPosition = Input.mousePosition;
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 10f));

                float minDistance = 1000f;
                
                int i = 0;
                foreach (var controlPointObj in controlPointsObjects)
                {
                    if (Vector3.Distance(controlPointObj.transform.position, worldPosition) < minDistance)
                    {
                        closestIndex = i;
                        minDistance = Vector3.Distance(controlPointObj.transform.position, worldPosition);
                    }
                    i++;
                }
                
                if (minDistance < 2f)
                {
                    isHold = true;
                }
            }
            else if (Input.GetMouseButtonUp(0) && isHold)
            {
                isHold = false;
            }

            if (isHold)
            {
                Vector3 screenPosition = Input.mousePosition;
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 10f));
                
                controlPoints[closestIndex] = worldPosition;
                controlPointsObjects[closestIndex].transform.position = worldPosition;
                LiveRefresh();
            }
            
            // Supprimer un point
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Vector3 screenPosition = Input.mousePosition;
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 10f));
                
                float minDistance = 1000f;
                
                int i = 0;
                foreach (var controlPointObj in controlPointsObjects)
                {
                    if (Vector3.Distance(controlPointObj.transform.position, worldPosition) < minDistance)
                    {
                        closestIndex = i;
                        minDistance = Vector3.Distance(controlPointObj.transform.position, worldPosition);
                    }
                    i++;
                }

                controlPoints.RemoveAt(closestIndex);
                Destroy(controlPointsObjects[closestIndex]);
                controlPointsObjects.RemoveAt(closestIndex);
                LiveRefresh();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                if (step >= 100) step = 100;
                else if (step > 10) step += 10;
                else step++;
                
                Debug.Log(step);
                LiveRefresh();
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                if (step <= 2) step = 2;
                else if (step < 10) step--;
                else if (step < 20) step -= 2;
                else step -= 10;
                
                Debug.Log(step);
                LiveRefresh();
            }
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                ExtrudeBezierCurve(controlPoints, heightES, scaleES);
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                List<Vector3> pathPoints = new List<Vector3>();
                pathPoints.Add(new Vector3(0f, 0f, 0f));
                pathPoints.Add(new Vector3(0f, 0f, 6f));
                pathPoints.Add(new Vector3(0f, -4f, 10f));
                pathPoints.Add(new Vector3(0f, 4f, 14f));
                pathPoints.Add(new Vector3(0f, -4f, 18f));
                pathPoints.Add(new Vector3(0f, 4f, 22f));
                pathPoints.Add(new Vector3(0f, -4f, 26f));
                GeneralizedExtrudeBezierCurve(controlPoints, pathPoints);
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                List<Vector3> pathPoints = new List<Vector3>();
                pathPoints.Add(new Vector3(0f, 0f, 0f));
                pathPoints.Add(new Vector3(0f, 0f, 6f));
                pathPoints.Add(new Vector3(0f, -4f, 10f));
                pathPoints.Add(new Vector3(0f, 4f, 14f));
                pathPoints.Add(new Vector3(0f, -4f, 18f));
                pathPoints.Add(new Vector3(0f, 4f, 22f));
                pathPoints.Add(new Vector3(0f, -4f, 26f));
                GeneralizedExtrudeBezierCurveWithNormals(controlPoints, pathPoints);
            }
        }


        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearPoints();
            ClearBezier();
            inputEnabled = true;
            cButton.gameObject.SetActive(false);
            pButton.gameObject.SetActive(false);
            controlPoints.Clear();
            controlPointsObjects.Clear();
            UpdateLineRenderer();
            polygonClosed = false;
        }
        
    }

     public void ExtrudeBezierCurve(List<Vector3> controlPoints, int height, float scale)
    {
        int numPoints = step;
        List<Vector3> bezierPoints = new List<Vector3>();

        // Parcours les valeurs de paramètre t de 0 à 1 et calcule les points sur la courbe de Bézier
        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (float)(numPoints - 1);
            Vector3 point = DeCasteljau(controlPoints, t);
            bezierPoints.Add(point);
        }
        
        List<Vector3> nextBezierPoints = new List<Vector3>();
        List<Vector3> vertices = new List<Vector3>(bezierPoints);
        List<int> tris = new List<int>();
        int sizeB = bezierPoints.Count;

        List<Vector3> scaleOffset = new List<Vector3>();
        for (int i = 0; i < bezierPoints.Count; i++)
        {
            Vector3 p = bezierPoints[i];
            scaleOffset.Add(new Vector3((p.x * scale) - p.x, (p.y * scale) - p.y, (p.z * scale) - p.z));
        }
        // Subdivision
        for (int i = 0; i < height + 1; i++)
        {
            // Creation de la prochaine ligne du mesh
            for (int n = 0; n < sizeB; n++)
            {
                Vector3 p = bezierPoints[n];
                Vector3 o = scaleOffset[n];
                nextBezierPoints.Add(new Vector3(p.x + (o.x / height), p.y + (o.y / height), p.z + 1));
            }
            vertices.AddRange(nextBezierPoints);
            bezierPoints.Clear();
            bezierPoints.AddRange(nextBezierPoints);
            nextBezierPoints.Clear();
        }
        
        // Face avant
        for (int n = 0; n < sizeB - 1; n++)
        {
            tris.Add(0);
            tris.Add(n);
            tris.Add(n + 1);
        }
        // Face arrière
        for (int n = 0; n < sizeB - 1; n++)
        {
            tris.Add(height * sizeB + n + 1);
            tris.Add(height * sizeB + n);
            tris.Add(height * sizeB);
        }
        for (int i = 0; i < height; i++)
        {
            for (int n = 0; n < sizeB; n++)
            {
                // Premier triangle
                tris.Add(i * sizeB + n);
                tris.Add((i + 1) * sizeB + n);
                tris.Add(i * sizeB + (n + 1) % sizeB);

                // Second triangle
                tris.Add((i + 1) * sizeB + n);
                tris.Add((i + 1) * sizeB + (n + 1) % sizeB);
                tris.Add(i * sizeB + (n + 1) % sizeB);
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateUVDistributionMetric(0);
        
        GameObject meshGameObject = new GameObject("Mesh");
        meshGameObject.tag = "Extru";
        MeshRenderer mr = meshGameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        MeshFilter filter = meshGameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filter.mesh = mesh;
        
        // Texture
        Material material = new Material(Shader.Find("Standard"));
        material.color = Color.red;
        mr.material = material;
        
        meshGameObject.transform.position = new Vector3(0f, 0f, 0f);
    }
     
   public void GeneralizedExtrudeBezierCurveWithNormals(List<Vector3> controlPoints, List<Vector3> pathPoints)
    {
        int numPoints = step;
        List<Vector3> bezierPoints = new List<Vector3>();

        // Parcours les valeurs de paramètre t de 0 à 1 et calcule les points sur la courbe de Bézier
        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (float)(numPoints - 1);
            Vector3 point = DeCasteljau(controlPoints, t);
            bezierPoints.Add(point);
        }
        
        // Calcule des points bezier de la trajectoire à suivre
        List<Vector3> trajectoire = new List<Vector3>();
        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (float)(numPoints - 1);
            Vector3 point = DeCasteljau(pathPoints, t);
            trajectoire.Add(point);
        }
        
        List<Vector3> nextBezierPoints = new List<Vector3>();
        List<Vector3> vertices = new List<Vector3>(bezierPoints);
        List<int> tris = new List<int>();
        int sizeB = bezierPoints.Count;
        
        for (int i = 1; i < trajectoire.Count; i++)
        {
            Vector3 prevTrajectoire = trajectoire[i - 1];
            Vector3 posTrajectoire = trajectoire[i];

            Vector3 normalBezier = Vector3.Cross(prevTrajectoire - posTrajectoire, Vector3.up).normalized;
            Vector3 milieu = (prevTrajectoire + posTrajectoire) * 0.5f;

            // Création de la prochaine ligne du mesh
            for (int n = 0; n < sizeB; n++)
            {
                Vector3 p = bezierPoints[n] - milieu;
                Vector3 newP = Quaternion.LookRotation(posTrajectoire - prevTrajectoire, normalBezier) * p;
                nextBezierPoints.Add(newP + posTrajectoire + milieu);
            }
            
            vertices.AddRange(nextBezierPoints);
            nextBezierPoints.Clear();
        }
        
        // Triangulation
        // Face arrière
        for (int n = 0; n < sizeB - 1; n++)
        {
            tris.Add(0);
            tris.Add(n);
            tris.Add(n + 1);
        }
        // Face avant
        for (int n = 0; n < sizeB - 1; n++)
        {
            tris.Add((trajectoire.Count - 2) * sizeB + n + 1);
            tris.Add((trajectoire.Count - 2) * sizeB + n);
            tris.Add((trajectoire.Count - 2) * sizeB);
        }
        for (int i = 0; i < trajectoire.Count - 2; i++)
        {
            for (int n = 0; n < sizeB; n++)
            {
                // Premier triangle
                tris.Add(i * sizeB + n);
                tris.Add((i + 1) * sizeB + n);
                tris.Add(i * sizeB + (n + 1) % sizeB);

                // Second triangle
                tris.Add((i + 1) * sizeB + n);
                tris.Add((i + 1) * sizeB + (n + 1) % sizeB);
                tris.Add(i * sizeB + (n + 1) % sizeB);
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateUVDistributionMetric(0);

        GameObject meshGameObject = new GameObject("Mesh");
        meshGameObject.tag = "Extru";
        MeshRenderer mr = meshGameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        MeshFilter filter = meshGameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filter.mesh = mesh;
        
        // Texture
        Material material = new Material(Shader.Find("Standard"));
        material.color = Color.red;
        mr.material = material;
    }

     public void GeneralizedExtrudeBezierCurve(List<Vector3> controlPoints, List<Vector3> pathPoints)
    {
        int numPoints = step;
        int numPathPoints = numPoints;
        Vector3[] bezierPoints = new Vector3[numPoints];

        // Parcourt les valeurs de paramètre t de 0 à 1 et calcule les points sur la courbe de Bézier
        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (float)(numPoints - 1);
            Vector3 point = DeCasteljau(controlPoints, t);
            bezierPoints[i] = point;
        }

        Vector3[] bezierPathPoints = new Vector3[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (float)(numPoints - 1);
            Vector3 point = DeCasteljau(pathPoints, t);
            bezierPathPoints[i] = point;
        }
        

        // Crée une liste de vertices pour le maillage de l'extrusion
        List<Vector3> vertices = new List<Vector3>();

        // Ajoute les vertices pour chaque point le long de la courbe de Bézier
        for (int i = 0; i < numPoints; i++)
        {
            Vector3 bezierPoint = bezierPoints[i];

            for (int j = 0; j < numPathPoints; j++)
            {
                Vector3 pathPoint = bezierPathPoints[j];
                vertices.Add(bezierPoint + pathPoint);
            }
        }
        
        List<int> triangles = new List<int>();
        
        int numPointsPerLine = numPathPoints;

        // Ajoute les triangles pour chaque face de l'extrusion
        for (int i = 0; i < numPoints - 1; i++)
        {
            int startIndex = i * numPointsPerLine;

            for (int j = 0; j < numPointsPerLine - 1; j++)
            {
                // Triangle 1
                triangles.Add(startIndex + j);
                triangles.Add(startIndex + j + 1);
                triangles.Add(startIndex + j + numPointsPerLine);

                // Triangle 2
                triangles.Add(startIndex + j + numPointsPerLine);
                triangles.Add(startIndex + j + 1);
                triangles.Add(startIndex + j + numPointsPerLine + 1);
            }
        }
        
        // Ajoute les triangles pour la face manquante (fermeture du maillage)
        int startIndexLastRow = (numPoints - 1) * numPointsPerLine;
        int startIndexFirstRow = 0;
        for (int j = 0; j < numPointsPerLine - 1; j++)
        {
            // Triangle 1 (inversé)
            triangles.Add(startIndexLastRow + j);
            triangles.Add(startIndexLastRow + j + 1);
            triangles.Add(startIndexFirstRow + j);

            // Triangle 2 (inversé)
            triangles.Add(startIndexFirstRow + j);
            triangles.Add(startIndexLastRow + j + 1);
            triangles.Add(startIndexFirstRow + j + 1);
        }

        GameObject extrusionObject = new GameObject("Extrusion");
        MeshRenderer meshRenderer = extrusionObject.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = extrusionObject.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        meshFilter.mesh = mesh;

        // Texture
        Material material = new Material(Shader.Find("Standard"));
        material.color = Color.red;
        meshRenderer.material = material;
        
        extrusionObject.transform.position = new Vector3(0f, 0f, 0f);
    }



    private void ClearBezier()
    {
        Destroy(bezierLine);
    }

    private void ClearPoints()
    {
        GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag("Point");
        
        foreach (GameObject obj in objectsToDestroy)
        {
            Destroy(obj);
        }
    }

    private void LiveRefresh()
    {
        ClearBezier();
        if(lastMethod == "casteljau") GenerateCasteljau(controlPoints);
        else if (lastMethod == "pascale") GeneratePascale(controlPoints);
        UpdateLineRenderer();
        polygonClosed = false;
        ClosePolygon();
    }



    private Vector3 DeCasteljau(List<Vector3> controlPoints, float t)
    {
        List<Vector3> intermediatePoints = new List<Vector3>(controlPoints);

        while (intermediatePoints.Count > 1)
        {
            List<Vector3> newPoints = new List<Vector3>();

            for (int i = 0; i < intermediatePoints.Count - 1; i++)
            {
                Vector3 point = Vector3.Lerp(intermediatePoints[i], intermediatePoints[i + 1], t);
                newPoints.Add(point);
            }

            intermediatePoints = newPoints;
        }

        return intermediatePoints[0];
    }

    public void GenerateCasteljau(List<Vector3> controlPoints)
    {
        float startTime = Time.realtimeSinceStartup;
        
        if (controlPoints.Count < 2)
        {
            Debug.LogWarning("Il doit y avoir au moins deux points de contrôle pour générer une courbe de Bézier.");
            return;
        }
        
        int numPoints = step;
        Vector3[] bezierPoints = new Vector3[numPoints];

        // Parcourt les valeurs de paramètre t de 0 à 1 et calcule les points sur la courbe de Bézier
        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (float)(numPoints - 1);
            Vector3 point = DeCasteljau(controlPoints, t);
            bezierPoints[i] = point;
        }
        
        GameObject casteljauCurve = new GameObject("Casteljau Curve");
        LineRenderer bezierLineRenderer = casteljauCurve.AddComponent<LineRenderer>();
        bezierLineRenderer.positionCount = numPoints;
        
        bezierLineRenderer.startWidth = 0.1f;
        bezierLineRenderer.endWidth = 0.1f;
        bezierLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        bezierLineRenderer.startColor = Color.yellow;
        bezierLineRenderer.endColor = Color.yellow;
        
        bezierLineRenderer.SetPositions(bezierPoints);
        bezierLine = casteljauCurve;

        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log("Temps de calcul : " + elapsedTime + " secondes");
    }
    
    
    public void GeneratePascale(List<Vector3> controlPoints)
    {
        float startTime = Time.realtimeSinceStartup;
        
        if (controlPoints.Count < 2)
        {
            Debug.LogWarning("Il doit y avoir au moins deux points de contrôle pour générer une courbe de Bézier.");
            return;
        }
        
        int numPoints = step;
        Vector3[] bezierPoints = new Vector3[numPoints];

        // Calcule les coefficients binomiaux à l'aide du triangle de Pascal
        int n = controlPoints.Count - 1;
        int[] coefficients = new int[n + 1];
        for (int i = 0; i <= n; i++)
        {
            coefficients[i] = CalculateBinomialCoefficient(n, i);
        }
        
        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (float)(numPoints - 1);
            
            float x = 0f;
            float y = 0f;
            float z = 0f;

            for (int j = 0; j <= n; j++)
            {
                float blend = coefficients[j] * Mathf.Pow(t, j) * Mathf.Pow(1f - t, n - j);
                x += controlPoints[j].x * blend;
                y += controlPoints[j].y * blend;
                z += controlPoints[j].z * blend;
            }

            bezierPoints[i] = new Vector3(x, y, z);
        }
        
        GameObject pascaleCurve = new GameObject("Pascale Curve");
        LineRenderer bezierLineRenderer = pascaleCurve.AddComponent<LineRenderer>();
        bezierLineRenderer.positionCount = numPoints;
        
        bezierLineRenderer.startWidth = 0.1f;
        bezierLineRenderer.endWidth = 0.1f;
        bezierLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        bezierLineRenderer.startColor = Color.blue;
        bezierLineRenderer.endColor = Color.blue;
        
        bezierLineRenderer.SetPositions(bezierPoints);
        bezierLine = pascaleCurve;
        
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log("Temps de calcul : " + elapsedTime + " secondes");
    }
    
    
    private int CalculateBinomialCoefficient(int n, int k)
    {
        if (k > n - k)
        {
            k = n - k;
        }

        int coefficient = 1;
        for (int i = 0; i < k; i++)
        {
            coefficient = coefficient * (n - i) / (i + 1);
        }

        return coefficient;
    }


    private void UpdateLineRenderer()
    {
        lineRenderer.positionCount = controlPoints.Count;
        for (int i = 0; i < controlPoints.Count; i++)
        {
            lineRenderer.SetPosition(i, controlPoints[i]);
        }
    }
    
    
    private void ClosePolygon()
    {
        if (polygonClosed)
            return;
        
        if (controlPoints.Count >= 2)
        {
            controlPoints.Add(controlPoints[0]);
            UpdateLineRenderer();
            polygonClosed = true;
            controlPoints.RemoveAt(controlPoints.Count - 1);
        }
    }


    private GameObject CreateControlPoint(Vector3 position)
    {
        GameObject controlPoint = Instantiate(controlPointPrefab, position, Quaternion.identity);
        return controlPoint;
    }
}

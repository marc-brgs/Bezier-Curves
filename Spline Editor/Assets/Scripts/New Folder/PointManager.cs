using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class PointManager : MonoBehaviour
{
    public GameObject controlPointPrefab;  // Le préfabriqué du point de contrôle
    private List<Vector3> controlPoints = new List<Vector3>();  // Liste des points de contrôle
    private bool inputEnabled = true;  // Activation/désactivation de la détection des clics
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
        }


        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearPoints();
            ClearBezier();
           // lineRenderer.positionCount = 0;
            inputEnabled = true;
            cButton.gameObject.SetActive(false);
            pButton.gameObject.SetActive(false);
            controlPoints.Clear();
            controlPointsObjects.Clear();
            UpdateLineRenderer();
            polygonClosed = false;
        }
        
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
    
    public void GenerateCasteljau(List<Vector3> controlPoints)
    {
        // Vérifie si au moins deux points de contrôle sont présents
        if (controlPoints.Count < 2)
        {
            Debug.LogWarning("Il doit y avoir au moins deux points de contrôle pour générer une courbe de Bézier.");
            return;
        }

        // Définit le nombre de points sur la courbe de Bézier (par exemple, 100 pour une courbe plus lisse)
        int numPoints = step;

        // Crée un tableau pour stocker les points de la courbe de Bézier
        Vector3[] bezierPoints = new Vector3[numPoints];

        // Parcourt les valeurs de paramètre t de 0 à 1 et calcule les points sur la courbe de Bézier
        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (float)(numPoints - 1);

            // Initialise la liste des points intermédiaires avec les points de contrôle initiaux
            List<Vector3> intermediatePoints = new List<Vector3>(controlPoints);

            // Calcule les points intermédiaires en utilisant l'algorithme de De Casteljau
            while (intermediatePoints.Count > 1)
            {
                List<Vector3> newPoints = new List<Vector3>();

                for (int j = 0; j < intermediatePoints.Count - 1; j++)
                {
                    Vector3 point = Vector3.Lerp(intermediatePoints[j], intermediatePoints[j + 1], t);
                    newPoints.Add(point);
                }

                intermediatePoints = newPoints;
            }

            // Le dernier point restant est le point de la courbe de Bézier correspondant à la valeur de t
            bezierPoints[i] = intermediatePoints[0];
        }

        // Crée un GameObject vide pour contenir les points de la courbe de Bézier
        GameObject casteljauCurve = new GameObject("Casteljau Curve");

        // Ajoute un composant LineRenderer au GameObject pour afficher la courbe de Bézier
        LineRenderer bezierLineRenderer = casteljauCurve.AddComponent<LineRenderer>();
        bezierLineRenderer.positionCount = numPoints;
        
        bezierLineRenderer.startWidth = 0.1f;
        bezierLineRenderer.endWidth = 0.1f;
        bezierLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        bezierLineRenderer.startColor = Color.yellow;
        bezierLineRenderer.endColor = Color.yellow;

        // Assigne les points de la courbe de Bézier au LineRenderer
        bezierLineRenderer.SetPositions(bezierPoints);
        bezierLine = casteljauCurve;
    }

    
    
    public void GeneratePascale(List<Vector3> controlPoints)
    {
        // Vérifie si au moins deux points de contrôle sont présents
        if (controlPoints.Count < 2)
        {
            Debug.LogWarning("Il doit y avoir au moins deux points de contrôle pour générer une courbe de Bézier.");
            return;
        }

        // Définit le nombre de points sur la courbe de Bézier (par exemple, 100 pour une courbe plus lisse)
        int numPoints = step;

        // Crée un tableau pour stocker les points de la courbe de Bézier
        Vector3[] bezierPoints = new Vector3[numPoints];

        // Calcule les coefficients binomiaux à l'aide du triangle de Pascal
        int n = controlPoints.Count - 1;
        int[] coefficients = new int[n + 1];
        for (int i = 0; i <= n; i++)
        {
            coefficients[i] = CalculateBinomialCoefficient(n, i);
        }

        // Parcourt les valeurs de paramètre t de 0 à 1 et calcule les points sur la courbe de Bézier
        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (float)(numPoints - 1);

            // Calcule les coordonnées x, y et z de la courbe de Bézier pour la valeur de t
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

        // Crée un GameObject vide pour contenir les points de la courbe de Bézier
        GameObject pascaleCurve = new GameObject("Pascale Curve");

        // Ajoute un composant LineRenderer au GameObject pour afficher la courbe de Bézier
        LineRenderer bezierLineRenderer = pascaleCurve.AddComponent<LineRenderer>();
        bezierLineRenderer.positionCount = numPoints;
        
        bezierLineRenderer.startWidth = 0.1f;
        bezierLineRenderer.endWidth = 0.1f;
        bezierLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        bezierLineRenderer.startColor = Color.blue;
        bezierLineRenderer.endColor = Color.blue;
        
        bezierLineRenderer.SetPositions(bezierPoints);
        bezierLine = pascaleCurve;
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
        // Définit le nombre de positions du LineRenderer sur la taille de la liste des points de contrôle
        lineRenderer.positionCount = controlPoints.Count;

        // Parcourt la liste des points de contrôle et assigne chaque position au LineRenderer
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
            // Ajoute le premier point de contrôle à la fin de la liste
            controlPoints.Add(controlPoints[0]);

            // Met à jour le LineRenderer avec les nouveaux points de contrôle
            UpdateLineRenderer();

            // Marque le polygone comme étant fermé
            polygonClosed = true;
            controlPoints.RemoveAt(controlPoints.Count - 1);
        }
    }


    private GameObject CreateControlPoint(Vector3 position)
    {
        // Instancie le préfabriqué du point de contrôle à la position spécifiée
        GameObject controlPoint = Instantiate(controlPointPrefab, position, Quaternion.identity);
        return controlPoint;
    }
}

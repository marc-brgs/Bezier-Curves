using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    public GameObject controlPointPrefab;  // Le préfabriqué du point de contrôle

    private List<Vector3> controlPoints = new List<Vector3>();  // Liste des points de contrôle
    private bool inputEnabled = true;  // Activation/désactivation de la détection des clics

    private LineRenderer lineRenderer;
    
    private bool polygonClosed = false;



    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        // Vérifie si la détection des clics est activée
        if (inputEnabled)
        {
            // Vérifie si le bouton gauche de la souris est enfoncé
            if (Input.GetMouseButtonDown(0))
            {
                // Obtient la position du clic dans l'espace de l'écran
                Vector3 screenPosition = Input.mousePosition;

                // Convertit la position du clic de l'écran à la position dans le monde
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 10f));

                // Ajoute le point de contrôle à la liste
                controlPoints.Add(worldPosition);

                // Crée le point de contrôle à la position cliquée
                CreateControlPoint(worldPosition);
                
                UpdateLineRenderer();
            }

            // Vérifie si le bouton droit de la souris est enfoncé
            if (Input.GetMouseButtonDown(1))
            {
                // Termine la séquence de positionnement en désactivant la détection des clics
                inputEnabled = false;
                ClosePolygon();
                
                // Génère la courbe de Bézier associée aux points de contrôle
                GenerateBezierCurve(controlPoints);
            }
        }
    }
    
    
    
    
    private void GenerateBezierCurve(List<Vector3> controlPoints)
{
    // Vérifie si au moins deux points de contrôle sont présents
    if (controlPoints.Count < 2)
    {
        Debug.LogWarning("Il doit y avoir au moins deux points de contrôle pour générer une courbe de Bézier.");
        return;
    }

    // Définit le nombre de points sur la courbe de Bézier (par exemple, 100 pour une courbe plus lisse)
    int numPoints = 100;

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
    GameObject bezierCurve = new GameObject("Bezier Curve");

    // Ajoute un composant LineRenderer au GameObject pour afficher la courbe de Bézier
    LineRenderer bezierLineRenderer = bezierCurve.AddComponent<LineRenderer>();
    bezierLineRenderer.positionCount = numPoints;
    
    bezierLineRenderer.startWidth = 0.1f;
    bezierLineRenderer.endWidth = 0.1f;
    bezierLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    bezierLineRenderer.startColor = Color.blue;
    bezierLineRenderer.endColor = Color.blue;

    // Assigne les points de la courbe de Bézier au LineRenderer
    bezierLineRenderer.SetPositions(bezierPoints);
}

    
    
    
    
    
    
    
    
    
    /*
    
    private void GenerateBezierCurve(List<Vector3> controlPoints)
    {
        // Vérifie si au moins deux points de contrôle sont présents
        if (controlPoints.Count < 2)
        {
            Debug.LogWarning("Il doit y avoir au moins deux points de contrôle pour générer une courbe de Bézier.");
            return;
        }

        // Définit le nombre de points sur la courbe de Bézier (par exemple, 100 pour une courbe plus lisse)
        int numPoints = 100;

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
        GameObject bezierCurve = new GameObject("Bezier Curve");

        // Ajoute un composant LineRenderer au GameObject pour afficher la courbe de Bézier
        LineRenderer bezierLineRenderer = bezierCurve.AddComponent<LineRenderer>();
        bezierLineRenderer.positionCount = numPoints;
        
        bezierLineRenderer.startWidth = 0.1f;
        bezierLineRenderer.endWidth = 0.1f;
        bezierLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        bezierLineRenderer.startColor = Color.blue;
        bezierLineRenderer.endColor = Color.blue;
        
        bezierLineRenderer.SetPositions(bezierPoints);
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

    
    */

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
        // Vérifie si le polygone est déjà fermé
        if (polygonClosed)
            return;

        // Vérifie si au moins deux points de contrôle sont présents
        if (controlPoints.Count >= 2)
        {
            // Ajoute le premier point de contrôle à la fin de la liste
            controlPoints.Add(controlPoints[0]);

            // Met à jour le LineRenderer avec les nouveaux points de contrôle
            UpdateLineRenderer();

            // Marque le polygone comme étant fermé
            polygonClosed = true;
        }
    }


    private void CreateControlPoint(Vector3 position)
    {
        // Instancie le préfabriqué du point de contrôle à la position spécifiée
        GameObject controlPoint = Instantiate(controlPointPrefab, position, Quaternion.identity);
    }
}

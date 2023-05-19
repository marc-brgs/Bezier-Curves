# Spline Editor

Créer, d’éditer et d’utiliser des courbes paramétriques en utilisant la technique des courbes Splines.

## Démarrage

L'architecture du projet est :
```
.
└───Screenshots
└───Spline Editor (Unity Project)
```

> Le __projet Unity__, lancez le projet à l'aide du `Unity Hub`. Une fois le projet lancé, il suffit d'appuyer sur play pour tester le projet via l'éditeur.  

Les spécifications du projet Unity sont :  
* __Version du moteur:__ Unity 2020.3.4f1  

### Démarrage du projet Unity
Pour avoir un aperçu du projet, il faut lancer la scène `Assets/Scenes/SplineScene.unity`.

## Fonctionnalité

Les splines sont dans le path `Assets/Scripts/Splines/`. Ils ont été rangés suivant l'architecture des scripts, cette architecture a été pensée pour être flexible et pour éviter le doublon de code (exemple : deux mêmes méthodes mais avec des valeurs différentes).  

On a alors:  
* CurveSpline (limité à 4 points)
  * BezierSpline
  * HermiteSpline
* JonctionSplines
  * BezierJonctionSpline
  * HermiteJonctionSpline
* ContinuitySplines
  * BSpline
  * CatmullRomSpline
* SurfaceSpline
  * BezierSurfaceSpline
  * BSplineSurfaceSpline
  * CatmullSurfaceSpline  


Chaque spline hérite de `Spline.cs` qui donne accès à différentes méthodes : `GetPointAt(float t)`, `GetVelocityAt(float t)`, `GetDirectionAt(float t)`.  
`GetVelocityAt` retourne la dérivée du calcul dans `GetPointAt` ; `GetDirectionAt` renvoie `GetVelocityAt` en "normalisé". 

> Note: La partie éditeur se trouve dans les dossiers `Editor` et permet l'édition en temps-réel des points des splines dans la fenêtre `Scene`.  

### Screenshots des fonctionnalités

![Alt text](\Screenshots\Splines.png "Bezier Jonction Spline Edition")
![Alt text](\Screenshots\BezierJonctionSplineEdition.gif "Bezier Jonction Spline Edition")
![Alt text](\Screenshots\AddPointsExample.gif "Add Points with example")
![Alt text](\Screenshots\ExampleFollowSpline.gif "Example Follow Spline")


## Auteur
Chateigner Mathieu
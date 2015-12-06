using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class ProceduralCreate : MonoBehaviour 
{
   // public InputField radiusField;
   // public InputField probaField;
    public int nbSalles = 20; // Nombre de salles valides à générer
	public float proba = 20f; // Probabilité de générer une salle

    //public Camera mainCamera;

    //int textValue;

    // Dimensions d'une salle
    public float roomW;
	public float roomH;

	////////// Tableaux utilisés pour la génération procédurale //////////

	public int[,] dungeonMap; // Tableau contenant toutes les salles du donjon en mémoire.
	public int[,] tampon; // Tableau qui contient toutes les salles générées via leurs coordonnées, qui est lu par l'algo pour générer de nouvelles salles
	int tamponIndex; // Définit la position de l'algorithme dans le tableau tampon, qui contient les coordonnées de toutes les cases générées.
	int index = 1; // Définit la position du curseur lors de la génération de nouvelles cases.

	public int[,] tamponGenerate;
	int tamponGenerateIndex; // Définit la quantité de salles valides adjacentes à la case en cours de traitement

    //////////////////////////////////////////////////////////////////////

	GameObject dungeon;

	Vector3 genPosition; // Position à laquelle sera générée chaque nouvelle salle, en fonction de sa position dans dungeonMap

	int sallesAdjacentes; // Contient l'indice de position des salles adjacentes à la salle en cours de traitement.  

	int actualCoordX;
	int actualCoordY;

	int rand; // Compris entre nbMin et nbMax
	int nbMin; // Check si on peut encore générer des cases
	int nbMax; // Pour ne pas dépasser le nombre maximum de cases
	int nbSallesGen = 0; // Nombre de salles générées par l'algorithme au total
	int nbCasesGen; // Nombre de cases générées par l'algorithme à partir de la case en cours de traitement
	int loopIndex; // Index utilisé pour la boucle de génération des salles à partir de la case en cours de traitement
	string roomName; // Nom de la case en sortie


	// Fonction de création du donjon
	void DungeonSpawn () 
	{
		Destroy (GameObject.Find("Donjon"));
		dungeon = new GameObject();
		dungeon.name = "Donjon";

		// La première salle est générée au milieu du tableau
		actualCoordX = nbSalles;
		actualCoordY = nbSalles;
		tamponIndex = 0;
		index = 1;
		nbSallesGen = 0;

		// On initialise la taille des deux tableaux 
		dungeonMap = new int[nbSalles*3,nbSalles*3];
		tampon = new int[nbSalles+1,2];

		// On génère la salle initiale
		dungeonMap[actualCoordY,actualCoordX] = 1;
		tampon[tamponIndex,0] = actualCoordY;
		tampon[tamponIndex,1] = actualCoordX;


		for (tamponIndex = 0; tamponIndex < nbSalles+1; tamponIndex++)
		{
			//Réinitialisation du nombre de cases générées
			nbCasesGen = 0;

			// Réinitialisation du tableau tampon utilisé pour la détection des salles adjacentes
			tamponGenerate = new int[4,3];
			tamponGenerateIndex = 0;

			// On stocke les coordonnées de la case en cours de traitement
			actualCoordY = tampon [tamponIndex,0];
			actualCoordX = tampon [tamponIndex,1];

			// Reset l'indice de cases adjacentes
			sallesAdjacentes = 0;

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////// Vérification des salles adjacentes ///////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // On check chaque case libre et on retourne nbSallesAdjacentes                                                                                 ////
            // On store également les coordonnées de chaque case vérifiée et valide ainsi qu'un indice de direction pour chaque salle adjacente :           ////
            // 1000 : Nord                                                                                                                                  ////
            // 100 : Est                                                                                                                                    ////
            // 10 : Sud                                                                                                                                     ////
            // 1 : Ouest                                                                                                                                    ////
            // Le résultat, une fois lu après traitement, permet de savoir où sont les salles adjacentes qui ont été créées                                 ////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            ////////// Check la salle au Nord //////////

            if (dungeonMap[actualCoordY +1, actualCoordX] == 0)
			{
				// On store les coordonnées de la case valide dans tamponGenerate
				tamponGenerate [tamponGenerateIndex, 0] = actualCoordY +1;
				tamponGenerate [tamponGenerateIndex, 1] = actualCoordX;
				// On ajoute 1000 à la troisième case, qui sert d'indice de position de la salle adjacente.
				tamponGenerate [tamponGenerateIndex, 2] = 1000;

				tamponGenerateIndex ++;
			}
			// Si la case vérifiée est déjà occupée par une salle valide, on augmente la variable sallesAdjacentes, qui permet de stocker la position des salles adjacentes
			else if (dungeonMap[actualCoordY +1 , actualCoordX] >= 1) 
			{
				sallesAdjacentes += 1000;
			}

			////////// Check la salle à l'Est //////////
		
			if (dungeonMap[actualCoordY, actualCoordX +1] == 0)
			{
				tamponGenerate [tamponGenerateIndex, 0] = actualCoordY;
				tamponGenerate [tamponGenerateIndex, 1] = actualCoordX +1;
				tamponGenerate [tamponGenerateIndex, 2] = 100;

				tamponGenerateIndex ++;
			}
			else if (dungeonMap[actualCoordY, actualCoordX +1] >= 1) 
			{
				sallesAdjacentes += 100;
			}

			////////// Check la salle au Sud //////////

			if (dungeonMap[actualCoordY -1 , actualCoordX] == 0) 
			{
				tamponGenerate [tamponGenerateIndex, 0] = actualCoordY - 1;
				tamponGenerate [tamponGenerateIndex, 1] = actualCoordX;
				tamponGenerate [tamponGenerateIndex, 2] = 10;

				tamponGenerateIndex ++;
			}
			else if (dungeonMap[actualCoordY -1, actualCoordX] >= 1) 
			{
				sallesAdjacentes += 10;
			}

			////////// Check la salle à l'Ouest //////////

			if (dungeonMap[actualCoordY, actualCoordX -1] == 0)
			{
				tamponGenerate [tamponGenerateIndex, 0] = actualCoordY;
				tamponGenerate [tamponGenerateIndex, 1] = actualCoordX -1;
				tamponGenerate [tamponGenerateIndex, 2] = 1;

				tamponGenerateIndex ++;
			}
			else if (dungeonMap[actualCoordY, actualCoordX -1] >= 1) 
			{
				sallesAdjacentes += 1;
			}

            if (index <= nbSalles)
            {
                // On cherche ici à faire en sorte que l'on soit forcé de générer au moins une salle s'il n'y en a pas d'autre au-dessus dans le tableau tampon.
                // Cela permet d'éviter que l'algorithme se termine avant que toutes les salles ne soient générées.

                if (tampon[tamponIndex + 1, 0] != 0 || nbSalles - nbSallesGen == 0) nbMin = 0;
                else nbMin = 1;

                // On fait en sorte qu'on ne puisse pas créer trop de salles

                if (nbSalles - nbSallesGen < 4) nbMax = nbSalles - nbSallesGen;
                else nbMax = 4;

                // On génère un entier selon le nombre de cases disponibles.
                rand = Random.Range(nbMin, nbMax);
                if (rand > tamponGenerateIndex) rand = tamponGenerateIndex;

                // On tente de créer une salle sur une chaque position autour de la case initiale, en sens horaire.
                for (loopIndex = 0; loopIndex < tamponGenerateIndex; loopIndex++)
                {
                    RandomGenerate(tamponGenerate[loopIndex, 0], tamponGenerate[loopIndex, 1], tamponGenerate[loopIndex, 2]);
                }
                while (index < nbSalles && tampon[index, 0] != 0 && tampon[index, 0] != -1)
                {
                    index++;
                }
            }
            // On attribue à la case en cours de traitement l'indice de cases adjacentes
            dungeonMap[actualCoordY, actualCoordX] = sallesAdjacentes;
        }
		ProceduralGenerate ();
	}

	void RandomGenerate (int posY, int posX, int direction)
	{
		if (dungeonMap [posY, posX] == 0 )
		{
			// On fait en sorte de ne pas générer trop de salles
			if ( rand - nbCasesGen == tamponGenerateIndex - loopIndex )
			{
				dungeonMap [posY, posX] = 1;
				sallesAdjacentes += direction;
				
				tampon [ index,0 ] = posY ;
				tampon [ index,1 ] = posX ;

				index++;
				nbSallesGen++;
				nbCasesGen++;
			}
			else 
			{
				float probaCheck;
				probaCheck = Random.value;
				
				if (proba >= probaCheck && nbSallesGen < nbSalles) 
				{
					dungeonMap [posY, posX] = 1;
					sallesAdjacentes += direction;
										
					tampon [ index,0 ] = posY ;
					tampon [ index,1 ] = posX ;

					index++;
					nbSallesGen++;
					nbCasesGen++;
				}
				else dungeonMap [posY,posX] = -1;
			}
		}
	}

	void ProceduralGenerate()
	{
		// On génère les salles valides à partir du tableau tampon
		for ( int xy = 0; xy < nbSalles+1; xy++)
		{
			// Si la case est valide, on génère une salle ( Le contraire ne devrait jamais arriver ici, cela sert surtout à debug )
			if ( dungeonMap [tampon [xy,0], tampon[xy,1]] > 0 )
			{
				// Calcul de la position de la salle à partir des coordonnées de celle-ci
				genPosition = new Vector3 (tampon[xy,1] * roomW - ( nbSalles * roomW ),0,tampon[xy,0] * roomH - ( nbSalles * roomH ));

                string roomID; // Récupère l'indice de salles adjacentes de la salle à générer
				int randomPick; // Permet de choisir une salle aléatoire dans une catégorie donnée

				roomID = dungeonMap[tampon [xy,0], tampon[xy,1]].ToString();
				roomID = string.Concat ("Rooms/",roomID);

                // On récupère la quantité de fichiers .prefab dans le dossier correspondant
                DirectoryInfo dir = new DirectoryInfo(string.Concat("Assets/Resources/", roomID));
                FileInfo[] info = dir.GetFiles("*.prefab");
                int nbFichiers = info.Length;

                // On génère ensuite un entier aléatoire qui correspondra à la salle à générer
                if (nbFichiers > 1)randomPick = Random.Range(1, nbFichiers+1);
				else randomPick = 1;

                // Ici on génère un string qui correspondra au chemin ou se trouvent les salles
                roomID = string.Concat (roomID,"/");
				roomID = string.Concat (roomID,randomPick.ToString());

                // On charge ensuite une salle à partir du chemin et de l'entier spécifiés
				GameObject g = (GameObject)Instantiate ( Resources.Load (roomID) as GameObject, genPosition, this.transform.rotation );
                roomName = string.Concat ( dungeonMap [tampon [xy,0], tampon[xy,1]].ToString(), " Salle Valide numéro " , xy.ToString());
				g.name = roomName;
				g.transform.parent = dungeon.transform;
			}
		}
	}

    // Update is called once per frame
    void Update()
    {
		/*
        if (Input.GetButtonDown("Jump"))
        {
            //mainCamera.orthographicSize = 100f / ((50f / (float)nbSalles));

            textValue = int.Parse(radiusField.text);
            nbSalles = textValue;

            proba = float.Parse(probaField.text) / 100;

            DungeonSpawn();
        }
		*/
    }
    void Start()
    {
		/*
        radiusField.text = nbSalles.ToString();
        probaField.text = (proba*100).ToString();
		*/
		DungeonSpawn();
		
		
    }


}

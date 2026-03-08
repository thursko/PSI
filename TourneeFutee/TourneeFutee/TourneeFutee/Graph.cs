namespace TourneeFutee
{
    public class Graph
    {

        // TODO : ajouter tous les attributs que vous jugerez pertinents 
        private Matrix matAdj;
        private bool isDirected;
        private float noEdgeValue;
        private Dictionary<string, int> nameToIndex;
        private List<string> vertexNames;
        private List<float> vertexValues;
        // --- Construction du graphe ---

        // Contruit un graphe (`directed`=true => orienté)
        // La valeur `noEdgeValue` est le poids modélisant l'absence d'un arc (0 par défaut)
        public Graph(bool directed, float noEdgeValue = 0)
        {
            // TODO : implémenter
            this.isDirected = directed;
            this.noEdgeValue = noEdgeValue;

            this.matAdj = new Matrix(0, 0, noEdgeValue);

            this.nameToIndex = new Dictionary<string, int>();
            this.vertexNames = new List<string>();
            this.vertexValues = new List<float>();
        }


        // --- Propriétés ---

        // Propriété : ordre du graphe
        // Lecture seule
        public int Order
        {
            get { return this.matAdj.NbRows; } // TODO : implémenter
                    // pas de set
        }

        // Propriété : graphe orienté ou non
        // Lecture seule
        public bool Directed
        {
            get { return this.isDirected; }   // TODO : implémenter
                    // pas de set
        }


        // --- Gestion des sommets ---

        // Ajoute le sommet de nom `name` et de valeur `value` (0 par défaut) dans le graphe
        // Lève une ArgumentException s'il existe déjà un sommet avec le même nom dans le graphe
        public void AddVertex(string name, float value = 0)
        {
            // TODO : implémenter
            if (nameToIndex.ContainsKey(name))
            {
                throw new ArgumentException("Un sommet nommé "+name+" existe déjà.");
            }

            int newIndex = this.Order;
            matAdj.AddRow(newIndex);
            matAdj.AddColumn(newIndex);

            nameToIndex.Add(name, newIndex);
            vertexNames.Add(name);
            vertexValues.Add(value);
        }


        // Supprime le sommet de nom `name` du graphe (et tous les arcs associés)
        // Lève une ArgumentException si le sommet n'a pas été trouvé dans le graphe
        public void RemoveVertex(string name)
        {
            if (!nameToIndex.ContainsKey(name))
            {
                throw new ArgumentException("Le sommet "+name+" est introuvable.");
            }
            int indexToRemove = nameToIndex[name];
            matAdj.RemoveRow(indexToRemove);
            matAdj.RemoveColumn(indexToRemove);
            vertexNames.RemoveAt(indexToRemove);
            vertexValues.RemoveAt(indexToRemove);
            nameToIndex.Remove(name);
            for (int i = indexToRemove; i < vertexNames.Count; i++)
            {
                nameToIndex[vertexNames[i]] = i;
            }
        }

        // Renvoie la valeur du sommet de nom `name`
        // Lève une ArgumentException si le sommet n'a pas été trouvé dans le graphe
        public float GetVertexValue(string name)
        {
            if (!nameToIndex.ContainsKey(name))
            {
                throw new ArgumentException("Le sommet " + name + " est introuvable.");
            }
            int index = nameToIndex[name];
            return vertexValues[index];
        }

        // Affecte la valeur du sommet de nom `name` à `value`
        // Lève une ArgumentException si le sommet n'a pas été trouvé dans le graphe
        public void SetVertexValue(string name, float value)
        {
            if (!nameToIndex.ContainsKey(name))
            {
                throw new ArgumentException("Le sommet " + name + " est introuvable.");
            }
            int index = nameToIndex[name];
            vertexValues[index] = value;
        }


        // Renvoie la liste des noms des voisins du sommet de nom `vertexName`
        // (si ce sommet n'a pas de voisins, la liste sera vide)
        // Lève une ArgumentException si le sommet n'a pas été trouvé dans le graphe
        public List<string> GetNeighbors(string vertexName)
        {
            if (!nameToIndex.ContainsKey(vertexName))
            {
                throw new ArgumentException("Le sommet "+vertexName+" n'existe pas dans le graphe.");
            }

            List<string> neighborNames = new List<string>();

            int rowIndex = nameToIndex[vertexName];
            int nbColumns = matAdj.NbColumns;
            for (int j = 0; j < nbColumns; j++)
            {
                float weight = matAdj.GetValue(rowIndex, j);

                if (weight != noEdgeValue)
                {
                    neighborNames.Add(vertexNames[j]);
                }
            }
            return neighborNames;
        }

        // --- Gestion des arcs ---

        /* Ajoute un arc allant du sommet nommé `sourceName` au sommet nommé `destinationName`, avec le poids `weight` (1 par défaut)
         * Si le graphe n'est pas orienté, ajoute aussi l'arc inverse, avec le même poids
         * Lève une ArgumentException dans les cas suivants :
         * - un des sommets n'a pas été trouvé dans le graphe (source et/ou destination)
         * - il existe déjà un arc avec ces extrémités
         */
        public void AddEdge(string sourceName, string destinationName, float weight = 1)
        {
            if (!nameToIndex.ContainsKey(sourceName))
            {
                throw new ArgumentException("Le sommet " + sourceName + " n'existe pas dans le graphe.");
            }
            if (!nameToIndex.ContainsKey(destinationName))
            {
                throw new ArgumentException("Le sommet " + destinationName + " n'existe pas dans le graphe.");
            }
            int i = nameToIndex[sourceName];
            int j = nameToIndex[destinationName];
            if (matAdj.GetValue(i, j) != noEdgeValue)
            {
                throw new ArgumentException("Un arc existe déjà entre "+sourceName+" et "+destinationName+".");
            }
            matAdj.SetValue(i, j, weight);
            if(!this.isDirected)
            {
                matAdj.SetValue(j,i,weight);
            }
        }

        /* Supprime l'arc allant du sommet nommé `sourceName` au sommet nommé `destinationName` du graphe
         * Si le graphe n'est pas orienté, supprime aussi l'arc inverse
         * Lève une ArgumentException dans les cas suivants :
         * - un des sommets n'a pas été trouvé dans le graphe (source et/ou destination)
         * - l'arc n'existe pas
         */
        public void RemoveEdge(string sourceName, string destinationName)
        {
            if (!nameToIndex.ContainsKey(sourceName))
            {
                throw new ArgumentException("Le sommet " + sourceName + " n'existe pas dans le graphe.");
            }
            if (!nameToIndex.ContainsKey(destinationName))
            {
                throw new ArgumentException("Le sommet " + destinationName + " n'existe pas dans le graphe.");
            }
            int i = nameToIndex[sourceName];
            int j = nameToIndex[destinationName];
            if (matAdj.GetValue(i, j) == noEdgeValue)
            {
                throw new ArgumentException("Il n'y a pas d'arc existant entre "+sourceName+" et "+destinationName+".");
            }
            matAdj.SetValue(i, j, noEdgeValue);
            if (!this.isDirected)
            {
                matAdj.SetValue(j, i, noEdgeValue);
            }
            // TODO : implémenter
        }

        /* Renvoie le poids de l'arc allant du sommet nommé `sourceName` au sommet nommé `destinationName`
         * Si le graphe n'est pas orienté, GetEdgeWeight(A, B) = GetEdgeWeight(B, A) 
         * Lève une ArgumentException dans les cas suivants :
         * - un des sommets n'a pas été trouvé dans le graphe (source et/ou destination)
         * - l'arc n'existe pas
         */
        public float GetEdgeWeight(string sourceName, string destinationName)
        {
            // TODO : implémenter
            int i = vertexNames.IndexOf(sourceName);
            int j = vertexNames.IndexOf(destinationName);

            if (i < 0 || j < 0)
                throw new ArgumentException();

            float weight = matAdj.GetValue(i, j);

            if (weight == noEdgeValue)
                throw new ArgumentException();

            return weight;
        }

        /* Affecte le poids l'arc allant du sommet nommé `sourceName` au sommet nommé `destinationName` à `weight` 
         * Si le graphe n'est pas orienté, affecte le même poids à l'arc inverse
         * Lève une ArgumentException si un des sommets n'a pas été trouvé dans le graphe (source et/ou destination)
         */
        public void SetEdgeWeight(string sourceName, string destinationName, float weight)
        {
            // TODO : implémenter
            int i = vertexNames.IndexOf(sourceName);
            int j = vertexNames.IndexOf(destinationName);

            if (i < 0 || j < 0)
                throw new ArgumentException();

            matAdj.SetValue(i, j, weight);

            if (!isDirected)
                matAdj.SetValue(j, i, weight);
        }

        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 

    }


}

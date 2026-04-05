namespace TourneeFutee
{
    // Résout le problème de voyageur de commerce défini par le graphe `graph`
    // en utilisant l'algorithme de Little
    public class Little
    {
        // TODO : ajouter tous les attributs que vous jugerez pertinents 
        private Graph graphe;
        // Instancie le planificateur en spécifiant le graphe modélisant un problème de voyageur de commerce
        public Little(Graph graph)
        {
            this.graphe = graph;
            // TODO : implémenter
        }

        // Trouve la tournée optimale dans le graphe `this.graph`
        // (c'est à dire le cycle hamiltonien de plus faible coût)
        public Tour ComputeOptimalTour()
        {
            // TODO : implémenter
            int n = graphe.Order;
            var vertices = graphe.GetVertices();

            Matrix m = new Matrix(n, n, float.MaxValue);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                    {
                        m.SetValue(i, j, float.MaxValue);
                    }
                    else
                    {
                        try
                        {
                            m.SetValue(i, j, graphe.GetEdgeWeight(vertices[i], vertices[j]));
                        }
                        catch
                        {
                            m.SetValue(i, j, float.MaxValue);
                        }
                    }
                }
            }
            ReduceMatrix(m);

            Tour tour = new Tour();
            List<(string source, string destination)> chosenSegments = new List<(string, string)>();

            while (chosenSegments.Count < n)
            {
                var (i, j, _) = GetMaxRegret(m);

                string src = vertices[i];
                string dst = vertices[j];
                if (IsForbiddenSegment((src, dst), chosenSegments, n))
                {
                    m.SetValue(i, j, float.MaxValue);
                    continue;
                }
                float cost = graphe.GetEdgeWeight(src, dst);
                tour.AddSegment(src, dst, cost);
                chosenSegments.Add((src, dst));
                for (int k = 0; k < n; k++)
                {
                    m.SetValue(i, k, float.MaxValue);
                    m.SetValue(k, j, float.MaxValue);
                }
                m.SetValue(j, i, float.MaxValue);
                ReduceMatrix(m);
            }

            return tour;
        }

        // --- Méthodes utilitaires réalisant des étapes de l'algorithme de Little


        // Réduit la matrice `m` et revoie la valeur totale de la réduction
        // Après appel à cette méthode, la matrice `m` est *modifiée*.
        public static float ReduceMatrix(Matrix m)
        {
            // TODO : implémenter
            float MinLigne = 0;
            float MinColonne = 0;
            float somme = 0;
            for(int i=0;i<m.NbRows; i++)
            {
                MinLigne = float.MaxValue;
                for (int j=0;j<m.NbColumns;j++)
                {
                    float val = m.GetValue(i, j);
                    if (val < MinLigne)
                    {
                        MinLigne = val;
                    }
                }
                if (MinLigne == float.MaxValue || MinLigne == 0)
                {
                    continue;
                }              
                somme += MinLigne;
                for(int a=0;a<m.NbColumns;a++)
                {
                    m.SetValue(i, a, m.GetValue(i, a) - MinLigne);
    
                }
            }
            for (int k = 0; k < m.NbColumns; k++)
            {
                MinColonne = float.MaxValue;
                for (int l = 0; l < m.NbRows; l++)
                {
                    float val = m.GetValue(l, k);
                    if (val < MinColonne)
                    {
                        MinColonne = val;
                    }
                }
                if (MinColonne == float.MaxValue || MinColonne == 0)
                {
                    continue;
                }
                somme += MinColonne;
                for(int b=0;b<m.NbRows;b++)
                {
                    m.SetValue(b, k, m.GetValue(b, k) - MinColonne);                    
                }
            }
            return somme;
        }

        // Renvoie le regret de valeur maximale dans la matrice de coûts `m` sous la forme d'un tuple `(int i, int j, float value)`
        // où `i`, `j`, et `value` contiennent respectivement la ligne, la colonne et la valeur du regret maximale
        public static (int i, int j, float value) GetMaxRegret(Matrix m)
        {
            // TODO : implémenter
            int bestI = -1;
            int bestJ = -1;
            float maxRegret = float.MinValue;
            for(int i=0; i<m.NbRows; i++)
            {
                for(int j=0; j<m.NbColumns;j++)
                {
                    if (m.GetValue(i,j)==0)
                    {
                        float minRow = float.MaxValue;
                        float minCol = float.MaxValue;
                        for (int k = 0; k < m.NbColumns; k++)
                        {
                            if (k != j && m.GetValue(i,k) < minRow)
                            {
                                minRow = m.GetValue(i,k);
                            }
                        }
                        for (int k = 0; k < m.NbRows; k++)
                        {
                            if (k != i && m.GetValue(k,j)< minCol)
                            {
                                minCol = m.GetValue(k,j);
                            }
                        }

                        float regret = minRow + minCol;

                        if (regret > maxRegret)
                        {
                            maxRegret = regret;
                            bestI = i;
                            bestJ = j;
                        }
                    }
                }
            }
            return (bestI, bestJ, maxRegret);


        }

        /* Renvoie vrai si le segment `segment` est un trajet parasite, c'est-à-dire s'il ferme prématurément la tournée incluant les trajets contenus dans `includedSegments`
         * Une tournée est incomplète si elle visite un nombre de villes inférieur à `nbCities`
         */
        public static bool IsForbiddenSegment((string source, string destination) segment, List<(string source, string destination)> includedSegments, int nbCities)
        {

            Dictionary<string, string> successeurs = new Dictionary<string, string>();
            foreach (var seg in includedSegments)
            {
                successeurs[seg.source] = seg.destination;
            }
            int longueurChaine = 1; 
            string courant = segment.destination;
            while (successeurs.ContainsKey(courant))
            {
                courant = successeurs[courant];
                longueurChaine++;

                if (courant == segment.source)
                {
                    return longueurChaine < nbCities;
                }

                if (longueurChaine > nbCities)
                    break;
            }
            return false;
        }

        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 

    }
}

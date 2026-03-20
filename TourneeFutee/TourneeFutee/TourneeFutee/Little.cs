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
            return new Tour();
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
                if(i==0)
                {
                    MinLigne = m.GetValue(i, 1);
                }
                else
                {
                    MinLigne = m.GetValue(i, 0);
                }
                for (int j=0;j<m.NbColumns;j++)
                {
                    if(j!=i)
                    {
                        if (MinLigne > m.GetValue(i, j))
                        {
                            MinLigne = m.GetValue(i, j);
                        }
                    }
                    
                }
                somme += MinLigne;
                for(int a=0;a<m.NbColumns;a++)
                {
                    if(a!=i)
                    {
                        m.SetValue(i, a, m.GetValue(i, a) - MinLigne);
                    }
                }
            }
            for (int k = 0; k < m.NbColumns; k++)
            {
                if(k==0)
                {
                    MinColonne = m.GetValue(1, k);
                }
                else
                {
                    MinColonne = m.GetValue(0, k);
                }
                for (int l = 0; l < m.NbRows; l++)
                {
                    if(l!=k)
                    {
                        if (MinColonne > m.GetValue(l, k))
                        {
                            MinColonne = m.GetValue(l, k);
                        }
                    }
                }
                somme += MinColonne;
                for(int b=0;b<m.NbRows;b++)
                {
                    if(k!=b)
                    {
                        m.SetValue(b, k, m.GetValue(b, k) - MinColonne);
                    }
                }
            }
            return somme;
        }

        // Renvoie le regret de valeur maximale dans la matrice de coûts `m` sous la forme d'un tuple `(int i, int j, float value)`
        // où `i`, `j`, et `value` contiennent respectivement la ligne, la colonne et la valeur du regret maximale
        public static (int i, int j, float value) GetMaxRegret(Matrix m)
        {
            // TODO : implémenter
            

        }

        /* Renvoie vrai si le segment `segment` est un trajet parasite, c'est-à-dire s'il ferme prématurément la tournée incluant les trajets contenus dans `includedSegments`
         * Une tournée est incomplète si elle visite un nombre de villes inférieur à `nbCities`
         */
        public static bool IsForbiddenSegment((string source, string destination) segment, List<(string source, string destination)> includedSegments, int nbCities)
        {

            // TODO : implémenter
            return false;   
        }

        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 

    }
}

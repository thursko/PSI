namespace TourneeFutee
{
    public class Matrix
    {
        // TODO : ajouter tous les attributs que vous jugerez pertinents 
        public List<List<float>> valeurs;
        private float defaultValue;

        /* Crée une matrice de dimensions `nbRows` x `nbColums`.
         * Toutes les cases de cette matrice sont remplies avec `defaultValue`.
         * Lève une ArgumentOutOfRangeException si une des dimensions est négative
         */
        public Matrix(int nbRows = 0, int nbColumns = 0, float defaultValue = 0)
        {
            // TODO : implémenter
            if (nbRows < 0)
            {
                throw new ArgumentException(nameof(nbRows),"Nombre de lignes négatif");
            }
            if(nbColumns < 0)
            {
                throw new ArgumentException(nameof(nbColumns), "Nombre de colonnes négatif");
            }
            this.valeurs = new List<List<float>>();
            this.defaultValue = defaultValue;
            for (int i = 0; i < nbRows; i++)
            {
                for (int j = 0; j < nbColumns; j++)
                {
                    this.valeurs[i].Add(this.defaultValue);
                }
            }
        }

        // Propriété : valeur par défaut utilisée pour remplir les nouvelles cases
        // Lecture seule
        public float DefaultValue
        {
            get { return this.DefaultValue; } // TODO : implémenter
                 // pas de set
        }

        // Propriété : nombre de lignes
        // Lecture seule
        public int NbRows
        {
            get { return this.valeurs.Count(); } // TODO : implémenter
                 // pas de set
        }

        // Propriété : nombre de colonnes
        // Lecture seule
        public int NbColumns
        {
            get { return this.valeurs[0].Count(); } // TODO : implémenter
                 // pas de set
        }

        /* Insère une ligne à l'indice `i`. Décale les lignes suivantes vers le bas.
         * Toutes les cases de la nouvelle ligne contiennent DefaultValue.
         * Si `i` = NbRows, insère une ligne en fin de matrice
         * Lève une ArgumentOutOfRangeException si `i` est en dehors des indices valides
         */
        public void AddRow(int i)
        {
            if
            List<float> list = new List<float>();
            for(int j=0;j<this.NbRows+1;j++)
            {
                list.Add(this.defaultValue);
            }
            this.valeurs.Insert(i,list);
            // TODO : implémenter
        }

        /* Insère une colonne à l'indice `j`. Décale les colonnes suivantes vers la droite.
         * Toutes les cases de la nouvelle ligne contiennent DefaultValue.
         * Si `j` = NbColums, insère une colonne en fin de matrice
         * Lève une ArgumentOutOfRangeException si `j` est en dehors des indices valides
         */
        public void AddColumn(int j)
        {
            // TODO : implémenter
            for(int i=0;i<this.NbRows;i++)
            {
                this.valeurs[i].Insert(j,this.defaultValue);
            }
        }

        // Supprime la ligne à l'indice `i`. Décale les lignes suivantes vers le haut.
        // Lève une ArgumentOutOfRangeException si `i` est en dehors des indices valides
        public void RemoveRow(int i)
        {
            // TODO : implémenter
            this.valeurs.RemoveAt(i);
        }

        // Supprime la colonne à l'indice `j`. Décale les colonnes suivantes vers la gauche.
        // Lève une ArgumentOutOfRangeException si `j` est en dehors des indices valides
        public void RemoveColumn(int j)
        {
            for(int i=0;i<this.NbRows;i++)
            {
                this.valeurs[i].RemoveAt(j);
            }
            // TODO : implémenter
        }

        // Renvoie la valeur à la ligne `i` et colonne `j`
        // Lève une ArgumentOutOfRangeException si `i` ou `j` est en dehors des indices valides
        public float GetValue(int i, int j)
        {
            // TODO : implémenter
            if(i < 0 || j < 0 || i >= this.NbRows || j >= this.NbColumns)
            {
                Console.WriteLine("Getvalue; indices outofrange");
                return 0;
            }
            else
            {
                return this.valeurs[i][j];
            }
        }

        // Affecte la valeur à la ligne `i` et colonne `j` à `v`
        // Lève une ArgumentOutOfRangeException si `i` ou `j` est en dehors des indices valides
        public void SetValue(int i, int j, float v)
        {
            // TODO : implémenter
            if (i < 0 || j < 0 || i >= this.NbRows || j >= this.NbColumns)
            {
                Console.WriteLine("SetValue: indices outofrange");
            }
            else
            {
                this.valeurs[i][j] = v;
            }

        }

        // Affiche la matrice
        public void Print()
        {
            // TODO : implémenter
            if(valeurs!=null&&valeurs.Count>0)
            {
                foreach(List<float> a in valeurs)
                {
                    foreach(float n in a)
                    {
                        Console.Write(n+" ");
                    }
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("Matrice vide ou nulle");
            }
        }


        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 

    }


}

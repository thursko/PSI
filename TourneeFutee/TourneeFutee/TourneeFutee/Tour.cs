namespace TourneeFutee
{
    // Modélise une tournée dans le cadre du problème du voyageur de commerce
    public class Tour
    {
        // TODO : ajouter tous les attributs que vous jugerez pertinents 
        
        private List<(string source, string destination)> segments;
        private float cost;
        // propriétés
        public Tour()
        {
            segments = new List<(string, string)>();
            cost = 0;
        }
        public Tour(List<string> l, float f)
        {
            var result = new List<(string source, string destination)>();

            for (int i = 0; i < l.Count - 1; i += 2)
            {
                result.Add((l[i], l[i + 1]));
            }
            segments = result;
            cost = f;
        }
        // Coût total de la tournée
        public float Cost
        {
            get { return this.cost; }    // TODO : implémenter
        }

        // Nombre de trajets dans la tournée
        public int NbSegments
        {
            get { return segments.Count; }   // TODO : implémenter
        }

        public void AddSegment(string source, string destination, float segmentCost)
        {
            segments.Add((source, destination));
            cost += segmentCost;
        }
        // Renvoie vrai si la tournée contient le trajet `source`->`destination`
        public bool ContainsSegment((string source, string destination) segment)
        {
            return segments.Contains(segment);   // TODO : implémenter 
        }


        // Affiche les informations sur la tournée : coût total et trajets
        public void Print()
        {
            // TODO : implémenter 
            Console.WriteLine("Coût total : "+cost);
            Console.WriteLine("Segments :");

            foreach (var (source, destination) in segments)
            {
                Console.WriteLine(source + " -> " + destination);
            }
        }

        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 

    }
}

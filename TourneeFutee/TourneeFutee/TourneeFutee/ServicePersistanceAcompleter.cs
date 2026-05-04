using System;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;

namespace TourneeFutee
{
    /// <summary>
    /// Service de persistance permettant de sauvegarder et charger
    /// des graphes et des tournées dans une base de données MySQL.
    /// </summary>
    public class ServicePersistance
    {
        // ─────────────────────────────────────────────────────────────────────
        // Attributs privés
        // ─────────────────────────────────────────────────────────────────────

        private readonly string _connectionString;
        private readonly MySqlConnection _connection;

        // TODO : si vous avez besoin de maintenir une connexion ouverte,
        //        ajoutez un attribut MySqlConnection ici.

        // ─────────────────────────────────────────────────────────────────────
        // Constructeur
        // ─────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Instancie un service de persistance et se connecte automatiquement
        /// à la base de données <paramref name="dbname"/> sur le serveur
        /// à l'adresse IP <paramref name="serverIp"/>.
        /// Les identifiants sont définis par <paramref name="user"/> (utilisateur)
        /// et <paramref name="pwd"/> (mot de passe).
        /// </summary>
        /// <param name="serverIp">Adresse IP du serveur MySQL.</param>
        /// <param name="dbname">Nom de la base de données.</param>
        /// <param name="user">Nom d'utilisateur.</param>
        /// <param name="pwd">Mot de passe.</param>
        /// <exception cref="Exception">Levée si la connexion échoue.</exception>
        public ServicePersistance(string serverIp, string dbname, string user, string pwd)
        {
          // TODO : initialiser et ouvrir la connexion à la base de données
            _connectionString = $"server={serverIp};database={dbname};uid={user};pwd={pwd};";

            try
            {
                _connection = new MySqlConnection(_connectionString) ;
                _connection.Open();
            }
            catch(MySqlException ex)
            {
                Console.WriteLine("ERREUR DE CONNECTION A LA BDD : "+ex.ToString());
                throw;
            }

        }

        // ─────────────────────────────────────────────────────────────────────
        // Méthodes publiques
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Sauvegarde le graphe <paramref name="g"/> en base de données
        /// (sommets et arcs inclus) et renvoie son identifiant.
        /// </summary>
        /// <param name="g">Le graphe à sauvegarder.</param>
        /// <returns>Identifiant du graphe en base de données (AUTO_INCREMENT).</returns>
        public uint SaveGraph(Graph g)
        {
            // TODO : implémenter la sauvegarde du graphe
            //
            // Ordre recommandé :
            //   1. INSERT dans la table Graphe -> récupérer l'id avec LAST_INSERT_ID()
            //   2. Pour chaque sommet de g : INSERT dans Sommet (valeur + graphe_id)
            //      -> conserver la correspondance sommet C# <-> id BdD
            //   3. Pour chaque arc de la matrice d'adjacence (poids != +inf) :
            //      INSERT dans Arc (sommet_source_id, sommet_dest_id, poids, graphe_id)
            //
            // Exemple pour récupérer l'id généré :
            //   uint id = Convert.ToUInt32(cmd.ExecuteScalar());

            using (var conn = OpenConnection())
            {
                var cmd = new MySqlCommand(
                    "INSERT INTO Graphe (est_oriente, nom, ordre) VALUES (@o, @n, @ord); SELECT LAST_INSERT_ID();",
                    conn
                );

                cmd.Parameters.AddWithValue("@o", g.Directed ? 1 : 0);
                cmd.Parameters.AddWithValue("@n", "Graph Metro");
                cmd.Parameters.AddWithValue("@ord", g.Order);

                uint graphId = Convert.ToUInt32(cmd.ExecuteScalar());

                var vertices = g.GetVertices();
                var map = new Dictionary<string, uint>();

                for (int i = 0; i < vertices.Count; i++)
                {
                    var cmdS = new MySqlCommand(
                        "INSERT INTO Sommet (graphe_id, nom, indice, valeur) VALUES (@gid, @nom, @ind, @val); SELECT LAST_INSERT_ID();",
                        conn
                    );

                    cmdS.Parameters.AddWithValue("@gid", graphId);
                    cmdS.Parameters.AddWithValue("@nom", vertices[i]);
                    cmdS.Parameters.AddWithValue("@ind", i);
                    cmdS.Parameters.AddWithValue("@val", g.GetVertexValue(vertices[i]));

                    uint id = Convert.ToUInt32(cmdS.ExecuteScalar());
                    map[vertices[i]] = id;
                }

                for (int i = 0; i < vertices.Count; i++)
                {
                    string source = vertices[i];
                    var neighbors = g.GetNeighbors(source);

                    foreach (var dest in neighbors)
                    {
                        float poids = g.GetEdgeWeight(source, dest);

                        var cmdA = new MySqlCommand(
                            "INSERT INTO Arc (graphe_id, sommet_source, sommet_dest, poids) VALUES (@gid, @s, @d, @p)",
                            conn
                        );

                        cmdA.Parameters.AddWithValue("@gid", graphId);
                        cmdA.Parameters.AddWithValue("@s", map[source]);
                        cmdA.Parameters.AddWithValue("@d", map[dest]);
                        cmdA.Parameters.AddWithValue("@p", poids);

                        cmdA.ExecuteNonQuery();
                    }
                }

                return graphId;
            }
        }

        /// <summary>
        /// Charge depuis la base de données le graphe identifié par <paramref name="id"/>
        /// et renvoie une instance de la classe <see cref="Graph"/>.
        /// </summary>
        /// <param name="id">Identifiant du graphe à charger.</param>
        /// <returns>Instance de <see cref="Graph"/> reconstituée.</returns>
        public Graph LoadGraph(uint id)
        {
            // TODO : implémenter le chargement du graphe
            //
            // Ordre recommandé :
            //   1. SELECT dans Graphe WHERE id = @id -> récupérer IsOriented, etc.
            //   2. SELECT dans Sommet WHERE graphe_id = @id -> reconstruire les sommets
            //      (respecter l'ordre d'insertion pour que les indices de la matrice
            //       correspondent à ceux sauvegardés)
            //   3. SELECT dans Arc WHERE graphe_id = @id -> reconstruire la matrice
            //      d'adjacence en utilisant les correspondances sommet_id <-> indice

            using (var conn = OpenConnection())
            {
                bool directed = false;

                var cmd = new MySqlCommand(
                    "SELECT est_oriente FROM Graphe WHERE id = @id",
                    conn
                );
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        directed = reader.GetBoolean(0);
                }

                var graph = new Graph(directed);

                var idToName = new Dictionary<uint, string>();

                var cmdS = new MySqlCommand(
                    "SELECT id, nom, valeur FROM Sommet WHERE graphe_id = @id ORDER BY indice",
                    conn
                );

                cmdS.Parameters.AddWithValue("@id", id);

                using (var reader = cmdS.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        uint sid = reader.GetUInt32(0);
                        string nom = reader.GetString(1);

                        graph.AddVertex(nom);
                        idToName[sid] = nom;
                    }
                }

                var cmdA = new MySqlCommand(
                    "SELECT sommet_source, sommet_dest, poids FROM Arc WHERE graphe_id = @id",
                    conn
                );
                cmdA.Parameters.AddWithValue("@id", id);

                using (var reader = cmdA.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string s = idToName[reader.GetUInt32(0)];
                        string d = idToName[reader.GetUInt32(1)];
                        float p = reader.GetFloat(2);


                        try
                        {
                            graph.AddEdge(s, d, p);
                        }
                        catch
                        {
                        }
                    }
                }

                return graph;
            }
        }

        /// <summary>
        /// Sauvegarde la tournée <paramref name="t"/> (effectuée dans le graphe
        /// identifié par <paramref name="graphId"/>) en base de données
        /// et renvoie son identifiant.
        /// </summary>
        /// <param name="graphId">Identifiant BdD du graphe dans lequel la tournée a été calculée.</param>
        /// <param name="t">La tournée à sauvegarder.</param>
        /// <returns>Identifiant de la tournée en base de données (AUTO_INCREMENT).</returns>
        public uint SaveTour(uint graphId, Tour t)
        {
            // TODO : implémenter la sauvegarde de la tournée
            //
            // Ordre recommandé :
            //   1. INSERT dans Tournee (cout_total, graphe_id) -> récupérer l'id
            //   2. Pour chaque sommet de la séquence (avec son numéro d'ordre) :
            //      INSERT dans EtapeTournee (tournee_id, numero_ordre, sommet_id)
            //
            // Attention : conserver l'ordre des étapes est essentiel pour
            //             pouvoir reconstruire la tournée fidèlement au chargement.

            using (var conn = OpenConnection())
            {
                var cmd = new MySqlCommand(
                    "INSERT INTO Tournee (graphe_id, cout_total) VALUES (@gid, @c); SELECT LAST_INSERT_ID();",
                    conn
                );

                cmd.Parameters.AddWithValue("@gid", graphId);
                cmd.Parameters.AddWithValue("@c", t.Cost);

                uint tourId = Convert.ToUInt32(cmd.ExecuteScalar());

                var map = new Dictionary<string, uint>();

                var cmdMap = new MySqlCommand(
                    "SELECT id, nom FROM Sommet WHERE graphe_id = @gid",
                    conn
                );
                cmdMap.Parameters.AddWithValue("@gid", graphId);

                using (var reader = cmdMap.ExecuteReader())
                {
                    while (reader.Read())
                        map[reader.GetString(1)] = reader.GetUInt32(0);
                }

                var vertices = t.Vertices;

                for (int i = 0; i < vertices.Count; i++)
                {
                    var cmdStep = new MySqlCommand(
                        "INSERT INTO EtapeTournee (tournee_id, numero_ordre, sommet_id) VALUES (@tid, @ord, @sid)",
                        conn
                    );

                    cmdStep.Parameters.AddWithValue("@tid", tourId);
                    cmdStep.Parameters.AddWithValue("@ord", i);
                    cmdStep.Parameters.AddWithValue("@sid", map[vertices[i]]);

                    cmdStep.ExecuteNonQuery();
                }

                return tourId;
            }
        }

        /// <summary>
        /// Charge depuis la base de données la tournée identifiée par <paramref name="id"/>
        /// et renvoie une instance de la classe <see cref="Tour"/>.
        /// </summary>
        /// <param name="id">Identifiant de la tournée à charger.</param>
        /// <returns>Instance de <see cref="Tour"/> reconstituée.</returns>
        public Tour LoadTour(uint id)
        {
            // TODO : implémenter le chargement de la tournée
            //
            // Ordre recommandé :
            //   1. SELECT dans Tournee WHERE id = @id -> récupérer cout_total et graphe_id
            //   2. SELECT dans EtapeTournee JOIN Sommet WHERE tournee_id = @id
            //      ORDER BY numero_ordre -> reconstruire la séquence ordonnée de sommets
            //   3. Construire et retourner l'instance Tour

            using (var conn = OpenConnection())
            {
                float cost = 0;

                var cmd = new MySqlCommand(
                    "SELECT cout_total FROM Tournee WHERE id = @id",
                    conn
                );
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        cost = reader.GetFloat(0);
                }

                var sequence = new List<string>();

                var cmdStep = new MySqlCommand(
                    @"SELECT S.nom 
              FROM EtapeTournee E
              JOIN Sommet S ON E.sommet_id = S.id
              WHERE E.tournee_id = @id
              ORDER BY E.numero_ordre",
                    conn
                );
                cmdStep.Parameters.AddWithValue("@id", id);

                using (var reader = cmdStep.ExecuteReader())
                {
                    while (reader.Read())
                        sequence.Add(reader.GetString(0));
                }

                return new Tour(sequence, cost);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Méthodes utilitaires privées (à compléter selon vos besoins)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Crée et retourne une nouvelle connexion MySQL ouverte.
        /// Encadrez toujours l'appel dans un bloc using pour garantir la fermeture.
        /// </summary>
        private MySqlConnection OpenConnection()
        {
            var conn = new MySqlConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace XnaTest.DataAccessLayer
{
    public class KinectPongDAL
    {
        private static SQLiteConnection dbConnection;
        private SQLiteCommand insertHighscoreCommand;
        private SQLiteCommand removeInvalidHighscores;
        private SQLiteCommand selectTopScores;
        private SQLiteCommand selectLowestResult;
        private SQLiteDataReader reader;

        public KinectPongDAL()
        {
            if (dbConnection == null)
            {
                dbConnection = new SQLiteConnection("Data Source=../../../../../db/KinectPong.sqlite;Version=3;");
            }
            insertHighscoreCommand = new SQLiteCommand("INSERT INTO high_scores (name, result) VALUES (?, ?)", dbConnection);
            selectTopScores = new SQLiteCommand("SELECT name, result FROM high_scores ORDER BY result ASC", dbConnection);
            removeInvalidHighscores = new SQLiteCommand("DELETE FROM high_scores WHERE result >= ?", dbConnection);
            selectLowestResult = new SQLiteCommand("SELECT result FROM high_scores ORDER BY result ASC", dbConnection);
            
        }

        public Dictionary<String, long> GetHighScores()
        {
            Dictionary<String, long> topResults = new Dictionary<String, long>();
            dbConnection.Open();
            reader = this.selectTopScores.ExecuteReader();
            while (reader.Read())
            {
                topResults.Add(reader["name"].ToString(), (long)reader["result"]);
            }
            reader.Dispose();

            dbConnection.Close();

            return topResults;
        }

        public void InsertHighScore(String name, long result, int maximumTopScores)
        {
            insertHighscoreCommand.Parameters.Add(new SQLiteParameter("param1", name));
            insertHighscoreCommand.Parameters.Add(new SQLiteParameter("param2", result));
            dbConnection.Open();
            int insertRes = insertHighscoreCommand.ExecuteNonQuery();

            // leave only TOP maximumTopScores
            reader = selectTopScores.ExecuteReader();
            List<long> topResults = new List<long>();
            while (reader.Read())
            {
                topResults.Add((long)(reader["result"]));
            }
            reader.Dispose();

            long resultToRemove = -1;
            if (topResults.Count > maximumTopScores)
            {
                // read resultOfLast and go to next first that is different (maybe in some cases maximumTopScores and next one will have same result)
                long resultOfLast = topResults[maximumTopScores-1];
                for (int i = maximumTopScores - 1; i < topResults.Count; i++)
                {
                    if (!topResults[i].Equals(resultOfLast))
                    {
                        resultToRemove = topResults[i];
                    }
                }
            }

            if (resultToRemove != -1)
            {
                removeInvalidHighscores.Parameters.Add(new SQLiteParameter("param1", resultToRemove));
                int deleteRes = removeInvalidHighscores.ExecuteNonQuery();
            }

            dbConnection.Close();
        }

        /// <summary>
        /// Database has table with highscore. This method will search for lowest result. If there is smaller number of maximum allowed top scores it will return -1
        /// </summary>
        /// <param name="maximumTopScores"></param>
        /// <returns></returns>
        public long getLowestHighscore(int maximumTopScores)
        {
            long numberOfScores = -1;
            long lowestScore = -1;

            dbConnection.Open();
            reader = selectLowestResult.ExecuteReader();

            int counter = 0;
            while (reader.Read())
            {
                lowestScore = (long)reader["result"];
                counter++;
            }
            reader.Dispose();
            dbConnection.Close();
            if (numberOfScores < maximumTopScores)
            {
                return -1;
            }

            return lowestScore;
        }
    }
}

﻿using System;
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
        private SQLiteDataReader reader;

        public KinectPongDAL()
        {
            if (dbConnection == null)
            {
                dbConnection = new SQLiteConnection("Data Source=../../../../../db/KinectPong.sqlite;Version=3;");
                insertHighscoreCommand = new SQLiteCommand("INSERT INTO high_scores (name, result) VALUES (?, ?)", dbConnection);
                selectTopScores = new SQLiteCommand("SELECT * FROM high_scores ORDER BY result DESC", dbConnection);
                removeInvalidHighscores = new SQLiteCommand("DELETE FROM high_scores WHERE result >= ?", dbConnection);
            }
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
                long resultOfLast = topResults[maximumTopScores];
                for (int i = maximumTopScores; i < topResults.Count; i++)
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

        public long getLowestHighscore()
        {
            return -1;
        }
    }
}

using System;

namespace Ambrus.Repository
{
    using System.Collections.Generic;


    public interface IGameRepository
    {

        void SaveScore(string playerName, int score);

        IEnumerable<Entry> LoadScores();
    }
}
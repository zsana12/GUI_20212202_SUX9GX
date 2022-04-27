using System;

namespace Ambrus.Logic
{

    using System.Collections.Generic;
    using Ambrus.Repository;

    public interface IGameRepository
    {

        void SaveScore(string playerName, int score);

        IEnumerable<Entry> LoadScores();

    }
}

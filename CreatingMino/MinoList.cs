using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatingMino {
    internal class MinoList {
        public int CellCount { get; }
        public List<Mino> MinoCollection { get; }

        public MinoList(int cellCount, List<Mino> minoCollection) {
            CellCount = cellCount;
            MinoCollection = minoCollection;
        }
    }
}

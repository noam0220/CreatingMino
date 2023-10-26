using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatingMino {
    public class Mino {
        public List<int[]> Cells { get; }
        public int Height => CalcHeight();
        public int Width => CalcWidth();

        public Mino(List<int[]> cells) {
            Cells = cells;
        }

        private int CalcHeight() {
            return Cells.Max(y => y[0]) - Cells.Min(y => y[0]) + 1;
        }

        private int CalcWidth() {
            return Cells.Max(y => y[1]) - Cells.Min(y => y[1]) + 1;
        }

        public int[] CalcSize() {
            return new int[] { Height, Width };
        }
    }
}

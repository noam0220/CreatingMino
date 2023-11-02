using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatingMino {
    public class MinoList {
        public int BlockCount { get; }
        public List<Mino> MinoCollection { get; }

        public MinoList(int blockCount, List<Mino> minoCollection) {
            BlockCount = blockCount;
            MinoCollection = minoCollection;
        }

        //ミノを出力する。(N >= 9ではスクロールしきれず全パターンを表示できない。)
        public static void PrintMino(MinoList minoList) {

            //問題 : ミノを出力する処理を高速化しよう。
            foreach(var mino in minoList.MinoCollection) {
                int[] tmpBlock = new int[2];
                Mino printMino = mino;
                for(int row = 0; row < mino.Height; row++) {
                    for(int col = 0; col < mino.Width; col++) {
                        tmpBlock[0] = row; tmpBlock[1] = col;
                        if(printMino.Blocks.Any(c => c.SequenceEqual(tmpBlock))) {
                            Console.Write("■");
                        } else {
                            Console.Write("　");
                        }
                    }
                    Console.Write("\n");
                }
                Console.Write("\n");
            }
            Console.Write("ブロック数 " + (minoList.BlockCount + 1) + " の時 : " + minoList.MinoCollection.Count() + "個\n\n"); 
        }
    }
}

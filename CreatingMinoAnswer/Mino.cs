using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatingMinoAnswer {
    public class Mino {
        //ミノの形状を表現するクラス。Blocksには各ブロックの位置をxy平面上の格子点に対応付け、その座標(y座標, x座標)が入る。
        //原点の座標は(0, 0)とし、xは正の方向に、yは負の方向に進む。
        //Heightにはその形状の縦の長さ、Widthには横の長さが入る。
        //
        // 　0  1  … x
        //0　■ ■
        //1　■ 　
        //2　■
        //:
        //y
        //このような形状のデータは、[ {"Blocks" : "{0, 0}, {0, 1}, {1, 0}, {2, 0}", "Height" : "3", "Width" : "2"}]となる。
        public List<int[]> Blocks { get; }
        public int Height => CalcHeight();
        public int Width => CalcWidth();

        public Mino(List<int[]> blocks) {
            Blocks = blocks;
        }

        private int CalcHeight() {
            return Blocks.Max(y => y[0]) - Blocks.Min(y => y[0]) + 1;
        }

        private int CalcWidth() {
            return Blocks.Max(x => x[1]) - Blocks.Min(x => x[1]) + 1;
        }

        public int[] CalcSize() {
            return new int[] { Height, Width };
        }

        //ブロックを追加する方向
        public static string[] directionList = new string[] { "above", "right", "below", "left" };
    }
}

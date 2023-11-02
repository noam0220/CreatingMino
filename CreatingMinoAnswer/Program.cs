using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CreatingMinoAnswer {
    internal class Program {
        //ブロックがN個の全てのテトリミノの形状と、その個数を求める。
        //縦が長くなるように出力する。
        //180°回転させたときに等しくなるミノ同士は同一のミノとみなし、どちらか一つのみを出力する。
        //　ブロック数 4 の時 : 7個
        //　■　　■ ■　　■ ■　　■ 　　　　 ■　　■ ■　　　 ■
        //　■　　■ 　　　　 ■　　■ ■　　■ ■　　■ ■　　■ ■
        //　■　　■ 　　　　 ■　　■ 　　　■　　　　　　　　■ 
        //　■
        //　(出力例, N = 4)

        //各大きさのミノの結果を保持
        static MinoList nextMinoList = new MinoList(1, new List<Mino> {
            new Mino (new List<int[]>{ new int[] { 0, 0 } })
        });

        //全ての結果を保持
        static List<MinoList> minoLists = new List<MinoList> { nextMinoList };

        //全てのブロック数のミノを出力するかどうか決める。
        static bool printAllFlg = true;
        static List<int> printList = new List<int>();

        //今回の使用ブロック数
        const int N = 4;

        //回転して出力させる。
        //const int degree = 0;

        static void Main(string[] args) {
            //変数名を考えてもらう。
            Stopwatch sw = Stopwatch.StartNew();

            CreateMino(minoLists.First());

            //回転させて出力させる？
            //minoLists = Mino.RoteteMino(minoLists, degree);
            
            if (!printAllFlg) {
                printList.Add(N);
            } else {
                for (int i = 1; i <= N; i++) {
                    printList.Add(i);
                }
            }

            MinoList.PrintMino(minoLists, printList);

            sw.Stop();

            Console.WriteLine("経過時間: " + sw.ElapsedMilliseconds + " ms");

        }

        //BlockCountがiにおける全てのテトリミノの形状と、その個数を求める。

        private static void CreateMino(MinoList oMinoList) {

            nextMinoList = new MinoList(oMinoList.BlockCount + 1, new List<Mino> { });

            foreach(var oMino in oMinoList.MinoCollection) {
                var testBlocks = new List<int[]>();

                //ブロックの外側に、1マス分の空のブロックを追加し、枠を作る。
                foreach(var block in oMino.Blocks) {
                    testBlocks.Add(new int[] { block[0] + 1, block[1] + 1 });
                }
                //AddMino()でtestBlocksに対して操作すると反復処理が行えなくなるので、
                //中身のみコピーしておく。
                var tmpBlocks = testBlocks.Select(b => b.ToArray()).ToList();

                //問題　繰り返しを取り除こう。
                //上→右→下→左の順に、各ブロックの1つ隣に新たなブロックを追加できるか確認する。
                var tmpBlock = new int[2];
                foreach(var cell in testBlocks) {
                    foreach(var direction in Mino.directionList) {
                        switch(direction) {
                            case "above": tmpBlock[0] = cell[0] - 1; tmpBlock[1] = cell[1]; break;
                            case "right": tmpBlock[0] = cell[0]; tmpBlock[1] = cell[1] + 1; break;
                            case "below": tmpBlock[0] = cell[0] + 1; tmpBlock[1] = cell[1]; break;
                            case "left" : tmpBlock[0] = cell[0]; tmpBlock[1] = cell[1] - 1; break;
                        }
                        //選択された場所に、既にブロックが置かれていなければ、その形状をリストに追加する。
                        if(!testBlocks.Any(b => b.SequenceEqual(tmpBlock))) {
                            tmpBlocks = AddMino(tmpBlocks, tmpBlock);
                        }
                    }
                }
            }

            minoLists.Add(nextMinoList);

            //i個のブロックのミノのパターンを使用して(i + 1)個のパターンを求めて、
            //それをN個まで繰り返した後、、メインの処理を終了する。
            if(nextMinoList.BlockCount < N) { CreateMino(nextMinoList); }
        }

        //適切な形状であれば、ミノを生成しリストに追加する。
        //元のミノの形状と検証する新しいブロックの座標を受け取り、元の形状を返す。
        private static List<int[]> AddMino(List<int[]> blocks, int[] tBlock) {
            blocks.Add(tBlock);

            var blocksWithNoSpace = RemoveSpaces(blocks);

            int height = blocksWithNoSpace.Max(y => y[0]) - blocksWithNoSpace.Min(y => y[0]) + 1; 
            int width = blocksWithNoSpace.Max(x => x[1]) - blocksWithNoSpace.Min(x => x[1]) + 1;

            if(height < width) {
                //重複チェックのため、縦より横の方が長い場合、図形を時計回りに90°回転させる
                var tmpBlocks = new List<int[]>();
                foreach(var block in blocksWithNoSpace) {
                    tmpBlocks.Add(new int[] { block[1], height - block[0] });
                }
                blocksWithNoSpace = RemoveSpaces(tmpBlocks);
                (height, width) = (width, height);
            }

            //既に生成されているミノと比較して、重複がない場合のみミノを生成し、リストに追加する。
            if(CheckNoDuplication(blocksWithNoSpace, height, width)) {
                nextMinoList.MinoCollection.Add(new Mino(blocksWithNoSpace));
            };
            blocks.Remove(tBlock);
            return blocks;
        }

        //左端、上端の隙間を取り除く。
        //与えらえた形状を左上に詰めた形状にして返す。
        private static List<int[]> RemoveSpaces(List<int[]> blocks) {
            blocks = blocks.OrderBy(y => y[0]).ThenBy(x => x[1]).ToList();

            //xMin(各座標の中で最小のx座標の値)を算出し、左端に何マスの隙間があるか計算する。 yMin(最小のy座標の値)も同様。
            int xMin = blocks.Min(x => x[1]);
            int yMin = blocks.Min(y => y[0]);

            var blocksWithNoSpace = new List<int[]>();

            foreach(var block in blocks) {
                blocksWithNoSpace.Add(new int[] { block[0] - yMin, block[1] - xMin });
            }

            return blocksWithNoSpace;
        }

        //重複しているミノの生成を防ぐ。
        //与えられた形状に重複が無かった場合、trueを返す。
        private static bool CheckNoDuplication(List<int[]> blocks, int height, int width) {
            bool sameFlg;
            foreach(var mino in nextMinoList.MinoCollection) {
                if(height == mino.Height && width == mino.Width) {
                    var rBlocks = blocks.Select(b => b.ToArray()).ToList();
                    var tmpBlocks = new List<int[]>();
                    for(int i = 0; i < 4; i++) {
                        var differ = i % 2 == 0 ? height : width;
                        //tmpBlocksに、rBlocksを90°回転させた座標を代入して、チェックを行う。
                        foreach(var rBlock in rBlocks) {
                            tmpBlocks.Add(new int[] { rBlock[1], differ - 1 - rBlock[0] });
                        }
                        tmpBlocks = tmpBlocks.OrderBy(y => y[0]).ThenBy(x => x[1]).ToList();
                        rBlocks = tmpBlocks.Select(b => b.ToArray()).ToList();
                        sameFlg = true;
                        //全ての座標が一致していた場合のみ、sameFlgがtrueのまま通り、重複していると判定される。
                        for(int j = 0; j < mino.Blocks.Count; j++) {
                            if(!Enumerable.SequenceEqual(tmpBlocks[j], mino.Blocks[j])) { sameFlg = false; }
                        }
                        if(sameFlg) { return false; }
                        tmpBlocks.Clear();
                    }
                }
            }
            return true;
        }
    }
}

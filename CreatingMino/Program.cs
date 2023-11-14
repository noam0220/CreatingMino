using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CreatingMino {
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
        static MinoList beforeMinoList = new MinoList(3, new List<Mino> {
            new Mino (new List<int[]>{ new int[] { 0, 0 }, new int[] { 1, 0 }, new int[] { 2, 0 } }),
            new Mino (new List<int[]>{ new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 1, 0 } })
        });

        //結果を保持
        static MinoList minoList = new MinoList(N - 1, new List<Mino>());

        //方向
        static string[] directionList = new string[] { "above", "right", "below", "left" };

        //今回の使用セル数
        const int N = 4;

        //回転して出力させる。
        //const int degree = 0;

        static void Main(string[] args) {
            //変数名を考えてもらう。
            Stopwatch sw = Stopwatch.StartNew();

            CreateMino(beforeMinoList);

            MinoList.PrintMino(minoList);

            sw.Stop();

            Console.WriteLine("経過時間: " + sw.ElapsedMilliseconds + " ms");

        }

        private static void CreateMino(MinoList oMinoList) {

            foreach(var oMino in oMinoList.MinoCollection) {
                var testBlocks = new List<int[]>();
                //問題?:正しい処理にしよう。
                //ミスの例
                //var tmpCell = new int[2];
                //foreach(var cell in oMino.Cells) {
                //    tmpCell[0] = cell[0] + 1; tmpCell[1] = cell[1] + 1;
                //    testCells.Add(tmpCell);
                //}

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
                //選択された場所に、既にブロックが置かれていなければ、その形状をリストに追加する。
                foreach(var block in testBlocks) {
                    tmpBlock[0] = block[0] - 1; tmpBlock[1] = block[1];
                    if(!testBlocks.Any(b => b.SequenceEqual(tmpBlock))) {
                        tmpBlocks = AddMino(tmpBlocks, tmpBlock);
                    }
                    tmpBlock[0] = block[0]; tmpBlock[1] = block[1] + 1;
                    if(!testBlocks.Any(b => b.SequenceEqual(tmpBlock))) {
                        tmpBlocks = AddMino(tmpBlocks, tmpBlock);
                    }
                    tmpBlock[0] = block[0] + 1; tmpBlock[1] = block[1];
                    if(!testBlocks.Any(b => b.SequenceEqual(tmpBlock))) {
                        tmpBlocks = AddMino(tmpBlocks, tmpBlock);
                    }
                    tmpBlock[0] = block[0]; tmpBlock[1] = block[1] - 1;
                    if(!testBlocks.Any(b => b.SequenceEqual(tmpBlock))) {
                        tmpBlocks = AddMino(tmpBlocks, tmpBlock);
                    }
                }
            }
        }

        //適切な形状であれば、ミノを生成しリストに追加する。
        //元のミノの形状と検証する新しいブロックの座標を受け取り、元の形状を返す。
        private static List<int[]> AddMino(List<int[]> blocks, int[] tBlock) {
            blocks.Add(tBlock);
            blocks = blocks.OrderBy(y => y[0]).ThenBy(x => x[1]).ToList();

            //処理を共通化しよう。

            //xMin(各座標の中で最小のx座標の値)を算出し、左端に何マスの隙間があるか計算する。 yMin(最小のy座標の値)も同様。
            int xMin = blocks.Min(x => x[1]);
            int yMin = blocks.Min(y => y[0]);

            var blocksWithNoSpace = new List<int[]>();

            foreach(var block in blocks) {
                blocksWithNoSpace.Add(new int[] { block[0] - yMin, block[1] - xMin });
            }

            int height = blocksWithNoSpace.Max(y => y[0]) - blocksWithNoSpace.Min(y => y[0]) + 1;
            int width = blocksWithNoSpace.Max(x => x[1]) - blocksWithNoSpace.Min(x => x[1]) + 1;

            if(height < width) {
                //縦より横の方が長い場合、図形を時計回りに90°回転させる
                var tmpBlocks = new List<int[]>();
                var tmpBlocksWithNoSpace = new List<int[]>();
                foreach(var block in blocksWithNoSpace) {
                    tmpBlocks.Add(new int[] { block[1], height - block[0] });
                }
                tmpBlocks = tmpBlocks.OrderBy(x => x[0]).ThenBy(y => y[1]).ToList();

                xMin = tmpBlocks.Min(x => x[1]);
                yMin = tmpBlocks.Min(y => y[0]);

                foreach(var block in tmpBlocks) {
                    tmpBlocksWithNoSpace.Add(new int[] { block[0] - yMin, block[1] - xMin });
                }

                blocksWithNoSpace = tmpBlocksWithNoSpace;
                (height, width) = (width, height);
            }

            //既に生成されているミノと比較して、重複がない場合のみミノを生成し、リストに追加する。
            if(CheckNoDuplication(blocksWithNoSpace, height, width)) {
                minoList.MinoCollection.Add(new Mino(blocksWithNoSpace));
            };
            blocks.Remove(tBlock);
            return blocks;
        }

        //重複しているミノの生成を防ぐ。
        //与えられた形状に重複が無かった場合、trueを返す。
        private static bool CheckNoDuplication(List<int[]> blocks, int height, int width) {
            bool sameFlg;
            foreach(var mino in minoList.MinoCollection) {
                if(height == mino.Height && width == mino.Width) {
                    //同じかどうか比較
                    var rBlocks = blocks.Select(b => b.ToArray()).ToList();
                    var tmpBlocks = new List<int[]>();
                    //問題:見やすくしよう。
                    for (int i = 0; i < 4; i++) {
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
                    //問題?:90°,270°回転させた形状とも比較させよう。
                    //for(int i = 0; i < mino.Blocks.Count; i++) {
                    //    if(!Enumerable.SequenceEqual(blocks[i], mino.Blocks[i])) { sameFlg = false; }
                    //}
                    //if(sameFlg) { return false; }

                    //本当は、ここに90°回転させて同じかどうかも比較する必要あり

                    //180°回転させて、同じかどうか比較
                    //var tmpBlock2 = new List<int[]>{ };
                    //foreach(var block in blocks) {
                    //    tmpBlocks2.Add(new int[] { height - block[0] - 1, width - block[1] - 1 });
                    //}

                    //tmpBlock2 = tmpBlocks2.OrderBy(y => y[0]).ThenBy(x => x[1]).ToList();
                    //sameFlg = true;
                    //for(int i = 0; i < mino.Block.Count; i++){
                    //    if(!Enumerable.SequenceEqual(tmpBlocks2[i], mino.Blocks[i])) { sameFlg = false; }
                    //}
                    //if(sameFlg) { return false; }
                }
            } 
            return true;
        }
    }
}

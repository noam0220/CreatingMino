using System.Diagnostics;

namespace CreatingMinoAnswer {
    internal class Program {
        //ブロックがN個のミノの全ての形状と、その個数を求める。
        //縦が長くなるように出力する。
        //回転させたときに等しくなるミノ同士は同一のミノとみなし、どちらか一つのみを出力する。
        //
        //　■　　■ ■　　■ ■　　■ 　　　　 ■　　■ ■　　■
        //　■　　■ 　　　　 ■　　■ ■　　■ ■　　■ ■　　■ ■
        //　■　　■ 　　　　 ■　　■ 　　　■　　　　　　　　　 ■ 
        //　■
        //
        //　ブロック数 4 の時 : 7個
        //　(出力例, N = 4)

        //ブロック数1のミノ。
        static Mino firstMino = new Mino(new List<int[]> { new int[] { 0, 0 } });

        //全ての結果を保持する。
        static List<Mino> minoList = new List<Mino> { firstMino };

        //ブロックが(i + 1)個のミノを保持する。
        static List<Mino> nextMinoList = new List<Mino> { };

        //出力するミノを保持する。
        static List<Mino> printMinoList = new List<Mino> { };

        //今回の使用ブロック数。
        const int N = 4;

        static void Main(string[] args) {

            var sw = Stopwatch.StartNew();

            CreateMino(minoList);

            //ブロック数がNのミノのみ出力する。
            foreach(var mino in minoList) {
                if(mino.Blocks.Count() == N) { 
                    printMinoList.Add(mino);
                }
            }

            Mino.PrintMino(printMinoList);

            sw.Stop();

            Console.WriteLine("経過時間: " + sw.ElapsedMilliseconds + " ms");

        }

        //ブロック数がi個の全てのミノの形状と、その個数を求める。
        //【問題3 - 解答】処理を汎用化しよう。(Nに4以外の自然数も入れられるようにする。)
        private static void CreateMino(List<Mino> oMinoList) {

            nextMinoList = new List<Mino>();

            foreach(var oMino in oMinoList) {
                var testBlocks = new List<int[]>();

                //ブロックの外側に1マス分の空のブロックを追加し、枠を作る。
                foreach(var block in oMino.Blocks) {
                    testBlocks.Add(new int[] { block[0] + 1, block[1] + 1 });
                }

                //AddMino()をtestBlocksに対して行うと反復処理が行えなくなるので、
                //"testBlocks"の中身を"tmpBlocks"にコピーしておく。
                var tmpBlocks = testBlocks.Select(b => b.ToArray()).ToList();
                var newBlock = new int[2];
                var settable = true;

                //上→右→下→左の順に、ブロックの隣に新たなブロックを置けるか確認する。
                //【問題2 - 解答】重複を取り除こう。
                foreach(var tBlock in testBlocks) {
                    foreach(var direction in Mino.directionMap.Keys) {
                        //tBlockの1つ上、右、下、左隣の座標を求める。
                        newBlock = Mino.GetNewBlock(tBlock, direction);

                        //tmpBlockに、既にブロックが置かれていなければ、その形状をminoListに追加する。
                        foreach(var block in testBlocks) {
                            if(block[0] == newBlock[0] && block[1] == newBlock[1]) {
                                settable = false;
                            }
                        }
                        if(settable == true) {
                            AddMino(tmpBlocks, newBlock);
                            tmpBlocks.Remove(newBlock);
                        }
                        settable = true;
                    }
                }
            }

            minoList.AddRange(nextMinoList);

            //i個のブロックのミノを使用して(i + 1)個のミノを生成して、
            //それをN個まで繰り返した後、処理が終了する。
            if(nextMinoList.First().Blocks.Count() < N) { CreateMino(nextMinoList); }
        }

        //適切な形状であれば、ミノを生成しリストに追加する。
        private static void AddMino(List<int[]> blocks, int[] nBlock) {
            blocks.Add(nBlock);

            var blocksWithNoSpace = RemoveSpaces(blocks);

            var height = blocksWithNoSpace.Max(y => y[0]) - blocksWithNoSpace.Min(y => y[0]) + 1;
            var width = blocksWithNoSpace.Max(x => x[1]) - blocksWithNoSpace.Min(x => x[1]) + 1;

            //縦より横の長さの方が長い場合
            if(height < width) {
                //形状を時計回りに90°回転させて、長さを(縦)≥(横)にする。
                var tmpBlocks = new List<int[]>();
                foreach(var block in blocksWithNoSpace) {
                    tmpBlocks.Add(new int[] { block[1], height - block[0] });
                }
                blocksWithNoSpace = RemoveSpaces(tmpBlocks);
                (height, width) = (width, height);
            }

            //既に生成されているミノと比較して、重複がない場合のみミノを生成し、リストに追加する。
            if(CheckNoDuplication(blocksWithNoSpace, height, width)) {
                nextMinoList.Add(new Mino(blocksWithNoSpace));
            };
        }

        //CreateMino()で加えられた外枠を取り除き、隙間を取り除いた形状にして返す。
        private static List<int[]> RemoveSpaces(List<int[]> blocks) {
            blocks = blocks.OrderBy(y => y[0]).ThenBy(x => x[1]).ToList();

            //xMin(各座標の中で最小のx座標の値)を算出し、左端に何マスの隙間があるか計算する。 yMin(最小のy座標の値)も同様。
            var xMin = blocks.Min(x => x[1]);
            var yMin = blocks.Min(y => y[0]);

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
            foreach(var mino in nextMinoList) {
                //縦と横の長さが同じ場合のみ重複をチェックする。
                if(height == mino.Height && width == mino.Width) {
                    //同じかどうか比較
                    var rBlocks = blocks.Select(b => b.ToArray()).ToList();
                    var tmpBlocks = new List<int[]>();
                    for(var i = 0; i < 4; i++) {
                        var differ = i % 2 == 0 ? height : width;
                        //tmpBlocksに、rBlocksを90°回転させた座標を代入して、チェックを行う。
                        foreach(var rBlock in rBlocks) {
                            tmpBlocks.Add(new int[] { rBlock[1], differ - 1 - rBlock[0] });
                        }
                        tmpBlocks = tmpBlocks.OrderBy(y => y[0]).ThenBy(x => x[1]).ToList();
                        rBlocks = tmpBlocks.Select(b => b.ToArray()).ToList();
                        sameFlg = true;
                        //全ての座標が一致していた場合、重複していると判定される。
                        for(var j = 0; j < mino.Blocks.Count; j++) {
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

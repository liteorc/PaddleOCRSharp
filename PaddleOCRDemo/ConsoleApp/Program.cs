using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PaddleOCRSharp;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var orc = new SimOCR();
            orc.Test();
            Console.ReadKey();
        }
    }

    class SimOCR
    {
        private PaddleOCREngine _engine;

        public SimOCR()
        {            
            //自带轻量版中英文模型V3模型
            OCRModelConfig config = null;
            //服务器中英文模型
            //OCRModelConfig config = new OCRModelConfig();
            //string root = System.IO.Path.GetDirectoryName(typeof(OCRModelConfig).Assembly.Location);
            //string modelPathroot = root + @"\inferenceserver";
            //config.det_infer = modelPathroot + @"\ch_ppocr_server_v2.0_det_infer";
            //config.cls_infer = modelPathroot + @"\ch_ppocr_mobile_v2.0_cls_infer";
            //config.rec_infer = modelPathroot + @"\ch_ppocr_server_v2.0_rec_infer";
            //config.keys = modelPathroot + @"\ppocr_keys.txt";

            //英文和数字模型V3
            //OCRModelConfig config = new OCRModelConfig();
            //string root = System.IO.Path.GetDirectoryName(typeof(OCRModelConfig).Assembly.Location);
            //string modelPathroot = root + @"\en_v3";
            //config.det_infer = modelPathroot + @"\en_PP-OCRv3_det_infer";
            //config.cls_infer = modelPathroot + @"\ch_ppocr_mobile_v2.0_cls_infer";
            //config.rec_infer = modelPathroot + @"\en_PP-OCRv3_rec_infer";
            //config.keys = modelPathroot + @"\en_dict.txt";

            //OCR参数
            OCRParameter oCRParameter = new OCRParameter();
            oCRParameter.cpu_math_library_num_threads = 10;//预测并发线程数
            oCRParameter.enable_mkldnn = true;//web部署该值建议设置为0,否则出错，内存如果使用很大，建议该值也设置为0.
            oCRParameter.cls = false; //是否执行文字方向分类；默认false
            oCRParameter.det = true;//是否开启方向检测，用于检测识别180旋转
            oCRParameter.use_angle_cls = false;//是否开启方向检测，用于检测识别180旋转
            oCRParameter.det_db_score_mode = true;//是否使用多段线，即文字区域是用多段线还是用矩形，
            _engine = new PaddleOCREngine(config, oCRParameter);
        }
        public void Test()
        {
            var dir = @"C:\Users\Zhenq\Documents\MuMu共享文件夹\Screenshots";
            var files = Directory.GetFiles(dir);
            foreach (var file in files)
            {
                StringBuilder sb = new StringBuilder();
                var imagebyte = File.ReadAllBytes(file);
                var ocrResult = _engine.DetectText(imagebyte);


                var tbs = ocrResult.TextBlocks;
                sb.Append($"玩家昵称：{GetTextByYPos(tbs, -1, 505)}，");
                sb.Append($"{GetTextByYPos(tbs, -1, 622)}，"/*.Replace("玩家ID：", "")*/);
                sb.Append($"所属公会：{GetTextByYPos(tbs, 432, 785)}，");
                sb.Append($"攻击力：{GetTextByYPos(tbs, -1, 1486)}，");
                sb.Append($"生命值：{GetTextByYPos(tbs, 657, 1486)}");
                Console.WriteLine( sb.ToString() );
            }
        }

        private string GetTextByYPos(List<TextBlock> collection, int xPos, int yPos, int offset = 10)
        {
            foreach (var item in collection)
            {
                var p = item.BoxPoints[0];
                if ( (p.Y >= (yPos - offset) && p.Y <= (yPos + offset))
                    && 
                    (xPos < 0 || p.X >= (xPos - offset) && p.X <= (xPos + offset))
                    )
                {
                    return item.Text;
                }
            }
            return "空";
        }

    }
}

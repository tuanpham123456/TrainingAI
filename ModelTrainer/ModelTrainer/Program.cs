//using System;
//using Microsoft.ML;
//using Microsoft.ML.Data;
//using VietnameseTextCorrectionServer.Models;

//namespace ModelTrainer
//{
//    class Program
//    {
//        private static readonly string _dataPath = @"Data\accent_data.csv";
//        private static readonly string _modelPath = @"MLModels\AccentModel.zip";

//        static void Main(string[] args)
//        {
//            // Tạo thư mục lưu mô hình nếu chưa tồn tại
//            System.IO.Directory.CreateDirectory("MLModels");

//            // Khởi tạo MLContext
//            var mlContext = new MLContext();

//            // Bước 1: Tải dữ liệu
//            Console.WriteLine("Đang tải dữ liệu...");
//            IDataView dataView = mlContext.Data.LoadFromTextFile<AccentData>(
//                path: _dataPath,
//                hasHeader: true,
//                separatorChar: ',');

//            // Bước 2: Chia dữ liệu thành tập huấn luyện và kiểm tra
//            Console.WriteLine("Đang chia dữ liệu thành tập huấn luyện và kiểm tra...");
//            var trainTestData = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
//            var trainingData = trainTestData.TrainSet;
//            var testingData = trainTestData.TestSet;

//            // Bước 3: Xây dựng pipeline
//            Console.WriteLine("Đang xây dựng pipeline...");
//            var pipeline = mlContext.Transforms.Text.FeaturizeText(
//                    outputColumnName: "Features",
//                    inputColumnName: nameof(AccentData.InputText))
//                .Append(mlContext.Transforms.Conversion.MapValueToKey(
//                    outputColumnName: "Label",
//                    inputColumnName: nameof(AccentData.OutputText)))
//                .AppendCacheCheckpoint(mlContext)
//                .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
//                .Append(mlContext.Transforms.Conversion.MapKeyToValue(
//                    outputColumnName: "PredictedLabel",
//                    inputColumnName: "PredictedLabel"));

//            // Bước 4: Huấn luyện mô hình
//            Console.WriteLine("Đang huấn luyện mô hình...");
//            var model = pipeline.Fit(trainingData);

//            // Bước 5: Đánh giá mô hình
//            Console.WriteLine("Đang đánh giá mô hình...");
//            var predictions = model.Transform(testingData);
//            var metrics = mlContext.MulticlassClassification.Evaluate(
//                data: predictions,
//                labelColumnName: "Label",
//                predictedLabelColumnName: "PredictedLabel");

//            // In kết quả đánh giá
//            Console.WriteLine($"Độ chính xác (MicroAccuracy): {metrics.MicroAccuracy:P2}");
//            Console.WriteLine($"Độ chính xác (MacroAccuracy): {metrics.MacroAccuracy:P2}");
//            Console.WriteLine($"Log Loss: {metrics.LogLoss:F2}");

//            // Bước 6: Lưu mô hình
//            Console.WriteLine("Đang lưu mô hình...");
//            mlContext.Model.Save(model, trainingData.Schema, _modelPath);
//            Console.WriteLine($"Mô hình đã được lưu tại {_modelPath}");

//            Console.WriteLine("Huấn luyện mô hình hoàn tất.");
//        }
//    }
//}
using System;
using System.IO;
using System.Collections.Generic;

namespace ModelTrainer
{
    class Program
    {
        private static readonly string _dataPath = @"Data\accent_data.csv";
        private static readonly string _dictionaryPath = @"Data\dictionary.txt";

        static void Main(string[] args)
        {
            // Tạo thư mục Data nếu chưa tồn tại
            Directory.CreateDirectory("Data");

            // Bước 1: Tải dữ liệu
            Console.WriteLine("Đang tải dữ liệu...");
            if (!File.Exists(_dataPath))
            {
                Console.WriteLine($"Không tìm thấy tệp dữ liệu tại đường dẫn: {_dataPath}");
                return;
            }
            var lines = File.ReadAllLines(_dataPath);

            // Bước 2: Xây dựng từ điển các cụm từ
            Console.WriteLine("Đang xây dựng từ điển...");
            var phraseSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (!string.IsNullOrEmpty(trimmedLine))
                {
                    // Thêm cụm từ vào từ điển (không phân biệt chữ hoa/thường)
                    phraseSet.Add(trimmedLine);
                }
            }

            // Bước 3: Lưu từ điển
            Console.WriteLine("Đang lưu từ điển...");
            File.WriteAllLines(_dictionaryPath, phraseSet);

            Console.WriteLine($"Từ điển đã được lưu tại {_dictionaryPath}");
            Console.WriteLine("Hoàn tất.");
        }
    }
}

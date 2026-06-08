using ClientApp.Models;
using ClientApp.Services;
using ClientApp.Logging;
using Serilog;

//HTTP
using System.Text.Json;

namespace ClientApp
{
    class Program
    {

        // JsonSerializerOptions を再利用のためにキャッシュする
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };


        static async Task Main(string[] args)
        {

            LoggerConfig.Configure();
            Log.Information("クライアントアプリが起動しました");
            //
            var api = new ApiClientService();


            // メニューを表示してユーザー操作を受付
            ConsoleColor originalColor = Console.ForegroundColor;
            //コンソールに表示
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("========== クライアントリクエスト ==========");
            // Options in Yellow
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("追加するには「1」を入力");
            Console.WriteLine("全取得するには「2」を入力");
            Console.WriteLine("更新するには「3」を入力");
            Console.WriteLine("削除するには「4」を入力");
            Console.WriteLine("LINQ エラーレコードするには「5」を入力");
            Console.WriteLine("LINQ 金額 Dese するには「6」を入力");
            Console.WriteLine("LINQ 金額 平均 するには「7」を入力");
            // Exit option in Red
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("終了するには「8」を入力");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=========================================");

            
            Console.ForegroundColor = originalColor;

            while (true)
            {

                Console.WriteLine("リクエストを入力してください。");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await Add(api);
                        break;

                    case "2":
                        await GetAll(api);
                        break;

                    case "3":
                        await Update(api);
                        break;

                    case "4":
                        await Delete(api);
                        break;

                    case "5":
                        await ShowErrorRecords(api);
                        break;

                    case "6":
                        await ShowAmountDesc(api);
                        break;

                    case "7":
                        await ShowAverage(api);
                        break;

                    case "8":
                        return;
                }

            }
        }

        // サーバーから全件データを取得する共通メソッド
        // ***********LINQ操作******************
       
        static async Task<List<DeviceLog>>
       GetRecords(ApiClientService api)
        {
            var request =
                new ClientRequest
                {
                    Action = "GetAll"
                };

            var response =
                await api.SendAsync(request);

            // null-safe handling for response.Data and reuse cached options
            string json = response?.Data?.ToString();

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<DeviceLog>();
            }

            return JsonSerializer
                .Deserialize<List<DeviceLog>>(json, JsonOptions)
                ?? new List<DeviceLog>();
        }

        // ErrorCodeが設定されているデータを抽出
        static async Task ShowErrorRecords(ApiClientService api)
        {
            var records = await GetRecords(api);

            var errors = records.Where(x => !string.IsNullOrWhiteSpace(x.ErrorCode)).ToList();

            Console.WriteLine();
            Console.WriteLine("===== エラーレコード =====");

            //LINQ Operation 1 .Where()
            errors.ForEach(x =>
                Console.WriteLine(
                    $"{x.DeviceName} | {x.ErrorCode}"));
        }

        // Amountを降順で並び替え
        static async Task ShowAmountDesc(ApiClientService api)
        {
            var records = await GetRecords(api);

            //LINQ Operation 2 .OrderByDescending()
            var sorted =
                records
                .OrderByDescending(
                    x => x.Amount)
                .ToList();

            Console.WriteLine();
            Console.WriteLine(
                "===== 金額 Desc =====");

            sorted.ForEach(x =>
                Console.WriteLine(
                    $"{x.DeviceName} : {x.Amount}"));
        }

        // エラー無しデータの平均金額を算出

        static async Task ShowAverage(ApiClientService api)
        {
            var records = await GetRecords(api);


            //LINQ Operation 3 .where() ,.Average() Calculate average amount for records without errors.
            double avg =
                records
                .Where(x =>
                    string.IsNullOrWhiteSpace(
                        x.ErrorCode))
                .DefaultIfEmpty()
                .Average(x =>
                    (double)x.Amount);

            Console.WriteLine();

            Console.WriteLine(
                $"平均金額 = {avg:F2}");
        }

        // 登録用データを入力
        static async Task Add(ApiClientService api)
        {
            Console.Write("Name: ");
            string name =Console.ReadLine();

            Console.Write("Amount: ");
            decimal amount = decimal.Parse(Console.ReadLine());

            Console.Write("Error: ");
            string error = Console.ReadLine();

            var request = new ClientRequest
                {
                    Action = "Add",

                    DeviceLog =
                        new DeviceLog
                        {
                            DeviceName = name,
                            Amount = amount,
                            ErrorCode = error
                        }
                };

            var response = await api.SendAsync(request);

            Console.WriteLine(response?.Message);
        }

        // サーバーから全件データを取得
        static async Task GetAll(ApiClientService api)
        {
            var request =
                new ClientRequest
                {
                    Action = "GetAll"
                };

            var response = await api.SendAsync(request);

            Console.WriteLine(response?.Message);
            string json = response?.Data?.ToString();

            if (string.IsNullOrWhiteSpace(json))
            {
                Console.WriteLine();
                Console.WriteLine("No records returned.");
                return;
            }

            // キャッシュされたJsonOptionsを再利用し、null結果を防ぐ
            var records = JsonSerializer.Deserialize<List<DeviceLog>>(json, JsonOptions)
            ?? new List<DeviceLog>();

            Console.WriteLine();

            Console.WriteLine(
                "--------------------------------------------------------------");

            Console.WriteLine(
                $"{"ID",-5} {"Device",-15} {"Amount",-10} {"Error",-10}");

            Console.WriteLine(
                "--------------------------------------------------------------");

            foreach (var item in records)
            {
                Console.WriteLine(
                    $"{item.Id,-5} " +
                    $"{item.DeviceName,-15} " +
                    $"{item.Amount,-10} " +
                    $"{item.ErrorCode,-10}");
            }

            Console.WriteLine(
                "--------------------------------------------------------------");
        }

        // 更新対象データを入力
        static async Task Update(ApiClientService api)
        {
            Console.Write("Id: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Name: ");
            string name = Console.ReadLine();

            Console.Write("Amount: ");
            decimal amount = decimal.Parse(Console.ReadLine());

            Console.Write("Error: ");
            string error = Console.ReadLine();

            var request =
                new ClientRequest
                {
                    Action = "Update",
                    DeviceLog = new DeviceLog
                    {
                        Id = id,
                        DeviceName = name,
                        Amount = amount,
                        ErrorCode = error
                    }
                };

            var response = await api.SendAsync(request);
            Console.WriteLine(response?.Message);
        }


        // 削除対象IDを入力
        static async Task Delete(ApiClientService api)
        {
            Console.Write("Id: ");
            int id = int.Parse(Console.ReadLine());
            var request =
                new ClientRequest
                {
                    Action = "Delete",
                    Id = id
                };
            var response = await api.SendAsync(request);
            Console.WriteLine(response?.Message);

        }

    }
}




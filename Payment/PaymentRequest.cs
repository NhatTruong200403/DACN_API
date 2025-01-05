using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GoWheels_WebAPI.Payment
{
    public static class PaymentRequest
    {
        public static async Task<string> SendPaymentRequestAsync(string endpoint, string postJsonString)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(postJsonString, Encoding.UTF8, "application/json");

                    // Set timeout cho HttpClient
                    client.Timeout = TimeSpan.FromSeconds(15); // tương tự như Timeout trong HttpWebRequest

                    // Gửi yêu cầu POST bất đồng bộ
                    HttpResponseMessage response = await client.PostAsync(endpoint, content);

                    // Đảm bảo yêu cầu đã thành công
                    /response.EnsureSuccessStatusCode();

                    // Đọc nội dung phản hồi bất đồng bộ
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    return jsonResponse;
                }
            }
            catch (HttpRequestException e)
            {
                // Xử lý lỗi trong khi gửi yêu cầu
                return $"Request error: {e.Message}";
            }
            catch (TaskCanceledException e)
            {
                // Xử lý lỗi do hết thời gian chờ (timeout)
                return $"Request timed out: {e.Message}";
            }
        }
    }
}

namespace iRechargeDemoApi.Models.Application
{
    public class ApiResponseModel<T>
    {
        public bool Result { get; set; }
        public string Message { get; set; } = "";
        public T? Data { get; set; }
    }
}

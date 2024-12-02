namespace LiveOrderBook.Presentation.BlazorWebApp.ViewModels;

public class ResultViewModel<T>
{
    public T Data { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public int StatusCode { get; set; }

    public ResultViewModel() { }

    public ResultViewModel(T data, int statusCode)
    {
        Data = data;
        StatusCode = statusCode;
    }

    public ResultViewModel(T data, List<string> errors, int statusCode)
    {
        Data = data;
        Errors = errors;
        StatusCode = statusCode;
    }

    public ResultViewModel(List<string> errors, int statusCode)
    {
        Errors = errors;
        StatusCode = statusCode;
    }

    public ResultViewModel(string error, int statusCode)
    {
        Errors.Add(error);
        StatusCode = statusCode;
    }
}
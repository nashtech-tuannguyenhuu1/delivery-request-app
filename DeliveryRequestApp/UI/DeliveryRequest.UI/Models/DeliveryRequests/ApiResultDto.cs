namespace DeliveryRequest.UI.Models.DeliveryRequests;

/// <summary>Mirrors Core.Domain.ResultModel&lt;T&gt; returned by the API.</summary>
public class ApiResultDto<T>
{
    public T? Data { get; set; }

    public bool IsError { get; set; }

    public string? ErrorMessage { get; set; }
}

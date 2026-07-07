using System.ComponentModel.DataAnnotations;

namespace DeliveryRequest.UI.Models.DeliveryRequests;

public class DeliveryRequestFormViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Pickup address is required.")]
    [StringLength(400, ErrorMessage = "Pickup address cannot exceed 400 characters.")]
    public string PickupAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "Delivery address is required.")]
    [StringLength(400, ErrorMessage = "Delivery address cannot exceed 400 characters.")]
    public string DeliveryAddress { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
}

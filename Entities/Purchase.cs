namespace Qydha.Entities;

public class Purchase
{
    public Guid Id { get; set; }
    public string IAPHub_Purchase_Id { get; set; } = string.Empty;
    public Guid User_Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime Purchase_Date { get; set; }
    public string ProductSku { get; set; } = string.Empty;

    public int Number_Of_Days
    {
        get; set;
    }


}

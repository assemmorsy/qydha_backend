namespace Qydha.Models;

public class WebHookDto
{
    
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime CreateDate { get; set; }
    public WebhookData? Data { get; set; }
    public Guid? OldUserId { get; set; }
    public Guid? NewUserId { get; set; }

    public override string ToString()
    {
        return
        @$"
        Transaction type : {Type}   
        Transaction id : {Id}   
        CreateDate : {CreateDate}
        OldUserId : {OldUserId}
        NewUserId : {NewUserId}
        Data : 
            userId : {Data?.UserId}
            PurchaseId : {Data?.Id}
            PurchaseDate : {Data?.PurchaseDate}
            ProductSku : {Data?.ProductSku}
        ";
    }
}


public class WebhookData
{
    public string Id { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public string ProductSku { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}
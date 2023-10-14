using System.Data;
using Dapper;
using Microsoft.Extensions.Options;
using Qydha.Entities;
using Qydha.Helpers;
using Qydha.Models;

namespace Qydha.Services;

public class SubscriptionRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IUserRepo _userRepo;
    private readonly SubscriptionSetting _subscriptionSetting;


    public SubscriptionRepo(IDbConnection dbConnection, IUserRepo userRepo, IOptions<SubscriptionSetting> subscriptionSetting)
    {
        _dbConnection = dbConnection;
        _userRepo = userRepo;
        _subscriptionSetting = subscriptionSetting.Value;
    }
    public async Task<OperationResult<bool>> AddPurchase(Purchase purchase)
    {
        var findUserRes = await _userRepo.FindUserById(purchase.User_Id.ToString());
        if (!findUserRes.IsSuccess)
            return new() { Error = findUserRes.Error };
        if (findUserRes.Data!.Free_Subscription_Used >= _subscriptionSetting.FreeSubscriptionsAllowed)
            return new() { Error = new() { Code = ErrorCodes.FreeSubscriptionUsedExceededTheAllowedNumber, Message = "Free Subscription Used Exceeded the Allowed Number" } };

        var sql = @"INSERT INTO 
                    Purchases ( iaphub_purchase_id , user_id , type , purchase_date , productSku ,number_of_days) 
                    VALUES ( @IAPHub_Purchase_Id,  @User_Id , @Type ,@Purchase_Date ,@ProductSku , @Number_Of_Days )
                    returning Id;";

        await _dbConnection.QuerySingleAsync<Guid>(sql,  purchase );

        return new() { Data = true, Message = "Purchase Saved Successfully." };
    }
}

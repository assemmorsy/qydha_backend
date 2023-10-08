using System.Data;
using Dapper;
using Qydha.Entities;

namespace Qydha.Services;

public class UpdatePhoneOTPRequestRepo
{
    private readonly IDbConnection _dbConnection;
    public UpdatePhoneOTPRequestRepo(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }
    public async Task<UpdatePhoneRequest> AddAsync(UpdatePhoneRequest otp_request)
    {
        var sql = @"INSERT INTO 
                    update_phone_requests (  phone , otp , created_on , user_id ) 
                    VALUES ( @Phone , @OTP ,@Created_On ,@User_Id )
                    RETURNING Id;";

        var otp_requestId = await _dbConnection.QuerySingleAsync<Guid>(sql, otp_request);
        otp_request.Id = otp_requestId;
        return otp_request;
    }

    public async Task<UpdatePhoneRequest?> FindAsync(string reqId)
    {
        var sql = @"select *  from update_phone_requests  where id = @requestId";
        var otp_request = await _dbConnection.QuerySingleOrDefaultAsync<UpdatePhoneRequest?>(sql, new { requestId = Guid.Parse(reqId) });
        return otp_request;
    }

}

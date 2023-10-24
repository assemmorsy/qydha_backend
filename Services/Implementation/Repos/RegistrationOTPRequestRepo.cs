using System.Data;
using Dapper;
using Qydha.Entities;

namespace Qydha.Services;

public class RegistrationOTPRequestRepo
{
    private readonly IDbConnection _dbConnection;
    public RegistrationOTPRequestRepo(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }
    public async Task<RegistrationOTPRequest> AddAsync(RegistrationOTPRequest otp_request)
    {
        var sql = @"INSERT INTO 
                    registration_otp_request ( username,  password_hash, phone ,otp ,created_on ,user_id , fcm_token ) 
                    VALUES ( @Username , @Password_Hash, @Phone, @OTP ,@Created_On ,@User_Id  ,@FCM_Token)
                    RETURNING Id;";
        var otp_requestId = await _dbConnection.QuerySingleAsync<Guid>(sql, otp_request);
        otp_request.Id = otp_requestId;
        return otp_request;
    }

    public async Task<RegistrationOTPRequest?> FindAsync(string reqId)
    {
        var sql = @"select *  from registration_otp_request where id = @requestId";
        var otp_request = await _dbConnection.QuerySingleOrDefaultAsync<RegistrationOTPRequest?>(sql, new { requestId = Guid.Parse(reqId) });
        return otp_request;
    }

}

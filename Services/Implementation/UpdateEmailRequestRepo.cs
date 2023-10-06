using System.Data;
using Dapper;
using Qydha.Entities;

namespace Qydha.Services;

public class UpdateEmailRequestRepo
{
    private readonly IDbConnection _dbConnection;
    public UpdateEmailRequestRepo(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }
    public async Task<UpdateEmailRequest> AddAsync(UpdateEmailRequest emailRequest)
    {
        var sql = @"INSERT INTO 
                    update_email_requests (  id ,email , otp , created_on , user_id ) 
                    VALUES ( @Id , @Email , @OTP ,@Created_On ,@User_Id )
                    RETURNING Id;";

        var emailRequestId = await _dbConnection.QuerySingleAsync<Guid>(sql, emailRequest);
        emailRequest.Id = emailRequestId;
        return emailRequest;
    }

    public async Task<UpdateEmailRequest?> FindAsync(string reqId)
    {
        var sql = @"select * from update_email_requests  where id = @requestId";
        var emailRequest = await _dbConnection.QuerySingleOrDefaultAsync<UpdateEmailRequest?>(sql, new { requestId = Guid.Parse(reqId) });
        return emailRequest;
    }

}

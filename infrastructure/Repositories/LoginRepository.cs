using domain.DTOs;
using domain.DTOs.Requests.Login;
using domain.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace infrastructure.Repositories
{
    public class LoginRepository : ILogin
    {
        private readonly sqlWrapper _sqlWrapper;
        public LoginRepository(sqlWrapper sql_wrapper)
        {
            _sqlWrapper = sql_wrapper;
        }

        public async Task<DataSet> loginByPasswordAsync(LoginRequestDto request_body)
        {
            SqlParameter[] parameters = 
               {
                    new SqlParameter() {ParameterName="flag", DbType = DbType.String, Value="check_login_by_password" },
                    new SqlParameter() {ParameterName="username", DbType = DbType.String, Value=request_body.username },
                    new SqlParameter() {ParameterName="encrypted_password", DbType = DbType.String, Value=request_body.password }
               };
            return await _sqlWrapper.getDataSetAfterProcedureExecuteAsync("conn_one_box", "users.adm_manage_user", parameters);
        }

        public async Task<DataTable> createUserAsync(userCreationRequestDto request_body, tokenInfoResponseDto token)
        {
            SqlParameter[] parameters =
               {
                    new SqlParameter() {ParameterName="flag", DbType = DbType.String, Value="create_user" },
                    new SqlParameter() {ParameterName="ukey", DbType = DbType.String, Value=request_body.ukey },
                    new SqlParameter() {ParameterName="username", DbType = DbType.String, Value=request_body.username },
                    new SqlParameter() {ParameterName="encrypted_password", DbType = DbType.String, Value=request_body.encrypted_password },
                    new SqlParameter() {ParameterName="access_token", DbType = DbType.String, Value=token.access_token },
                    new SqlParameter() {ParameterName="access_token_expiry", DbType = DbType.DateTime, Value=token.access_token_expirty},
                    new SqlParameter() {ParameterName="refresh_token", DbType = DbType.String, Value=token.refresh_token},
                    new SqlParameter() {ParameterName="refresh_token_expiry", DbType = DbType.DateTime, Value=token.refresh_token_expirty},
                    new SqlParameter() {ParameterName="issuer", DbType = DbType.String, Value=token.issuer},
                    new SqlParameter() {ParameterName="audience", DbType = DbType.String, Value=token.audience}
               };
            return await _sqlWrapper.getDataTableAfterProcedureExecuteAsync("conn_one_box", "users.adm_manage_user", parameters);
        }

        public async Task<DataTable> loginChecksAsync(LoginRequestDto request_body)
        {
            SqlParameter[] parameters =
               {
                    new SqlParameter() {ParameterName="flag", DbType = DbType.String, Value="verify_user" },
                    new SqlParameter() {ParameterName="username", DbType = DbType.String, Value=request_body.username }
               };
            return await _sqlWrapper.getDataTableAfterProcedureExecuteAsync("conn_one_box", "users.adm_manage_user", parameters);
        }

        public async Task<DataTable> updateLoginAttempts(string ukey)
        {
            SqlParameter[] parameters =
               {
                    new SqlParameter() {ParameterName="flag", DbType = DbType.String, Value="user_login_attempt_log" },
                    new SqlParameter() {ParameterName="ukey", DbType = DbType.String, Value=ukey }
               };
            return await _sqlWrapper.getDataTableAfterProcedureExecuteAsync("conn_one_box", "users.adm_manage_user", parameters);
        }
    }
}

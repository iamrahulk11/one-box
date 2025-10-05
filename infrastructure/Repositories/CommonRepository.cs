using domain.DTOs;
using domain.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace infrastructure.Repositories
{
    public class CommonRepository : ICommon
    {
        private readonly sqlWrapper _sqlWrapper;
        public CommonRepository(sqlWrapper sql_wrapper)
        {
            _sqlWrapper = sql_wrapper;
        }

        public async Task userTokenInfoAsync(string ukey, tokenInfoResponseDto token)
        {
            SqlParameter[] parameters =
               {
                    new SqlParameter() {ParameterName="flag", DbType = DbType.String, Value="save_token_info" },
                    new SqlParameter() {ParameterName="ukey", DbType = DbType.String, Value=ukey },
                    new SqlParameter() {ParameterName="access_token", DbType = DbType.String, Value=token.access_token },
                    new SqlParameter() {ParameterName="access_token_expiry", DbType = DbType.DateTime, Value=token.access_token_expirty},
                    new SqlParameter() {ParameterName="refresh_token", DbType = DbType.String, Value=token.refresh_token},
                    new SqlParameter() {ParameterName="refresh_token_expiry", DbType = DbType.DateTime, Value=token.refresh_token_expirty},
                    new SqlParameter() {ParameterName="issuer", DbType = DbType.String, Value=token.issuer},
                    new SqlParameter() {ParameterName="audience", DbType = DbType.String, Value=token.audience}
               };
            await _sqlWrapper.getDataTableAfterProcedureExecuteAsync("conn_one_box", "users.adm_manage_user", parameters);
        }
    }
}

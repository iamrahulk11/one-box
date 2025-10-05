using domain.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace infrastructure.Repositories
{
    public class UserRepository : IUser
    {
        private readonly sqlWrapper _sqlWrapper;
        public UserRepository(sqlWrapper sql_wrapper)
        {
            _sqlWrapper = sql_wrapper;
        }
        public async Task<DataTable> fetchUserDetailsAsync(string ukey)
        {
            SqlParameter[] parameters =
               {
                    new SqlParameter() {ParameterName="flag", DbType = DbType.String, Value="check_login_by_password" },
                    new SqlParameter() {ParameterName="ukey", DbType = DbType.String, Value=ukey },
               };
            return await _sqlWrapper.getDataTableAfterProcedureExecuteAsync("conn_one_box", "users.adm_manage_user", parameters);
        }
    }
}

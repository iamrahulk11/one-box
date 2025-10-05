using DataBridge;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;


namespace infrastructure
{
    public class sqlWrapper
    {
        #region Declarations
        private readonly IConfiguration _iConfig;
        //SQL Helper
        sqlHelper _sqlHelper = sqlHelper.GetInstance();
        #endregion

        public sqlWrapper(IConfiguration iConfig)
        {
            _iConfig = iConfig;
        }

        internal async Task<DataSet> getDataSetAfterProcedureExecuteAsync(string conName, string procedureName, SqlParameter[] parameters)
        {
            return await _sqlHelper.getDataSetAfterProcedureExecuteAsync(_iConfig.GetConnectionString(conName), procedureName, parameters);
        }

        internal async Task<DataTable> getDataTableAfterProcedureExecuteAsync(string conName, string procedureName, SqlParameter[] parameters)
        {
            return await _sqlHelper.getDataTableAfterProcedureExecuteAsync(_iConfig.GetConnectionString(conName), procedureName, parameters);
        }
    }
}

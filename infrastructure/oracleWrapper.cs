using DataBridge;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace infrastructure
{
    public class oracleWrapper
    {
        #region Declarations
        private readonly IConfiguration _iConfig;
        //Oracle Helper
        oracleHelper _oracleHelper = oracleHelper.GetInstance();
        #endregion

        public oracleWrapper(IConfiguration iConfig)
        {
            _iConfig = iConfig;
        }

        internal async Task<DataSet> getDataSetAfterProcedureExecuteAsync(string conName, string procedureName, OracleParameter[] parameters)
        {
            return await _oracleHelper.getDataSetAfterProcedureExecuteAsync(_iConfig.GetConnectionString(conName), procedureName, parameters);
        }

        internal async Task<DataTable> getDataTableAfterProcedureExecuteAsync(string conName, string procedureName, OracleParameter[] parameters)
        {
            return await _oracleHelper.getDataTableAfterProcedureExecuteAsync(_iConfig.GetConnectionString(conName), procedureName, parameters);
        }
    }
}

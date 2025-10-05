using domain.DTOs;
using helpers;
using System.Data;
using System.Net;

namespace domain
{
    public class validator
    {
        baseResponseDto<object> _baseResponse = new();
        resultResponseDto _resultResponse = new();
        int statusCodes = (int)HttpStatusCode.OK;

        //Base validation with flag check with flag_message
        public (int, baseResponseDto<object>) validate(DataTable _dtResponse)
        {
            if (_dtResponse == null || _dtResponse.Rows.Count == 0)
            {
                _resultResponse.flag = Convert.ToInt16(_dtResponse.Rows[0]["flag"].ToString());
                _resultResponse.flag_message = responseMessages.DATA_NOT_FOUND;
                statusCodes = (int)HttpStatusCode.BadRequest;
                _baseResponse.Result = _resultResponse;
                return (statusCodes, _baseResponse);
            }

            if (_dtResponse.Rows[0]["flag"].ToString() != "0")
            {
                _resultResponse.flag = Convert.ToInt16(_dtResponse.Rows[0]["flag"].ToString());
                _resultResponse.flag_message = _dtResponse.Rows[0]["flag_message"].ToString();
                statusCodes = (int)HttpStatusCode.BadRequest;
                _baseResponse.Result = _resultResponse;
                return (statusCodes, _baseResponse);
            }

            return (statusCodes, _baseResponse);
        }

        //Base validation without flag check without Flag Message
        public (int, baseResponseDto<object>) validateWithoutFlag(DataTable _dtResponse)
        {
            if (_dtResponse == null || _dtResponse.Rows.Count != 0)
            {
                _resultResponse.flag = Convert.ToInt16(_dtResponse.Rows[0]["flag"].ToString());
                _resultResponse.flag_message = responseMessages.DATA_NOT_FOUND;
                statusCodes = (int)HttpStatusCode.BadRequest;
                _baseResponse.Result = _resultResponse;
                return (statusCodes, _baseResponse);
            }

            return (statusCodes, _baseResponse);
        }

        //Base validation for DataSet
        public (int, baseResponseDto<object>) validateDataSet(DataSet _dsResponse)
        {
            if (_dsResponse == null || _dsResponse.Tables.Count != 0)
            {
                _resultResponse.flag = Convert.ToInt16(_dsResponse.Tables[0].Rows[0]["flag"].ToString());
                _resultResponse.flag_message = responseMessages.DATA_NOT_FOUND;
                statusCodes = (int)HttpStatusCode.BadRequest;
                _baseResponse.Result = _resultResponse;
                return (statusCodes, _baseResponse);
            }

            return (statusCodes, _baseResponse);
        }
    }
}

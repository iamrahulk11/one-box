using domain.DTOs;
using domain.DTOs.Requests.Login;
using domain.DTOs.Responses.User;
using domain.Interfaces;
using domain.Wrapper;
using helpers;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace domain.Services
{
    public class LoginService
    {
        baseResponseDto<object> _baseResponse = new();
        resultResponseDto _resultResponse = new();
        int statusCodes = (int)HttpStatusCode.OK;

        private readonly ILogin _login;
        private readonly ICommon _common;
        private readonly IUser _user;
        private readonly IConfiguration _configs;
        private readonly BloomFilterWrapper _bloomFilter;
        private readonly BloomFilterPopulator _bloomFilterPopulate;

        public LoginService(ILogin login, IConfiguration configs, BloomFilterWrapper bloomFilterWrapper, BloomFilterPopulator bloomFilterPopulate, IUser user, ICommon common)
        {
            _login = login;
            _configs = configs;
            _bloomFilter = bloomFilterWrapper;
            _bloomFilterPopulate = bloomFilterPopulate;
            _user = user;
            _common = common;
        }
        public async Task<(int, baseResponseDto<object>)> processLoginByPasswordAsync(LoginRequestDto request_body)
        {
            //get login details from database
            DataTable _dtLoginDetails = await _login.loginChecksAsync(request_body);
            //validating the response
            var _validationResponse = new validator().validate(_dtLoginDetails);
            if (_validationResponse.Item1 != 200)
            {
                return _validationResponse;
            }

            string ukey = _dtLoginDetails.Rows[0]["ukey"].ToString();
            string hashedPassword = _dtLoginDetails.Rows[0]["encrypted_password"].ToString();

            // validate password
            bool _verifyPassword = PasswordHelper.pwdVerifyPassword(request_body.password, hashedPassword);
            if (!_verifyPassword)
            {                
                DataTable _dtLoginAttemptLog = await _login.updateLoginAttempts(ukey);
                _validationResponse = new validator().validate(_dtLoginAttemptLog);
                if (_validationResponse.Item1 != 200)
                {
                    return _validationResponse;
                }
            }

            //fetch user details
            DataTable _dtUserDetails = await _user.fetchUserDetailsAsync(ukey);            
            userProfileResponseDto _userProfileResponseDto = new dataTableConversion().ToModel<userProfileResponseDto>(_dtUserDetails);
            
            //bind token
            tokenInfoResponseDto _token = await jwtToken(ukey);
            _userProfileResponseDto.access_token = _token.access_token;
            _userProfileResponseDto.refresh_token = _token.refresh_token;

            //success api response
            _resultResponse.flag_message = _dtLoginDetails.Rows[0]["flag_message"].ToString();
            _baseResponse.Result = _resultResponse;
            _baseResponse.Data = _userProfileResponseDto;

            return (statusCodes, _baseResponse);
        }
        public async Task<(int, baseResponseDto<object>)> processRefreshTokenAsync(refreshTokenRequestDto request_body)
        {
            string _secretKey = _configs["jwtConfiguration:Secret"].ToString();
            string _issuer = _configs["jwtConfiguration:Issuer"].ToString();
            string _audience = _configs["jwtConfiguration:Audience"].ToString();
            int _accessTokenExpiryInMin = Convert.ToInt32(_configs["jwtConfiguration:AccessTokenExpiryInMin"].ToString());

            //validate token
            JwtSecurityToken _tokenValid = new jwt(_secretKey, _issuer, _audience, _accessTokenExpiryInMin).validateToken(request_body.refresh_token);
            if (_tokenValid == null)
            {
                _resultResponse.flag_message = responseMessages.TOKEN_EXPIRE;
                statusCodes = (int)HttpStatusCode.BadRequest;
                _baseResponse.Result = _resultResponse;
                return (statusCodes, _baseResponse);
            }

            //fetching username from valid token
            string username = _tokenValid.Claims.FirstOrDefault(c => c.Type == "username").Value.ToString();
            //bind token
            tokenResponseDto _token = await jwtToken(username);

            //success api response
            _resultResponse.flag_message = responseMessages.TOKEN_REFRESHED_SUCCESSFULLY;
            _baseResponse.Result = _resultResponse;
            _baseResponse.Data = _token;

            return (statusCodes, _baseResponse);
        }
        public async Task<(int, baseResponseDto<object>)> processCreateUserAsync(userCreationRequestDto request_body)
        {
            //hash password
            request_body.encrypted_password = PasswordHelper.pwdHashPassword(request_body.password);

            //generate token
            tokenInfoResponseDto _token = await jwtToken(request_body.ukey);

            //get create user 
            DataTable _dtSaveUserDetails = await _login.createUserAsync(request_body, _token);
            //validating the response
            var _validationResponse = new validator().validate(_dtSaveUserDetails);
            if (_validationResponse.Item1 != 200)
            {
                return _validationResponse;
            }

            userProfileResponseDto _userProfileResponseDto = new dataTableConversion().ToModel<userProfileResponseDto>(_dtSaveUserDetails);

            //Populate this new user into existing bloom filter file
            await _bloomFilterPopulate.PopulateAsync(request_body.username, true);
            
            _userProfileResponseDto.access_token = _token.access_token;
            _userProfileResponseDto.refresh_token = _token.refresh_token;

            //success api response
            _resultResponse.flag_message = responseMessages.USER_CREATED_SUCCESSFULLY;
            _baseResponse.Result = _resultResponse;
            _baseResponse.Data = _userProfileResponseDto;

            return (statusCodes, _baseResponse);
        }
        public (int, baseResponseDto<object>) processUserExistsAsync(usernameRequestDto request_body)
        {
            bool _userExists = _bloomFilter.Contains(request_body.username);
            if (_userExists)
            {
                _resultResponse.flag = 1;
                _resultResponse.flag_message = responseMessages.USERNAME_ALREADY_EXISTS;
                statusCodes = (int)HttpStatusCode.BadRequest;
                _baseResponse.Result = _resultResponse;
                return (statusCodes, _baseResponse);
            }

            _resultResponse.flag_message = responseMessages.USERNAME_AVAILABLE;
            _baseResponse.Result = _resultResponse;
            _baseResponse.Data = null;

            return (statusCodes, _baseResponse);
        }

        private async Task<tokenInfoResponseDto> jwtToken(string ukey)
        {
            tokenInfoResponseDto _tokens = new();

            List<Claim> authClaims = new List<Claim>
            {
                new Claim("ukey",  ukey),
            };

            string _secretKey = _configs["jwtConfiguration:Secret"].ToString();
            string _issuer = _configs["jwtConfiguration:Issuer"].ToString();
            string _audience = _configs["jwtConfiguration:Audience"].ToString();
            Int32 _accessTokenExpiryInMin = Convert.ToInt32(_configs["jwtConfiguration:AccessTokenExpiryInMin"].ToString());
            Int32 _refreshTokenExpiryInMin = Convert.ToInt32(_configs["jwtConfiguration:RefreshTokenExpiryInMin"].ToString());

            //generating access_token and bind info
            _tokens.access_token = new jwt(_secretKey, _issuer, _audience, _accessTokenExpiryInMin).createToken(authClaims);
            _tokens.refresh_token = new jwt(_secretKey, _issuer, _audience, _refreshTokenExpiryInMin).createToken(authClaims);
            _tokens.issuer = _issuer;
            _tokens.audience = _audience;
            _tokens.access_token_expirty = DateTime.Now.ToLocalTime().AddMinutes(_accessTokenExpiryInMin);
            _tokens.refresh_token_expirty = DateTime.Now.ToLocalTime().AddMinutes(_refreshTokenExpiryInMin);

            await _common.userTokenInfoAsync(ukey, _tokens);

            return _tokens;
        }
    }
}

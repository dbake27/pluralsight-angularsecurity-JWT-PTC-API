using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PtcApi.Model;

namespace PtcApi.Security
{
    public class SecurityManager

    {
        private JwtSettings _settings = null;
        public SecurityManager(JwtSettings settings)   
        {
            _settings = settings;
        } 
        public AppUserAuth ValidateUser(AppUser user)
        {
            AppUserAuth ret = new AppUserAuth();
            AppUser authuser = null;

            using (var db = new PtcDbContext())
            {
                //attempt to validate User
                authuser = db.Users.Where(
                    u => u.UserName.ToLower() == user.UserName.ToLower()  //user.UserName is passed from angular app
                    && u.Password == user.Password).FirstOrDefault();     //user.Password is passed from angular app
            }

            if (authuser != null)
            {  //build user security object
                ret = BuildUserAuthObject(authuser);
            }
            return ret;
        }
        protected List<AppUserClaim> GetUserClaims(AppUser authUser)
        {
            List<AppUserClaim> list = new List<AppUserClaim>();

            try
            {
                using (var db = new PtcDbContext())
                {
                    list = db.Claims.Where(
                        u => u.UserId == authUser.UserId).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Exception trying to retreive user claims.", ex);
            }
            return list;
        }

           protected string BuildJwtToken(AppUserAuth authUser) 
           {
               SymmetricSecurityKey key = new SymmetricSecurityKey(
                   Encoding.UTF8.GetBytes( _settings.Key));

               //create standard JWT Claims
               List<Claim>  jwtClaims = new List<Claim>();
               jwtClaims.Add(new Claim(JwtRegisteredClaimNames.Sub,
                authUser.UserName));
               jwtClaims.Add(new Claim(JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString()));
                
                //add custom claims
                jwtClaims.Add(new Claim("isAuthenticated",
                 authUser.IsAuthenticated.ToString().ToLower()));

               jwtClaims.Add(new Claim("canAccessProducts",
                authUser.CanAccessProducts.ToString().ToLower()));

               jwtClaims.Add(new Claim("canAddProduct",
                authUser.CanAddProduct.ToString().ToLower()));

               jwtClaims.Add(new Claim("canSaveProduct",
                authUser.CanSaveProduct.ToString().ToLower()));

               jwtClaims.Add(new Claim("canAccessCategories",
                authUser.CanAccessCategories.ToString().ToLower()));

                jwtClaims.Add(new Claim("canAddCategory",
                authUser.CanAddCategory.ToString().ToLower()));
           


             //create the JwtSecurity Token Object
              var token = new JwtSecurityToken(
              issuer: _settings.Issuer,
              audience: _settings.Audience,
              claims: jwtClaims, 
              notBefore: DateTime.UtcNow,
              expires:  DateTime.UtcNow.AddMinutes(
                  _settings.MinutesToExpiration),
              signingCredentials: new SigningCredentials(key,
                 SecurityAlgorithms.HmacSha256)    
            );
                //create a string representation of the JwtToken
             return new JwtSecurityTokenHandler().WriteToken(token);
        }   
       

        protected AppUserAuth BuildUserAuthObject(AppUser authuser) 
        {
            AppUserAuth ret = new AppUserAuth();
            List<AppUserClaim> claims = new List<AppUserClaim>();

            //set User Properties
            ret.UserName = authuser.UserName;
            ret.IsAuthenticated = true;
            ret.BearerToken = new Guid().ToString();

            //get all claims for user
            claims = GetUserClaims( authuser);

            //loop thru all claims and set property of user object

            foreach (AppUserClaim claim in claims)
            {
                try
                {
                    //todo: check datatype of Claimvalue
                    typeof(AppUserAuth).GetProperty(claim.ClaimType).SetValue(ret, Convert.ToBoolean(claim.ClaimValue), null);
                }
                catch
                {
                }
                
            }
             ret.BearerToken = BuildJwtToken(ret);
            return ret;
        }


    }


}
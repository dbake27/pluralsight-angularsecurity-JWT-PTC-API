using System;
using System.Collections.Generic;
using System.Linq;
using PtcApi.Model;

namespace PtcApi.Security
{
    public class SecurityManager

    {
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
            return ret;
        }


    }


}
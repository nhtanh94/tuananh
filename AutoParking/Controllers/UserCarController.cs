using AutoParking.Entities;
using AutoParking.Models;
using AutoParking.Models.RepsponseModel;
using AutoParking.Models.RequestModel;
using AutoParking.Token;
using AutoParking.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AutoParking.Controllers
{
    public class UserCarController : BaseController
    {
        /// <summary>
        /// LẤY DANH SÁCH  NHÂN VIÊN
        /// </summary>
        /// <param name="Code">
        /// Code: Mã tòa nhà M1,M2
        /// </param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/get-list-user")]
        public HttpResponseMessage GetListUser(string Code)
        {
            if (IsBodyNull(Code))
            {
                return ResponseFail(Constants.BODY_NOT_FOUND);
            }
            if (!CheckCode.checkcode(Code))
            {
                return ResponseFail(Constants.CODEERROR);
            }
            try
            {
                using(DB db = new DB(Code))
                {
                    var listUser = db.UserCars.Where(x => x.IDFunct.Equals("Nh")).Select(x => new
                    {
                        x.ID,
                        x.NameUser,
                        x.Account,
                        x.Sex,
                        x.Working,
                        x.IDFunct
                    }).ToList();
                    return ResponseSuccess(listUser);
                }
               

              
            }
            catch
            {
                return ResponseFail(Constants.FAILD);
            }
        }

    }
}

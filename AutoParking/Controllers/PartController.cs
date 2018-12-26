using AutoParking.Entities;
using AutoParking.Models;
using AutoParking.Models.RequestModel;
using AutoParking.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AutoParking.Controllers
{
    public class PartController : BaseController
    {
        /// <summary>
        /// LẤY DANH SÁCH LOẠI XE
        /// </summary>
        /// <param name="Code">
        ///Code : Mã tòa nhà M1,M2
        /// </param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/list-part")]
        public HttpResponseMessage GetListPart(string Code)
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
                IOrderedEnumerable<Part> listPart;
                using (DB db = new DB(Code))
                {
                    listPart = db.Parts.ToList().OrderBy(x => x.ID);
                 }
             
                return ResponseSuccess(listPart);
            }
            catch
            {
                return ResponseFail(Constants.FAILD);
            }         
           
        }
    }
}

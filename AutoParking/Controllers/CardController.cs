using AutoParking.Models;
using AutoParking.Models.RequestModel;
using AutoParking.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoParking.Entities;
using AutoParking.Models.RepsponseModel;

namespace AutoParking.Controllers
{
    public class CardController : BaseController
    {
      /// <summary>
      /// LẤY DANH SÁCH THẺ XE
      /// </summary>
      /// <param name="Code">
      /// Code : Mã tòa nhà M1,M2
      /// </param>
      /// <returns></returns>
        [HttpGet]
        [Route("api/v1/list-card")]
        public HttpResponseMessage GetListCard(string Code)
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
                var model = new CardViewModel();
                return ResponseSuccess(model.GetResponseCard(Code));
            }
            catch
            {
                return ResponseFail(Constants.FAILD);
            }           
            
        }
        //Có thẻ chênh lệnh giữa thẻ vé tháng và thẻ vãng lai đã tạo vé tháng
        //do 1 số thẻ vé tháng tự gõ mã thẻ nên số thẻ vé tháng nhiều hơn số thẻ vé tháng đã tạo
        //hoặc do thẻ bị xóa đi 1 phần.
        /// <summary>
        /// DANH SÁCH THẺ CHƯA TẠO VÉ THÁNG
        /// </summary>
        /// <param name="Code">
        /// Code : Mã tòa nhà
        /// </param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/list-card-create-ticketmonth")]
        public HttpResponseMessage GetListCardCreateTicketMonth(string Code)
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
                var model = new CardViewModel();
                return ResponseSuccess(model.GetResponseCardCreateTicketMonth(Code));
            }
            catch
            {
                return ResponseFail(Constants.FAILD);
            }

        }
        /// <summary>
        /// XÓA THẺ XE
        /// </summary>
        /// <param name="request">
        /// Code : Mã tòa nhà M1, M2
        /// ID : Mã Thẻ
        /// </param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/v1/delete-card")]
        public HttpResponseMessage DeleteCard([FromBody]RequestDeleteCard request)
        {
            if (IsBodyNull(request))
            {
                return ResponseFail(Constants.BODY_NOT_FOUND);
            }
            if (!CheckCode.checkcode(request.Code))
            {
                return ResponseFail(Constants.CODEERROR);
            }
            try
            {
                SmartCard card;
                using(DB db = new DB(request.Code))
                {
                    card = db.SmartCards.Find(request.ID);
                    db.SmartCards.Remove(card);
                    db.SaveChanges();
                }                                        
                 var model = new CardViewModel();
                 string info = "Xóa thẻ : " + request.ID + "\n   -STT : " + card.Identify + "\n   -Mã thẻ : " + request.ID + "\n   -Loại thẻ : " + model.GetPartName("" + card.Type,request.Code) + "\n   -Sử dụng : " + card.Using;
                    var logModel = new LogViewModel();
                    logModel.AddLog(ActionType.DELETE_CARD_INFO, DateTime.Now, info,request.Code);
                    return ResponseSuccess(Constants.SUCCESS,model.Statistical(request.Code));
                
               
            }
            catch
            {
                return ResponseSuccess(Constants.FAILD);
            }
        }
  
    }
}

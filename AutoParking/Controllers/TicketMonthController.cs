using AutoParking.Entities;
using AutoParking.Models;
using AutoParking.Models.RepsponseModel;
using AutoParking.Models.RequestModel;
using AutoParking.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AutoParking.Controllers
{
    public class TicketMonthController : BaseController
    {
        /// <summary>
        /// LẤY DANH SÁCH VÉ THÁNG
        /// </summary>
        /// <param name="Code">
        /// Code : Mã Tòa Nhà M1,M2
        /// </param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/list-ticket-month")]
        public HttpResponseMessage LisTicketMonth(string Code)
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
                var model = new TicketMonthViewModel();
                return ResponseSuccess(model.ListTicketMonth(Code));
            }
            catch
            {
                return ResponseFail(Constants.FAILD);
            }           
          
        }


        /// <summary>
        /// XÓA VÉ THÁNG
        /// </summary>
        /// <param name="request">
        /// Code : Mã Tòa Nhà M1,M2
        /// rowID: Lấy từ API Danh sách vé tháng
        /// </param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/v1/delete-ticket-month")]
        public HttpResponseMessage DeleteTicketMonth([FromBody]RequestDeleteTicketMonth request)
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
                TicketMonth ticketMonth;
                using(DB db = new DB(request.Code))
                {
                     ticketMonth = db.TicketMonths.SingleOrDefault(x => x.RowID == request.rowID && x.Status < 2);
                    if (ticketMonth == null)
                    {
                        return ResponseFail(Constants.NOT_FOUND);
                    }
                    ticketMonth.Status = 2;
                    ticketMonth.Amount = ticketMonth.ChargesAmount;
                    ticketMonth.Account = Constants.ACCOUNT;
                    ticketMonth.DayUnLimit = DateTime.Now;
                    db.SaveChanges();
                }
              

                new LogViewModel().AddTicketMonthLog(ticketMonth, ActionType.DELETE_TICKET_MONTH_CARD, ticketMonth.Account, request.Code);

                return ResponseSuccess(Constants.SUCCESS);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Xóa thẻ thất bại: " + e.Message);
                return ResponseSuccess(Constants.FAILD);
            }
           
        }
        /// <summary>
        /// NGƯNG SỬ DỤNG VÉ THÁNG
        /// </summary>
        /// <param name="request">
        /// Code: Mã tòa nhà M1,M2
        /// rowID lấy trong danh sách thẻ tháng mỗi thẻ có 1 rowID khác nhau
        /// </param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/v1/block-ticket-month")]
        public HttpResponseMessage BlockTicketMonth([FromBody]RequestDeleteTicketMonth request)
        {
            if (IsBodyNull(request))
            {
                return ResponseFail(Constants.BODY_NOT_FOUND);
            }
            if (!CheckCode.checkcode(request.Code))
            {
                return ResponseFail(Constants.CODEERROR);
            }

            TicketMonth ticketMonth;
            TicketMonth ticketMonthResult;
            using (DB db = new DB(request.Code))
            {
                try
                {
                    ticketMonth = db.TicketMonths.SingleOrDefault(x => x.RowID == request.rowID && x.Status < 2);
                    if (ticketMonth == null)
                    {
                        return ResponseFail(Constants.NOT_FOUND);
                    }
                    ticketMonth.Status = 3;
                    ticketMonth.Amount = ticketMonth.ChargesAmount;
                    ticketMonth.Account = Constants.ACCOUNT;
                    ticketMonth.DayUnLimit = DateTime.Now;
                    db.SaveChanges();

                    var smartCard = db.SmartCards.SingleOrDefault(x => x.ID == ticketMonth.ID);
                    if (smartCard == null)
                    {
                        return ResponseFail(Constants.NOT_FOUND);
                    }
                    smartCard.Using = false;
                    db.SaveChanges();

                    ticketMonthResult = db.TicketMonths.SingleOrDefault(x => x.RowID == request.rowID && x.Status ==3);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Xóa thẻ thất bại: " + e.Message);
                    return ResponseSuccess(Constants.FAILD);
                }
            }
            new LogViewModel().AddTicketMonthLog(ticketMonth, ActionType.STOP_USING_TICKET_MONTH_CARD, ticketMonth.Account, request.Code);

            return ResponseSuccess(ticketMonthResult);


        }

        /// <summary>
        /// KÍCH HOẠT LẠI VÉ THÁNG
        /// </summary>
        /// <param name="request">
        /// rowID lấy từ  API list-ticket-month-block mỗi thẻ có 1 rowID khác nhau
        /// </param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/v1/unblock-ticket-month")]
        public HttpResponseMessage UnBlockTicketMonth([FromBody]RequestDeleteTicketMonth request)
        {
            if (IsBodyNull(request))
            {
                return ResponseFail(Constants.BODY_NOT_FOUND);
            }
            if (!CheckCode.checkcode(request.Code))
            {
                return ResponseFail(Constants.CODEERROR);
            }


            TicketMonth ticketMonth;
            TicketMonth ticketMonthResult;
            using (DB db = new DB(request.Code))
            {
                try
                {
                    ticketMonth = db.TicketMonths.SingleOrDefault(x => x.RowID == request.rowID && x.Status == 3);
                    if (ticketMonth == null)
                    {
                        return ResponseFail(Constants.NOT_FOUND);
                    }
                    ticketMonth.Status = 1;
                    ticketMonth.Amount = ticketMonth.ChargesAmount;
                    ticketMonth.Account = Constants.ACCOUNT;
                    ticketMonth.DayUnLimit = DateTime.Now;
                    db.SaveChanges();

                    var smartCard = db.SmartCards.SingleOrDefault(x => x.ID == ticketMonth.ID);
                    if (smartCard == null)
                    {
                        return ResponseFail(Constants.NOT_FOUND);
                    }
                    smartCard.Using = true;
                    db.SaveChanges();
                    ticketMonthResult = db.TicketMonths.SingleOrDefault(x => x.RowID == request.rowID && x.Status < 2);

                }
                catch (Exception e)
                {
                    Debug.WriteLine("Xóa thẻ thất bại: " + e.Message);
                    return ResponseSuccess(Constants.FAILD);
                }
                
            }
            new LogViewModel().AddTicketMonthLog(ticketMonth, ActionType.UNBLOCK_TICKET_MONTH_CARD, ticketMonth.Account, request.Code);

            return ResponseSuccess(ticketMonthResult);

        }
        /// <summary>
        /// LẤY DANH SÁCH VÉ THÁNG NGƯNG SỬ DỤNG
        /// </summary>
        /// <param name="Code">
        /// Code: Mã tòa nhà
        /// </param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/list-ticket-month-block")]
        public HttpResponseMessage GetListTicketMonthBlock(string Code)
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
                var model = new TicketMonthViewModel();
                return ResponseSuccess(model.ListTicketMonthBlock(Code));              
            }
            catch
            {
                return ResponseFail(Constants.FAILD);
            }

        }
        /// <summary>
        /// XEM DOANH THU VÉ THÁNG
        /// </summary>
        /// <param name="request">
        ///  FromDate : Ngày bắt đầu tìm kiếm yyyy-MM-dd HH:mm:ss
        ///  ToDate : Ngày Kết thúc  tìm kiếm yyyy-MM-dd HH:mm:ss
        /// </param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/ticket-month-report")]
        public HttpResponseMessage TicketMonthReport([FromBody] RequestTicketMonthReport request)
        {
            if (IsBodyNull(request))
            {
                return ResponseFail(Constants.BODY_NOT_FOUND);
            }
            if (request.fromDate > request.toDate)
            {
                return ResponseFail(Constants.MSG_ERROR_DATE);
            }
            if (!CheckCode.checkcode(request.Code))
            {
                return ResponseFail(Constants.CODEERROR);
            }
            try
            {
                var model = new TicketMonthViewModel();
                return ResponseSuccess(model.TicketMonthReport(request.Code, request.fromDate, request.toDate));
            }
            catch
            {
                return ResponseFail(Constants.FAILD);
            }
           

        }
        //SUA DEN DAY
        /// <summary>
        /// TẠO VÉ THÁNG MỚI
        /// </summary>
        /// <param name="request">
        /// IdAccountCreate : ID tài khoản người tạo (ID nhận được khi login), 
        ///Stt : số thứ tự (Chọn từ stt của thẻ trong danh sách thẻ), 
        ///ID : Mã thẻ  (Chọn từ mã thẻ trong danh sách thẻ), 
        ///Digit : Biển số, 
        ///TenKH : Tên Khách Hàng, 
        ///CMND : Số CMND, 
        ///Company : Tên Cty, 
        ///Email : Địa chỉ mail, 
        ///Address : Địa chỉ nhà, 
        ///CarKind : Hiệu xe vd(Eciter 150), 
        ///IDPart : Id loại xe lấy trong ds Loại Xe(GetParts), 
        ///DateStart : Ngày đăng ký. Ví dụ: 2018-11-20, 
        ///DateEnd : Ngày hết hạng. Ví dụ: 2018-12-20, 
        ///Note : Ghi chú, 
        ///Amount : Số tiền thu mạc định lấy theo số tiền thu của loại xe hoặc nhân viện tự nhập .
        /// </param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/create-ticket-month")]
        public HttpResponseMessage CreateTicketMonth([FromBody]RequestTicketMonthCreate request)
        {
            if (IsBodyNull(request))
            {
                return ResponseFail(Constants.BODY_NOT_FOUND);
            }
            if (request.fromDate > request.toDate)
            {
                return ResponseFail(Constants.MSG_ERROR_DATE);
            }
            if (!CheckCode.checkcode(request.Code))
            {
                return ResponseFail(Constants.CODEERROR);
            }
            else
            {
                using (DB db = new DB(request.Code))
                {
                    try
                    {

                        {

                        }
                        if (!ModelState.IsValid)
                        {
                            return ResponseFail(Constants.MSG_ERROR_WRONG_PARAMETER_AND_TYPE);
                        }
                        var userCar = new UserCarViewModel().GetUserCar(request.IdAccountCreate, request.Code);
                        if (userCar == null)
                        {
                            return ResponseFail(Constants.MSG_ERROR_ID_USER_CAR);
                        }

                        var part = db.Parts.Find(request.IDPart);
                        if (part == null)
                        {
                            return ResponseFail(Constants.MSG_ERROR_ID_PART);
                        }

                        var viewModel = new TicketMonthViewModel();
                        int rowId = viewModel.GetMaxTicketMonthId(request.Code) + 1;

                        var DateStart = DateTime.Parse(String.Format("{0:yyyy-MM-dd 00:00:00}", request.fromDate));
                        var DateEnd = DateTime.Parse(String.Format("{0:yyyy-MM-dd 23:59:59}", request.toDate));

                        var ticketMonth = new TicketMonth()
                        {
                            RowID = rowId,
                            Stt = request.Stt,
                            ID = request.ID,
                            ProcessDate = DateTime.Now,
                            Digit = request.Digit,
                            TenKH = request.TenKH,
                            CMND = request.CMND,
                            Email = request.Email,
                            Company = request.Company,
                            Address = request.Address,
                            DateStart = DateStart,
                            DateEnd = DateEnd,
                            CarKind = request.CarKind,
                            IDPart = request.IDPart,
                            Amount = request.Amount.ToString(),
                            Status = 0,
                            Images = request.ID,
                            Account = userCar.NameUser,
                            Note = request.Note,
                            DayUnLimit = DateTime.Now,
                        };

                        db.TicketMonths.Add(ticketMonth);
                        db.SaveChanges();

                        new LogViewModel().AddTicketMonthLog(ticketMonth, ActionType.CREATE_TICKET_MONTH_CARD, ticketMonth.Account, request.Code);
                        var newticketmonth =new TicketMonthViewModel();
                        return ResponseSuccess(newticketmonth.GetTicketMonthID(request.Code, rowId));

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Lỗi tạo ticket month: " + e.Message);
                        return ResponseFail(Constants.MSG_ERROR_WRONG_PARAMETER);
                    }
                }

                  

            }
        }
        /// <summary>
        /// CHỈNH SỬA CẬP NHẬT LẠI VÉ THÁNG
        /// </summary>
        /// <param name="request">
        /// rowID lấy trong DS thẻ tháng
        /// IdAccountCreate : ID tài khoản người tạo (ID nhận được khi login), 
        ///Stt : số thứ tự (Chọn từ stt của thẻ trong danh sách thẻ), 
        ///Digit : Biển số, 
        ///TenKH : Tên Khách Hàng, 
        ///CMND : Số CMND, 
        ///Company : Tên Cty, 
        ///Email : Địa chỉ mail, 
        ///Address : Địa chỉ nhà, 
        ///CarKind : Hiệu xe vd(Eciter 150), 
        ///IDPart : Id loại xe lấy trong ds Loại Xe(GetParts), 
        ///DateStart : Ngày đăng ký. Ví dụ: 2018-11-20, 
        ///DateEnd : Ngày hết hạng. Ví dụ: 2018-12-20, 
        ///Note : Ghi chú, 
        ///Amount : Số tiền thu mạc định lấy theo số tiền thu của loại xe hoặc nhân viện tự nhập .
        /// </param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/v1/edit-ticket-month")]
        public HttpResponseMessage EditTicketMonth([FromBody]RequestTicketMonthEdit request)
        {

            if (IsBodyNull(request))
            {
                return ResponseFail(Constants.BODY_NOT_FOUND);
            }
            if (request.fromDate > request.toDate)
            {
                return ResponseFail(Constants.MSG_ERROR_DATE);
            }
            if (!CheckCode.checkcode(request.Code))
            {
                return ResponseFail(Constants.CODEERROR);
            }
            else
            {
                using(DB db = new DB(request.Code))
                {
                    try
                    {
                        if (!ModelState.IsValid)
                        {
                            return ResponseFail(Constants.MSG_ERROR_WRONG_PARAMETER_AND_TYPE);
                        }

                        var userCar = new UserCarViewModel().GetUserCar(request.IdAccountEdit, request.Code);
                        if (userCar == null)
                        {
                            return ResponseFail(Constants.MSG_ERROR_ID_USER_CAR);
                        }
                        if(!string.IsNullOrEmpty(request.IDPart))
                        {
                            var part = db.Parts.Find(request.IDPart);
                            if (part == null)
                            {
                                return ResponseFail(Constants.MSG_ERROR_ID_PART);
                            }
                        }
                     

                        var viewModel = new TicketMonthViewModel();
                        var ticketMonthOld = db.TicketMonths.SingleOrDefault(x => x.RowID == request.rowID);
                        if (ticketMonthOld == null)
                        {
                            return ResponseFail(Constants.MSG_ERROR_ROW_ID);
                        }
                        if(request.fromDate != null && request.toDate != null)
                        {
                            var DateStart = DateTime.Parse(String.Format("{0:yyyy-MM-dd 00:00:00}", request.fromDate));
                            var DateEnd = DateTime.Parse(String.Format("{0:yyyy-MM-dd 23:59:59}", request.toDate));

                            var ticketMonthNew = new TicketMonth();
                            ticketMonthNew.RowID = ticketMonthOld.RowID;
                            ticketMonthNew.ID = ticketMonthOld.ID;
                            ticketMonthNew.Stt = !string.IsNullOrEmpty(request.Stt.ToString())?(int)request.Stt : ticketMonthOld.Stt;
                            ticketMonthNew.ProcessDate = ticketMonthOld.ProcessDate;
                            ticketMonthNew.Digit = string.IsNullOrEmpty(request.Digit) ? ticketMonthOld.Digit : request.Digit;
                            ticketMonthNew.TenKH = string.IsNullOrEmpty(request.TenKH)? ticketMonthOld.TenKH : request.TenKH;
                            ticketMonthNew.CMND = string.IsNullOrEmpty(request.CMND)? ticketMonthOld.CMND : request.CMND;
                            ticketMonthNew.Email = string.IsNullOrEmpty(request.Email) ? ticketMonthOld.Email : request.Email;
                            ticketMonthNew.Company = string.IsNullOrEmpty(request.Company)? ticketMonthOld.Company : request.Company;
                            ticketMonthNew.Address = string.IsNullOrEmpty(request.Address)? ticketMonthOld.Address : request.Address;
                            ticketMonthNew.DateStart = DateStart;
                            ticketMonthNew.DateEnd = DateEnd;
                            ticketMonthNew.CarKind = string.IsNullOrEmpty(request.CarKind) ? ticketMonthOld.CarKind : request.CarKind;
                            ticketMonthNew.IDPart = string.IsNullOrEmpty(request.IDPart)? ticketMonthOld.IDPart : request.IDPart;
                            ticketMonthNew.Note = request.Note == null ? ticketMonthOld.Note : request.Note;
                            ticketMonthNew.Amount = string.IsNullOrEmpty(request.Amount.ToString()) ? ticketMonthOld.Amount : request.Amount.ToString();
                            ticketMonthNew.ChargesAmount = ticketMonthOld.ChargesAmount;
                            ticketMonthNew.Status = ticketMonthOld.Status == 0 ? 1 : ticketMonthOld.Status;
                            ticketMonthNew.Account = ticketMonthOld.Account;
                            ticketMonthNew.Images = ticketMonthOld.Images;
                            ticketMonthNew.DayUnLimit = DateTime.Now;

                            new LogViewModel().AddTicketMonthLogEdit(ticketMonthOld, ticketMonthNew, ActionType.EDIT_TICKET_MONTH_CARD, userCar.NameUser, request.Code);

                            db.TicketMonths.Remove(ticketMonthOld);
                            db.SaveChanges();

                            db.TicketMonths.Add(ticketMonthNew);
                            db.SaveChanges();
                            return ResponseSuccess(ticketMonthNew);
                        }
                        else
                        {
                            var ticketMonthNew = new TicketMonth();
                            ticketMonthNew.RowID = ticketMonthOld.RowID;
                            ticketMonthNew.ID = ticketMonthOld.ID;
                            ticketMonthNew.Stt = !string.IsNullOrEmpty(request.Stt.ToString()) ? (int)request.Stt : ticketMonthOld.Stt;
                            ticketMonthNew.ProcessDate = ticketMonthOld.ProcessDate;
                            ticketMonthNew.Digit = string.IsNullOrEmpty(request.Digit) ? ticketMonthOld.Digit : request.Digit;
                            ticketMonthNew.TenKH = string.IsNullOrEmpty(request.TenKH) ? ticketMonthOld.TenKH : request.TenKH;
                            ticketMonthNew.CMND = string.IsNullOrEmpty(request.CMND) ? ticketMonthOld.CMND : request.CMND;
                            ticketMonthNew.Email = string.IsNullOrEmpty(request.Email) ? ticketMonthOld.Email : request.Email;
                            ticketMonthNew.Company = string.IsNullOrEmpty(request.Company) ? ticketMonthOld.Company : request.Company;
                            ticketMonthNew.Address = string.IsNullOrEmpty(request.Address) ? ticketMonthOld.Address : request.Address;
                            ticketMonthNew.DateStart = ticketMonthOld.DateStart;
                            ticketMonthNew.DateEnd = ticketMonthOld.DateEnd;
                            ticketMonthNew.CarKind = string.IsNullOrEmpty(request.CarKind) ? ticketMonthOld.CarKind : request.CarKind;
                            ticketMonthNew.IDPart = string.IsNullOrEmpty(request.IDPart) ? ticketMonthOld.IDPart : request.IDPart;
                            ticketMonthNew.Note = request.Note == null ? ticketMonthOld.Note : request.Note;
                            ticketMonthNew.Amount = string.IsNullOrEmpty(request.Amount.ToString()) ? ticketMonthOld.Amount : request.Amount.ToString();
                            ticketMonthNew.ChargesAmount = ticketMonthOld.ChargesAmount;
                            ticketMonthNew.Status = ticketMonthOld.Status == 0 ? 1 : ticketMonthOld.Status;
                            ticketMonthNew.Account = ticketMonthOld.Account;
                            ticketMonthNew.Images = ticketMonthOld.Images;
                            ticketMonthNew.DayUnLimit = DateTime.Now;

                            new LogViewModel().AddTicketMonthLogEdit(ticketMonthOld, ticketMonthNew, ActionType.EDIT_TICKET_MONTH_CARD, userCar.NameUser, request.Code);

                            db.TicketMonths.Remove(ticketMonthOld);
                            db.SaveChanges();
                            db.TicketMonths.Add(ticketMonthNew);
                            db.SaveChanges();

                            var newticketmonth = new TicketMonthViewModel();
                            return ResponseSuccess(newticketmonth.GetTicketMonthID(request.Code,request.rowID));
                        }
                    

                       

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Lỗi edit ticket month: " + e.Message);
                        return ResponseFail(Constants.MSG_ERROR_WRONG_PARAMETER);
                    }
                }

           

            }
        }
        /// <summary>
        /// ĐỔI MÃ THẺ VÉ THÁNG
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/v1/update-ticket-month-id")]
        public HttpResponseMessage UpdateTicketMonthId([FromBody] RequestUpdateTicketMonthId request)
        {

            if (IsBodyNull(request))
            {
                return ResponseFail(Constants.BODY_NOT_FOUND);
            }
            if (!CheckCode.checkcode(request.Code))
            {
                return ResponseFail(Constants.CODEERROR);
            }
            using (DB db = new DB(request.Code))
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return ResponseFail(Constants.MSG_ERROR_WRONG_PARAMETER);
                    }
                    var userCar = new UserCarViewModel().GetUserCar(request.AccountID, request.Code);
                    if (userCar == null)
                    {
                        return ResponseFail(Constants.MSG_ERROR_ID_USER_CAR);
                    }

                    var ticketMonthOld = db.TicketMonths.SingleOrDefault(x => x.RowID == request.RowID);
                    if (ticketMonthOld == null)
                    {
                        return ResponseFail(Constants.MSG_ERROR_ROW_ID);
                    }

                    var ticketMonthNew = new TicketMonth()
                    {
                        RowID = ticketMonthOld.RowID,
                        ID = request.NewID,
                        Stt = request.Stt,
                        ProcessDate = ticketMonthOld.ProcessDate,
                        Digit = ticketMonthOld.Digit,
                        TenKH = ticketMonthOld.TenKH,
                        CMND = ticketMonthOld.CMND,
                        Email = ticketMonthOld.Email,
                        Company = ticketMonthOld.Company,
                        Address = ticketMonthOld.Address,
                        DateStart = ticketMonthOld.DateStart,
                        DateEnd = ticketMonthOld.DateEnd,
                        CarKind = ticketMonthOld.CarKind,
                        IDPart = ticketMonthOld.IDPart,
                        Note = ticketMonthOld.Note,
                        Amount = ticketMonthOld.Amount,
                        ChargesAmount = ticketMonthOld.ChargesAmount,
                        Status = ticketMonthOld.Status,
                        Account = ticketMonthOld.Account,
                        DayUnLimit = ticketMonthOld.DayUnLimit,
                        Images = ticketMonthOld.Images
                    };

                    new LogViewModel().AddTicketMonthLogEdit(ticketMonthOld, ticketMonthNew, ActionType.UPDATE_LOST_TICKET_MONTH_CARD, userCar.NameUser, request.Code);

                    db.TicketMonths.Remove(ticketMonthOld);
                    db.SaveChanges();

                    db.TicketMonths.Add(ticketMonthNew);
                    db.SaveChanges();

                    
                    return ResponseSuccess( );
                }
                catch
                {
                    return ResponseFail(Constants.FAILD);
                }
            }

       


        }
        // DA FIX
        /// <summary>
        /// TÌM KIẾM VÉ THÁNG HẾT HẠN / SẮP HẾT HẠN
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/search-ticket-month-expires")]
        public HttpResponseMessage SearchTicketMonthExpires([FromBody]RequestSearchTicketMonthExpires request)
        {
            if (IsBodyNull(request))
            {
                return ResponseFail(Constants.BODY_NOT_FOUND);
            }
            if (!CheckCode.checkcode(request.Code))
            {
                return ResponseFail(Constants.CODEERROR);
            }

            using (DB db = new DB(request.Code))
            {
                if (string.IsNullOrEmpty(request.SearchString))
                {

                    string sql = @"select * from (
	                            select rowid,stt,id,digit,tenkh,cmnd,company,email,images,address,amount,chargesamount,note, 
	                            case when datediff(day,getdate(),dateend) >=0 then N'Còn hạn' else N'Hết hạn' end as status, 
	                            abs(datediff(day,getdate(),dateend)) as songay,datestart,dateend ,account 
	                            from (select *   from ticketmonth where rowid in( select stt from (
		                            select max(rowid) as stt,id from ticketmonth  group by id) t) and status in(0,1)) tc)
                            t order by status desc,songay ";

                    var kq = db.Database.SqlQuery<TicketMonthsExpires>(sql).ToList();
                    return ResponseSuccess(kq);
                }
                else
                {
                    int stt = -1;

                    string searchcondition = "";

                    if (int.TryParse(request.SearchString, out stt))
                        searchcondition = " and (digit like N'%" + request.SearchString + "%' or tenkh like N'%" + request.SearchString + "%' or address like N'%" + request.SearchString + "%'  or company like N'%" + request.SearchString + "%' or stt = " + request.SearchString + " or id ='" + request.SearchString + "')";
                    else
                        searchcondition = " and (digit like N'%" + request.SearchString + "%' or tenkh like N'%" + request.SearchString + "%' or address like N'%" + request.SearchString + "%'  or company like N'%" + request.SearchString + "%' or id ='" + request.SearchString + "')";


                    string sql = @"select * from (
	                            select rowid,stt,id,digit,tenkh,cmnd,company,email,images,address,amount,chargesamount,note, 
	                            case when datediff(day,getdate(),dateend) >=0 then N'Còn hạn' else N'Hết hạn' end as status, 
	                            abs(datediff(day,getdate(),dateend)) as songay,datestart,dateend ,account 
	                            from (select *   from ticketmonth where rowid in( select stt from (
		                            select max(rowid) as stt,id from ticketmonth  group by id) t) and status in(0,1) " + @" " + searchcondition + @" ) tc)
                            t order by status desc,songay ";
                    try
                    {
                        var kq = db.Database.SqlQuery<TicketMonthsExpires>(sql).ToList();
                        return ResponseSuccess(kq);
                    }
                    catch
                    {
                        return ResponseFail(Constants.FAILD);
                    }
                }
            }

          

        }
        /// <summary>
        /// TÌM KIẾM VÉ THÁNG
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/search-ticket-month")]
        public HttpResponseMessage SearchTicketMonth([FromBody]RequestSearchTicketMonthExpires request)
        {

            if (IsBodyNull(request))
            {
                return ResponseFail(Constants.BODY_NOT_FOUND);
            }
            if (!CheckCode.checkcode(request.Code))
            {
                return ResponseFail(Constants.CODEERROR);
            }
            using (DB db = new DB(request.Code))
            {
                try
                {

                    int stt = 0;
                    int.TryParse(request.SearchString, out stt);
                    string sql = @" select rowid,stt,id,digit,tenkh,address,(select top 1 name from part where id = idpart) as partName,dateStart,dateEnd,
                            (select top 1 dateend from car c where c.id = t.id and matthe = 1 order by timeend desc) as dateLost,note,account,status,DayUnlimit
                            from ticketmonth t 
                            where 
	                            (status = 3 or  
	                            rowid in( select stt from (select max(rowid) as stt,id from ticketmonth  group by id) t) and status in(0,1))
	                            and (stt = " + stt + " or id like '%" + request.SearchString + @"%' or digit like N'%" + request.SearchString + @"%' or tenkh like N'%" + request.SearchString + @"%' or address like N'%" + request.SearchString + @"%' or company like N'%" + request.SearchString + @"%')
                            order by dateLost";
                    var kq = db.Database.SqlQuery<TicketMonthsLost>(sql).ToList();
                    return ResponseSuccess(kq);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Không thể search: Lỗi: " + e.Message);
                    return ResponseFail(Constants.FAILD);
                }
            }

       

        }
        /// <summary>
        /// TÌM KIẾM VÉ THÁNG ĐÃ NGƯNG SỬ DỤNG
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/search-ticket-month-stop-using")]
        public HttpResponseMessage SearchTicketMonthStopUsing([FromBody]RequestSearchTicketMonthExpires request)
        {
            if (IsBodyNull(request))
            {
                return ResponseFail(Constants.BODY_NOT_FOUND);
            }
            if (!CheckCode.checkcode(request.Code))
            {
                return ResponseFail(Constants.CODEERROR);
            }
            using (DB db  = new DB(request.Code))
            {
                try
                {

                    int stt = 0;
                    int.TryParse(request.SearchString, out stt);
                    string sql = @" select rowid,stt,id,digit,tenkh,address,(select top 1 name from part where id = idpart) as partName,dateStart,dateEnd,
                            (select top 1 dateend from car c where c.id = t.id and matthe = 1 order by timeend desc) as dateLost,note,account,status,DayUnlimit
                            from ticketmonth t 
                            where 
	                            (status = 3 or  
	                            rowid in( select stt from (select max(rowid) as stt,id from ticketmonth  group by id) t) and status in(3))
	                            and (stt = " + stt + " or id like '%" + request.SearchString + @"%' or digit like N'%" + request.SearchString + @"%' or tenkh like N'%" + request.SearchString + @"%' or address like N'%" + request.SearchString + @"%' or company like N'%" + request.SearchString + @"%')
                            order by dateLost";
                    var kq = db.Database.SqlQuery<TicketMonthsLost>(sql).ToList();
                    return ResponseSuccess(kq);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Không thể search: Lỗi: " + e.Message);
                    return ResponseFail(Constants.FAILD);
                }
            }

            

        }
        /// <summary>
        /// GIA HẠN VÉ THÁNG 1 HOẶC NHIỀU VÉ
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/renewal-ticket-month")]
        public HttpResponseMessage RenewalTicketMonth([FromBody]RequestRenewalTicketMonth request)
        {
            if (IsBodyNull(request))
            {
                return ResponseFail(Constants.BODY_NOT_FOUND);
            }
            if (!CheckCode.checkcode(request.Code))
            {
                return ResponseFail(Constants.CODEERROR);
            }
            
            else
            {
                using(DB db = new DB(request.Code))
                {
                    try
                    {
                        var viewModel = new TicketMonthViewModel();

                        var userCar = new UserCarViewModel().GetUserCar(request.AccountID, request.Code);
                        if (userCar == null)
                        {
                            return ResponseFail(Constants.MSG_ERROR_ID_USER_CAR);
                        }

                        foreach (string rowID in request.rowid)
                        {
                            //Cập nhật Amount
                            var ticketMonthOld = db.TicketMonths.Where(x => x.RowID.ToString() == rowID).OrderByDescending(x => x.RowID).First();
                            ticketMonthOld.ChargesAmount = ticketMonthOld.Amount;
                            db.SaveChanges();

                            var DateStart = DateTime.Parse(String.Format("{0:yyyy-MM-dd 00:00:00}", request.dateStart));
                            var DateEnd = DateTime.Parse(String.Format("{0:yyyy-MM-dd 23:59:59}", request.dateEnd));

                            //Tạo thẻ mới
                            var ticketMonthNew = new TicketMonth()
                            {
                                RowID = viewModel.GetMaxTicketMonthId(request.Code) + 1,
                                ID = ticketMonthOld.ID,
                                Stt = ticketMonthOld.Stt,
                                ProcessDate = DateTime.Now,
                                Digit = ticketMonthOld.Digit,
                                TenKH = ticketMonthOld.TenKH,
                                CMND = ticketMonthOld.CMND,
                                Email = ticketMonthOld.Email,
                                Company = ticketMonthOld.Company,
                                Address = ticketMonthOld.Address,
                                DateStart = DateStart,
                                DateEnd = DateEnd,
                                CarKind = ticketMonthOld.CarKind,
                                IDPart = ticketMonthOld.IDPart,
                                Note = ticketMonthOld.Note,
                                Amount = ticketMonthOld.Amount,
                                ChargesAmount = "0",
                                Status = 0,
                                Account = userCar.NameUser,
                                DayUnLimit = ticketMonthOld.DayUnLimit,
                                Images = ticketMonthOld.Images
                            };

                            db.TicketMonths.Add(ticketMonthNew);
                            db.SaveChanges();

                            new LogViewModel().AddTicketMonthLogEdit(ticketMonthOld, ticketMonthNew, ActionType.RENEWAL_TICKET_MONTH_CARD, userCar.NameUser, request.Code);
                        }


                        string sql = string.Format(@"select top {0} * from (

                                select rowid, stt, id, digit, tenkh, cmnd, company, email, images, address, amount, chargesamount, note,
	                            case when datediff(day, getdate(), dateend) >= 0 then N'Còn hạn' else N'Hết hạn' end as status, 
	                            abs(datediff(day, getdate(), dateend)) as songay,datestart,dateend ,account

                                from(select * from ticketmonth where rowid in (select stt from(
                                    select max(rowid) as stt, id from ticketmonth  group by id) t) and status in(0, 1)) tc)
                            t order by RowID desc,songay",request.rowid.Count().ToString());
                        var kq = db.Database.SqlQuery<TicketMonthsExpires>(sql).ToList();
                        return ResponseSuccess(kq);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Không thể gia hạn vé tháng: Lỗi: " + e.Message);
                        return ResponseFail(Constants.FAILD);
                    }
                }

        


            }


        }
    }
}

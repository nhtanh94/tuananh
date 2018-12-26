using AutoParking.Controllers;
using AutoParking.Entities;
using AutoParking.Models.RepsponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutoParking.Models
{
    public class TicketMonthViewModel : BaseController
    {
        public int GetMaxTicketMonthId(string Code)
        {
            using(DB db = new DB(Code))
            {
                return db.TicketMonths.Max(x => x.RowID);
            }

        }

        public TicketMonth GetTicketMonth(int id, string Code)
        {
            using(DB db = new DB(Code))
            {
                return db.TicketMonths.SingleOrDefault(x => x.RowID == id);
            }
           


        }
        public object ListTicketMonth(string code)
        {
            try
            {
                object listTicketMonth;
                using (DB db = new DB(code))
                {
                     listTicketMonth =( from TicketMonths in db.TicketMonths
                                          where
                                                (from t in (
                                                  (from TicketMonths0 in db.TicketMonths
                                                   group TicketMonths0 by new
                                                   {
                                                       TicketMonths0.ID
                                                   } into g
                                                   select new
                                                   {
                                                       stt = (int?)g.Max(p => p.RowID),
                                                       g.Key.ID
                                                   }))
                                                 select new
                                                 {
                                                     t.stt
                                                 }).Contains(new { stt = (Int32?)TicketMonths.RowID }) &&
                                            (TicketMonths.Status < 2)
                                          select new
                                          {
                                              RowID = TicketMonths.RowID,
                                              Stt = TicketMonths.Stt,
                                              TicketMonths.ID,
                                              ProcessDate = TicketMonths.ProcessDate,
                                              Digit = TicketMonths.Digit,
                                              TenKH = TicketMonths.TenKH,
                                              TicketMonths.CMND,
                                              Company = TicketMonths.Company,
                                              Email = TicketMonths.Email,
                                              Address = TicketMonths.Address,
                                              CarKind = TicketMonths.CarKind,
                                              IDPart = TicketMonths.IDPart,
                                              DateStart = TicketMonths.DateStart,
                                              DateEnd = TicketMonths.DateEnd,
                                              Note = TicketMonths.Note,
                                              Amount = TicketMonths.Amount,
                                              ChargesAmount = TicketMonths.ChargesAmount,
                                              Status = TicketMonths.Status,
                                              Account = TicketMonths.Account,
                                              Images = TicketMonths.Images,
                                              DayUnLimit = TicketMonths.DayUnLimit,
                                              Name = ((from Parts in db.Parts
                                                       where
                                                        Parts.ID == TicketMonths.IDPart
                                                       select new
                                                       {
                                                           Parts.Name
                                                       }).Take(1).FirstOrDefault().Name)
                                          }).ToList();
                    
                }
                return listTicketMonth;
            }

            catch
            {
                return null;
            }



        }
        public object GetTicketMonthID(string code,int rowid)
        {
            try
            {
                object listTicketMonth;
                using (DB db = new DB(code))
                {
                    listTicketMonth = (from TicketMonths in db.TicketMonths
                                       where
                                             (from t in (
                                               (from TicketMonths0 in db.TicketMonths
                                                group TicketMonths0 by new
                                                {
                                                    TicketMonths0.ID
                                                } into g
                                                select new
                                                {
                                                    stt = (int?)g.Max(p => p.RowID),
                                                    g.Key.ID
                                                }))
                                              select new
                                              {
                                                  t.stt
                                              }).Contains(new { stt = (Int32?)TicketMonths.RowID }) &&
                                         (TicketMonths.Status < 2)
                                       select new
                                       {
                                           RowID = TicketMonths.RowID,
                                           Stt = TicketMonths.Stt,
                                           TicketMonths.ID,
                                           ProcessDate = TicketMonths.ProcessDate,
                                           Digit = TicketMonths.Digit,
                                           TenKH = TicketMonths.TenKH,
                                           TicketMonths.CMND,
                                           Company = TicketMonths.Company,
                                           Email = TicketMonths.Email,
                                           Address = TicketMonths.Address,
                                           CarKind = TicketMonths.CarKind,
                                           IDPart = TicketMonths.IDPart,
                                           DateStart = TicketMonths.DateStart,
                                           DateEnd = TicketMonths.DateEnd,
                                           Note = TicketMonths.Note,
                                           Amount = TicketMonths.Amount,
                                           ChargesAmount = TicketMonths.ChargesAmount,
                                           Status = TicketMonths.Status,
                                           Account = TicketMonths.Account,
                                           Images = TicketMonths.Images,
                                           DayUnLimit = TicketMonths.DayUnLimit,
                                           Name = ((from Parts in db.Parts
                                                    where
                                                     Parts.ID == TicketMonths.IDPart
                                                    select new
                                                    {
                                                        Parts.Name
                                                    }).Take(1).FirstOrDefault().Name)
                                       }).SingleOrDefault(x =>x.RowID == rowid);

                }
                return listTicketMonth;
            }

            catch
            {
                return null;
            }



        }
        public object ListTicketMonthBlock(string code)
        {
            object listTicketBlock;
            using (DB db = new DB(code))
            {
                 listTicketBlock = (from tm in db.TicketMonths
                                      join tl in db.TicketLogs on tm.ID equals tl.ID
                                      where tm.Status == 3 && tl.ProcessType.Equals(ActionType.STOP_USING_TICKET_MONTH_CARD)
                                      where tl.DateProcess == (
                                            (from tl2 in db.TicketLogs
                                             where tl2.ProcessType.Equals(ActionType.STOP_USING_TICKET_MONTH_CARD) && tl2.ID.Equals(tm.ID)
                                             orderby tl2.DateProcess descending
                                             select tl2).FirstOrDefault().DateProcess
                                          )
                                      select new
                                      {
                                          tm.RowID,
                                          tm.Stt,
                                          tm.ID,
                                          tm.ProcessDate,
                                          tm.Digit,
                                          tm.TenKH,
                                          tm.CMND,
                                          tm.Company,
                                          tm.Email,
                                          tm.Address,
                                          tm.CarKind,
                                          tm.IDPart,
                                          tm.DateStart,
                                          tm.DateEnd,
                                          tm.Note,
                                          tm.Amount,
                                          tm.ChargesAmount,
                                          tm.Status,
                                          tm.Account,
                                          tm.Images,
                                          tm.DayUnLimit,
                                          LastTimeUpdate = tl.DateProcess
                                      }).ToList();

            }

            return listTicketBlock;
        }

        public object TicketMonthReport(string code ,DateTime fromDate,DateTime toDate)
        {
            string sql1 = string.Format("select COUNT(RowID) as Quantity, SUM(convert(int,amount)) as Price from TicketMonth t where  t.processdate between '{0}' and '{1}' and amount >0 and status in (0,1,3)", fromDate.ToString("yyyy-MM-dd HH:mm:ss"), toDate.ToString("yyyy-MM-dd HH:mm:ss"));
            string sql2 = string.Format("select *,ROW_NUMBER() OVER(ORDER BY COMPANY, t.dateStart) AS Row,  (select name from part p where p.id = t.idpart) as Name from ticketmonth t where  t.processdate between '{0}' and '{1}' and amount >0 and status in (0,1,3)", fromDate.ToString("yyyy-MM-dd HH:mm:ss"),toDate.ToString("yyyy-MM-dd HH:mm:ss"));
          
            ResponseTicketMonthReport doanhThuVT;
            using(DB db = new DB(code))
            {
                try
                {
                    doanhThuVT = new ResponseTicketMonthReport();
                    doanhThuVT.TicketMonths = new List<TicketMonthResult>();
                    var query1 = db.Database.SqlQuery<ResponseTicketMonthReport>(sql1);
                    foreach (ResponseTicketMonthReport t in query1)
                    {
                        doanhThuVT.Price = t.Price;
                        doanhThuVT.Quantity = t.Quantity;
                    }
                    var query2 = db.Database.SqlQuery<TicketMonthResult>(sql2).ToList();
                    doanhThuVT.TicketMonths.AddRange(query2);
                    return doanhThuVT;
                }
                catch
                {
                    return null;
                }
            }
          
        }
        public List<TicketMonthsExpires> SearchTicketMonthExpires(string Code, string Sreach)
        {
            

            using (DB db = new DB(Code))
            {
                if (string.IsNullOrEmpty(Sreach))
                {

                    string sql = @"select * from (
	                            select rowid,stt,id,digit,tenkh,cmnd,company,email,images,address,amount,chargesamount,note, 
	                            case when datediff(day,getdate(),dateend) >=0 then N'Còn hạn' else N'Hết hạn' end as status, 
	                            abs(datediff(day,getdate(),dateend)) as songay,datestart,dateend ,account 
	                            from (select *   from ticketmonth where rowid in( select stt from (
		                            select max(rowid) as stt,id from ticketmonth  group by id) t) and status in(0,1)) tc)
                            t order by status desc,songay ";

                    var kq = db.Database.SqlQuery<TicketMonthsExpires>(sql).ToList();
                    return kq;
                }
                else
                {
                    int stt = -1;

                    string searchcondition = "";

                    if (int.TryParse(Sreach, out stt))
                        searchcondition = " and (digit like N'%" + Sreach + "%' or tenkh like N'%" + Sreach + "%' or address like N'%" + Sreach + "%'  or company like N'%" + Sreach + "%' or stt = " + Sreach + " or id ='" + Sreach + "')";
                    else
                        searchcondition = " and (digit like N'%" + Sreach + "%' or tenkh like N'%" + Sreach + "%' or address like N'%" + Sreach + "%'  or company like N'%" + Sreach + "%' or id ='" + Sreach + "')";


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
                        return kq;
                    }
                    catch
                    {
                        return null;
                    }
                }
            }



        }
    }
}
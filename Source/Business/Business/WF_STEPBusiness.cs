using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;
using Business.BaseBusiness;
using Business.CommonBusiness;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using PagedList;
using Business.CommonModel.WFSTEP;



namespace Business.Business
{
    public class WF_STEPBusiness : BaseBusiness<WF_STEP>
    {
        public WF_STEPBusiness(UnitOfWork unitofwork)
            : base(unitofwork)
        {
        }
        public PageListResultBO<WF_STEP_BO> GetDaTaByPage(int idStream, WF_STEP_SEARCHBO searchModel, int pageIndex = 1, int pageSize = 20)
        {
            var query = from tbl in this.context.WF_STEP
                        where tbl.WF_ID == idStream

                        join tbl_state1 in this.context.WF_STATE on tbl.STATE_BEGIN equals tbl_state1.ID into jstate
                        from stateBegin in jstate.DefaultIfEmpty()

                        join tbl_state2 in this.context.WF_STATE on tbl.STATE_END equals tbl_state2.ID into jstate2
                        from stateEnd in jstate2.DefaultIfEmpty()

                        select new WF_STEP_BO
                           {
                               ID = tbl.ID,
                               WF_ID = tbl.WF_ID,
                               NAME = tbl.NAME,
                               GHICHU = tbl.GHICHU,
                               STATE_BEGIN = tbl.STATE_BEGIN,
                               STATE_END = tbl.STATE_END,
                               ICON = tbl.ICON,
                               IS_RETURN = tbl.IS_RETURN,
                               create_at = tbl.create_at,
                               create_by = tbl.create_by,
                               edit_at = tbl.edit_at,
                               edit_by = tbl.edit_by,
                               TrangThaiBatDau = stateBegin != null ? stateBegin.STATE_NAME : "",
                               TrangThaiKetThuc = stateEnd != null ? stateEnd.STATE_NAME : "",
                           };
            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.sortQuery))
                {
                    query = query.OrderBy(searchModel.sortQuery);
                }
                else
                {
                    query = query.OrderByDescending(x => x.ID);
                }
            }
            else
            {
                query = query.OrderByDescending(x => x.ID);
            }
            var resultmodel = new PageListResultBO<WF_STEP_BO>();
            if (pageSize == -1)
            {
                var dataPageList = query.ToList();
                resultmodel.Count = dataPageList.Count;
                resultmodel.TotalPage = 1;
                resultmodel.ListItem = dataPageList;
            }
            else
            {
                var dataPageList = query.ToPagedList(pageIndex, pageSize);
                resultmodel.Count = dataPageList.TotalItemCount;
                resultmodel.TotalPage = dataPageList.PageCount;
                resultmodel.ListItem = dataPageList.ToList();
            }
            return resultmodel;
        }
        public WF_STEP_BO GetDaTaByID(int ID)
        {
            var query = from tbl in this.context.WF_STEP
                        where tbl.ID == ID
                        join tbl_state1 in this.context.WF_STATE on tbl.STATE_BEGIN equals tbl_state1.ID into jstate
                        from stateBegin in jstate.DefaultIfEmpty()

                        join tbl_state2 in this.context.WF_STATE on tbl.STATE_END equals tbl_state2.ID into jstate2
                        from stateEnd in jstate2.DefaultIfEmpty()

                        join tbl_config in this.context.WF_STEP_CONFIG on tbl.ID equals tbl_config.WF_STEP_ID into jconfig
                        from config in jconfig.DefaultIfEmpty()

                        join tbl_MainProcess in this.context.WF_STEP_USER_PROCESS.Where(x => x.IS_XULYCHINH == true) on tbl.ID equals tbl_MainProcess.WF_STEP_ID into jmainprocess
                        from mainprocess in jmainprocess.DefaultIfEmpty()

                        join tbl_JoinProcess in this.context.WF_STEP_USER_PROCESS.Where(x => x.IS_XULYCHINH == false) on tbl.ID equals tbl_JoinProcess.WF_STEP_ID into jjoinprocess
                        from joinprocess in jjoinprocess.DefaultIfEmpty()
                        select new WF_STEP_BO
                           {
                               ID = tbl.ID,
                               WF_ID = tbl.WF_ID,
                               NAME = tbl.NAME,
                               GHICHU = tbl.GHICHU,
                               STATE_BEGIN = tbl.STATE_BEGIN,
                               STATE_END = tbl.STATE_END,
                               ICON = tbl.ICON,
                               IS_RETURN = tbl.IS_RETURN,
                               create_at = tbl.create_at,
                               create_by = tbl.create_by,
                               edit_at = tbl.edit_at,
                               edit_by = tbl.edit_by,
                               REQUIRED_REVIEW = tbl.REQUIRED_REVIEW,
                               TrangThaiBatDau = stateBegin != null ? stateBegin.STATE_NAME : "",
                               TrangThaiKetThuc = stateEnd != null ? stateEnd.STATE_NAME : "",
                               ConfigStep = config,
                               NguoiXuLyChinh = mainprocess,
                               NguoiThamGiaXuLy = joinprocess
                           };
            var resultmodel = query.FirstOrDefault();
            return resultmodel;
        }

        public GoModel GetDataByStream(int idStream)
        {
            var result = new GoModel();
            result.nodeKeyProperty = "id";
            result.nodeDataArray = (this.context.WF_STATE.Where(x => x.WF_ID == idStream)
                .Select(x => new NodeItem()
                {
                    text = x.STATE_NAME,
                    id = x.ID,
                    loc = x.LOCATION
                })).ToList();
            var lstStep = this.context.WF_STEP.Where(x => x.WF_ID == idStream).ToList();
            foreach (var item in lstStep.ToList())
            {
                if (item.IS_RETURN == true)
                {
                    var backStep = new WF_STEP();
                    backStep.STATE_BEGIN = item.STATE_END;
                    backStep.STATE_END = item.STATE_BEGIN;
                    //backStep.NAME = "Trả về - " + item.NAME;
                    backStep.NAME = "Trả về";
                    lstStep.Add(backStep);
                }
            }
            result.linkDataArray = (lstStep
                .Select(x => new StepItem()
                {
                    from = x.STATE_BEGIN.Value,
                    text = x.NAME,
                    to = x.STATE_END.Value
                })).ToList();

            return result;
        }

        public List<WF_STEP> GetListNextStep(int wfid, int currentState)
        {
            var query = this.context.WF_STEP.Where(x => x.WF_ID == wfid && x.STATE_BEGIN == currentState).ToList();

            return query;
        }
        public List<StepBackBO> GetListNextStepBack(WF_PROCESS process)
        {
            var query = (from step in this.context.WF_STEP.Where(x => x.WF_ID == process.WF_ID && x.STATE_END == process.CURRENT_STATE && x.IS_RETURN==true)
                         join tblLog in this.context.WF_LOG.Where(x => x.ITEM_ID == process.ITEM_ID && x.IS_RETURN != true && x.ITEM_TYPE == process.ITEM_TYPE && x.NGUONHAN_ID == process.USER_ID && x.STEP_ID != null)
                         on step.ID equals tblLog.STEP_ID into jlog
                         where jlog.Any()
                         select new StepBackBO()
                         {
                             ID = step.ID,
                             //NAME = "Trả về " + step.NAME,
                             NAME = "Trả về ",
                             STATE_BEGIN = step.STATE_END,
                             STATE_END = step.STATE_BEGIN,
                             WF_ID = step.WF_ID,
                             Log = jlog.OrderByDescending(x => x.create_at).FirstOrDefault()
                         }).ToList();
            return query;
        }
    }
}


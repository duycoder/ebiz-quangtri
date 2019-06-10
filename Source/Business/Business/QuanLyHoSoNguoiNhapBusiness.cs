/**
 * The HiNet License
 *
 * Copyright 2015 Hinet JSC. All rights reserved.
 * HINET PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/** 
* @author  NAMDV
*/

using Business.BaseBusiness;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Business
{
    public class QuanLyHoSoNguoiNhapBusiness : BaseBusiness<QUANLY_HOSO_NGUOINHAP>
    {
        public QuanLyHoSoNguoiNhapBusiness(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        public void Save(QUANLY_HOSO_NGUOINHAP item)
        {
            try
            {
                if (item.ID == 0)
                {
                    this.repository.Insert(item);
                }
                else
                {
                    this.repository.Update(item);
                }
                this.repository.Save();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<QUANLY_HOSO_NGUOINHAP> GetData(long id)
        {
            var result = from nguoinhan in this.context.QUANLY_HOSO_NGUOINHAP
                         where nguoinhan.HOSO_ID.HasValue && nguoinhan.HOSO_ID.Value == id
                         select nguoinhan;
            return result.ToList();
        }

        public void DeleteByHoSo(long? hoSoId = 0)
        {
            var listData = this.repository.All().Where(x => x.HOSO_ID == hoSoId).ToList();
            this.DeleteAll(listData);
        }

        public void DeleteAll(List<QUANLY_HOSO_NGUOINHAP> listData)
        {
            if (listData.Any())
            {
                foreach (var item in listData)
                {
                    this.repository.Delete(item);
                    this.repository.Save();
                }
            }
        }

        public List<long> GetByHoSo(long? hoSoId = 0)
        {
            return this.repository.All().Where(x => x.HOSO_ID == hoSoId && x.USER_ID.HasValue).Select(x => x.USER_ID.Value).ToList();
        }
        public void Delete(object id)
        {
            this.repository.Delete(id);
            this.repository.Save();
        }

        public void DeleteByHoSo(int hoSoId)
        {
            if (hoSoId > 0)
            {
                var listHoSo = this.repository.All().Where(x => x.HOSO_ID == hoSoId).ToList();
                foreach (var item in listHoSo)
                {
                    this.repository.Delete(item.ID);
                    this.repository.Save();
                }
            }
        }

        public string GetText(long? hoSoId = 0)
        {
            string result = string.Empty;
            var source = (from nn in this.context.QUANLY_HOSO_NGUOINHAP
                          join nd in this.context.DM_NGUOIDUNG
on nn.USER_ID equals nd.ID
                          where nn.HOSO_ID == hoSoId
                          select nd.HOTEN).ToList();
            if (source.Any())
            {
                return string.Join( ",", source);
            }
            return result;
        }
    }
}

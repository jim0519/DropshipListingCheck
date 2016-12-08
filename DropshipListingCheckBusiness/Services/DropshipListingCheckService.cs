using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;
using DropshipListingCheckData;
using Common;

namespace DropshipListingCheckBusiness.Services
{
    public class DropshipListingCheckService : IDropshipListingCheckService
    {
        #region Fields

        private readonly IRepository<OnlyonlineData> _onlyonlineDataRepository;
        private readonly IRepository<DealsdirectData> _dealsdirectDataRepository;
        #endregion

        public DropshipListingCheckService(IRepository<OnlyonlineData> onlyonlineDataRepository
            , IRepository<DealsdirectData> dealsdirectDataRepository)
        {
            this._onlyonlineDataRepository = onlyonlineDataRepository;
            this._dealsdirectDataRepository = dealsdirectDataRepository;
        }




        public IList<OnlyonlineData> GetOnlyonlineData()
        {
            return _onlyonlineDataRepository.Table.ToList();
        }

        public IList<DealsdirectData> GetDealsdirectData()
        {
            return _dealsdirectDataRepository.Table.ToList();
        }
    }
}

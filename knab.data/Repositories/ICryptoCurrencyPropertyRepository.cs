using knab.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace knab.DataAccess.Repositories
{
    public interface ICryptoCurrencyPropertyRepository
    {
        Task AddCryptoCurrencypropertyAsync(CryptoCurrencyProperty currencyproperty);
        Task GetCryptoCurrencyPropertiesAsync();
    }
}

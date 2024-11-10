using BookShop.Common.Models.Postgress.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookShop.Common.DataContext.Postgress.Mappers
{
    public class ImageLinkMapper : SqlMapper.TypeHandler<List<ImageLink>>
    {
        public override void SetValue(IDbDataParameter parameter, List<ImageLink> value)
        {
            parameter.Value = JsonSerializer.Serialize(value); // Преобразование в строку
            parameter.DbType = DbType.String; // Убедитесь, что тип установлен правильно
        }

        public override List<ImageLink> Parse(object value)
        {
            return JsonSerializer.Deserialize<List<ImageLink>>(value.ToString()); // Преобразование из строки в List<ImageLink>
        }
    }
}

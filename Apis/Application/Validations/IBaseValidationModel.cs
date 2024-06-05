using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validations
{
    public interface IBaseValidationModel
    {
        public Task Validate(object validator, IBaseValidationModel modelObj);
    }
}

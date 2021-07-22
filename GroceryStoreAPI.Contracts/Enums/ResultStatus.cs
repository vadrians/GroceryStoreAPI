using System;
using System.Collections.Generic;
using System.Text;

namespace GroceryStoreAPI.Contracts.Enums
{
    public enum ResultStatus
    {
        Succeeded = 200,
        BadRequest = 400,
        MissingResource = 404,
        InternalError = 500
    }
}

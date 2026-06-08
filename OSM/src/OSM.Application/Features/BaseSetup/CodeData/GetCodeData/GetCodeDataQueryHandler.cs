using MediatR;
using OSM.Application.Abstractions.Data;
using OSM.Application.Common;

namespace OSM.Application.Features.BaseSetup.CodeData.GetCodeData
{
    public sealed class GetCodeDataQueryHandler(IDapperHelper DapperHelper) : IRequestHandler<GetCodeDataQuery, Result<List<CodeDataResponse>>>
    {
        public async Task<Result<List<CodeDataResponse>>> Handle(GetCodeDataQuery request, CancellationToken cancellationToken)
        {
            string sql = @"SELECT 
                             D.[Table_Code]
                            ,D.[Data_Code]
                            ,D.[Data_Value]
                            ,D.[Sort_Order]
                          FROM
                          [Code_Table] T 
                          INNER JOIN [Code_Data] D ON T.[Table_Code] = D.[Table_Code]
                          WHERE D.[IsDeleted] = 0 AND T.[IsDeleted] = 0 AND D.[Table_Code] = @TableCode
                          ORDER BY D.[Sort_Order]";

           var data = await DapperHelper.QueryAsync<CodeDataResponse>(sql,new { TableCode = request.tableCode });
            return Result.Success(data.ToList());
        }
    }
}

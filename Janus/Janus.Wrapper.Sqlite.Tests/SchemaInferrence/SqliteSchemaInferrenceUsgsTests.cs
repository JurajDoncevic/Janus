﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.Sqlite.Tests.SchemaInferrence;
public class SqliteSchemaInferrenceUsgsTests : SqliteSchemaInferrenceTests
{
    public override string ConnectionString => "Data Source=./usgs-lower-us.db;";

    public override string DataSourceName => "usgs";

    public override string ExpectedSchemaString => "(usgs ((main ((quakes ((time DATETIME 0 True False)(latitude DECIMAL 1 True False)(longitude DECIMAL 2 True False)(depth DECIMAL 3 True False)(mag DECIMAL 4 True False)(magType STRING 5 True False)(nst INT 6 True False)(gap DECIMAL 7 True False)(dmin DECIMAL 8 True False)(rms DECIMAL 9 True False)(net STRING 10 True False)(id STRING 11 True False)(updated DATETIME 12 True False)(place STRING 13 True False)(type STRING 14 True False)(horizontalError DECIMAL 15 True False)(depthError DECIMAL 16 True False)(magError DECIMAL 17 True False)(magNst INT 18 True False)(status STRING 19 True False)(locationSource STRING 20 True False)(magSource STRING 21 True False)))))))";
}